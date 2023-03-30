using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Helpers
{
    // <summary>
    // Color Class that is Serializable and partly compatible with UnityEngine.Color
    // </summary>    [Serializable]
    public class ColorSer
    {
        public float r;
        public float g;
        public float b;
        public float a;


        /// <summary>
        /// Initializes a new instance of the ColorSer class with the specified r, g, b, and a components.
        /// </summary>
        /// <param name="r">Red component of the color (0-1)</param>
        /// <param name="g">Green component of the color (0-1)</param>
        /// <param name="b">Blue component of the color (0-1)</param>
        /// <param name="a">Alpha component of the color (0-1)</param>
        public ColorSer(float r, float g, float b, float a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }
        
        /// <summary>
        /// Initializes a new instance of the ColorSer class with the specified r, g, and b components and default alpha (1).
        /// </summary>
        /// <param name="r">Red component of the color (0-1)</param>
        /// <param name="g">Green component of the color (0-1)</param>
        /// <param name="b">Blue component of the color (0-1)</param>          
        public ColorSer(float r, float g, float b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            a = 1f;
        }



        /// <summary>
        /// Initializes a new instance of the ColorSer class from the specified UnityEngine.Color object.
        /// </summary>
        /// <param name="color">The UnityEngine.Color object to create a new instance from.</param>
        public ColorSer(Color color)
        {
            this.r = color.r;
            this.g = color.g;
            this.b = color.b;
            this.a = color.a;
        }



        /// <summary>
        /// Converts the ColorSer object to a UnityEngine.Color object.
        /// </summary>
        /// <returns>The converted UnityEngine.Color object.</returns>
        public Color ToUnityColor()
        {
            return new Color(r, g, b, a);
        }
    }   
}
