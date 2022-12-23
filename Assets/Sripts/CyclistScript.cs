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
/// Contains properties of the cyclist traffic entity
/// </summary>
public class CyclistScript : TrafficEntity
{
    ///At scene start, all Awake calls happen before all Start calls
    public override void Awake()
    {
        base.Awake();
        maxSpeed = 35;
        TurningSmoothness = 70;
        roadUser = true;
        distanceFromTrafficLight = 150;
        sensors = GameObject.FindGameObjectsWithTag("cyclist sensor");
    }

    ///OnTriggerStay is called each frame where another object is within a trigger collider attached to this object
    public override void OnTriggerStay2D(Collider2D other)
    {
        base.OnTriggerStay2D(other);

        ///stand still if trying to cross roundabout but car is too close
        if (other.gameObject.tag == "roundabout zone")
        {
            speed = maxSpeed;

            foreach (GameObject car in GameObject.FindGameObjectsWithTag("car"))
            {
                float distance = Vector2.Distance(transform.position, car.transform.position);
                if (car.GetComponent<CarScript>().exitingOrEnteringRoundabout && distance < 150)
                {
                    speed = 0.1f;
                }
            }
        }
    }
 
  
}
