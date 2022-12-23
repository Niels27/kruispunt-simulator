using WebSocketSharp;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System;
using Random = UnityEngine.Random;

/// <summary>
/// Act as a blueprint for the different types of traffic users
/// </summary>
public class TrafficEntity : MonoBehaviour
{

    public MessageHandlerScript messageHandler;
    public BridgeScript BridgeScript;
    public TrafficLightController trafficLightController;
    public SpawnScript spawnScript;
    public List<GameObject> wayPointsToFollow;
    public GameObject[] sensors;

    public float maxSpeed;
    public float speed;
    public float TurningSmoothness;
    public int routeId;
    public int waypointIndex;
    public int number;
    public int waypointId;
    public int waypointRouteId;
    public int onSensorId;
    public int behindGameObjectNr;
    public float distanceFromTrafficLight;

    public bool onSensor;
    public bool onTrafficLightZone;
    public bool onRoundabout;
    public bool onBridgeWarningZone;
    public bool exitingOrEnteringRoundabout;
    public bool onRoundaboutWaitingZone;
    public bool onBridge;
    public bool bridgeCrossed;
    public bool behindGameObject;
    public bool fastMode;
    public bool reachedDestination;
    public bool hasSpawned;
    public bool roadUser;

    public Quaternion rotation;
    public Quaternion waypointRotation;

    ///At scene start, all Awake calls happen before all Start calls
    public virtual void Awake()
    {
        messageHandler = GameObject.Find("MessageHandler").GetComponent<MessageHandlerScript>();
        BridgeScript = GameObject.Find("Bridge Controller").GetComponent<BridgeScript>();
        trafficLightController = GameObject.Find("Traffic Lights Controller").GetComponent<TrafficLightController>();
        spawnScript = GameObject.Find("Spawner").GetComponent<SpawnScript>();
        rotation = transform.rotation;
    }
    /// Start is called before the first frame update
    public virtual void Start()
    {
        ///Teleport the gameobject to the first waypoint
        if (wayPointsToFollow.Count != 0)
        {
            transform.position = wayPointsToFollow[waypointIndex].transform.position;
            transform.rotation = wayPointsToFollow[waypointIndex].transform.rotation;
        }

        ///Set the  speed
        if (fastMode)
            speed = 1000;
        else
            speed = maxSpeed;
    }

    /// Update is called once per frame
    public virtual void Update()
    {
        if (wayPointsToFollow.Count != 0 && hasSpawned)
        {
            if (onTrafficLightZone)
                ReactToTrafficLight();
            StartCoroutine(FollowwayPoints());
        }
    }
    /// <summary>
    /// React to traffic lights by adjusting the speed when nearing them
    /// </summary>
    public virtual void ReactToTrafficLight()
    {
        foreach (GameObject trafficLightObject in trafficLightController.TrafficLightObjects)
        {
            if (trafficLightObject.GetComponent<TrafficLightScript>().routeId == routeId)
            {
                float distance = Vector2.Distance(trafficLightObject.transform.position, transform.position) - distanceFromTrafficLight;
                if (distance < 40) { distance = 0; }
                string state = trafficLightObject.GetComponent<TrafficLightScript>().state;
                switch (state)
                {
                    case "RED":
                        if (distance < speed)
                            speed = distance;
                        break;
                    case "ORANGE":
                        if (distance < 50)
                            speed = maxSpeed * 1.3f;
                        else
                            speed = maxSpeed * 0.5f;
                        break;
                    case "GREEN":
                        speed = maxSpeed;
                        break;
                    case "GREENRED":
                    case "REDGREEN":
                        speed = 0;
                        break;
                    case "BLINKING":
                        if (distance < speed)
                            speed = distance;
                        break;
                    case "ON":
                        break;
                    case "OFF":
                        break;
                }
            }
        }
    }
    /// <summary>
    /// Make the traffic user follow the wayPointsToFollow with the same Id, one by one
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator FollowwayPoints()
    {
        transform.position = Vector2.MoveTowards(transform.position, wayPointsToFollow[waypointIndex].transform.position, speed * Time.deltaTime);
        waypointRotation = wayPointsToFollow[waypointIndex].transform.rotation;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, waypointRotation, Time.deltaTime * TurningSmoothness);    ///Rotate the gameobject according to the waypoint rotation
        waypointId = waypointIndex;
        waypointRouteId = wayPointsToFollow[waypointIndex].GetComponent<RouteScript>().routeId;

