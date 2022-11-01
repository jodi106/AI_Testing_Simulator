using Models;
using System.Collections.Generic;
using System.Numerics;

namespace Assets.Repos
{
    public class MapRepository
    {
        private readonly List<Map> _maps;
        public MapRepository()
        {
            _maps = new List<Map>()
            {
                new Map(new Vector2((float)-2.06,(float)396.38), new Vector2((float)-2.04,(float)330.61), "Town01"),
                new Map(new Vector2((float)-7.46,(float)193.74), new Vector2((float)105.39,(float)306.56),"Town02"),
                new Map(new Vector2((float)-149.06,(float)248.99), new Vector2((float)-209.27,(float)207.63),"Town03"),
                new Map(new Vector2((float)-515.27,(float)414.14), new Vector2((float)-396.14,(float)436.0),"Town04"),
                new Map(new Vector2((float)-276.04,(float)211.06), new Vector2((float)-207.87,(float)209.06),"Town05"),
                new Map(new Vector2((float)-114.59,(float)109.97), new Vector2((float)-68.73,(float)141.21),"Town10HD")
            };
        }

        public List<Map> ListAllMaps()
        {
            return _maps;
        }


    }
}
