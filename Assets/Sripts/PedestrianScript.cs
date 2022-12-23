using WebSocketSharp;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System;

/// <summary>
/// Contains properties of the pedestrian traffic entity
/// </summary>
public class PedestrianScript : TrafficEntity
{
    public bool onIslandZone;
    public int routeSwitch;

    ///At scene start, all Awake calls happen before all Start calls
    public override void Awake()
    {
        base.Awake();
        maxSpeed = 25;
        TurningSmoothness = 200;
        roadUser = true;
        distanceFromTrafficLight = 150;
        sensors = GameObject.FindGameObjectsWithTag("pedestrian sensor");
    }
    /// Start is called before the first frame update


    ///OnTriggerEnter is called once when another object enters a trigger collider attached to this object
    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        if (collision.gameObject.tag == "island zone")
        {
            onIslandZone = true;
            if (routeId == 31 || routeId == 33 || routeId == 35 || routeId == 37)
                routeSwitch = 1;
            else
                routeSwitch = -1;
        }
    }

    ///OnTriggerStay is called each frame where another object is within a trigger collider attached to this object
    public override void OnTriggerStay2D(Collider2D other)
    {
        base.OnTriggerStay2D(other);

        ///run away from cyclist
        if (other.gameObject.tag == "cyclist")
            speed = maxSpeed * 4;
        ///stand still if trying to cross roundabout but car is too close
        if (other.gameObject.tag == "roundabout zone")
        {
            speed = maxSpeed;

            foreach (GameObject car in GameObject.FindGameObjectsWithTag("car"))
            {
                float distance = Vector2.Distance(transform.position, car.transform.position);
                if (car.GetComponent<CarScript>().exitingOrEnteringRoundabout && distance < 100)
                {
                    speed = 0.1f;
                }
            }
        }
    }

    ///OnTriggerExit is called once when another object leaves a trigger collider attached to this object
    public override void OnTriggerExit2D(Collider2D other)
    {
        base.OnTriggerExit2D(other);
        if (other.gameObject.tag == "island zone")
        {
            onIslandZone = false;
        }
    }
   
}
