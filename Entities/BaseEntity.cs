using System;

public class BaseEntity
{
	public BaseEntity(int id, Coord3D coord)
	{
        Id = id;
        Coord = coord;
    }

    public int Id { get; set; }
    public Coord3D Coord { get; set; }
}
