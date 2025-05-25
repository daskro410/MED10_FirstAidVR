using System.Collections;
using UnityEngine;

public class Route : MonoBehaviour
{
    public Transform[] points;

    private Vector3 gizmoPosition;

    private void OnDrawGizmos()
    {
        for (int i = 0; i < points.Length; i++)
        {
            gizmoPosition = points[i].position;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(gizmoPosition, 0.2f);
            if (i > 0)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(points[i - 1].position, gizmoPosition);
            }
        }
    }

    public Transform GetNextWaypoint(Transform currentWaypoint)
    {
        if (currentWaypoint == null || points.Length == 0)
        {
            return transform.GetChild(0); // Return the first waypoint if currentWaypoint is null or no waypoints exist
        }

        // If the ambulance has reached the last point in the route
        if (currentWaypoint.GetSiblingIndex() == points.Length - 1)
        {
            Debug.Log("Reached the end of the route.");

            

            return null; // Return null to indicate the end of the route
        }

        if (currentWaypoint.GetSiblingIndex() < points.Length - 1)
        {
            return transform.GetChild(currentWaypoint.GetSiblingIndex() + 1);
        }

        return null;
    }
}
