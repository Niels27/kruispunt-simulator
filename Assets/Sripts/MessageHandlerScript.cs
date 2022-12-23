using WebSocketSharp;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
///  MessageHandler keeps track of incoming messages and sends the right messages back over the websocket
/// </summary>
[System.Serializable]
public class MessageHandlerScript : MonoBehaviour
{
    WebSocket ws;
    private TrafficLightController trafficLightController;
    private BridgeScript bridgeScript;
    private List<string> Messagelist = new List<string>();
    public static ConnectSimulator data;
    public string websocketURI = "ws:///keyslam.com:8080";
    public static bool connectBroker;
    public bool waitForBridgeRoadSensorEmpty;
    public bool waitForBridgeWaterSensorEmpty;
    public bool waitForBarriersState;
    public bool waitForBridgeState;


    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        ws = new WebSocket(websocketURI); ///Set the websocket URI
        bridgeScript = GameObject.Find("Bridge Controller").GetComponent<BridgeScript>();
        trafficLightController = GameObject.Find("Traffic Lights Controller").GetComponent<TrafficLightController>();

        ///Connect the simulator when opening the websocket
        ws.OnOpen += (sender, e) =>
        {
            Debug.Log("Connection established with " + ((WebSocket)sender).Url);
            ConnectSimulator();
        };

        ///Add incoming messages from the websocket to a list
        ws.OnMessage += (sender, e) =>
        {
            Debug.Log("Message received from " + ((WebSocket)sender).Url + ", Message data : " + e.Data);

            lock (Messagelist)
            {
                Messagelist.Add(e.Data);
            }
        };

        ///Log websocket errors
        ws.OnError += (sender, e) =>
        {
            Debug.Log(e.Message);
        };

        ///When the websocket is closed
        ws.OnClose += (sender, e) =>
        {
            if (connectBroker)
                Debug.Log("Connection has been closed " + e.Code + e.Reason);
        };

        ///Connect to the websocket
        if (connectBroker)
            ws.Connect();

    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        if (ws == null)
        {
            return;
        }
        ///Manually (dis)connect the websocket
        if (Input.GetKeyDown(KeyCode.C))
        {
            ws.Connect();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            ws.Close();
        }
        ///Process each message in the list of messages from the websocket and then remove the message
        lock (Messagelist)
        {
            if (Messagelist.Count != 0)
            {
                foreach (string msg in Messagelist.ToList())
                {
                    EventType e_message = JsonUtility.FromJson<EventType>(msg);
                    Debug.Log(msg);
                    ProcessMessage(e_message, msg);
                    Messagelist.Remove(msg);
                }
            }
        }

        ///Send acknowledge messages to the websocket if the controller requested conditions are met
        if (waitForBridgeRoadSensorEmpty && bridgeScript.aboveIsClear)
        {
            SendEventTypeInfo(EventTypes.ACKNOWLEDGE_BRIDGE_ROAD_EMPTY);
            waitForBridgeRoadSensorEmpty = false;
        }
        if (waitForBridgeWaterSensorEmpty && bridgeScript.underIsClear)
        {
            SendEventTypeInfo(EventTypes.ACKNOWLEDGE_BRIDGE_WATER_EMPTY);
            waitForBridgeWaterSensorEmpty = false;
        }
        if (waitForBarriersState && bridgeScript.barriersRequest == bridgeScript.barriersState)
        {
            string barriersState = bridgeScript.barriersState;
            SendStateInfo(barriersState, EventTypes.ACKNOWLEDGE_BARRIERS_STATE);
            waitForBarriersState = false;
        }

