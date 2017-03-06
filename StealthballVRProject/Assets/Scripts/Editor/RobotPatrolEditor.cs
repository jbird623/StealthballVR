using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RobotPatrol))]
public class RobotPatrolEditor : Editor {

    RobotPatrol targetPatrol;

    void OnSceneGUI () {
        targetPatrol = (RobotPatrol)target;

        for (int i = 0; i < targetPatrol.route.Count; ++i) {
            EditorGUI.BeginChangeCheck();
            Vector3 newPoint = Handles.PositionHandle(targetPatrol.route[i], Quaternion.identity);

            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(target, "PatrolRouteChange");
                targetPatrol.route[i] = newPoint;
            }

            //Handles.DrawSphere();

            Handles.color = Color.blue;
            Handles.DrawLine(targetPatrol.route[i], i < targetPatrol.route.Count - 1 ? targetPatrol.route[i+1] : targetPatrol.route[0]);
        }
    }

    public override void OnInspectorGUI () {
        base.OnInspectorGUI();

        targetPatrol = (RobotPatrol)target;


    }

}
