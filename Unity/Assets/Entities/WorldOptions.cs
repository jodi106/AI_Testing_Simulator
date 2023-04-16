using Assets.Enums;
using System;

namespace Entity
{
    [Serializable]
    /// <summary>Creates WorldOptions Object. Contains user specified world settings for the scenario</summary>
    public class WorldOptions : ICloneable
    {
        /// <summary>
        /// Creates a new WorldOptions object with default values.
        /// </summary>        
        public WorldOptions()
        {
            Date_Time = "12:00:00";
            FogVisualRange = 100000;
            SunIntensity = 0.8F;
            SunAzimuth = 1.5;
            CloudState = CloudState.Free;
            PrecipitationType = PrecipitationType.Dry;
            PrecipitationIntensity = 0;
            FrictionScaleFactor = 1;
            SunElevation = 1.3;
        }

        /// <summary>
        /// Creates a new WorldOptions object with user-specified settings.
        /// </summary>
        /// <param name="dateTime">The date and time of the scenario.</param>
        /// <param name="fogVisualRange">The visual range of the fog.</param>
        /// <param name="sunIntensity">The intensity of the sun.</param>
        /// <param name="sunAzimuth">The azimuth of the sun.</param>
        /// <param name="sunElevation">The elevation of the sun.</param>
        /// <param name="cloudState">The state of the clouds.</param>
        /// <param name="precipitationType">The type of precipitation.</param>
        /// <param name="precipitationIntensity">The intensity of the precipitation.</param>
        /// <param name="frictionScaleFactor">The scale factor for friction.</param>        
        public WorldOptions(string dateTime, float fogVisualRange, float sunIntensity, double sunAzimuth, double sunElevation, CloudState cloudState, PrecipitationType precipitationType, float precipitationIntensity, double frictionScaleFactor)
        {
            Date_Time = dateTime;
            FogVisualRange = fogVisualRange;
            SunIntensity = sunIntensity;
            SunAzimuth = sunAzimuth;
            CloudState = cloudState;
            PrecipitationType = precipitationType;
            PrecipitationIntensity = precipitationIntensity;
            FrictionScaleFactor = frictionScaleFactor;
            SunElevation = sunElevation;
        }


        public float SunIntensity { get; set; } // double: 0.0 to 1.0; user can edit in GUI advanced options
        public CloudState CloudState { get; set; } // has enum; user can edit in basic options
        public PrecipitationType PrecipitationType { get; set; } // has enum; user can edit in basic options
        public float PrecipitationIntensity { get; set; } // double: 0.0 to 1.0; user can edit in GUI advanced options
        // new valus added
        public string Date_Time { get; set; } // must be in this format: 022-09-24T12:00:00 ; user can edit in basic options
        public double SunAzimuth { get; set; } // double: 0.0 to 2pi (6.28); 3.14 is sunset(dark); 1.5 is good value; user can edit in GUI advanced options
        public double SunElevation { get; set; } // double: -pi to pi (-3.14 to 3.14); user can edit in GUI advanced options
        public double FogVisualRange { get; set; } // good value: 100000 (should be used); double: 0.0 to infinitive; user can edit in GUI advanced options
        public double FrictionScaleFactor { get; set; } // good value: 1 (should be used); double: 0.0 to infinitive; user can edit in GUI advanced options


        /// <summary>
        /// Creates a deep copy of the WorldOptions object.
        /// </summary>
        /// <returns>A deep copy of the WorldOptions object.</returns>
        public object Clone()
        {
            WorldOptions cloneWorldOptions = new();
            cloneWorldOptions.SunIntensity = this.SunIntensity;
            cloneWorldOptions.CloudState = this.CloudState;
            cloneWorldOptions.PrecipitationType = this.PrecipitationType;
            cloneWorldOptions.PrecipitationIntensity = this.PrecipitationIntensity;
            cloneWorldOptions.Date_Time = string.Copy(this.Date_Time);
            cloneWorldOptions.SunAzimuth = this.SunAzimuth;
            cloneWorldOptions.SunElevation = this.SunElevation;
            cloneWorldOptions.FogVisualRange = this.FogVisualRange;
            cloneWorldOptions.FrictionScaleFactor = this.FrictionScaleFactor;

            return cloneWorldOptions;
        }
    }

}
