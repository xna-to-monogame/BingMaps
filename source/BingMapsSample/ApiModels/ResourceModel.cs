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

public class ResourceModel
{
    [JsonPropertyName("__type")]
    public string Type { get; set; }

    [JsonPropertyName("bbox")]
    public List<double> Bbox { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("point")]
    public PointModel Point { get; set; }

    [JsonPropertyName("address")]
    public AddressModel Address { get; set; }

    [JsonPropertyName("confidence")]
    public string Confidence { get; set; }

    [JsonPropertyName("entityType")]
    public string EntityType { get; set; }

    [JsonPropertyName("geocodePoints")]
    public List<GeocodePointModel> GeocodePoints { get; set; }

    [JsonPropertyName("matchCodes")]
    public List<string> MatchCodes { get; set; }
}