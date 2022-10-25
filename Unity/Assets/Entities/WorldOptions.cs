namespace Entities
{
    public class WorldOptions
    {
        public WorldOptions(float rainIntensity, float fogIntensity)
        {
            RainIntersity = rainIntensity;
            FogIntensity = fogIntensity;
        }

        public float RainIntersity { get; set; }
        public float FogIntensity { get; set; }
    }

}