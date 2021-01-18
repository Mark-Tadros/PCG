//Controls and Generates the Roads and Buildings based on each other Value.
using System.Collections.Generic;
using UnityEngine;

public class PCG : MonoBehaviour
{
    public bool Drawing = false;
    [HideInInspector] public RoadManager roadManagerScript;
    Queue<Road> roadsQueue = new Queue<Road>();

    //Controls all of the Roads Settings.
    public Transform roadsParent;

    //Controls all of the Buildings Settings.
    public List<GameObject> Buildings;
    public List<GameObject> Trees;
    List<GameObject> buildingsList = new List<GameObject>();
    public Transform buildingsParent;

    List<Road> Roads;
    [HideInInspector] public List<Vector3> Points;
    public List<RoadCross> Intersections;
    List<RoadCross> intersectionsList = new List<RoadCross>();

    public Material roadMaterial;

    private void Awake()
	{
        roadManagerScript = new RoadManager(100f);
        //Resets the Roads and applies the Material.
        intersectionsList.Clear();
        AssignMaterials();
    }
    void AssignMaterials()
    {
        roadsParent.gameObject.GetComponent<MeshFilter>().mesh = new Mesh();
        roadsParent.gameObject.GetComponent<MeshRenderer>().material = roadMaterial;
    }
    //Creates a Queue and creates the roads one by one whilst dequeuing them.
    private void Update()
	{
		if(Drawing)
		{
            if (roadsQueue.Count > 0)
            {
                Road currentRoad = roadsQueue.Dequeue();
                CheckRoad(currentRoad);
            }
            else Drawing = false;
		}
	}
    public void Reset()
	{
        roadManagerScript = new RoadManager(100f);

        //Deletes the Buildings.
        Drawing = false;
        roadsQueue.Clear();
        foreach (Transform building in buildingsParent) { Destroy(building.gameObject); }
        buildingsList.Clear();

        //Deletes the Roads.
        intersectionsList.Clear();
        AssignMaterials();
        Points.Clear(); Roads.Clear(); Intersections.Clear();
    }
	public void AddPoint(Vector3 _point) { Points.Add(_point); CreateRoad(); }
    //Once all the points and values are set, create the new mesh road.
	public void CreateRoad()
	{
		if (Points.Count > 1) roadManagerScript.FindCentre(Points);
        for (int i = 0; i < roadManagerScript.Roads.Count; i++) AddRoads(roadManagerScript.Roads[i]);
        Roads = new List<Road>(roadManagerScript.Roads);
        Intersections = new List<RoadCross>(roadManagerScript.Intersections);
    }    
    //Takes each road, and calculates the left and right of it, before generating where to put the buildings.
	public void CheckRoad(Road road)
	{
        Vector2 direction = (road.endPoint.Position - road.startPoint.Position).normalized;
        float distance = Vector2.Distance(road.startPoint.Position, road.endPoint.Position);

        Vector2 current = road.startPoint.Position;
        bool side = true;
        for (float x = 1; x < distance || side; x += 1.5f)
        {
            if (x > distance && side) { side = false; x = 1; }

            Vector2 per = new Vector2(-direction.y, direction.x);
            if (side) per *= -1;

            for (int i = 0; i < 3; i++)
            {
                Vector2 roadOffset = per.normalized * (1  + 0.5f);
                Vector2 roadPosition = road.startPoint.Position + (direction * x) + roadOffset;

                if (x - 1.5f < 0 || x + 1.5f > distance) continue;

                Vector3 center = new Vector3(roadPosition.x, 0, roadPosition.y);

                GameObject Building = Instantiate(Buildings[Random.Range(0, Buildings.Count)], center, Quaternion.identity);
                Building.transform.parent = buildingsParent.transform;

                if (CheckValidPlacement(Building)) { buildingsList.Add(Building); break; }
                else Destroy(Building.gameObject);
            }
        }
	}
    //Uses perlin noise to place the building in a random way.
	public void AddBuildings()
	{
		Drawing = true;
        for (int i = 0; i < Roads.Count; i++) roadsQueue.Enqueue(Roads[i]);
    }
    //Prevents numerous buildings from being spawned inside each other.
    private bool CheckValidPlacement(GameObject building)
    {
        if (buildingsList.Count == 0) return true;
        foreach (GameObject other in buildingsList)
        {
            if (Vector3.Distance(building.transform.position, other.transform.position) > 25f) continue;
            else if (building.GetComponent<BoxCollider>().bounds.Intersects(other.GetComponent<BoxCollider>().bounds)) return false;
        }
		return true;
	}
    //This gets called as each new road gets created.
	void AddRoads(Road road) { AddRoadMesh(road); }
    //Math equation used to get the Offset.
	Vector3[] GetVerticeOffset(Point main, Point other)
	{
		Vector3[] result = new Vector3[2];

		Vector3 startPosition = new Vector3 (main.Position.x, 0, main.Position.y);
		Vector3 endPosition = new Vector3 (other.Position.x, 0, other.Position.y);
		
		Vector3 vector = (startPosition - endPosition).normalized;
        startPosition -= vector * 0.5f;
        endPosition += vector * 0.5f;
		
		Vector3 centrePosition = Vector3.Cross(startPosition - endPosition, Vector3.down).normalized;
		
		result[0] = startPosition + centrePosition * (0.5f);
		result[1] = endPosition - centrePosition * (0.5f);

		return result;
	}
	void AddRoadMesh(Road segment)
	{
        Mesh mesh = roadsParent.gameObject.GetComponent<MeshFilter>().mesh;

        List<int> triangles = mesh.vertexCount == 0 ? new List<int>() : new List<int> (mesh.triangles);

        List<Vector3> vertices = new List<Vector3>(mesh.vertices);
		List<Vector3> normals = new List<Vector3>(mesh.normals);
		List<Vector2> uvs = new List<Vector2>(mesh.uv);

		int lastNumber = vertices.Count;

		Vector3 startPosition = new Vector3 (segment.startPoint.Position.x, 0, segment.startPoint.Position.y);
		Vector3 endPosition = new Vector3 (segment.endPoint.Position.x, 0, segment.endPoint.Position.y);

		Vector3 segmentVector = (startPosition - endPosition).normalized;
        startPosition -= segmentVector * 0.5f;
        endPosition += segmentVector * 0.5f;

		Vector3 centrePosition = Vector3.Cross(startPosition - endPosition, Vector3.down).normalized;

		Vector3 verticesTopLeft = startPosition + centrePosition * (0.5f);
		Vector3 verticesTopRight = startPosition - centrePosition * (0.5f);
		Vector3 verticesBottomLeft = endPosition + centrePosition * (0.5f);
		Vector3 verticesBottomRight = endPosition - centrePosition * (0.5f);

		vertices.AddRange (new Vector3[]{ verticesTopLeft, verticesTopRight, verticesBottomLeft, verticesBottomRight });

		triangles.AddRange(new int[]{ lastNumber, lastNumber + 2, lastNumber + 1});
		triangles.AddRange(new int[]{ lastNumber + 1, lastNumber + 2, lastNumber + 3});

		normals.AddRange (new Vector3[]{ Vector3.up, Vector3.up, Vector3.up, Vector3.up});

		float length = Vector3.Distance (startPosition, endPosition) * 0.5f;
		uvs.AddRange(new Vector2[]{ new Vector2 (0, length), new Vector2 (1, length), new Vector2 (0, 0), new Vector2 (1, 0) });

		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.uv = uvs.ToArray();

		mesh.RecalculateNormals();
	}
}

public class CompareRoads : IComparer<Vector3>
{
	public Vector3 center { get; set; }
	public CompareRoads(Vector3 center){ this.center = center; }
	public int Compare(Vector3 startPosition, Vector3 endPosition)
	{
		float newStartPosition = Mathf.Atan2 (startPosition.x - center.x, startPosition.z - center.z) * Mathf.Rad2Deg;
		float newEndPosition = Mathf.Atan2 (endPosition.x - center.x, endPosition.z - center.z) * Mathf.Rad2Deg;

        newStartPosition += newStartPosition;
        newEndPosition += newEndPosition;

        if (newStartPosition < 0) newStartPosition = 360;
        else newStartPosition = 0;

        if (newEndPosition < 0) newEndPosition = 360;
        else newEndPosition = 0;

        if (newStartPosition > newEndPosition) return -1;
        else return 1;
	}
}