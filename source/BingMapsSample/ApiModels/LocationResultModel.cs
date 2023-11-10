#region File Description

//-----------------------------------------------------------------------------
// Button.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

#endregion

#region Using Statements

using System.Collections.Generic;
using System.Text.Json.Serialization;

#endregion


namespace BingMapsSample.ApiModels;


public class LocationResultModel
{
    [JsonPropertyName("authenticationResultCode")]
    public string AuthenticationResultCode { get; set; }

    [JsonPropertyName("brandLogoUri")]
    public string BrandLogoUri { get; set; }

    [JsonPropertyName("copyright")]
    public string Copyright { get; set; }

    [JsonPropertyName("resourceSets")]
    public List<ResourceSetModel> ResourceSets { get; set; }

    [JsonPropertyName("statusCode")]
    public int StatusCode { get; set; }

    [JsonPropertyName("statusDescription")]
    public string StatusDescription { get; set; }

    [JsonPropertyName("traceId")]
    public string TraceId { get; set; }
}



