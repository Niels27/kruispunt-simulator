using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System;

/// <summary>
/// The hitbox of a car object
/// </summary>
public class CarHitbox : MonoBehaviour
{
    public int frontCarNumber;
    public float frontCarspeed;
    public float speed;
    private float maxSpeed;

    /// Start is called before the first frame update
    void Start()
    {
        maxSpeed = GetComponentInParent<CarScript>().maxSpeed;
    }

    /// Update is called once per frame
    void Update()
    {
        speed = GetComponentInParent<CarScript>().speed;
    }

    ///OnTriggerEnter is called once when another object enters a trigger collider attached to this object
    void OnTriggerEnter2D(Collider2D collision)
    {
        ///When colliding with another car
        if (collision.gameObject.tag == "car")
        {
            GetComponentInParent<CarScript>().behindGameObject = true;
            GetComponentInParent<CarScript>().behindGameObjectNr = collision.gameObject.GetComponent<CarScript>().number;
        }
    }

    ///OnTriggerStay is called each frame where another object is within a trigger collider attached to this object
    void OnTriggerStay2D(Collider2D other)
    {
        ///While colliding with another car
        if (other.gameObject.tag == "car")
        {
            frontCarspeed = other.gameObject.GetComponent<CarScript>().speed;
            frontCarNumber = other.gameObject.GetComponent<CarScript>().number;
            GetComponentInParent<CarScript>().speed = frontCarspeed * 0.5f;  ///slow down the other car based on this cars maxSpeed so that they never collide
        }

    }

    ///OnTriggerExit is called once when another object leaves a trigger collider attached to this object
    void OnTriggerExit2D(Collider2D other)
    {
        ///When no longer colliding with another car
        if (other.gameObject.tag == "car")
        {
            GetComponentInParent<CarScript>().behindGameObject = false;
            GetComponentInParent<CarScript>().speed = maxSpeed;
        }
    }
}
