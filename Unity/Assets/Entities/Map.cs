using System;
using System.Numerics;

namespace Models
{
    [Serializable]
    public class Map
    {
        /// <summary>
        /// Initializes a new instance of the Map class with given parameters.
        /// </summary>
        /// <param name="topLeft">The top-left corner of the map.</param>
        /// <param name="bottomRight">The bottom-right corner of the map.</param>
        /// <param name="url">The URL of the map.</param>
        public Map(Vector2 topLeft, Vector2 bottomRight, string url)
        {
            TopLeft = topLeft;
            BottomRight = bottomRight;
            Url = url;
        }


        /// <summary>
        /// Initializes a new instance of the Map class with given parameters.
        /// </summary>
        /// <param name="topLeft">The top-left corner of the map.</param>
        /// <param name="bottomRight">The bottom-right corner of the map.</param>
        /// <param name="url">The URL of the map.</param>
        /// <param name="xConversionConstant">The conversion constant for X-axis.</param>
        /// <param name="yConversionConstant">The conversion constant for Y-axis.</param>
        public Map(Vector2 topLeft, Vector2 bottomRight, string url, double xConversionConstant, double yConversionConstant)
        {
            TopLeft = topLeft;
            BottomRight = bottomRight;
            Url = url;

            this.xConversionConstant = xConversionConstant;
            this.yConversionConstant = yConversionConstant;
        }

        public Vector2 TopLeft { get; set; }
        public Vector2 BottomRight { get; set; }
        public string Url { get; set; }

        public double xConversionConstant { get; set; }
        public double yConversionConstant { get; set; }

        public double xcenterOffset { get; set; }
        public double ycenterOffset { get; set; }
    }


    





}