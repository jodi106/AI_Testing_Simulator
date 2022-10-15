using System;

public class Pedestrian : BaseEntity
{
	public Pedestrian(int id, Coord3D coord, PedestrianOptions options, Path path)
	{
        Id = id;
        Coord = coord;
        Options = options;
        Path = path;
    }

    public PedestrianOptions Options { get; set; }
    public Path Path { get; set; }
}
