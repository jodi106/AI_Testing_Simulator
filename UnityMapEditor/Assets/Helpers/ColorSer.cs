using System;
using UnityEngine;

namespace Assets.Helpers
{
    //Color Class partly compatible with UnityEngine.Color, that is Serializable
    [Serializable]
    public class ColorSer
    {
        public float r;
        public float g;
        public float b;
        public float a;

        public ColorSer(float r, float g, float b, float a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }
        
        public ColorSer(float r, float g, float b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            a = 1f;
        }

        public ColorSer(Color color)
        {
            this.r = color.r;
            this.g = color.g;
            this.b = color.b;
            this.a = color.a;
        }

        public Color ToUnityColor()
        {
            return new Color(r, g, b, a);
        }
    }   
}