        ///When a checkpoint is reached, set the goal for the next waypoint 
        float distance = Vector2.Distance(wayPointsToFollow[waypointIndex].transform.position, transform.position);
        if (transform.position == wayPointsToFollow[waypointIndex].transform.position || distance < 10)
        {
            waypointIndex += 1;
        }
        ///When reaching last checkpoint, start despawning
        if (waypointIndex == wayPointsToFollow.Count)
        {
            wayPointsToFollow.Clear();
            reachedDestination = true;
            SelfDestruct();

        }
        yield return null;
    }

    ///OnTriggerEnter is called once when another object enters a trigger collider attached to this object
    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        ///Stop the fastMode when entering the crossroad
        if (collision.gameObject.tag == "fast zone")
        {
            fastMode = false;
            speed = maxSpeed;
        }

        ///when colliding with a sensor, send the sensor information to the websocket
        if (collision.gameObject.tag.Contains("sensor"))
        {
            foreach (GameObject sensor in sensors)
            {
                if (collision.gameObject == sensor)
                {
                    if (!onSensor)
                    {
                        onSensorId = collision.gameObject.GetComponent<SensorScript>().sensorId;
                        if (MessageHandlerScript.connectBroker)
                            messageHandler.SendSensorInfo(routeId, sensor.GetComponent<SensorScript>().sensorId, true);

                        onSensor = true;
                    }
                }
            }
        }

        ///Keep track of entered road zones
        if (roadUser)
        {
            if (collision.gameObject.tag == "bridge zone")
            {
                BridgeScript.bridgeCollisions += 1;
                onBridge = true;
                bridgeCrossed = true;
            }
            if (collision.gameObject.tag == "bridge warning zone")
                onBridgeWarningZone = true;
            if (collision.gameObject.tag == "roundabout exit zone")
            {
                exitingOrEnteringRoundabout = true;
                onRoundaboutWaitingZone = false;
            }
            if (collision.gameObject.tag == "roundabout zone")
            {
                onRoundaboutWaitingZone = true;
            }

            if (collision.gameObject.tag == "roundabout")
            {
                onRoundabout = true;
            }

            if (collision.gameObject.tag == "bridge zone")
            {
                onBridge = true;
            }

        }

        if (collision.gameObject.tag == "trafficlight zone")
        {
            onTrafficLightZone = true;
        }

       

    }

    ///OnTriggerStay is called each frame where another object is within a trigger collider attached to this object
    public virtual void OnTriggerStay2D(Collider2D other)
    {
        if (roadUser)
        {
            ///React to the bridge state but not if on the bridge
            if (other.gameObject.tag == "bridge waiting zone" && !onBridge)
            {
                if ((BridgeScript.bridgeState == "UP" || BridgeScript.barriersState == "DOWN" || (BridgeScript.warningLightsState == "ON" && !onBridge)) && !bridgeCrossed)
                {
                    speed = 0;
                }
                else
                {
                    speed = maxSpeed;
                }
            }
            ///Slow down if the bridge warning lights are on and car is approaching the bridge
            if (other.gameObject.tag == "bridge warning zone" && BridgeScript.warningLightsState == "ON")
            {
                if (speed > maxSpeed / 2)
                    speed *= 0.5f;
            }
        }

    }
    ///OnTriggerExit is called once when another object leaves a trigger collider attached to this object
    public virtual void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "fast zone")
        {
            fastMode = true;
            speed = maxSpeed * 10;
        }
        if (other.gameObject.tag == gameObject.tag)
        {
            speed = maxSpeed;
        }
        if (other.gameObject.tag.Contains("sensor"))
        {
            onSensor = false;
            if (MessageHandlerScript.connectBroker)
                messageHandler.SendSensorInfo(routeId, onSensorId, false); ///Communicate to the websocket that gameobject left the sensor
        }
        if (other.gameObject.tag == "trafficlight zone")
        {
            onTrafficLightZone = false;
        }

        if (roadUser)
        {
            if (other.gameObject.tag == "bridge warning zone")
            {
                onBridgeWarningZone = false;
                speed = maxSpeed;
            }
            if (other.gameObject.tag == "roundabout")
                onRoundabout = false;
            if (other.gameObject.tag == "roundabout zone")
                onRoundaboutWaitingZone = false;
            if (other.gameObject.tag == "bridge zone")
            {
                onBridge = false;
                BridgeScript.bridgeCollisions -= 1;
            }
            if (other.gameObject.tag == "roundabout exit zone")
            {
                exitingOrEnteringRoundabout = false;
            }
        }
        else
        {
            if (other.gameObject.tag == "bridge zone")
            {
                BridgeScript.underIsClear = true;
                BridgeScript.boatCrossing = false;
            }
        }

    }
    ///When this gameobject is disabled
    public void OnDisable()
    {
        Destroy(gameObject);
    }
    /// <summary>
    /// Destroy this gameobject and its children
    /// </summary>
    public void SelfDestruct()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        Destroy(this.gameObject);
    }
}
