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


public class GeocodePointModel
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("coordinates")]
    public List<double> Coordinates { get; set; }

    [JsonPropertyName("calculationMethod")]
    public string CalculationMethod { get; set; }

    [JsonPropertyName("usageTypes")]
    public List<string> UsageTypes { get; set; }
}