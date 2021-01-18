//Gets called through the Road script to check if they can be placed.
using System.Collections.Generic;
using UnityEngine;

public class RoadSetup : MonoBehaviour
{
    public float pointsDisplaySize = 0.1f;
    public bool disableEditor;

    public List<Vector3> points;

    public void Empty () { if (points == null) Reset (); }
    public void Reset () { points = new List<Vector3> (); }
    public void AddPoint (Vector3 p) { points.Add (p); }
}