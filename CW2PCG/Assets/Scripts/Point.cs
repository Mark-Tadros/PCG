//Stores values for each road it belongs to.
using UnityEngine;

public class Point
{
    public Vector2 Position;
    public Road Road;

    public Point() { }
    public Point(Vector2 position, Road road = null)
    {
        Position = new Vector2(position.x, position.y);
        Road = road;
    }
    public Vector3 GetVector3() { return new Vector3(Position.x, 0, Position.y); }
    public override bool Equals(object other) { return (Vector2.Distance((other as Point).Position, Position) < 0.01f); }
}