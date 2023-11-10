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
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace BingMapsSample;

public class TileInformation
{
    #region Fields Properties

    private Texture2D? _image = null;
    private Vector2 _imageSize;
    private bool _isPerformingRequest = false;
    private bool _isDisposed;
    public Texture2D? Image
    {
        get => _image;
        set
        {
            _image = value;
            _imageSize = value == null ? Vector2.Zero : new Vector2(value.Width, value.Height);
        }
    }

    public bool IsRequestCanceled { get; private set; }

    #endregion

    #region Initialization

    public TileInformation()
    {
        IsRequestCanceled = false;
    }

    ~TileInformation() => Dispose(false);

    #endregion

    #region Public Methods

    public void UnloadImage()
    {
        if(_image != null)
        {
            _image.Dispose();
            _image = null;
        }
    }

    #endregion

    #region IDisposable Members and Related Methods

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool isDisposing)
    {
        if (_isDisposed) { return; }

        if(isDisposing)
        {
            //  Dispose unamanged resources
        }

        UnloadImage();
        _isDisposed = true;
    }

    #endregion
}
