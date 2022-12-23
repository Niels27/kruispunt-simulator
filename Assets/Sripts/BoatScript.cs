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
/// Contains properties of the boat traffic entity
/// </summary>
public class BoatScript : TrafficEntity
{

    ///At scene start, all Awake calls happen before all Start calls
    public override void Awake()
    {
        base.Awake();
        maxSpeed = 50;
        TurningSmoothness = 300;
        roadUser = false;
        distanceFromTrafficLight = 200;
        sensors = GameObject.FindGameObjectsWithTag("boat sensor");
    }


    ///OnTriggerEnter is called once when another object enters a trigger collider attached to this object
   public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        if (collision.gameObject.tag == "bridge zone")
        {
            BridgeScript.underIsClear = false;
            BridgeScript.boatCrossing = true;
        }
  
    }

    ///OnTriggerStay is called each frame where another object is within a trigger collider attached to this object
    public override void OnTriggerStay2D(Collider2D other)
    {
        base.OnTriggerStay2D(other);

        if (other.gameObject.tag == "boat")
        {
            if (number > other.GetComponent<BoatScript>().number)
            {
                float distance = Vector2.Distance(transform.position, other.transform.position) - 150;
                if (distance < 100)
                {
                    speed = distance - 50;
                }
                if (distance < 60)
                {
                    speed = 0;
                }
            }
        }
    }

    ///OnTriggerExit is called once when another object leaves a trigger collider attached to this object
    public override void OnTriggerExit2D(Collider2D other)
    {
        base.OnTriggerExit2D(other);

        if (other.gameObject.tag == "boat")
        {
            speed = maxSpeed;
        }
        if (other.gameObject.tag == "bridge zone")
        {
            BridgeScript.underIsClear = true;
            BridgeScript.boatCrossing = false;
        }
    }
 
}



