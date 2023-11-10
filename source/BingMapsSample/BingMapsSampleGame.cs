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
using System.IO;
using System.Threading.Tasks;
using Android.App;
using BingMapsSample.ApiModels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

#endregion

namespace BingMapsSample;

public class BingMapsSampleGame : Game
{
    public static GraphicsDeviceManager GraphicsDeviceManager;
    public static BingApiService BingApiService;
    #region Fields

    private SpriteBatch _spriteBatch;
    private readonly GeoCoordinate _startingCoordinate;
    private BingMapsViewer _bingMapsViewer;
    private Button _switchButton;

    #endregion

    #region Initializations

    public BingMapsSampleGame()
    {
        GraphicsDeviceManager = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";

        // Frame rate is 30 fps by default for Windows Phone.
        TargetElapsedTime = TimeSpan.FromTicks(333333);

        GraphicsDeviceManager.IsFullScreen = true;
        GraphicsDeviceManager.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        GraphicsDeviceManager.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        GraphicsDeviceManager.SupportedOrientations = Microsoft.Xna.Framework.DisplayOrientation.Portrait;
        GraphicsDeviceManager.ApplyChanges();

        _startingCoordinate = new GeoCoordinate(47.639597, -122.12845);

        TouchPanel.EnabledGestures = GestureType.Tap | GestureType.FreeDrag | GestureType.DragComplete;
    }

    #endregion

    #region Loading
    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        #region Hide Key

        string apiKey = "";

        #endregion

        //using (Stream stream = TitleContainer.OpenStream("Content/apikey.txt"))
        //{
        //    byte[] apikeyBytes = new byte[stream.Length];
        //    for (int i = 0; i < apikeyBytes.Length; i++)
        //    {
        //        apikeyBytes[i] = (byte)stream.ReadByte();
        //    }
        //    apiKey = System.Text.Encoding.UTF8.GetString(apikeyBytes);
        //}

        BingApiService = new BingApiService(apiKey);

        Texture2D blank = Content.Load<Texture2D>("blank");
        SpriteFont buttonFont = Content.Load<SpriteFont>("Font");
        _switchButton = new Button($"Switch to \nRoad view", buttonFont, Color.White, Color.Black, new Rectangle(10, 10, 100, 60), blank);
        _switchButton.Click = (button) =>
        {
            if (_bingMapsViewer == null) { return; }

            BingMapsViewType previousType = _bingMapsViewer.ViewType;

            _bingMapsViewer.ViewType = _bingMapsViewer.ViewType switch
            {
                BingMapsViewType.Aerial => BingMapsViewType.Road,
                _ => BingMapsViewType.Aerial
            };

            button.Text = $"Switch to\n{previousType} view";
            _bingMapsViewer.RefreshImages();
        };

        Texture2D defaultImage = Content.Load<Texture2D>("noImage");
        Texture2D unavailableImage = Content.Load<Texture2D>("noImage");
        _bingMapsViewer = new BingMapsViewer(defaultImage, unavailableImage, _startingCoordinate, 1, 15);
    }

    #endregion

    #region Update and Render

    protected override async void Update(GameTime gameTime)
    {
        // Allows the game to exit
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
        {
            Exit();
        }

        await HandleInput();

        base.Update(gameTime);
    }

    private async Task HandleInput()
    {
        //  Read all gesture
        while(TouchPanel.IsGestureAvailable)
        {
            GestureSample sample = TouchPanel.ReadGesture();

            if(_switchButton.HandleInput(sample))
            {
                return;
            }

            if(sample.GestureType == GestureType.Tap)
            {
                string location = await KeyboardInput.Show("Select Location", "Type in a location to focus on");
                LocationResultModel result = BingMapsSampleGame.BingApiService.GetLocationModelAsync(location);
                if(IsLocationResultValid(result))
                {
                    double longitude = result.ResourceSets[0].Resources[0].Point.Coordinates[0];
                    double lattitude = result.ResourceSets[0].Resources[0].Point.Coordinates[1];

                    GeoCoordinate coordinte = new GeoCoordinate(longitude, lattitude);
                    _bingMapsViewer.CenterOnLocation(coordinte);
                }
            }
            else if(sample.GestureType  == GestureType.FreeDrag)
            {
                _bingMapsViewer.MoveByOffset(sample.Delta);
            }
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Gray);
        
        _spriteBatch.Begin();
        _bingMapsViewer.Draw(_spriteBatch);
        _switchButton.Draw(_spriteBatch);
        _spriteBatch.End();

        base.Draw(gameTime);
    }

    #endregion

    #region Non-Public Methods

    private bool IsLocationResultValid(LocationResultModel result) 
    {
        if(result == null)
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(Activity.ApplicationContext);
            alert.SetTitle("No Results");
            alert.SetMessage("No results returned from location given");
            alert.SetPositiveButton("OK", (sender, args) => { });
            alert.Show();
            return false;
        }

        if(result.ResourceSets.Count == 0 || result.ResourceSets[0].Resources.Count == 0)
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(Activity.ApplicationContext);
            alert.SetTitle("Invalid Location");
            alert.SetMessage("The requested location was not recognized by the system.");
            alert.SetPositiveButton("OK", (sender, args) => { });
            alert.Show();
            return false;
        }

        if (result.ResourceSets[0].Resources[0].Point == null ||
            result.ResourceSets[0].Resources[0].Point.Coordinates.Count != 2)
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(Activity.ApplicationContext);
            alert.SetTitle("Invalid location result");
            alert.SetMessage("The location result is missing data.");
            alert.SetPositiveButton("OK", (sender, args) => { });
            alert.Show();
            return false;
        }

        return true;
    }

    protected override void OnExiting(object sender, EventArgs args)
    {
        if(_bingMapsViewer != null)
        {
            _bingMapsViewer.ActiveTiles.Dispose();
        }

        base.OnExiting(sender, args);
    }

    #endregion
}
