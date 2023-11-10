
namespace BingMapsSample.ApiModels.Imagery
{
    public partial class StaticMapEndpoint
    {
        /// <summary>
        /// Specifies whether to change the display of overlapping pushpins so that they display separtely on the map.
        /// </summary>
        public enum DeclutterPins
        {
            /// <summary>
            /// Do not declutter puspin icons.
            /// </summary>
            DoNotDecluter = 0,

            /// <summary>
            /// Declutter pushpin icons
            /// </summary>
            Declutter = 1
        }

        /// <summary>
        /// Specifies the resolution of the labels on the image to retrieve.
        /// </summary>
        public enum DPI
        {
            /// <summary>
            /// Default image resolution.
            /// </summary>
            Default = 0,

            /// <summary>
            /// High resolution labels.
            /// </summary>
            Large = 1,
        }

        /// <summary>
        /// The image format to use for the static map.
        /// </summary>
        public enum Format
        {
            /// <summary>
            /// Use GIF image format.
            /// </summary>
            GIF,

            /// <summary>
            /// Use JPEG image format.  JPEG format is the default for Road, Aerial, and AerialWithLabels imagery.
            /// </summary>
            JPEG,

            /// <summary>
            /// Use PNG image format. PNG is the default format for OrdnanceSurvey imagery.
            /// </summary>
            PNG
        }

        /// <summary>
        /// The type of imagery
        /// </summary>
        public enum ImagerySet
        { 
            /// <summary>
            /// Aerial imagery
            /// </summary>
            Aerial,

            /// <summary>
            /// Aerial imagery with road overlay.
            /// </summary>
            AerialWithLabels,

            /// <summary>
            /// Aerial imagery with on-demand road overlay.
            /// </summary>
            AerialWithLabelsOnDemand,

            /// <summary>
            /// Street-level imagery
            /// </summary>
            StreetSide,

            /// <summary>
            /// Bird's eye (oblique-angle) imagery
            /// </summary>
            BirdsEye,

            /// <summary>
            /// Bird's Eye (oblique-angle) imagery with raod overlay.
            /// </summary>
            BirdsEyeWIthLabels,

            /// <summary>
            /// Roads without additional imagery.
            /// </summary>
            Road,

            /// <summary>
            /// A dark version of the raod maps
            /// </summary>
            CanvasDark,

            /// <summary>
            /// A lighter version of the road maps which also has some of the details such as hill shading disabled.
            /// </summary>
            CanvasLight,

            /// <summary>
            /// A grayscale version of the road maps.
            /// </summary>
            CanvasGray
        }

        /// <summary>
        /// A display layer that renders on top of the imagery set.
        /// </summary>
        public enum MapLayer
        {
            /// <summary>
            /// Bulding footprints. This layer is only visible on Road map  imagery type.
            /// </summary>
            Basemap,

            /// <summary>
            /// Bulding footprints. This layer is only visible on Road map  imagery type.
            /// </summary>
            Buildings,

            /// <summary>
            /// Ordnance Survey imagery.  This layer is visible only in the UK.
            /// </summary>
            OrdnanceSurvey,

            /// <summary>
            /// Traffic flow layer
            /// </summary>
            TrafficFlow
        }

    }
}
