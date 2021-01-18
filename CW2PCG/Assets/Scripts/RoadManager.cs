//Controls how each Road is created.
using UnityEngine;
using System.Collections.Generic;

public class RoadManager 
{
    public List<Road> Roads;
    public List<RoadCross> Intersections;
    public float Scale;

    //Resets all the values and initialises them.
    public RoadManager(float scale)
	{
		Roads = new List<Road>();
		Intersections = new List<RoadCross>();
        Scale = scale;
    }
    //Splits the Road into two.
	public void Split()
	{
		List<Road> temporaryRoads = new List<Road> (Roads);
		for (int i = 0; i < temporaryRoads.Count; i++){ SplitRoad (temporaryRoads[i]); }
	}
    //Determines where the Centre is to create the road as every road needs a start and end Position, then draws a road for every two points made.
	public void FindCentre(List<Vector3> intersections)
	{
        Point startPoint = new Point();
        Point endPoint = new Point();

        for (int i = 0; i < intersections.Count; i++)
		{
			if (i == 0 || i % 2 == 0)
			{
                startPoint.Position.x = intersections[i].x;
                startPoint.Position.y = intersections[i].z;
            } 
			else
			{
                endPoint.Position.x = intersections[i].x;
                endPoint.Position.y = intersections[i].z;
                Road newRoad = new Road(startPoint, endPoint);
                Roads.Add(newRoad);
                RoadCross newRoadCross = new RoadCross(new List<Point>() { newRoad.startPoint });
               	Intersections.Add(newRoadCross);
            }
        }
    }

