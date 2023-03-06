using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Helpers
{

    //Vector3 Class partly compatible with UnityEngine.Vector3, that is Serializable
    [Serializable]
    public class Vector3Ser
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        
        public Vector3Ser(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(X, Y, Z);
        }
    }
    
}
