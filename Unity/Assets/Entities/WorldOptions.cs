namespace Entities
{
    public class WorldOptions
    {
        public WorldOptions(float rainIntensity, float fogIntensity, float sunIntensity, string cloudState, string precipitation, float precipitationIntensity)
        {
            RainIntersity = rainIntensity;
            FogIntensity = fogIntensity;
            SunIntensity = sunIntensity;
            CloudState = cloudState;
            Precipitation = precipitation;
            PrecipitationIntensity = precipitationIntensity;
        }

        public float RainIntersity { get; set; }
        public float FogIntensity { get; set; }
        public float SunIntensity { get; set; }
        public string CloudState { get; set; }
        public string Precipitation { get; set; }
        public float PrecipitationIntensity { get; set; }
    }

}