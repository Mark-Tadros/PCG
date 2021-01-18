using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (RoadSetup))]
public class RoadEditor : Editor {
    RoadSetup roadSetup;

    public override void OnInspectorGUI () {
        DrawDefaultInspector ();
        if (GUILayout.Button ("Reset")) {
            roadSetup.Reset ();
            SceneView.RepaintAll();
        }
    }

    void OnSceneGUI () {
        if (roadSetup.disableEditor) {
            return;
        }

        Event e = Event.current;

        // Mouse pos
        Ray mouseRay = HandleUtility.GUIPointToWorldRay (e.mousePosition);
        Vector3 mousePos = Vector3.zero;
        if (mouseRay.direction.y != 0) {
            float dstToXZPlane = Mathf.Abs (mouseRay.origin.y / mouseRay.direction.y);
            mousePos = mouseRay.GetPoint (dstToXZPlane);
        }

        if (e.type == EventType.MouseDown && e.button == 0 && e.modifiers != EventModifiers.Alt) {
            roadSetup.AddPoint (mousePos);
            HandleUtility.Repaint ();
        }

        if (e.type == EventType.Repaint) {
            Handles.color = Color.black;
            for (int i = 0; i < roadSetup.points.Count - 1; i += 2) {
                Handles.DrawLine (roadSetup.points[i], roadSetup.points[i + 1]);
            }

            // Draw points
            Handles.color = Color.white;
            for (int i = 0; i < roadSetup.points.Count; i++) {
                Handles.DrawSolidDisc (roadSetup.points[i], Vector3.up, roadSetup.pointsDisplaySize);
            }
        }

        // Don't allow clicking over empty space to deselect the object
        if (Event.current.type == EventType.Layout) {
            HandleUtility.AddDefaultControl (0);
        }
    }

    void OnEnable () {
        roadSetup = (RoadSetup) target;
        roadSetup.Empty();
    }
}