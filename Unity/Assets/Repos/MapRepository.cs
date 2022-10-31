using Assets.Enums;
using Dtos;
using System.Collections.Generic;

namespace Assets.Repos
{
    public class MapRepository
    {
        private readonly List<Map> _maps;
        public MapRepository()
        {
            _maps = new List<Map>()
            {
                new Map(new Coord(-2.059906005859375,396.3791809082031), new Coord(-2.0499608516693115,330.6099853515625), "Town01"),
                new Map(new Coord(-7.459729194641113,193.74075317382812), new Coord(105.3907470703125,306.55999755859375),"Town02"),
                new Map(new Coord(-149.0638427734375,248.99356079101562), new Coord(-209.27125549316406,207.6337432861328),"Town03"),
                new Map(new Coord(-515.26904296875,414.145751953125), new Coord(-396.1461181640625,436.00714111328125),"Town04"),
                new Map(new Coord(-276.0445251464844,211.0635528564453), new Coord(-207.87509155273438,209.06625366210938),"Town05"),
                new Map(new Coord(-114.59522247314453,109.97942352294922), new Coord(-68.72904205322266,141.21044921875),"Town10HD")
            };
        }

        public List<Map> ListAllMaps()
        {
            return _maps;
        }


    }
}
