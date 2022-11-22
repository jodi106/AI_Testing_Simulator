namespace Entity
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

        public float SunIntensity { get; set; } // double: 0.0 to 1.0; user can edit in GUI advanced options
        public string CloudState { get; set; } // has enum; user can edit in basic options
        public string PrecipitationTypes { get; set; } // has enum; user can edit in basic options
        public float PrecipitationIntensity { get; set; } // double: 0.0 to 1.0; user can edit in GUI advanced options
        // new valus added
        public string Date_Time { get; set; } // must be in this format: 022-09-24T12:00:00 ; user can edit in basic options
        public double SunAzimuth { get; set; } // double: 0.0 to 2pi (6.28); 3.14 is sunset(dark); 1.5 is good value; user can edit in GUI advanced options
        public double SunElevation { get; set; } // double: -pi to pi (-3.14 to 3.14); user can edit in GUI advanced options
        public double FogVisualRange { get; set; } // good value: 100000 (should be used); double: 0.0 to infinitive; user can edit in GUI advanced options
        public double FrictionScaleFactor { get; set; } // good value: 1 (should be used); double: 0.0 to infinitive; user can edit in GUI advanced options

    }

}
