#region File Description

//-----------------------------------------------------------------------------
// Button.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

#endregion

#region Using Statements

using System;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace BingMapsSample;

/// <summary>
/// Manages a map tile cache.  The class does not only manage a cache of miple tiles but is also responsible for
/// acquiring them from the Bing REST service.
/// </summary>
public class BingMapsTiles : IDisposable
{
    #region Fields, Properties, and Indexer

    public const int MIN_ZOOM_LEVEL = 1;
    public const int MAX_ZOOM_LEVEL = 23;

    /// <summary>
    ///     The amount of bytes to pre-allocate for image buffers.
    /// </summary>
    public const int INITIAL_IMAGE_BUFFER_SIZE = 150000;

    //  A plane of 5x5 (assuming ActiveTilePlaneSize is 5) tiles which will be loaded to memory for fast interaction.
    //  The center of hte screen will point to a location inside the tile at [2, 2].  [0, 0] is the top-left tile,
    //  while [4, 4] is the bottom-right tile
    private TileInformation[,] _activeTilePlane;

    private Vector2 _tileDimensions;
    private Rectangle _screenBounds;
    private CachePhase _state = CachePhase.Idle;
    private int _pendingRequestCount = 0;
    private GeoCoordinate _centerGeoCoordinate;
    private Texture2D _unavailableImage;
    private int _zoomLevel;
    private bool _isDisposed;

    /// <summary>
    /// Returns the tile information for hte specified coordiantes on the active tile plane.
    /// </summary>
    /// <remarks>
    /// The possible values supplied for <paramref name="x"/> and <paramref name="y"/> are under the assumption that
    /// <see cref="ActiveTilePlaneSize"/> is 5.
    /// </remarks>
    /// <param name="x">X-coordiante of the desired tile.  Must be between 0 and 4 inclusive.</param>
    /// <param name="y">Y-coordinate of the desired tile.  Must be between 0 and 4 inclusive.</param>
    /// <returns>Tile information of the specified tile.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if either <paramref name="x"/> or <paramref name="y"/> are out of bounds.
    /// </exception>
    public TileInformation this[int x, int y]
    {
        get
        {
            if (x < 0 || x >= ActiveTilePlaneSize)
            {
                throw new ArgumentOutOfRangeException(nameof(x), $"Valid values are 0 to {ActiveTilePlaneSize - 1} inclusive.");
            }

            if (y < 0 || y >= ActiveTilePlaneSize)
            {
                throw new ArgumentOutOfRangeException(nameof(y), $"Valid values are 0 to {ActiveTilePlaneSize - 1} inclusive.");
            }

            return _activeTilePlane[x, y];
        }
    }

    /// <summary>
    /// The zoome level used to display the map tiles.
    /// </summary>
    public int ZoomLevel
    {
        get => _zoomLevel;
        set
        {
            if (value < MIN_ZOOM_LEVEL || value > MAX_ZOOM_LEVEL)
            {
                throw new ArgumentException($"Valid values are {MIN_ZOOM_LEVEL} to {MAX_ZOOM_LEVEL} inclusive");
            }

            _zoomLevel = value;
        }
    }

    public BingMapsViewType ViewType { get; set; }

    //  The X and Y index of the center tile in the active tile plane.
    private int PlaneCenterIndex => (ActiveTilePlaneSize - 1) / 2;

    /// <summary>
    ///     The size of both of the active tile plane's dimensions.
    /// </summary>
    public int ActiveTilePlaneSize { get; private set; }

    #endregion

    #region Initialization

    public BingMapsTiles(Texture2D unavailableImage, Vector2 tileDimensions, int planeDimensions)
    {
        if (planeDimensions % 2 != 1 || planeDimensions < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(planeDimensions), "Plane dimensions must be an odd positive number.");
        }

        ActiveTilePlaneSize = planeDimensions;

        _activeTilePlane = new TileInformation[ActiveTilePlaneSize, ActiveTilePlaneSize];

