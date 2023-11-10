#region File Description

//-----------------------------------------------------------------------------
// Button.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

#endregion


namespace BingMapsSample
{
    public struct GeoCoordinate
    {
        public double Longitude;
        public double Latitude;

        public GeoCoordinate(double latitude, double longitude)
        {
            Longitude = longitude;
            Latitude = latitude;
        }

        public override string ToString() => $"{Longitude},{Latitude}";
    }
}
