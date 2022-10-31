
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

        public Coord TopLeft { get; }
        public Coord BottomRight { get; }
        public string Url { get; }
    }
}