        _tileDimensions = tileDimensions;
        _screenBounds = BingMapsSampleGame.GraphicsDeviceManager.GraphicsDevice.Viewport.Bounds;
        _unavailableImage = unavailableImage;
        //_zoomLevel = (MAX_ZOOM_LEVEL + MIN_ZOOM_LEVEL) / 2;
        _zoomLevel = 15;
    }

    ~BingMapsTiles() => Dispose(false);

    public async Task InitializeActiveTilePlane(GeoCoordinate centerGeocoordinate)
    {
        _centerGeoCoordinate = centerGeocoordinate;

        if (_pendingRequestCount != 0)
        {
            _state = CachePhase.CancellingRequests;
            CancelActiveRequetsAndResetImages();
        }
        else
        {
            _state = CachePhase.InitializingTiles;
            GetActivePlaneImagesAsync(centerGeocoordinate);
        }
    }

    #endregion

    #region Private Methods

    private void CancelActiveRequetsAndResetImages()
    {
        for (int x = 0; x < ActiveTilePlaneSize; x++)
        {
            for (int y = 0; y < ActiveTilePlaneSize; y++)
            {
                TileInformation cellInformation = _activeTilePlane[x, y];

                if (cellInformation != null)
                {
                    cellInformation.UnloadImage();
                }
            }
        }
    }

    private async Task GetActivePlaneImagesAsync(GeoCoordinate centerCoordinate)
    {
        Vector2 centerPixel = TileSystem.GeoCoordinateToVector2(centerCoordinate, _zoomLevel);

        int planeCenterIndex = PlaneCenterIndex;

        for (int x = 0; x < ActiveTilePlaneSize; x++)
        {
            int xDelta = x - planeCenterIndex;

            for (int y = 0; y < ActiveTilePlaneSize; y++)
            {
                if(x == 6 && y == 6)
                {
                    ;
                }
                int yDelta = y - planeCenterIndex;

                TileInformation? cellInformation = _activeTilePlane[x, y];

                //  Intiailzie or clean the active tile cube cell
                if(cellInformation == null)
                {
                    cellInformation = new TileInformation();
                    _activeTilePlane[x, y] = cellInformation;
                }
                else
                {
                    cellInformation.Dispose();
                }

                //  Calcuate the center geo-coordinate for the current tile
                Vector2 tileCenter = centerPixel + _tileDimensions * new Vector2(xDelta, yDelta);
                GeoCoordinate tileCenterGeoCoordiante;

                try
                {
                    tileCenterGeoCoordiante = TileSystem.Vector2ToGeoCoordinate(tileCenter, _zoomLevel);
                    Texture2D? image= await BingMapsSampleGame.BingApiService.GetImageFromServer(tileCenterGeoCoordiante,_tileDimensions, _zoomLevel, ViewType);
                    cellInformation.UnloadImage();

                    if (image != null)
                    {
                        cellInformation.Image = image;
                    }
                    else
                    {
                        cellInformation.Image = _unavailableImage;
                    }



                }
                catch(ArgumentOutOfRangeException)
                {
                    cellInformation.Image = _unavailableImage;
                }
            }
        }
    }

    private async Task MoveToNextPhaseAsync()
    {
        _state = _state switch
        {
            CachePhase.CancellingRequests => CachePhase.InitializingTiles,
            CachePhase.InitializingTiles => CachePhase.Idle,
            _ => _state
        };

        //  TODO: This might break
        if(_state == CachePhase.InitializingTiles)
        {
            GetActivePlaneImagesAsync(_centerGeoCoordinate);
        }
    }


    #endregion

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool isDisposing)
    {
        if (_isDisposed) { return; }

        if (isDisposing)
        {
            //  Dispose unmanaged objects
        }

        for (int x = 0; x < ActiveTilePlaneSize; x++)
        {
            for (int y = 0; y < ActiveTilePlaneSize; y++)
            {
                _activeTilePlane[x, y].Dispose();
            }
        }

        _isDisposed = true;
    }
}
