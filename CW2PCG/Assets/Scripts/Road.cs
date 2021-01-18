//Stores the Value for each specific road.
using UnityEngine;

public class Road
{
    public Point startPoint;
    public Point endPoint;

    public Road(Point startPosition, Point endPosition)
	{
		startPoint = new Point (startPosition.Position, this);
		endPoint = new Point (endPosition.Position, this);
	}
	public Point GetOther(Point main) { return startPoint.Equals(main) ? endPoint : startPoint; }
    public float Length() { return Vector2.Distance (startPoint.Position, endPoint.Position); }
	public override bool Equals(object other)
	{
        Road otherRoad = other as Road;
		return startPoint.Equals(otherRoad.startPoint) && endPoint.Equals(otherRoad.endPoint) || startPoint.Equals(otherRoad.endPoint) && endPoint.Equals(otherRoad.startPoint);
	}
}