        if (waitForBridgeState && bridgeScript.bridgeRequest == bridgeScript.bridgeState)
        {
            string bridgeState = bridgeScript.bridgeState;
            SendStateInfo(bridgeState, EventTypes.ACKNOWLEDGE_BRIDGE_STATE);
            waitForBridgeState = false;
        }
       
    }

    /// <summary>
    /// ///Connect simulator to the websocket
    /// </summary>
    public void ConnectSimulator()
    {
        ConnectSimulator connectSimulator = new ConnectSimulator(data.sessionName, 1, false, false, false, false);
        EventType<ConnectSimulator> e_connectSimulator = new EventType<ConnectSimulator>("", connectSimulator);
        e_connectSimulator.eventType = EventTypes.CONNECT_SIMULATOR;
        e_connectSimulator.data = connectSimulator;
        string json = JsonUtility.ToJson(e_connectSimulator);
        ws.Send(json);
    }

    /// <summary>
    /// Process incoming websocket messages based on their eventtype and change simulator components according to controller instructions
    /// </summary>
    /// <param name="e"> EventType</param>
    /// <param name="msg">message</param>
    public void ProcessMessage(EventType e, string msg)
    {
        switch (e.eventType)
        {
            ///Change specific car traffic lights to a specified state
            case EventTypes.SET_AUTOMOBILE_ROUTE_STATE:
                EventType<RouteInfo> automobile_routeInfo = JsonUtility.FromJson<EventType<RouteInfo>>(msg);
                int automobile_routeId = automobile_routeInfo.data.routeId;
                string automobile_state = automobile_routeInfo.data.state;
                trafficLightController.ChangeSprite(automobile_routeId, automobile_state);
                break;
            ///Change specific pedestrian traffic lights to a specified state
            case EventTypes.SET_PEDESTRIAN_ROUTE_STATE:
                EventType<RouteInfo> pedestrian_routeInfo = JsonUtility.FromJson<EventType<RouteInfo>>(msg);
                string pedestrian_state = pedestrian_routeInfo.data.state;
                int pedestrian_routeId = pedestrian_routeInfo.data.routeId;
                trafficLightController.ChangeSprite(pedestrian_routeId, pedestrian_state);
                break;
            ///Change specific ccyclist traffic lights to a specified state
            case EventTypes.SET_CYCLIST_ROUTE_STATE:
                EventType<RouteInfo> cyclist_routeInfo = JsonUtility.FromJson<EventType<RouteInfo>>(msg);
                string cyclist_state = cyclist_routeInfo.data.state;
                int cyclist_routeId = cyclist_routeInfo.data.routeId;
                trafficLightController.ChangeSprite(cyclist_routeId, cyclist_state);
                break;
            ///Change specific boat traffic lights to a specified state
            case EventTypes.SET_BOAT_ROUTE_STATE:
                EventType<RouteInfo> boat_routeInfo = JsonUtility.FromJson<EventType<RouteInfo>>(msg);
                string boat_state = boat_routeInfo.data.state;
                int boat_routeId = boat_routeInfo.data.routeId;
                trafficLightController.ChangeSprite(boat_routeId, boat_state);
                break;
            ///Change all the warning lights to a specified state
            case EventTypes.SET_BRIDGE_WARNING_LIGHT_STATE:
                EventType<StateInfo> warningLightsInfo = JsonUtility.FromJson<EventType<StateInfo>>(msg);
                string warningLights_state = warningLightsInfo.data.state;
                bridgeScript.warningLightsState = warningLights_state;
                break;
            ///Queue the specified barriers state request 
            case EventTypes.REQUEST_BARRIERS_STATE:
                EventType<StateInfo> barriersState = JsonUtility.FromJson<EventType<StateInfo>>(msg);
                string barriers_state = barriersState.data.state;
                bridgeScript.barriersRequest = barriers_state;
                waitForBarriersState = true;
                break;
            ///Queue the specified bridge state request  
            case EventTypes.REQUEST_BRIDGE_STATE:
                EventType<StateInfo> bridgeState = JsonUtility.FromJson<EventType<StateInfo>>(msg);
                string bridge_state = bridgeState.data.state;
                bridgeScript.bridgeRequest = bridge_state;
                waitForBridgeState = true;
                break;
            ///Queue the specified bridge condition request
            case EventTypes.REQUEST_BRIDGE_ROAD_EMPTY:
                waitForBridgeRoadSensorEmpty = true;
                break;
            ///Queue the specified bridge condition request
            case EventTypes.REQUEST_BRIDGE_WATER_EMPTY:
                waitForBridgeWaterSensorEmpty = true;
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Send sensor information over the websocket when entities enter or exit sensor zones
    /// </summary>
    /// <param name="routeId">routeId</param>
    /// <param name="sensorid">sensorid</param>
    /// <param name="onSensor">onSensor</param>
    public void SendSensorInfo(int routeId, int sensorid, bool onSensor)
    {
        SensorInfo sensorInfo = new SensorInfo(routeId, sensorid);
        EventType<SensorInfo> e_sensorInfo = new EventType<SensorInfo>("", sensorInfo);
        if (onSensor)
            e_sensorInfo.eventType = EventTypes.ENTITY_ENTERED_ZONE;
        else
            e_sensorInfo.eventType = EventTypes.ENTITY_EXITED_ZONE;
        sensorInfo.routeId = routeId;
        sensorInfo.sensorId = sensorid;
        e_sensorInfo.data = sensorInfo;

        string json = JsonUtility.ToJson(e_sensorInfo);
        ws.Send(json);
    }

    /// <summary>
    /// Send route information over the websocket
    /// </summary>
    /// <param name="routeId">routeId</param>
    /// <param name="state">state</param>
    /// <param name="entityType">entityType</param>
    public void SendRouteInfo(int routeId, string state, string entityType)
    {
        RouteInfo routeInfo = new RouteInfo(routeId, state);
        EventType<RouteInfo> e_routeInfo = new EventType<RouteInfo>("", routeInfo);
        switch (entityType)
        {
            case "AUTOMOBILE":
                e_routeInfo.eventType = EventTypes.SET_AUTOMOBILE_ROUTE_STATE;
                break;
            case "CYCLIST":
                e_routeInfo.eventType = EventTypes.SET_CYCLIST_ROUTE_STATE;
                break;
            case "PEDESTRIAN":
                e_routeInfo.eventType = EventTypes.SET_PEDESTRIAN_ROUTE_STATE;
                break;
            case "BOAT":
                e_routeInfo.eventType = EventTypes.SET_BOAT_ROUTE_STATE;
                break;
        }
        routeInfo.routeId = routeId;
        routeInfo.state = state;
        e_routeInfo.data = routeInfo;

        string json = JsonUtility.ToJson(e_routeInfo);
        ws.Send(json);
    }

    /// <summary>
    /// Send state information over the websocket. (e.g bridge and barriers state)
    /// </summary>
    /// <param name="state">state</param>
    /// <param name="eventType">eventType</param>
    public void SendStateInfo(string state, string eventType)
    {
        StateInfo stateInfo = new StateInfo(state);
        EventType<StateInfo> e_stateInfo = new EventType<StateInfo>("", stateInfo);
        stateInfo.state = state;
        e_stateInfo.eventType = eventType;
        e_stateInfo.data = stateInfo;

        string json = JsonUtility.ToJson(e_stateInfo);
        ws.Send(json);
    }

    /// <summary>
    /// Send event type information over the websocket (e.g acknowledge messages)
    /// </summary>
    /// <param name="eventType">eventType</param>
    public void SendEventTypeInfo(string eventType)
    {
        EventType eventTypeInfo = new EventType();
        eventTypeInfo.eventType = eventType;

        string json = JsonUtility.ToJson(eventTypeInfo);
        ws.Send(json);
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled.
    /// </summary>
    private void OnDisable()
    {
        ///Close the websocket
        if (ws.IsAlive)
            ws.Close();
    }
    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    private void OnEnable()
    {


    }

}

/// <summary>
/// Contains the blueprint for an EventType
/// </summary>
[System.Serializable]
public class EventType
{
    public string eventType;
}
/// <summary>
/// Contains the blueprint for an EventType message
/// </summary>
/// <typeparam name="T"></typeparam>
[System.Serializable]
public class EventType<T>
{
    public string eventType;
    public T data;

    public EventType(string eventType, T data)
    {
        this.eventType = eventType;
        this.data = data;
    }
}
/// <summary>
/// Contains the blueprint for a route info message
/// </summary>
[System.Serializable]
public class RouteInfo
{
    public int routeId;
    public string state;
    public RouteInfo(int routeId, string state)
    {
        this.routeId = routeId;
        this.state = state;
    }
}
/// <summary>
/// Contains the blueprint for a sensor info message
/// </summary>
[System.Serializable]
public class SensorInfo
{
    public int routeId;
    public int sensorId;

    public SensorInfo(int routeId, int sensorId)
    {
        this.routeId = routeId;
        this.sensorId = sensorId;
    }
}
/// <summary>
/// Contains the blueprint for a state info message
/// </summary>
[System.Serializable]
public class StateInfo
{
    public string state;

    public StateInfo(string state)
    {

        this.state = state;
    }
}
/// <summary>
/// Contains blueprint for connect simulator message
/// </summary>
[System.Serializable]
public class ConnectSimulator
{
    public string sessionName;
    public int sessionVersion;
    public bool discardParseErrors;
    public bool discardEventTypeErrors;
    public bool discardMalformedDataErrors;
    public bool discardInvalidStateErrors;

    public ConnectSimulator(string sessionName, int sessionVersion, bool discardEventTypeErrors, bool discardInvalidStateErrors, bool discardMalformedDataErrors, bool discardParseErrors)
    {
        this.sessionName = sessionName;
        this.sessionVersion = sessionVersion;
        this.discardEventTypeErrors = discardEventTypeErrors;
        this.discardInvalidStateErrors = discardInvalidStateErrors;
        this.discardMalformedDataErrors = discardMalformedDataErrors;
        this.discardParseErrors = discardParseErrors;
    }
}
/// <summary>
/// All the possible EventTypes to recieve or send
/// </summary>
[System.Serializable]
public class EventTypes
{
    public const string CONNECT_SIMULATOR = "CONNECT_SIMULATOR";
    public const string SET_AUTOMOBILE_ROUTE_STATE = "SET_AUTOMOBILE_ROUTE_STATE";
    public const string SET_PEDESTRIAN_ROUTE_STATE = "SET_PEDESTRIAN_ROUTE_STATE";
    public const string SET_BOAT_ROUTE_STATE = "SET_BOAT_ROUTE_STATE";
    public const string SET_CYCLIST_ROUTE_STATE = "SET_CYCLIST_ROUTE_STATE";
    public const string ENTITY_ENTERED_ZONE = "ENTITY_ENTERED_ZONE";
    public const string ENTITY_EXITED_ZONE = "ENTITY_EXITED_ZONE";
    public const string ACKNOWLEDGE_BRIDGE_STATE = "ACKNOWLEDGE_BRIDGE_STATE";
    public const string ACKNOWLEDGE_BARRIERS_STATE = "ACKNOWLEDGE_BARRIERS_STATE";
    public const string ACKNOWLEDGE_BRIDGE_ROAD_EMPTY = "ACKNOWLEDGE_BRIDGE_ROAD_EMPTY";
    public const string ACKNOWLEDGE_BRIDGE_WATER_EMPTY = "ACKNOWLEDGE_BRIDGE_WATER_EMPTY";
    public const string REQUEST_BRIDGE_STATE = "REQUEST_BRIDGE_STATE";
    public const string REQUEST_BARRIERS_STATE = "REQUEST_BARRIERS_STATE";
    public const string REQUEST_BRIDGE_ROAD_EMPTY = "REQUEST_BRIDGE_ROAD_EMPTY";
    public const string REQUEST_BRIDGE_WATER_EMPTY = "REQUEST_BRIDGE_WATER_EMPTY";
    public const string SET_BRIDGE_WARNING_LIGHT_STATE = "SET_BRIDGE_WARNING_LIGHT_STATE";
}
