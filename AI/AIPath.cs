//The class does not contain a full implementation of components, just an example
//Implementing the unity editor extension


#if UNITY_EDITOR
[CustomEditor(typeof(AIPath))]
public class PathCreator : Editor
{
    AIPath AIPath;

    bool LeftControlClick = false;

    private protected void OnEnable()
    {
        AIPath = (AIPath)target;
    }

    private protected Vector3 GetMousePositionOnMap()
    {
        Vector3 screenPosition = Event.current.mousePosition;
        screenPosition.y = Camera.current.pixelHeight - screenPosition.y;
        Ray ray = Camera.current.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
            return hit.point;

        return Vector3.zero;
    }

    private protected void OnSceneGUI()
    {
        if (!LeftControlClick)
            AIPath.MousePositionOnMap = GetMousePositionOnMap();

        int controlId = GUIUtility.GetControlID(FocusType.Passive);

        CheckingKeystrokes(controlId);
        СreatingPoint(controlId);
    }

    private protected void CheckingKeystrokes(int controlId)
    {
        if (Event.current.GetTypeForControl(controlId) == EventType.KeyUp && Event.current.keyCode == KeyCode.LeftControl)
        {
            LeftControlClick = false;
            GUIUtility.hotControl = 0;
            Event.current.Use();
        }
    }

    private protected void СreatingPoint(int controlId)
    {
        if (Event.current.GetTypeForControl(controlId) == EventType.KeyDown && Event.current.keyCode == KeyCode.LeftControl)
        {
            GUIUtility.hotControl = controlId;

            if (!LeftControlClick)
            {
                AIPathPoint point = Instantiate(AIPath.PathPointPrefab);
                point.name = $"Path Point {AIPath.PathPoints.Count.ToString()}";
                point.transform.position = GetMousePositionOnMap();
                point.transform.SetParent(AIPath.transform);
                point.DirectionMovementOnTheWay.DirectionMovement = AIPath.DirectionMovementOnTheWay.DirectionMovement;
                point.DirectionMovementOnTheWay.TrafficLane = AIPath.DirectionMovementOnTheWay.TrafficLane;
                AIPath.PathPoints.Add(point);

                LeftControlClick = true;
                Event.current.Use();
            }
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        DeletePoint();
    }

    private protected void DeletePoint()
    {
        if (GUILayout.Button("Delete Point") && AIPath.PathPoints.Count >= 1)
        {
            DestroyImmediate(AIPath.PathPoints.Last().gameObject);
            AIPath.PathPoints.Remove(AIPath.PathPoints.Last());
        }
    }
}
#endif
