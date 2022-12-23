using WebSocketSharp;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;


/// <summary>
/// Contains the properties of a traffic light
/// </summary>
public class TrafficLightScript : MonoBehaviour
{
    public int routeId;
    public string state;
    private TrafficLightController controller;
    private SpriteRenderer spriteRenderer;
    public bool blinking;
    private int times_blinked;
    private int blink_amount = 10;
    enum Light { red, orange, green, greenred, on, off }

    /// Start is called before the first frame update
    void Start()
    {
        state = "RED";
        spriteRenderer = GameObject.Find(this.gameObject.name).GetComponent<SpriteRenderer>();
        controller = GameObject.Find("Traffic Lights Controller").GetComponent<TrafficLightController>();
    }

    /// Update is called once per frame
    void Update()
    {

        GameObject.Find(this.gameObject.name).GetComponent<SpriteRenderer>().sprite = spriteRenderer.sprite;

        ///Make the trafficlights blink a certain amount
        if (blinking && times_blinked < blink_amount)
        {
            blinking = false;
            StartCoroutine(Blink());
        }

        ///Manually control the traffic lights
        if (Input.GetKeyDown(KeyCode.R))
        {
            state = "RED";
            spriteRenderer.sprite = controller.spriteColors[(int)Light.red];
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            state = "GREEN";
            spriteRenderer.sprite = controller.spriteColors[(int)Light.green];
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            state = "ORANGE";
            spriteRenderer.sprite = controller.spriteColors[(int)Light.orange];
        }
    }

    /// <summary>
    /// Make the light blink by repeatedly changing its sprite
    /// </summary>
    /// <returns></returns>
    IEnumerator Blink()
    {
        if (spriteRenderer.sprite == controller.spriteColors[(int)Light.off])
        {
            spriteRenderer.sprite = controller.spriteColors[(int)Light.red];
        }
        else
        {
            spriteRenderer.sprite = controller.spriteColors[(int)Light.off];
        }

        yield return new WaitForSeconds(0.5f);
        times_blinked++;
        blinking = true;
    }
}

