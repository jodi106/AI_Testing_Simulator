using Assets.Enums;
using System;

namespace Entity
{ 
    public class WorldOptions : ICloneable
    /// <summary>Creates WorldOptions Object. Contains user specified world settings for the scenario</summary>
    {
        public WorldOptions()
        {
            Date_Time = "2023-01-10T12:00:00";
            FogVisualRange = 100000;
            SunIntensity = 0.8F;
            SunAzimuth = 1.5;
            CloudState = CloudState.Free;
            PrecipitationType = PrecipitationType.Dry;
            PrecipitationIntensity = 0;
            FrictionScaleFactor = 1;
            SunElevation = 1.3;
        }
        public WorldOptions(string dateTime, float fogVisualRange, float sunIntensity, double sunAzimuth, double sunElevation, CloudState cloudState, PrecipitationType precipitationType, float precipitationIntensity, double frictionScaleFactor, string date, float visibility, TimeDay timeDay)
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

            TimeDay = timeDay;
            Date = date;



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

        public float visibility { get; set; }

        public TimeDay TimeDay { get; set; }

        public string Date { get; set; }

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

            cloneWorldOptions.visibility = this.visibility;
            cloneWorldOptions.TimeDay = this.TimeDay;
            cloneWorldOptions.Date = this.Date;

            return cloneWorldOptions;
        }
    }

}
