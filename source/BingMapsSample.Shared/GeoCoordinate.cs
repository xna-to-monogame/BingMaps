#region File Description

//-----------------------------------------------------------------------------
// Button.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

#endregion


namespace BingMapsSample.Shared
{
    public struct GeoCoordinate
    {
        public double Longitude;
        public double Latitude;

        public GeoCoordinate(double longitude, double latitude)
        {
            Longitude = longitude;
            Latitude = latitude;
        }
    }
}
