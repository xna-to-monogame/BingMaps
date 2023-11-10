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
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using BingMapsSample.ApiModels;
using BingMapsSample.ApiModels.Imagery;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace BingMapsSample;

public class BingApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public BingApiService(string apiKey)
    {
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new ArgumentNullException(nameof(apiKey), $"{nameof(apiKey)} cannot be null or an empty string.");
        }

        _apiKey = apiKey;

        _httpClient = new HttpClient();
    }

    public async Task<Texture2D?> GetImageFromServer(GeoCoordinate centerCoordinate, Vector2 tileDimensions, int zoomLevel, BingMapsViewType viewType)
    {
        Texture2D? result = null;

        StaticMapEndpoint endpoint = new StaticMapEndpoint(centerCoordinate, StaticMapEndpoint.ImagerySet.Aerial, zoomLevel, _apiKey);
        HttpResponseMessage? response = await endpoint.GetResponse(_httpClient);
        if(response != null && response.IsSuccessStatusCode)
        {
            Texture2D texture = Texture2D.FromStream(BingMapsSampleGame.GraphicsDeviceManager.GraphicsDevice, response.Content.ReadAsStream());

        }

        return result;
        //string endpoint = $"https://dev.virtualearth.net/REST/V1/Imagery/Map/{viewType}/{centerCoordinate.Latitude},{centerCoordinate.Longitude}/{zoomLevel}?mapSize={(int)tileDimensions.X},{(int)tileDimensions.Y}&key={_apiKey}";

        //Stream stream = null;

        //try
        //{
        //    HttpResponseMessage message = _httpClient.GetAsync(endpoint).ConfigureAwait(false).GetAwaiter().GetResult();
        //    stream = message.Content.ReadAsStream();
        //    Texture2D texture = Texture2D.FromStream(BingMapsSampleGame.GraphicsDeviceManager.GraphicsDevice, stream);
        //    return texture;
        //}
        //catch (Exception ex)
        //{
        //    Debug.WriteLine(ex.Message);
        //    return null;
        //}
        //finally
        //{
        //    if (stream != null)
        //    {
        //        stream.Dispose();
        //    }

        //}
    }

    public LocationResultModel GetLocationModelAsync(string locationToFocusOn)
    {
        string endpoint = $"https://dev.virtualearth.net/REST/v1/Locations?o=json&q={locationToFocusOn}&key={_apiKey}";

        LocationResultModel result = null;
        try
        {
            HttpResponseMessage message = _httpClient.GetAsync(endpoint).ConfigureAwait(false).GetAwaiter().GetResult();
            string json = message.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            LocationResultModel model = JsonSerializer.Deserialize<LocationResultModel>(json);
            return model;
            //result = _httpClient.GetFromJsonAsync<LocationResultModel>(endpoint);
        }
        catch { }

        return result;
    }
}
