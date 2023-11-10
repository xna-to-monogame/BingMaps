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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace BingMapsSample;

internal class BingMapsViewer
{
    #region Fields and Properties

    private readonly Texture2D _defaultImage;
    private readonly Texture2D _unavailableImage;
    private readonly Rectangle _screenBounds;
    private readonly Vector2 _screenCenterVector;
    private readonly Vector2 _tileDimensions;
    private readonly Vector2 _maxOffsetAbs;

    //  The geo position at the center of the current center tile
    private GeoCoordinate _centerGeoCoordinate;

    //  Offset used for drawing hte map tiles on the screen
    private Vector2 _offset = Vector2.Zero;

    public int ZoomLevel { get; private set; }
    public BingMapsViewType ViewType { get; set; }
    public BingMapsTiles ActiveTiles { get; set; }

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes a new BIng maps viewer
    /// </summary>
    /// <param name="services">
    /// The GamesServiceContainer containing.  
    /// It is assumed the container contains andinstance of both BingApiService and IGraphicsDeviceService.
    /// </param>
    /// <param name="defaultImage">The default image to show until actual image data is retrieved.</param>
    /// <param name="unavailableImage">The image to use for tiles where an image is not available.</param>
    /// <param name="location">The initial location to use.</param>
    /// <param name="zoomLevel">The zoome level (1 - 22).</param>
    /// <param name="gridSize">
    /// The size of the active tile grid around the desired location.  For example, specifying 3 will retrieve a 3x3
    /// set of images around the desired location.  This number must be positive and odd.
    /// </param>
    /// <exception cref="ArgumentException"></exception>
    public BingMapsViewer(Texture2D defaultImage, Texture2D unavailableImage, GeoCoordinate location, int zoomLevel, int gridSize)
    {
        ZoomLevel = zoomLevel;
        ViewType = BingMapsViewType.Aerial;

        _screenBounds = BingMapsSampleGame.GraphicsDeviceManager.GraphicsDevice.Viewport.Bounds;
        _tileDimensions = new Vector2(_screenBounds.Width, _screenBounds.Height);
        _screenCenterVector = _tileDimensions / 2;

        _defaultImage = defaultImage;
        _unavailableImage = unavailableImage;

        //  Initialize the tile set object
        ActiveTiles = new BingMapsTiles(_unavailableImage, _tileDimensions, gridSize)
        {
            ZoomLevel = zoomLevel,
            ViewType = ViewType
        };

        _maxOffsetAbs = _tileDimensions * ((ActiveTiles.ActiveTilePlaneSize - 1) / 2);

        //  Initialize the tile set with the appripriate tile images
        _centerGeoCoordinate = location;
        ActiveTiles.InitializeActiveTilePlane(location);
    }

    #endregion

    #region Rendering

    /// <summary>
    /// Draws the map on the screen.
    /// </summary>
    /// <param name="spriteBatch">
    /// The <see cref="Microsoft.Xna.Framework.Graphics.SpriteBatch"/> used to render. 
    /// It is assumed that SpriteBatch.Being has already been called.
    /// </param>
    public void Draw(SpriteBatch spriteBatch)
    {
        int centerIndex = (ActiveTiles.ActiveTilePlaneSize - 1) / 2;

        for (int i = 0; i < ActiveTiles.ActiveTilePlaneSize; i++)
        {
            for (int j = 0; j < ActiveTiles.ActiveTilePlaneSize; j++)
            {
                //  Add an offset ot the drawn image depending on its position in the tile matrix
                float x = (i - centerIndex) * _tileDimensions.X;
                float y = (j - centerIndex) * _tileDimensions.Y;
                Vector2 extraOffset = new Vector2(x, y);

                Texture2D? image = ActiveTiles[i, j].Image;

                if (image == null)
                {
                    image = _defaultImage;
                }

                if(image.Name != "noImage")
                {
                    ;
                }
                Rectangle dest;
                //dest.X = (int)(_screenCenterVector.X + _offset.X + extraOffset.X);
                //dest.Y = (int)(_screenCenterVector.Y + _offset.Y + extraOffset.Y);
                //dest.Width = BingMapsSampleGame.GraphicsDeviceManager.GraphicsDevice.Viewport.Width;
                //dest.Height = BingMapsSampleGame.GraphicsDeviceManager.GraphicsDevice.Viewport.Height;
                //spriteBatch.Draw(image, dest, null, Color.White, 0.0f, new Vector2(dest.Width, dest.Height) / 2, SpriteEffects.None, 0.0f);
                Vector2 scale;
                scale.X = BingMapsSampleGame.GraphicsDeviceManager.GraphicsDevice.Viewport.Width / image.Width;
                scale.Y = BingMapsSampleGame.GraphicsDeviceManager.GraphicsDevice.Viewport.Height / image.Height;
                spriteBatch.Draw(image, (_screenCenterVector + _offset + extraOffset) * scale, null, Color.White, 0, _tileDimensions / 2.0f, scale, SpriteEffects.None, 0.0f);

            }
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Refreshes the displayed images by getting them again from the REST service.
    /// </summary>
    public void RefreshImages()
    {
        ActiveTiles.ViewType = ViewType;
        ActiveTiles.InitializeActiveTilePlane(_centerGeoCoordinate);
    }

    /// <summary>
    /// Centers the view on the specified geo-coordinate.
    /// </summary>
    /// <param name="location">Geo-coordinateto center the view on.</param>
    public void CenterOnLocation(GeoCoordinate location)
    {
        _centerGeoCoordinate = location;
        _offset = Vector2.Zero;
        ActiveTiles.InitializeActiveTilePlane(location);
    }

    /// <summary>
    /// Move the map by the specified offset, but prevent a move that would transition to a tile that is off the active
    /// tile set.
    /// </summary>
    /// <param name="gestureOffset">The offset to move the map by.</param>
    public void MoveByOffset(Vector2 gestureOffset)
    {
        _offset += gestureOffset;

        Vector2 fixOffset = Vector2.Zero;
        if (_offset.X < -_maxOffsetAbs.X || _offset.X > _maxOffsetAbs.X)
        {
            fixOffset.X = -gestureOffset.X;
        }

        if (_offset.Y < _maxOffsetAbs.Y || _offset.Y > _maxOffsetAbs.Y)
        {
            fixOffset.Y = -gestureOffset.Y;
        }

        _offset += fixOffset;
    }

    #endregion
}
