using System;
using System.Numerics;

namespace Models
{
    [Serializable]
    public class Map
    {
        public Map(Vector2 topLeft, Vector2 bottomRight, string url)
        {
            TopLeft = topLeft;
            BottomRight = bottomRight;
            Url = url;
        }

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