using System;

public class Coord
{
	public Coord(float x, float y)
	{
        X = x;
        Y = y;
    }

    public float X { get; set; }
    public float Y { get; set; }
}

public class Coord3D : Coord
{
    public Coord3D(float x, float y,float z, float rot)
    {
        X = x;
        Y = y;
        Z = z;
        Rot = rot;
    }

    public float Z  { get; set; }
    public float Rot { get; set; }
}
