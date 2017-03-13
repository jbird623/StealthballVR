using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using JBirdEngine;
using JBirdEngine.EditorHelper;

[CustomEditor(typeof(RobotPatrol))]
public class RobotPatrolEditor : Editor {

    RobotPatrol targetPatrol;

    int routeIndex = -1;

    int nGonSides = 3;
    float nGonRadius = 1f;

    float xOffset = 0f;
    float zOffset = 0f;
    Vector3 offsetVector {
        get {
            return new Vector3(xOffset, 0f, zOffset);
        }
    }

    float thetaOffset {
        get {
            float tempValue = Vector2.Angle(Vector2.right, new Vector2(xOffset, zOffset));
            if (zOffset < 0) {
                tempValue = 360f - tempValue;
            }
            return tempValue;
        }
        set {
            float currentRadius = radiusOffset;
            xOffset = currentRadius * Mathf.Cos(value * Mathf.Deg2Rad);
            zOffset = currentRadius * Mathf.Sin(value * Mathf.Deg2Rad);
        }
    }
    float radiusOffset {
        get {
            return Mathf.Sqrt(xOffset * xOffset + zOffset * zOffset);
        }
        set {
            float currentTheta = thetaOffset;
            xOffset = value * Mathf.Cos(currentTheta * Mathf.Deg2Rad);
            zOffset = value * Mathf.Sin(currentTheta * Mathf.Deg2Rad);
        }
    }

    void OnSceneGUI () {
        targetPatrol = (RobotPatrol)target;

        for (int i = 0; i < targetPatrol.route.Count; ++i) {
            if (routeIndex == i) {
                this.UpdateListIndexViaHandle(ref targetPatrol.route, i, new PositionHandleDelegate(Handles.PositionHandle), targetPatrol.route[i], Quaternion.identity);
            }

            if (routeIndex == i) {
                if (xOffset != 0f || zOffset != 0f) {
                    Handles.color = new Color(0f, 0.75f, 0f, 0.25f);
                    Handles.SphereCap(0, targetPatrol.route[i] + new Vector3(xOffset, 0f, zOffset), Quaternion.identity, 0.35f);
                    Handles.ArrowCap(0, targetPatrol.route[i], Quaternion.LookRotation(offsetVector, Vector3.up), 1.5f);
                    Handles.DrawLine(targetPatrol.route[i], targetPatrol.route[i] + offsetVector);
                    Handles.ArrowCap(0, offsetVector + targetPatrol.route[i], Quaternion.LookRotation((i < targetPatrol.route.Count - 1 ? targetPatrol.route[i + 1] : targetPatrol.route[0]) - (offsetVector + targetPatrol.route[i]), Vector3.up), 1.5f);
                    Handles.DrawLine(targetPatrol.route[i] + offsetVector, i < targetPatrol.route.Count - 1 ? targetPatrol.route[i + 1] : targetPatrol.route[0]);
                }
                Handles.color = new Color(1f, 0f, 0f, 0.5f);
            }
            else {
                if (i == 0) {
                    Handles.color = new Color(0f, 1f, 1f, 0.5f);
                }
                else {
                    Handles.color = new Color(0f, 0f, 1f, 0.5f);
                }
            }

            if (Handles.Button(targetPatrol.route[i], Quaternion.identity, 0.35f, 0.42f, Handles.SphereCap)) {
                routeIndex = i;
                Repaint();
            }

            Handles.color = new Color(0f, 0f, 1f, 0.5f);
            Handles.DrawLine(targetPatrol.route[i], i < targetPatrol.route.Count - 1 ? targetPatrol.route[i+1] : targetPatrol.route[0]);
            Handles.ArrowCap(0, targetPatrol.route[i], Quaternion.LookRotation(((i < targetPatrol.route.Count - 1 ? targetPatrol.route[i + 1] : targetPatrol.route[0]) - targetPatrol.route[i]), Vector3.up), 1.5f);
        }

        // Draw search cone
        if (targetPatrol.visionEnabled) {
            Handles.color = new Color(1f, 1f, 0f, 0.5f);
            DrawViewCone();
        }

    }

