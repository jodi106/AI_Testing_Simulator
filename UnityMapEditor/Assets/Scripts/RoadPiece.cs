using System;
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
        private bool isLocked = false;

        public float width; 
        public float height;
   
        private float rotation = 0; 


        // Start is called before the first frame update
        void Start()
        {
            id = idCounter++;
            RoadManager.Instance.AddRoadToList(this);
        }

        public int getID()
        {
            return this.id;
        }
        
        public void setIsLocked(bool value)
        {
            isLocked = value;
            RoadManager.Instance.colorRoadPiece(isLocked ? SelectionColor.lockedSelected : SelectionColor.selected);
        }

        public bool getIsLocked()
        {
            return isLocked; 
        }

        public float getRotation()
        {
            return rotation;
        }

        public void setRotation(float rotation)
        {
            this.rotation = rotation;
        }

        
    }

}