using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Process user input in start menu and set values accordingly
/// </summary>
public class MenuScript : MonoBehaviour
{
    public string sessionName;
    public int sessionVersion;
    public ConnectSimulator data;

    public bool connectBroker;
    public bool hideSensors;
    public bool fastMode;
    public bool infiniteSpawn;
    public bool spawnCars;
    public bool spawnCyclists;
    public bool spawnPedestrians;
    public bool spawnBoats;
    public bool spawnBusses;

    public int carAmount;
    public int cyclistAmount;
    public int pedestrianAmount;
    public int boatAmount;
    public int busAmount;

    public float carSpawnDelay;
    public float cyclistSpawnDelay;
    public float pedestrianSpawnDelay;
    public float boatSpawnDelay;
    public float busSpawnDelay;


    /// Update is called once per frame
    void Update()
    {
        ///get variables from user input fields

        sessionName = GameObject.Find("session name").GetComponent<TMP_InputField>().text;
        sessionVersion = int.Parse(GameObject.Find("session version").GetComponent<TMP_InputField>().text);
        data = new ConnectSimulator(sessionName, sessionVersion, false, false, false, false);

        connectBroker = GameObject.Find("connect broker").GetComponent<Toggle>().isOn;
        hideSensors = GameObject.Find("hide sensors").GetComponent<Toggle>().isOn;
        fastMode = GameObject.Find("speed mode").GetComponent<Toggle>().isOn;
        infiniteSpawn = GameObject.Find("infinite spawn").GetComponent<Toggle>().isOn;
        spawnCars = GameObject.Find("spawn cars").GetComponent<Toggle>().isOn;
        spawnCyclists = GameObject.Find("spawn cyclists").GetComponent<Toggle>().isOn;
        spawnPedestrians = GameObject.Find("spawn pedestrians").GetComponent<Toggle>().isOn;
        spawnBoats = GameObject.Find("spawn boats").GetComponent<Toggle>().isOn;
        spawnBusses = GameObject.Find("spawn busses").GetComponent<Toggle>().isOn;

        carAmount = int.Parse(GameObject.Find("car amount").GetComponent<TMP_InputField>().text);
        pedestrianAmount = int.Parse(GameObject.Find("pedestrian amount").GetComponent<TMP_InputField>().text);
        cyclistAmount = int.Parse(GameObject.Find("cyclist amount").GetComponent<TMP_InputField>().text);
        boatAmount = int.Parse(GameObject.Find("boat amount").GetComponent<TMP_InputField>().text);
        busAmount = int.Parse(GameObject.Find("bus amount").GetComponent<TMP_InputField>().text);

        carSpawnDelay = float.Parse(GameObject.Find("car delay").GetComponent<TMP_InputField>().text);
        cyclistSpawnDelay = float.Parse(GameObject.Find("cyclist delay").GetComponent<TMP_InputField>().text);
        pedestrianSpawnDelay = float.Parse(GameObject.Find("pedestrian delay").GetComponent<TMP_InputField>().text);
        boatSpawnDelay = float.Parse(GameObject.Find("boat delay").GetComponent<TMP_InputField>().text);
        busSpawnDelay = boatSpawnDelay = float.Parse(GameObject.Find("bus delay").GetComponent<TMP_InputField>().text);
    }
}
