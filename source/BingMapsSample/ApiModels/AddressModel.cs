#region File Description

//-----------------------------------------------------------------------------
// Button.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

#endregion

#region Using Statements

using System.Text.Json.Serialization;

#endregion

namespace BingMapsSample.ApiModels;

// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
public class AddressModel
{
    [JsonPropertyName("adminDistrict")]
    public string AdminDistrict { get; set; }

    [JsonPropertyName("adminDistrict2")]
    public string AdminDistrict2 { get; set; }

    [JsonPropertyName("countryRegion")]
    public string CountryRegion { get; set; }

    [JsonPropertyName("formattedAddress")]
    public string FormattedAddress { get; set; }

    [JsonPropertyName("locality")]
    public string Locality { get; set; }
}