using System.Numerics;

namespace Models
{
    public class Map
    {
        public Map(Vector2 topLeft, Vector2 bottomRight,string url)
        {
            TopLeft = topLeft;
            BottomRight = bottomRight;
            Url = url;
        }

        public Vector2 TopLeft { get; set; }
        public Vector2 BottomRight { get; set; }
        public string Url { get; set; }
    }
}
