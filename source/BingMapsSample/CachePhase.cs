#region File Description

//-----------------------------------------------------------------------------
// Button.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

#endregion


namespace BingMapsSample;

/// <summary>
/// Used to denote the cache's current activity.
/// </summary>
public enum CachePhase { Idle, CancellingRequests, InitializingTiles, Disposing }
