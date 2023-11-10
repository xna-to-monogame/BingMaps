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


public class ResourceSetModel
{
    [JsonPropertyName("estimatedTotal")]
    public int EstimatedTotal { get; set; }

    [JsonPropertyName("resources")]
    public List<ResourceModel> Resources { get; set; }
}
