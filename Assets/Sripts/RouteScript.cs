using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains the properties of a route
/// </summary>
public class RouteScript : MonoBehaviour
{
    public int routeId;
    public int waypointId;
    public int pathAmount;
    public int pathId;
    public int wayPointAmount;

    public bool lastWaypoint;
    public bool splitPoint;
    public bool multiplePaths;

    private List<int> routesWithTwoPaths = new List<int> { 2, 4, 5, 8 };

    /// Start is called before the first frame update
    void Start()

    {
        ///count the amount of wayPoints with this routeid and pathid
        foreach (GameObject waypoint in GameObject.FindGameObjectsWithTag("waypoint"))
        {
            if (waypoint.GetComponent<RouteScript>().routeId == routeId)
            {
                if (waypoint.GetComponent<RouteScript>().pathId == pathId)
                    wayPointAmount += 1;
            }
        }
        ///routes with 2 paths
        if (routesWithTwoPaths.Contains(routeId))
        {
            multiplePaths = true;
            pathAmount = 2;
        }
    }

  
}
