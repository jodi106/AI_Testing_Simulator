using System;

public class Ego : BaseEntity
{
	public Ego(VehicleOptions options)
	{
        Options = options;
    }

    public VehicleOptions Options { get; set; }
}
