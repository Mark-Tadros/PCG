//Uses the Points and Roads classes to figure out where each point connects.
using System.Collections.Generic;

public struct RoadCross
{
	public List<Point> Points;
	public RoadCross(List<Point> points) { Points = points; }
    //Gets called when the Intersection gets connected with another.
	public bool Connected(RoadCross inter)
	{
		int count = 0;
		foreach (Point point in inter.Points) if (Points.Exists (f => f == point)) count++;

		if (count == Points.Count && count == inter.Points.Count) return true;
		else return false;
	}
}