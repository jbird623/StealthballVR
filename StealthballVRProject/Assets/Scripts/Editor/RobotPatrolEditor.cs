using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using JBirdEngine.EditorHelper;

[CustomEditor(typeof(RobotPatrol))]
public class RobotPatrolEditor : Editor {

    RobotPatrol targetPatrol;

    int routeIndex = -1;

    void OnSceneGUI () {
        targetPatrol = (RobotPatrol)target;

        for (int i = 0; i < targetPatrol.route.Count; ++i) {
            if (routeIndex == i) {
                this.UpdateListIndexViaHandle(ref targetPatrol.route, i, new PositionHandleDelegate(Handles.PositionHandle), targetPatrol.route[i], Quaternion.identity);
            }

            //Handles.DrawSphere();

            if (routeIndex == i) {
                Handles.color = Color.red;
            }
            else {
                Handles.color = Color.blue;
            }

            if (Handles.Button(targetPatrol.route[i], Quaternion.identity, 0.35f, 0.42f, Handles.SphereCap)) {
                routeIndex = i;
                Repaint();
            }

            Handles.color = Color.blue;
            Handles.DrawLine(targetPatrol.route[i], i < targetPatrol.route.Count - 1 ? targetPatrol.route[i+1] : targetPatrol.route[0]);
        }
    }

    public override void OnInspectorGUI () {
        base.OnInspectorGUI();

        targetPatrol = (RobotPatrol)target;

        routeIndex = EditorGUILayout.IntField(routeIndex);

        if (GUILayout.Button("Snap Patrol to Y-Plane")) {
            this.HandleCustomEditorMethod(LockRouteToYPlane);
        }
    }

    void LockRouteToYPlane () {
        for (int i = 0; i < targetPatrol.route.Count; ++i) {
            targetPatrol.route[i] = new Vector3(targetPatrol.route[i].x, targetPatrol.transform.position.y, targetPatrol.route[i].z);
        }
    }

}