    //Gets called when split, creating more roads based on the start and end point.
	void SplitRoad(Road road)
	{
		float splitPercentile = Random.Range (0.1f, 0.95f);

        Vector3 startPosition = road.startPoint.GetVector3();
		Vector3 endPosition = road.endPoint.GetVector3();
		float length = Vector3.Distance (startPosition, endPosition);
		length *= splitPercentile;

		Vector3 direction = (startPosition - endPosition).normalized;
		Vector3 splitPosition = endPosition + (direction * length);

		Vector3 perpendicularA = Vector3.Cross (startPosition - endPosition, Vector3.down).normalized;
		float newLength =  Random.Range(Random.Range(2f, 10f), Random.Range(40f, 50f));
		Vector3 splitPositionEnd = splitPosition + (perpendicularA * newLength);

		Road roadA = new Road (new Point(new Vector2 (splitPosition.x, splitPosition.z),null), 
		new Point(new Vector2 (splitPositionEnd.x, splitPositionEnd.z),null));
  	
		Vector3 perpendicularB = Vector3.Cross (startPosition - endPosition, Vector3.down).normalized * -1;
		Vector3 splitPositionEndOther = splitPosition + (perpendicularB * newLength);
		Road roadB = new Road (new Point (new Vector2 (splitPosition.x, splitPosition.z), null), 
		new Point (new Vector2 (splitPositionEndOther.x, splitPositionEndOther.z), null));

		bool validRoadA = false;
		bool validRoadB = false;

        //Determines where the split should be.
		if (!Intersecting(roadA, 7.5f)) 
		{
			Vector2 intersection = Vector3.zero;
			Road other = null;

			int count = Intersected(roadA, out intersection, out other, road);

			if (count <= 1)
			{
				Roads.RemoveAll(p => p.Equals(roadA));
				Roads.Add (roadA);
				validRoadA = true;
			}
			else if (count == 1)
			{
				Road[] segmentsA = Repair(other, new Point (intersection, other));
				Road[] segmentsB = Repair(roadA, new Point (intersection, roadA));
				
				List<Point> points = new List<Point>();

				if (segmentsA[0].Length() > 5.5f) points.Add(segmentsA [0].endPoint);
				else Roads.RemoveAll(p => p.Equals(segmentsA[0]));
				
				if (segmentsA[1].Length() > 5.5f) points.Add(segmentsA [1].endPoint);
				else Roads.RemoveAll(p => p.Equals(segmentsA[1]));
				
				if (segmentsB[0].Length() > 5.5f) points.Add(segmentsB [0].endPoint);
				else Roads.RemoveAll(p => p.Equals(segmentsB[0]));
				
				if (segmentsB[1].Length() > 5.5f) points.Add(segmentsB [1].endPoint);
				else Roads.RemoveAll(p => p.Equals(segmentsB[1]));

                RoadCross intersections = new RoadCross(points);
				Intersections.Add(intersections);
			}
		}

        //Checks if the road is intersecting with any other road, if they are, check any weird roads.
		if (!Intersecting(roadB, 7.5f)) 
		{
			Vector2 intersection = Vector3.zero;
			Road otherRoad = null;

			int roadCount = Intersected(roadB,out intersection,out otherRoad, road);

			if (roadCount <= 1)
			{
				Roads.RemoveAll(p => p.Equals(roadB));
				Roads.Add (roadB);
				validRoadB = true;
			}

			if (roadCount == 1)
			{
				Road[] segmentsA = Repair(otherRoad, new Point (intersection, otherRoad));
				Road[] segmentsB = Repair(roadB, new Point (intersection, roadB));

				List<Point> points = new List<Point>();
				if (segmentsA[0].Length() > 5.5f) points.Add(segmentsA [0].endPoint);
				else Roads.RemoveAll(p => p.Equals(segmentsA[0]));

				if (segmentsA[1].Length() > 5.5f) points.Add(segmentsA [1].endPoint);
				else Roads.RemoveAll(p => p.Equals(segmentsA[1]));

				if (segmentsB[0].Length() > 5.5f) points.Add(segmentsB [0].endPoint);
				else Roads.RemoveAll(p => p.Equals(segmentsB[0]));

				if (segmentsB[1].Length() > 5.5f) points.Add(segmentsB [1].endPoint);
				else Roads.RemoveAll(p => p.Equals(segmentsB[1]));

                RoadCross intersections = new RoadCross(points);
				Intersections.Add(intersections);
			}
		}

        //Goes through a list of all roads and checks the list through the main road value.
		if (validRoadA || validRoadB)
        {
			Road[] segments = Repair(road, new Point (new Vector2 (splitPosition.x, splitPosition.z), road));

			if (validRoadA && validRoadB)
            {
                RoadCross intersections = new RoadCross(new List<Point>{segments[0].endPoint, segments[1].endPoint,roadA.startPoint, roadB.startPoint});
				Intersections.Add(intersections);
			}
            else if (validRoadA)
            {
                RoadCross intersections = new RoadCross(new List<Point>{segments[0].endPoint, segments[1].endPoint, roadA.startPoint});
				Intersections.Add(intersections);
			}
            else if (validRoadB)
            {
                RoadCross intersections = new RoadCross(new List<Point>{segments[0].endPoint, segments[1].endPoint, roadB.startPoint});
				Intersections.Add(intersections);
			}
		}
	}
	float RoadDistance(Point Point, Road Road)
	{
		Vector2 roadCentrePosition = Road.endPoint.Position - Road.startPoint.Position;
		Vector2 pointCentrePosition = Point.Position - Road.startPoint.Position;
		
		float centrePositionOne = Vector2.Dot(pointCentrePosition, roadCentrePosition);
		if (centrePositionOne <= 0) return Vector2.Distance(Point.Position, Road.startPoint.Position);
		
		float centrePositionTwo = Vector2.Dot(roadCentrePosition, roadCentrePosition);
		if (centrePositionTwo <= centrePositionOne) return Vector2.Distance(Point.Position, Road.endPoint.Position);
		
		float centrePositionThree = centrePositionOne / centrePositionTwo;
		Vector2 finalPosition = Road.startPoint.Position + (roadCentrePosition * centrePositionThree);
		return Vector2.Distance(Point.Position, finalPosition);
	}
	int Intersected(Road segment, out Vector2 intersection, out Road other, Road skip)
	{
		intersection = Vector2.zero;
		other = null;

        int count = 0;
        Vector2 position = Vector2.zero;
		Vector2 positionUpdated = Vector3.zero;
		
		for (int i = 0; i< Roads.Count; i++)
        {
			Road roadsList = Roads[i];
			if (roadsList.Equals(skip)) continue;
			else if (Vector2.Distance(roadsList.startPoint.Position, segment.startPoint.Position) < 0.01f || Vector2.Distance (roadsList.endPoint.Position, segment.endPoint.Position) < 0.01f) continue;
			else if (Vector2.Distance(roadsList.startPoint.Position, segment.endPoint.Position) < 0.01f || Vector2.Distance (roadsList.endPoint.Position, segment.startPoint.Position) < 0.01f) continue;
			else if (DoubleIntersected(segment, roadsList, out positionUpdated, out position) != 0)
            {
				other = roadsList;
				intersection = new Vector2(positionUpdated.x, positionUpdated.y);
				count++;
			}
		}
		return count;
	}
	float Perpendicular(Vector2 xPosition, Vector2 zPosition){ return ((xPosition).x * (zPosition).y - (xPosition).y * (zPosition).x); }
    //Checks if two Roads are intersected within each other.
	int DoubleIntersected(Road firstRoad, Road secondRoad, out Vector2 firstPosition, out Vector2 lastPosition)
	{
		Vector2 firstPositionCentre = firstRoad.endPoint.Position - firstRoad.startPoint.Position;
		Vector2 secondPositionCentre = secondRoad.endPoint.Position - secondRoad.startPoint.Position;
		Vector2 thirdPositionCentre = firstRoad.startPoint.Position - secondRoad.startPoint.Position;
		float perpendicularDistance = Perpendicular(firstPositionCentre, secondPositionCentre);

		firstPosition = Vector2.zero;
		lastPosition = Vector2.zero;

		if (Mathf.Abs(perpendicularDistance) < 0.01f)
        {
			if (Perpendicular(firstPositionCentre, thirdPositionCentre) != 0 || Perpendicular(secondPositionCentre, thirdPositionCentre) != 0) return 0;
			
			float firstVectors = Vector2.Dot(firstPositionCentre, firstPositionCentre);
			float secondVectors = Vector2.Dot(secondPositionCentre, secondPositionCentre);

			if (firstVectors == 0 && secondVectors == 0)
            {
				if (firstRoad.startPoint.Position !=  secondRoad.startPoint.Position) return 0;
				firstPosition = secondRoad.startPoint.Position;                 
				return 1;
			}
			if (firstVectors == 0)
            {
				if (Inside(firstRoad.startPoint, secondRoad) == 0) return 0;
				firstPosition = firstRoad.startPoint.Position;
				return 1;
			}
			if (secondVectors == 0)
            {
				if (Inside(secondRoad.startPoint, firstRoad) == 0) return 0;
				firstPosition = secondRoad.startPoint.Position;
				return 1;
			}

            float positionDifference;
            float positionSubtraction;
			Vector2 centrePosition = firstRoad.endPoint.Position - secondRoad.startPoint.Position;

			if (secondPositionCentre.x != 0) { positionDifference = thirdPositionCentre.x / secondPositionCentre.x; positionSubtraction = centrePosition.x / secondPositionCentre.x; }
			else { positionDifference = thirdPositionCentre.y / secondPositionCentre.y; positionSubtraction = centrePosition.y / secondPositionCentre.y; }

			if (positionDifference > positionSubtraction) { float value = positionDifference; positionDifference = positionSubtraction; positionSubtraction = value; }
			if (positionDifference > 1 || positionSubtraction < 0) return 0;
            
            if (positionDifference < 0) positionDifference = 0;
            if (positionSubtraction > 1) positionSubtraction = 1;

            if (positionDifference == positionSubtraction)
            {  
				firstPosition = secondRoad.startPoint.Position + positionDifference * secondPositionCentre;
				return 1;
			}
			
			firstPosition = secondRoad.startPoint.Position + positionDifference * secondPositionCentre;
			lastPosition = secondRoad.startPoint.Position + positionSubtraction * secondPositionCentre;
			return 2;
		}
		
		float roadPosition = Perpendicular(secondPositionCentre, thirdPositionCentre) / perpendicularDistance;
		if (roadPosition < 0 || roadPosition > 1) return 0;

		float pointPosition = Perpendicular(firstPositionCentre, thirdPositionCentre) / perpendicularDistance;
		if (pointPosition < 0 || pointPosition > 1) return 0;
		
		firstPosition = firstRoad.startPoint.Position + roadPosition * firstPositionCentre;
		return 1;
	}
	int Inside(Point Point, Road Road)
	{
		if (Road.startPoint.Position.x != Road.endPoint.Position.x)
        {
			if (Road.startPoint.Position.x <= Point.Position.x && Point.Position.x <= Road.endPoint.Position.x) return 1;
			if (Road.startPoint.Position.x >= Point.Position.x && Point.Position.x >= Road.endPoint.Position.x) return 1;
		}
		else
        {
			if (Road.startPoint.Position.y <= Point.Position.y && Point.Position.y <= Road.endPoint.Position.y) return 1;
			if (Road.startPoint.Position.y >= Point.Position.y && Point.Position.y >= Road.endPoint.Position.y) return 1;
		}
		return 0;
	}
	bool Intersecting(Road segment, float max)
	{
		foreach (Road seg in Roads)
        {
			bool maxDistance = RoadDistance(seg.startPoint, segment) < max;
			bool minDistance = RoadDistance(seg.endPoint, segment) < max;

			if (maxDistance || minDistance) return true;
		}
		return false;
	}
	Road[] Repair(Road segment, Point splitPosition)
	{
		Roads.RemoveAll(p => p.Equals(segment));

		Road left = new Road (segment.startPoint, new Point(splitPosition.Position));
		Road right = new Road (segment.endPoint, new Point(splitPosition.Position));

		Roads.Add(left);
        Roads.Add(right);

		return new Road[] { left,right };
	}
}