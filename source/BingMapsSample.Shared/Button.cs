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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

#endregion

namespace BingMapsSample.Shared;

public class Button
{
    #region Fields and Properties

    private readonly Texture2D _texture;
    private readonly SpriteFont _font;
    private Vector2 _textPosition;
    private string _text;

    public Rectangle Bounds { get; set; }
    public Color BackgroundColor { get; set; }
    public Color TextColor { get; set; }

    public string Text
    {
        get => _text;
        set
        {
            if (_text == value) { return; }
            _text = value;
            CalculateTextPosition();
        }
    }

    public Action<Button>? Click { get; set; } = null;

    #endregion

    #region Initialization

    /// <summary>
    ///     Creates a new button instance.
    /// </summary>
    /// <param name="text">The text that will appear at the center of the button.</param>
    /// <param name="font">Font used to write the button's text.</param>
    /// <param name="textColor">The color for the button's text.</param>
    /// <param name="backgroundColor">The button's background color.</param>
    /// <param name="bounds">The button's bounds.</param>
    /// <param name="texture">The buttons's texture.</param>
    public Button(string text, SpriteFont font, Color textColor, Color backgroundColor, Rectangle bounds, Texture2D texture)
    {
        _texture = texture;
        _font = font;
        TextColor = textColor;
        BackgroundColor = backgroundColor;
        Bounds = bounds;
        _text= text;
        CalculateTextPosition();        
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Handles the user's taps on the button.
    /// </summary>
    /// <remarks>
    /// If the button is detected as taped, then the <see cref="Button.Click"/> action will be invoked.
    /// </remarks>
    /// <param name="sample">User's touch input.</param>
    /// <returns><see langword="true"/> if the button was pressed; otherwise, <see langword="false"/>.</returns>
    public bool HandleInput(GestureSample sample)
    {
        if (sample.GestureType != GestureType.Tap) { return false; }

        int x = (int)sample.Position.X - 1;
        int y = (int)sample.Position.Y - 1;
        Rectangle touchRect = new Rectangle(x, y, 2, 2);

        if (!Bounds.Intersects(touchRect)) { return false; }

        Click?.Invoke(this);
        return true;
    }

    /// <summary>
    /// Renders the button.
    /// </summary>
    /// <param name="spriteBatch">
    /// The <see cref="Microsoft.Xna.Framework.Graphics.SpriteBatch"/> used to render. 
    /// It is assumed that SpriteBatch.Being has already been called.
    /// </param>
    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_texture, Bounds, BackgroundColor);
        spriteBatch.DrawString(_font, _text, _textPosition, TextColor);
    }

    #endregion

    #region Private Methods

    private void CalculateTextPosition()
    {
        Vector2 measure = _font.MeasureString(_text);
        _textPosition = new Vector2(Bounds.Center.X - measure.X / 2, Bounds.Center.Y - measure.Y / 2);
    }

    #endregion 
}
