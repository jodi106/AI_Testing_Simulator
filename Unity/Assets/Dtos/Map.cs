
namespace Dtos
{
    public class Map
    {
        public Map(Coord topLeft, Coord bottomRight,string url)
        {
            TopLeft = topLeft;
            BottomRight = bottomRight;
            Url = url;
        }

        public Coord TopLeft { get; set; }
        public Coord BottomRight { get; set; }
        public string Url { get; set; }
    }
}
