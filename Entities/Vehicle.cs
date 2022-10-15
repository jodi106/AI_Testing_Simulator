using System;

public class Vehicle : BaseEntity
{
	public Vehicle(int id, Coord3D coord, VehicleOptions options, Path path)
	{
        Id = id;
        Coord = coord;
        Options = options;
        Path = path;
    }

    public VehicleOptions Options { get; set; }
    public Path Path { get; set; }
}
