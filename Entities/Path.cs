using System;

public class Path
{
	public Path(List<Event> path)
	{
        Path = path;
    }

    public List<Event> Path { get; }

	public Event getEventById(int id)
    {
        throw NotImplementedException;
    }
}
