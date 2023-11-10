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
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace BingMapsSample.Shared;

public class BingApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly IGraphicsDeviceService _graphicsDeviceService;

    public BingApiService(string apiKey, GameServiceContainer services)
    {
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new ArgumentNullException(nameof(apiKey), $"{nameof(apiKey)} cannot be null or an empty string.");
        }

        _apiKey = apiKey;
        
        _graphicsDeviceService = services.GetService<IGraphicsDeviceService>();
        if(_graphicsDeviceService is null)
        {
            throw new ArgumentException($"{services} does not contain an instance of {nameof(IGraphicsDeviceService)}", nameof(services));
        }
        _httpClient = new HttpClient();
    }

    public async Task<Texture2D?> GetImageFromServer(GeoCoordinate centerCoordinate, Vector2 tileDimensions, int zoomLevel, BingMapsViewType viewType)
    {
        string endpoint = $"http://dev.virtualearth.net/REST/V1/Imagery/Map/{viewType}/{centerCoordinate.Latitude},{centerCoordinate.Longitude}/{zoomLevel}?mapSize={(int)tileDimensions.X},{(int)tileDimensions.Y}&key={_apiKey}";

        try
        {
            using Stream stream = await _httpClient.GetStreamAsync(endpoint);
            Texture2D texture = Texture2D.FromStream(_graphicsDeviceService.GraphicsDevice, stream);
            return texture;
        }
        catch
        {
            return null;
        }
    }
}
