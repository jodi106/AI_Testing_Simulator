using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace scripts
{
    public class PrefabManager : MonoBehaviour
    {
        private static PrefabManager instance;
        public static PrefabManager Instance
        {
            get
            {
                return instance;
            }
        }

        public RoadPiece Straight;
        public RoadPiece Turn;

        // Start is called before the first frame update
        void Start()
        {
            instance = this;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public RoadPiece GetPieceOfType(RoadType roadType)
        {
            switch (roadType)
            {
                case RoadType.StraightRoad:
                    return Straight;
                case RoadType.Turn:
                    return Turn;
                default:
                    return null;
            }
        }
    }
}
