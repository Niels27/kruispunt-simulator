using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System;

/// <summary>
/// The hitbox of a cyclist object
/// </summary>
public class CyclistHitbox : MonoBehaviour
{
    private float speed;
    private float maxSpeed;

    /// Start is called before the first frame update
    void Start()
    {
        maxSpeed = GetComponentInParent<CyclistScript>().maxSpeed;
    }

 

    ///OnTriggerStay is called each frame where another object is within a trigger collider attached to this object
    void OnTriggerStay2D(Collider2D other)
    {
        ///While colliding with a pedestrian
        if (other.gameObject.tag == "pedestrian")
        {
            GetComponentInParent<CyclistScript>().speed = maxSpeed * 0.2f;
        }

    }

    ///OnTriggerExit is called once when another object leaves a trigger collider attached to this object
    void OnTriggerExit2D(Collider2D other)
    {
        ///When no longer colliding with pedestrian
        if (other.gameObject.tag == "pedestrian")
        {
            GetComponentInParent<CyclistScript>().speed = maxSpeed;
        }
    }
}
