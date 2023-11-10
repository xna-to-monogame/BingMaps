using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Xna.Framework;

namespace BingMapsSample.ApiModels.Imagery
{
    public partial class StaticMapEndpoint
    {
        const string _basePath = "https://dev.virtualearth.net/REST/V1/Imagery/Map";

        private double? _headingParameter = null;
        private int? _mapWidthParameter = null;
        private int? _mapHeightParameter = null;
        private double? _pitchParameter = null;
        private double? _orientationParameter = null;
        private int _zoomLevelParameter = 10;
        private int? _fieldOfViewParameter = null;
        private string _apiKey;

        /// <summary>
        /// (Required) The geo-coordinate on the earth where the map is centered.
        /// </summary>
        public GeoCoordinate CenterPointParameter { get; init; } = new GeoCoordinate(0, 0);

        /// <summary>
        /// (Required) The type of imagery
        /// </summary>
        public ImagerySet ImagerySetParameter { get; init; } = ImagerySet.Aerial;


        /// <summary>
        /// (Required) The level of zoom to display.  Must be a vlaue between 0 and 23
        /// </summary>
        public int ZoomLevelParameter
        {
            get => _zoomLevelParameter;
            init => _zoomLevelParameter = MathHelper.Clamp(value, 0, 20);
        }

        /// <summary>
        /// (Optional) A query string that is used to determien the map location to display.
        /// </summary>
        public string? QueryParameter { get; set; } = null;

        /// <summary>
        /// (Optional) Specifies whether to change the display of overlapping pushpins so that they are displayed 
        /// separatly on the map.
        /// </summary>
        public DeclutterPins? DeclutterPinsParameter { get; set; } = null;

        /// <summary>
        /// (Optional) Specifies the resolution of the labels on the image to retrieve.
        /// </summary>
        public DPI? DpiParameter { get; set; } = null;

        /// <summary>
        /// (Optional) The image format to use for the static map.
        /// </summary>
        public Format? FormatParameter { get; set; } = null;

        /// <summary>
        /// (Optional for StreetSide) Desired camera heading in degrees, clockwise from north.
        /// </summary>
        public double? HeadingParameter
        {
            get => _headingParameter;
            set
            {
                if (value.HasValue)
                {
                    _headingParameter = MathHelper.Clamp((float)value, 0, 360);
                }
                else
                {
                    _headingParameter = null;
                }
            }
        }

        /// <summary>
        /// (Optional) A display layer that renders on top of the imagery set.
        /// </summary>
        public MapLayer? MapLayerParameter { get; set; } = null;

        /// <summary>
        /// (Optional) The width, in pixels, of the static map output. Must be between 80 and 2000.
        /// </summary>
        public int? MapWidthParameter
        {
            get => _mapWidthParameter;
            set
            {
                if (value.HasValue)
                {
                    _mapWidthParameter = MathHelper.Clamp(value.Value, 80, 200);
                }
                else
                {
                    _mapWidthParameter = null;
                }

            }
        }

        /// <summary>
        /// (Optional) The height, in pixels, of the static map output. Must be between 80 and 1500.
        /// </summary>
        public int? MapHeightParameter
        {
            get => _mapHeightParameter;
            set
            {
                if (value.HasValue)
                {
                    _mapHeightParameter = MathHelper.Clamp(value.Value, 80, 1500);
                }
                else
                {
                    _mapHeightParameter = null;
                }
            }
        }

        /// <summary>
        /// (Optional for StreetSide) Controls the camera pitch angle. Positive values point the camera up toward the
        /// sky, negative values point down to the ground.  Value must be between -90 and 90.
        /// </summary>
        public double? PitchParameter
        {
            get => _pitchParameter;
            set
            {
                if (value.HasValue)
                {
                    _pitchParameter = MathHelper.Clamp((float)value.Value, -90, 90);
                }
                else
                {
                    _pitchParameter = null;
                }
            }
        }

