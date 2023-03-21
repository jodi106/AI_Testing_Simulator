using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace scripts
{
    public class RoadPiece : MonoBehaviour
    {

        private static int idCounter = 0;
        private int id;
        public List<RoadPiece> neighbors = new List<RoadPiece>();
        public RoadType roadType;


        // Start is called before the first frame update
        void Start()
        {
            id = idCounter++;
            RoadManager.Instance.AddRoadToList(this);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public int getID()
        {
            return this.id;
        }


    }

}