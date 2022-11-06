﻿namespace Models
{
    public class WorldOptions
    {
        public WorldOptions(float rainIntensity, float fogIntensity, float sunIntensity, string cloudState, string precipitationTypes, float precipitationIntensity)
        {
            RainIntersity = rainIntensity;
            FogIntensity = fogIntensity;
            SunIntensity = sunIntensity;
            CloudState = cloudState;
            PrecipitationTypes = precipitationTypes;
            PrecipitationIntensity = precipitationIntensity;
        }

        public float RainIntersity { get; set; }
        public float FogIntensity { get; set; }
        public float SunIntensity { get; set; }
        public string CloudState { get; set; } //create Enum possible values: cloudy, free, overcast, rainy
        public string PrecipitationTypes { get; set; } // possible values: dry, rain, snow
        public float PrecipitationIntensity { get; set; }
    }

}