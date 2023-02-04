using System;
using System.Collections.Generic;
using System.Text;

namespace ExportScenario.Entities
{
    public class WorldOptions
    /// <summary>Creates WorldOptions Object. Contains user specified world settings for the scenario</summary>
    {
        public WorldOptions(string dateTime, float fogVisualRange, float sunIntensity, double sunAzimuth, double sunElevation, string cloudState, string precipitationTypes, float precipitationIntensity, double frictionScaleFactor)
        {
            Date_Time = dateTime;
            FogVisualRange = fogVisualRange;
            SunIntensity = sunIntensity;
            SunAzimuth = sunAzimuth;
            CloudState = cloudState;
            PrecipitationTypes = precipitationTypes;
            PrecipitationIntensity = precipitationIntensity;
            FrictionScaleFactor = frictionScaleFactor;
            SunElevation = sunElevation;
        }

        public float SunIntensity { get; set; } // user can edit in GUI advanced options
        public string CloudState { get; set; } // user can edit in basic options
        public string PrecipitationTypes { get; set; } // user can edit in basic options
        public float PrecipitationIntensity { get; set; } // user can edit in GUI advanced options
        // new valus added
        public string Date_Time { get; set; } // user can edit in basic options
        public double SunAzimuth { get; set; } // user can edit in GUI advanced options
        public double SunElevation { get; set; } // user can edit in GUI advanced options
        public double FogVisualRange { get; set; } // user can edit in GUI advanced options
        public double FrictionScaleFactor { get; set; } // user can edit in GUI advanced options

    }

}
