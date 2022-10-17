namespace Entities
{
    public class WorldOptions
    {
        public WorldOptions(float rainIntersity, float fogIntensity)
        {
            RainIntersity = rainIntersity;
            FogIntensity = fogIntensity;
        }

        public float RainIntersity { get; set; }
        public float FogIntensity { get; set; }
    }

}