    void DrawViewCone () {
        Vector3 eyePosition = targetPatrol.transform.position + targetPatrol.eyeOffset;
        float azimuth = targetPatrol.searchSpread.x;
        float elevation = targetPatrol.searchSpread.y;
        float viewDistance = targetPatrol.searchSpread.z;

        // Calculate Lines
        Vector3 fromTopRight = VectorHelper.FromAzimuthAndElevation(azimuth, elevation, targetPatrol.transform.up, targetPatrol.transform.forward);
        Vector3 fromTopLeft = VectorHelper.FromAzimuthAndElevation(-azimuth, elevation, targetPatrol.transform.up, targetPatrol.transform.forward);
        Vector3 fromBottomRight = VectorHelper.FromAzimuthAndElevation(azimuth, -elevation, targetPatrol.transform.up, targetPatrol.transform.forward);
        Vector3 fromBottomLeft = VectorHelper.FromAzimuthAndElevation(-azimuth, -elevation, targetPatrol.transform.up, targetPatrol.transform.forward);
        // Draw Lines
        Handles.DrawLine(eyePosition, eyePosition + fromTopRight * viewDistance);
        Handles.DrawLine(eyePosition, eyePosition + fromTopLeft * viewDistance);
        Handles.DrawLine(eyePosition, eyePosition + fromBottomRight * viewDistance);
        Handles.DrawLine(eyePosition, eyePosition + fromBottomLeft * viewDistance);
        // Calculate Normals
        Vector3 normalRight = VectorHelper.FromAzimuthAndElevation(azimuth + 90f, 0f, targetPatrol.transform.up, targetPatrol.transform.forward);
        Vector3 normalLeft = VectorHelper.FromAzimuthAndElevation(-azimuth - 90f, 0f, targetPatrol.transform.up, targetPatrol.transform.forward);
        Vector3 normalTop = Vector3.Cross(fromTopLeft, fromTopRight);
        Vector3 normalBottom = Vector3.Cross(fromBottomRight, fromBottomLeft);
        // Draw Arcs
        Handles.DrawWireArc(eyePosition, normalRight, fromTopRight, elevation * 2f, viewDistance);
        Handles.DrawWireArc(eyePosition, normalLeft, fromBottomLeft, elevation * 2f, viewDistance);
        Handles.DrawWireArc(eyePosition, normalTop, fromTopLeft, Vector3.Angle(fromTopLeft, fromTopRight), viewDistance);
        Handles.DrawWireArc(eyePosition, normalBottom, fromBottomRight, Vector3.Angle(fromBottomLeft, fromBottomRight), viewDistance);
    }

    public override void OnInspectorGUI () {
        base.OnInspectorGUI();

        targetPatrol = (RobotPatrol)target;

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Route Editor Options:", EditorStyles.boldLabel);

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Node Index: ");

        routeIndex = EditorGUILayout.IntField(routeIndex);

        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Duplicate Node at Index")) {
            this.HandleCustomEditorMethod(DuplicateNodeAtIndex);
        }

        if (GUILayout.Button("Remove Node at Index")) {
            this.HandleCustomEditorMethod(RemoveNodeAtIndex);
        }

        if (GUILayout.Button("Insert Node Before Index")) {
            this.HandleCustomEditorMethod(InsertNodeBeforeIndex);
        }

        if (GUILayout.Button("Insert Node After Index")) {
            this.HandleCustomEditorMethod(InsertNodeAfterIndex);
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Offset:");

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("X: ");
        xOffset = EditorGUILayout.DelayedFloatField(xOffset);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Z: ");
        zOffset = EditorGUILayout.DelayedFloatField(zOffset);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("\u03B8: ");
        thetaOffset = EditorGUILayout.DelayedFloatField(thetaOffset);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("R: ");
        radiusOffset = EditorGUILayout.DelayedFloatField(radiusOffset);
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Insert Node at Offset")) {
            this.HandleCustomEditorMethod(InsertNodeAtOffset);
        }

        if (GUILayout.Button("Move Node to Offset")) {
            this.HandleCustomEditorMethod(MoveNodeToOffset);
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Insert Node at Robot Position")) {
            this.HandleCustomEditorMethod(CreateNodeAtRobotPosition);
        }

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("N-gon Sides: ");
        nGonSides = EditorGUILayout.IntField(nGonSides);

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("N-gon Radius: ");
        nGonRadius = EditorGUILayout.FloatField(nGonRadius);

        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Create Regular N-gon Path")) {
            this.HandleCustomEditorMethod(CreateNgonPath);
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Other Actions: ", EditorStyles.boldLabel);

        EditorGUILayout.Space();

        if (GUILayout.Button("Snap Patrol Route to Y-Plane")) {
            this.HandleCustomEditorMethod(LockRouteToYPlane);
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Move Robot to First Position")) {
            this.HandleCustomEditorMethod(MoveRobotToFirstPosition);
        }