        /// <summary>
        /// (Optional for Bird's Eye) The orientation of view for Bird's Eye imagery.  This option only applies to
        /// Bird's Eye imagery.
        /// </summary>
        public double? OrientationParameter
        {
            get => _orientationParameter;
            set
            {
                if (value.HasValue)
                {
                    _orientationParameter = MathHelper.Clamp((float)value.Value, 0, 360);
                }
                else
                {
                    _orientationParameter = null;
                }
            }
        }

        /// <summary>
        /// (Optional) One or more pushpin locations to display on teh map.
        /// </summary>
        public List<GeoCoordinate> PushPinCollectionParameter { get; } = new List<GeoCoordinate>();


        /// <summary>
        /// (Optional for Streetside) Specifies the horizontal field of view in degrees that should be shown
        /// in the image. It's a way of specifying the zoom level of the image. Either 
        /// <see cref="FieldOfViewParameter"/> or <see cref="ZoomLevelParameter"/> should be specified in
        /// the request, not both.  Must be a value between 15 and 120.
        /// </summary>
        public int? FieldOfViewParameter
        {
            get => _fieldOfViewParameter;
            set
            {
                if (value.HasValue)
                {
                    _fieldOfViewParameter = MathHelper.Clamp(value.Value, 15, 120);
                }
                else
                {
                    _orientationParameter = null;
                }
            }
        }

        public StaticMapEndpoint(GeoCoordinate centerPointParameter, ImagerySet imagerySetParameter, int zoomLevelParameter, string apiKey)
        {
            CenterPointParameter = centerPointParameter;
            ImagerySetParameter = imagerySetParameter;
            ZoomLevelParameter = zoomLevelParameter;
            _apiKey = apiKey;
        }

        public async Task<HttpResponseMessage?> GetResponse(HttpClient httpClient)
        {
            string url = BuildEndpointUrl();

            try
            {
                return await httpClient.GetAsync(url);
            }
            catch { return null; }

        }

        private string BuildEndpointUrl()
        {
            string url = _basePath;
            url += $"/{ImagerySetParameter}";
            url += $"/{CenterPointParameter}";

            //  If street side and a pitch param was specified, then we don't specify
            //  zoom.  Only use one or the other
            if (ImagerySetParameter != ImagerySet.StreetSide ||
               (ImagerySetParameter == ImagerySet.StreetSide && PitchParameter == null))
            {
                url += $"/{ZoomLevelParameter}";
            }

            string? queryParams = GetQueryParameter();
            if (!string.IsNullOrEmpty(queryParams))
            {
                url += $"?{queryParams}";
            }

            return url;
        }

        public string? GetQueryParameter()
        {
            NameValueCollection parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters.Add("key", _apiKey);

            if (DeclutterPinsParameter != null)
            {
                parameters.Add("declutterpins", $"{(int)DeclutterPinsParameter}");
            }

            if (DpiParameter != null)
            {
                parameters.Add("dpi", $"{(DpiParameter == DPI.Default ? "null" : "Large")}");
            }

            if (FormatParameter != null)
            {
                parameters.Add("format", $"{FormatParameter}");
            }

            if (ImagerySetParameter == ImagerySet.StreetSide)
            {
                if (HeadingParameter != null)
                {
                    parameters.Add("heading", $"{HeadingParameter}");
                }

                if (PitchParameter != null)
                {
                    parameters.Add("pitch", $"{PitchParameter}");
                }

                if (FieldOfViewParameter != null)
                {
                    parameters.Add("fieldOfView", $"{FieldOfViewParameter}");
                }
            }

            if (MapLayerParameter != null)
            {
                parameters.Add("mapLayer", $"{MapLayerParameter}");
            }

            if (MapWidthParameter != null && MapHeightParameter != null)
            {
                parameters.Add("mapSize", $"{MapWidthParameter},{MapHeightParameter}");
            }

            if ((ImagerySetParameter == ImagerySet.BirdsEye || ImagerySetParameter == ImagerySet.BirdsEyeWIthLabels)
                && OrientationParameter != null)
            {
                parameters.Add("orientation", $"{OrientationParameter}");
            }

            if (PushPinCollectionParameter.Count > 0)
            {
                parameters.Add("pushpin", string.Join(';', PushPinCollectionParameter));
            }

            return parameters.ToString();
        }
    }
}
