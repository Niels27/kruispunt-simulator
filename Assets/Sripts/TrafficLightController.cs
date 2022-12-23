using WebSocketSharp;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Manage the sprites of all traffic light objects
/// </summary>
public class TrafficLightController : MonoBehaviour
{
    public enum Light { red, orange, green, greenred, on, off }
    public Sprite[] spriteColors;
    public GameObject[] TrafficLightObjects;

    /// Start is called before the first frame update
    void Start()
    {
        TrafficLightObjects = GameObject.FindGameObjectsWithTag("trafficlight");
    }

  
    /// <summary>
    /// Change the traffic light sprite based on their state
    /// </summary>
    /// <param name="routeId"></param>
    /// <param name="state"></param>
    public void ChangeSprite(int routeId, string state)
    {
        foreach (GameObject TrafficLightObject in TrafficLightObjects)
        {
            if (TrafficLightObject.GetComponent<TrafficLightScript>().routeId == routeId)
            {
                TrafficLightObject.GetComponent<TrafficLightScript>().state = state;
                switch (state)
                {
                    case "RED":
                        TrafficLightObject.GetComponent<SpriteRenderer>().sprite = spriteColors[(int)Light.red];
                        TrafficLightObject.GetComponent<TrafficLightScript>().state = state;
                        break;
                    case "ORANGE":
                        TrafficLightObject.GetComponent<SpriteRenderer>().sprite = spriteColors[(int)Light.orange];
                        TrafficLightObject.GetComponent<TrafficLightScript>().state = state;
                        break;
                    case "GREEN":
                        TrafficLightObject.GetComponent<SpriteRenderer>().sprite = spriteColors[(int)Light.green];
                        TrafficLightObject.GetComponent<TrafficLightScript>().state = state;
                        break;
                    case "GREENRED":
                    case "REDGREEN":
                        TrafficLightObject.GetComponent<SpriteRenderer>().sprite = spriteColors[(int)Light.greenred];
                        TrafficLightObject.GetComponent<TrafficLightScript>().state = state;
                        break;
                    case "BLINKING":
                        TrafficLightObject.GetComponent<TrafficLightScript>().state = state;
                        TrafficLightObject.GetComponent<TrafficLightScript>().blinking = true;
                        break;
                    case "ON":
                        TrafficLightObject.GetComponent<SpriteRenderer>().sprite = spriteColors[(int)Light.on];
                        TrafficLightObject.GetComponent<TrafficLightScript>().state = state;
                        break;
                    case "OFF":
                        TrafficLightObject.GetComponent<SpriteRenderer>().sprite = spriteColors[(int)Light.off];
                        TrafficLightObject.GetComponent<TrafficLightScript>().state = state;
                        break;
                }
            }
        }
    }

}

