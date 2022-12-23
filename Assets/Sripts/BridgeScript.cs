using WebSocketSharp;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;

public class BridgeScript : MonoBehaviour
{
    public Sprite bridgeOpenSprite;
    public Sprite bridgeClosedSprite;
    public Sprite barrierOpenSprite;
    public Sprite barrierClosedSprite;
    public string bridgeState;
    public string barriersState;
    public bool boatCrossing;
    public bool aboveIsClear;
    public bool underIsClear;
    public string warningLightsState;
    public GameObject[] WarningLights;
    public GameObject bridge;
    public GameObject[] barriers;
    public Sprite[] spriteColors;
    public int bridgeCollisions;
    public string bridgeRequest;
    public string barriersRequest;
    private enum Light { off, on }

    /// Start is called before the first frame update
    void Start()
    {
        WarningLights = GameObject.FindGameObjectsWithTag("warning light");
        barriers = GameObject.FindGameObjectsWithTag("barrier");
        aboveIsClear = true;
        bridgeCollisions = 0;
        bridgeState = "DOWN";
        barriersState = "UP";
        warningLightsState = "OFF";
        bridge = GameObject.FindGameObjectWithTag("bridge");
        bridge.GetComponent<SpriteRenderer>().sortingLayerName = "background";
    }
    /// Update is called once per frame
    void Update()
    {
        ///Keep track of the amount of gameobjects on top of the bridge
        if (bridgeCollisions > 1)
            aboveIsClear = false;
        else
        {
            aboveIsClear = true;
        }

        ///Change warninglight sprite based on the state
        if (warningLightsState == "ON")
        {
            foreach (GameObject warningLight in WarningLights)
            {
                warningLight.GetComponent<SpriteRenderer>().sprite = spriteColors[(int)Light.on];
            }
        }
        else
        {
            foreach (GameObject warningLight in WarningLights)
            {
                warningLight.GetComponent<SpriteRenderer>().sprite = spriteColors[(int)Light.off];
            }
        }

        ///React to incoming barriers/bridge requests 
        if (barriersRequest == "DOWN" && bridgeState == "DOWN")
        {
            StartCoroutine(ChangeBarriersState(barriersRequest));
        }
        else if (barriersRequest == "UP" && bridgeState == "DOWN")
        {
            StartCoroutine(ChangeBarriersState(barriersRequest));
        }
        if (bridgeRequest == "UP" && barriersState == "DOWN" && bridgeState == "DOWN")
        {
            StartCoroutine(ChangeBridgeState(bridgeRequest));
        }
        else if (bridgeRequest == "DOWN" && barriersState == "DOWN" && bridgeState == "UP")
        {
            StartCoroutine(ChangeBridgeState(bridgeRequest));
        }

        ///Manually control the bridge and barriers
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (bridgeState == "DOWN")
            {
                warningLightsState = "ON";
                StartCoroutine(ChangeBarriersState("DOWN"));
                StartCoroutine(ChangeBridgeState("UP"));
            }
            else
            {
                StartCoroutine(ChangeBridgeState("DOWN"));
                StartCoroutine(ChangeBarriersState("UP"));
                warningLightsState = "OFF";
            }
        }
    }

    ///Raise or lower the bridge based on the controller request by changing its sprite, with added time delays for realism
    IEnumerator ChangeBridgeState(string request)
    {
        if (request == "UP")
        {
            yield return new WaitForSeconds(5f);
            bridgeState = "UP";


            bridge.GetComponent<SpriteRenderer>().sprite = bridgeOpenSprite;
        }
        else if (request == "DOWN")
        {

            bridge.GetComponent<SpriteRenderer>().sprite = bridgeClosedSprite;
            bridgeState = "DOWN";
            yield return null;

        }
    }

    ///Raise or lower the barriers based on the controller request by changing its sprite, with added time delays for realism
    IEnumerator ChangeBarriersState(string request)
    {
        if (request == "UP")
        {
            yield return new WaitForSeconds(4f);

            foreach (GameObject barrier in barriers)
            {
                barrier.GetComponent<SpriteRenderer>().sprite = barrierOpenSprite;
            }
            yield return new WaitForSeconds(1f);
            barriersState = "UP";

        }
        else if (request == "DOWN")
        {
            barriersState = "DOWN";
            yield return new WaitForSeconds(4f);

            foreach (GameObject barrier in barriers)
            {
                barrier.GetComponent<SpriteRenderer>().sprite = barrierClosedSprite;
            }
        }
    }

}
