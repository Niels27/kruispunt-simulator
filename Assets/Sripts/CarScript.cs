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

public class CarScript : TrafficEntity
{
    [Header("Car settings")]
    public float driftFactor = 1f;
    public float accelerationFactor = 50.0f;
    public float turnFactor = 2f;
    public float accelerationInput = 0;
    public float steeringInput = 0;
    public float rotationAngle = 0;
    public float velocityVsUp = 0;
    public bool controllable;

    public Vector2 inputVector = Vector2.zero;
    public Rigidbody2D carRigidbody2D;

    ///At scene start, all Awake calls happen before all Start calls
    public override void Awake()
    {
        base.Awake();

        maxSpeed = 80;
        if (controllable)
        {
            maxSpeed = 200;
        }
        TurningSmoothness = 70;
        roadUser = true;
        distanceFromTrafficLight = 130;
        sensors = GameObject.FindGameObjectsWithTag("car sensor");
        carRigidbody2D = GetComponent<Rigidbody2D>();
    }
    public override void Update()
    {
        if (controllable)
        {
            ApplySteering();
            ApplyEngineForce();
            KillOrthogonalVelocity();
        }
        else
        {
            base.Update();
        }

    }
    ///OnTriggerEnter is called once when another object enters a trigger collider attached to this object

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        ///Count the amount of ''accidents'' with cyclists, pedestrians, cars
        if (collision.gameObject.tag == "cyclist")
        {
            spawnScript.cyclistCollisions += 1;
            Debug.Log(gameObject.tag + " crashed with cyclist! routeIds " + routeId + " , " + collision.gameObject.GetComponent<CyclistScript>().routeId);
        }

        if (collision.gameObject.tag == "pedestrian")
        {
            spawnScript.pedestrianCollisions += 1;
            Debug.Log(gameObject.tag + "car crashed with pedestrian! routeIds " + routeId + " , " + collision.gameObject.GetComponent<PedestrianScript>().routeId);
        }

        if (collision.gameObject.tag == "car")
        {
            float distance = Vector2.Distance(transform.position, collision.gameObject.transform.position);
            if (distance < 30 && transform.rotation != collision.gameObject.transform.rotation)
            {
                spawnScript.carCollisions += 1;
                Debug.Log(gameObject.tag + "car crashed with car! routeIds " + routeId + " , " + collision.gameObject.GetComponent<CarScript>().routeId);
            }

        }
    }
    ///OnTriggerStay is called each frame where another object is within a trigger collider attached to this object
    public override void OnTriggerStay2D(Collider2D other)
    {
        base.OnTriggerStay2D(other);

        ///When entering the roundabout entrance, make sure its clear to enter, otherwise stand still
        if (other.gameObject.tag == "roundabout zone")
        {
            speed = maxSpeed;
            foreach (GameObject car in GameObject.FindGameObjectsWithTag("car"))
            {
                if (car.GetComponent<CarScript>().onRoundabout && car.GetComponent<CarScript>().routeId != routeId && car.transform.rotation != transform.rotation && car.GetComponent<CarScript>().speed > 1)
                {
                    float distance = Vector2.Distance(transform.position, car.transform.position);
                    if (50 <= distance && distance <= 250)
                    {
                        speed = 0;
                    }
                }
            }
            foreach (GameObject pedestrian in GameObject.FindGameObjectsWithTag("pedestrian"))
            {
                if (pedestrian.activeSelf && pedestrian.GetComponent<PedestrianScript>().exitingOrEnteringRoundabout)
                {
                    float distance = Vector2.Distance(transform.position, pedestrian.transform.position);
                    if (distance < 150)
                        speed = 0;
                }
            }
            foreach (GameObject cyclist in GameObject.FindGameObjectsWithTag("cyclist"))
            {
                if (cyclist.GetComponent<CyclistScript>().exitingOrEnteringRoundabout)
                {
                    float distance = Vector2.Distance(transform.position, cyclist.transform.position);
                    if (distance < 200)
                        speed = 0;
                }
            }
        }

    }
    ///OnTriggerExit is called once when another object leaves a trigger collider attached to this object
    public override void OnTriggerExit2D(Collider2D other)
    {
        base.OnTriggerExit2D(other);

    }
    public override void ReactToTrafficLight()
    {
        base.ReactToTrafficLight();
    }
    /// <summary>
    /// Engine calculations for player controlled car
    /// </summary>
    public void ApplyEngineForce()
    {
        if (accelerationInput == 0)
            carRigidbody2D.drag = Mathf.Lerp(carRigidbody2D.drag, 6.0f, Time.fixedDeltaTime * 15);
        else carRigidbody2D.drag = 0;

        velocityVsUp = Vector2.Dot(transform.up, carRigidbody2D.velocity);

        if (velocityVsUp > maxSpeed && accelerationInput > 0)
            return;

        if (velocityVsUp < -maxSpeed * 0.5f && accelerationInput < 0)
            return;

        if (carRigidbody2D.velocity.sqrMagnitude > maxSpeed * maxSpeed && accelerationInput > 0)
            return;

        Vector2 engineForceVector = transform.up * accelerationInput * accelerationFactor;

        carRigidbody2D.AddForce(engineForceVector, ForceMode2D.Force);
    }
    /// <summary>
    /// Steering calculations for player controlled car
    /// </summary>
    public void ApplySteering()
    {
        float minSpeedBeforeAllowTurningFactor = (carRigidbody2D.velocity.magnitude / 2);
        minSpeedBeforeAllowTurningFactor = Mathf.Clamp01(minSpeedBeforeAllowTurningFactor);

        rotationAngle -= steeringInput * turnFactor * minSpeedBeforeAllowTurningFactor;
        carRigidbody2D.MoveRotation(rotationAngle);
    }
    /// <summary>
    /// Drifting calculations for player controlled car
    /// </summary>
    public void KillOrthogonalVelocity()
    {
        Vector2 forwardVelocity = transform.up * Vector2.Dot(carRigidbody2D.velocity, transform.up);
        Vector2 rightVelocity = transform.right * Vector2.Dot(carRigidbody2D.velocity, transform.right);

        carRigidbody2D.velocity = forwardVelocity + rightVelocity * driftFactor;
    }
    /// <summary>
    /// Set the inputvector for x and y movement
    /// </summary>
    /// <param name="inputVector">inputVector</param>
    public void SetInputVector(Vector2 inputVector)
    {
        steeringInput = inputVector.x;
        accelerationInput = inputVector.y;
    }
    /// <summary>
    /// Get the velocity  magnitude
    /// </summary>
    /// <returns></returns>
    public float GetVelocityMagnitude()
    {
        return carRigidbody2D.velocity.magnitude;
    }
}
