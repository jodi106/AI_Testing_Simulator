namespace Entities
{
    public enum VehicleCategory
    {
        car,
        bike,
        motorcycle
    }

    public class VehicleOptions
    {
        public VehicleOptions(string model, VehicleCategory category)
        {
            Model = model;
            Category = category;
        }

        public string Model { get; set; }
        public VehicleCategory Category { get; set; }
    }
}