        if (GUILayout.Button("Test")) {
            Debug.LogFormat("(\u03B1: {0}, \u03B2: {1})", targetPatrol.targetTransform.position.GetAzimuth(targetPatrol.transform.position + targetPatrol.eyeOffset, targetPatrol.transform.up, targetPatrol.transform.forward), targetPatrol.targetTransform.position.GetElevation(targetPatrol.transform.position + targetPatrol.eyeOffset, targetPatrol.transform.up));
        }
    }

    void LockRouteToYPlane () {
        for (int i = 0; i < targetPatrol.route.Count; ++i) {
            targetPatrol.route[i] = new Vector3(targetPatrol.route[i].x, targetPatrol.transform.position.y, targetPatrol.route[i].z);
        }
    }

    void MoveRobotToFirstPosition () {
        if (targetPatrol.route.Count > 0) {
            targetPatrol.transform.position = targetPatrol.route[0];
        }
        if (targetPatrol.route.Count > 1) {
            targetPatrol.transform.rotation = Quaternion.LookRotation((targetPatrol.route[1] - targetPatrol.route[0]).normalized, Vector3.up);
        }
    }

    void DuplicateNodeAtIndex () {
        if (routeIndex < 0 || routeIndex >= targetPatrol.route.Count) {
            Debug.LogWarningFormat("RobotPatrolEditor: Invalid index '{0}'", routeIndex);
            return;
        }
        targetPatrol.route.Insert(routeIndex, targetPatrol.route[routeIndex]);
        ++routeIndex;
    }

    void RemoveNodeAtIndex () {
        if (routeIndex < 0 || routeIndex >= targetPatrol.route.Count) {
            Debug.LogWarningFormat("RobotPatrolEditor: Invalid index '{0}'", routeIndex);
            return;
        }
        targetPatrol.route.RemoveAt(routeIndex);
        --routeIndex;
    }

    void InsertNodeBeforeIndex () {
        if (routeIndex < 0 || routeIndex >= targetPatrol.route.Count) {
            Debug.LogWarningFormat("RobotPatrolEditor: Invalid index '{0}'", routeIndex);
            return;
        }
        targetPatrol.route.Insert(routeIndex, VectorHelper.Midpoint(targetPatrol.route[routeIndex], routeIndex == 0 ? targetPatrol.route[targetPatrol.route.Count - 1] : targetPatrol.route[routeIndex - 1]));
    }

    void InsertNodeAfterIndex () {
        if (routeIndex < 0 || routeIndex >= targetPatrol.route.Count) {
            Debug.LogWarningFormat("RobotPatrolEditor: Invalid index '{0}'", routeIndex);
            return;
        }
        targetPatrol.route.Insert(routeIndex + 1, VectorHelper.Midpoint(targetPatrol.route[routeIndex], routeIndex == targetPatrol.route.Count - 1 ? targetPatrol.route[0] : targetPatrol.route[routeIndex + 1]));
        ++routeIndex;
    }

    void CreateNodeAtRobotPosition () {
        if (routeIndex == -1) {
            routeIndex = targetPatrol.route.Count - 1;
        }
        if (routeIndex < 0 || routeIndex >= targetPatrol.route.Count) {
            Debug.LogWarningFormat("RobotPatrolEditor: Invalid index '{0}'", routeIndex);
            return;
        }
        targetPatrol.route.Insert(routeIndex + 1, targetPatrol.transform.position);
        ++routeIndex;
    }

    void CreateNgonPath () {
        Vector3 nGonCenter = targetPatrol.transform.position + targetPatrol.transform.right * nGonRadius;
        Vector3 radialVector = targetPatrol.transform.position - nGonCenter;
        float currentAngle = 0f;
        targetPatrol.route = new List<Vector3>();

        for (int i = 0; i < nGonSides; ++i) {
            targetPatrol.route.Add(nGonCenter + Quaternion.AngleAxis(currentAngle, Vector3.up) * radialVector);
            currentAngle += 360f / nGonSides;
        }
    }

    void InsertNodeAtOffset () {
        if (routeIndex < 0 || routeIndex >= targetPatrol.route.Count) {
            Debug.LogWarningFormat("RobotPatrolEditor: Invalid index '{0}'", routeIndex);
            return;
        }
        targetPatrol.route.Insert(routeIndex + 1, targetPatrol.route[routeIndex] + offsetVector);
        ++routeIndex;
    }

    void MoveNodeToOffset () {
        targetPatrol.route[routeIndex] += offsetVector;
    }

}
