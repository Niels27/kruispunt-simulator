using UnityEngine;
using System.Runtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

/// <summary>
/// Spawns traffic entities and give them the right attributes
/// </summary>
public class SpawnScript : MonoBehaviour
{
    public static int carAmount = 60;
    public static float carSpawnDelay = 2.0f;
    public bool carSpwnRndDelay;
    public float carSpwnRndDelayMin = 1;
    public float carSpwnRndDelayMax = 2;
    public static int cyclistAmount = 30;
    public static float cyclistSpawnDelay = 5.0f;
    public static int pedestrianAmount = 20;
    public static float pedestrianSpawnDelay = 8f;
    public static int boatAmount = 7;
    public static int busAmount = 5;
    public static float boatSpawnDelay = 35;
    public static float busSpawnDelay = 45;

    public static bool spawnCars;
    public static bool spawnCyclists;
    public static bool spawnPedestrians;
    public static bool spawnBoats;
    public static bool spawnBusses;
    public static bool infiniteSpawn;
    public static bool fastMode;
    public static bool hideSensors;

    public int carCollisions;
    public int cyclistCollisions;
    public int pedestrianCollisions;

    private int[] carRouteIds = { 1, 2, 3, 4, 5, 7, 8, 9, 10, 11, 12 };
    private int[] cyclistRouteIds = { 21, 22, 23, 24 };
    private int[] pedestrianRouteIds = { 31, 32, 33, 34, 35, 36, 37, 38 };
    private int[] boatRouteIds = { 41, 42 };
    private int[] busRouteIds = { 15, 5, 15 };
    public Sprite[] carSprites;

    private GameObject car;
    private GameObject bus;
    private GameObject cyclist;
    private GameObject pedestrian;
    private GameObject boat;

    public List<GameObject> wayPoints;
    public List<GameObject> carwayPoints = new List<GameObject>();
    public List<GameObject> cyclistwayPoints = new List<GameObject>();
    public List<GameObject> pedestrianwayPoints = new List<GameObject>();
    public List<GameObject> boatwayPoints = new List<GameObject>();
    public List<GameObject> buswayPoints = new List<GameObject>();
    public List<GameObject> carList = new List<GameObject>();
    public List<GameObject> cyclistList = new List<GameObject>();
    public List<GameObject> pedestrianList = new List<GameObject>();
    public List<GameObject> boatList = new List<GameObject>();
    public List<GameObject> busList = new List<GameObject>();

    private int rndmSprite;
    private float rndCarDelay;
    private float rndBonusmaxSpeed;

    private bool spawningCars;
    private bool spawningCyclists;
    private bool spawningPedestrians;
    private bool spawningBoats;
    private bool spawningBusses;

    private int spawnedCars = 1;
    private int spawnedCyclists = 1;
    private int spawnedPedestrians = 1;
    private int spawnedBoats = 1;
    private int spawnedBusses = 1;

    ///At scene start, all Awake calls happen before all Start calls
    void Awake()
    {
        cyclist = GameObject.Find("bike man");
        pedestrian = GameObject.Find("walking man");
        boat = GameObject.Find("boat");
        car = GameObject.Find("car_selfdriving");
        bus = GameObject.Find("bus");
        wayPoints = (GameObject.FindGameObjectsWithTag("waypoint")).ToList();

    }
    /// Start is called before the first frame update
    void Start()
    {
        if (hideSensors) ///hides zones and sensors used for collision
        {
            GameObject background = GameObject.Find("background");
            background.GetComponent<SpriteRenderer>().sortingLayerName = "background";
        }
        if (fastMode) ///enabled fastMode on all moving gameobjects
        {
            car.GetComponent<CarScript>().fastMode = true;
            pedestrian.GetComponent<PedestrianScript>().fastMode = true;
            cyclist.GetComponent<CyclistScript>().fastMode = true;
            boat.GetComponent<BoatScript>().fastMode = true;
            bus.GetComponent<CarScript>().fastMode = true;
        }
    }

    /// Update is called once per frame
    void Update()

    { ///randomize car spawning delay and bonus maxSpeed
        rndCarDelay = Random.Range(carSpwnRndDelayMin, carSpwnRndDelayMax);
        rndBonusmaxSpeed = Random.Range(-3, 3);
        if (carSpwnRndDelay)
            carSpawnDelay = rndCarDelay;

        ///Manually toggle sensor visibility
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (hideSensors)
            {
                GameObject background = GameObject.Find("background");
                background.GetComponent<SpriteRenderer>().sortingLayerName = "default";
                hideSensors = false;
            }
            else
            {
                GameObject background = GameObject.Find("background");
                background.GetComponent<SpriteRenderer>().sortingLayerName = "background";
                hideSensors = true;
            }
        }
        ///Automatically spawn cars up to a certain amount or manually when pressing a key
        if (((spawnCars && !spawningCars) && (spawnedCars <= carAmount || infiniteSpawn)) || Input.GetKeyDown(KeyCode.Alpha3))
        {
            StartCoroutine(SpawnCar());
        }
        ///Automatically spawn cyclists up to a certain amount or manually when pressing a key
        if (((spawnCyclists && !spawningCyclists) && (spawnedCyclists <= cyclistAmount || infiniteSpawn)) || Input.GetKeyDown(KeyCode.Alpha2))
        {
            StartCoroutine(SpawnCyclist());
        }
        ///Automatically spawn pedestrians up to a certain amount or manually when pressing a key
        if (((spawnPedestrians && !spawningPedestrians) && (spawnedPedestrians <= pedestrianAmount || infiniteSpawn)) || Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartCoroutine(SpawnPedestrian());
        }
        ///Automatically spawn boats up to a certain amount or manually when pressing a key
        if (((spawnBoats && !spawningBoats) && (spawnedBoats <= boatAmount || infiniteSpawn)) || Input.GetKeyDown(KeyCode.Alpha4))
        {
            StartCoroutine(SpawnBoat());
        }
        ///Automatically spawn busses at certain interval or manually when pressing a key
        if (((spawnBusses && !spawningBusses) && (spawnedBusses <= busAmount || infiniteSpawn)) || Input.GetKeyDown(KeyCode.Alpha9))
        {
            StartCoroutine(SpawnBus());
        }
        ///Manually despawn cars/busses
        if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Alpha0))
        {

            foreach (GameObject car in GameObject.FindGameObjectsWithTag("car"))
            {
                spawnedCars = 0;
                spawnedBusses = 0;
                if (car.activeSelf && car.transform.localPosition.x > -3000)
                    Despawn(car);
            }

            Debug.Log("cleared cars");
        }
        ///Manually despawn cyclists
        if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Alpha0))
        {
            spawnedCyclists = 0;
            foreach (GameObject cyclist in GameObject.FindGameObjectsWithTag("cyclist"))
            {
                if (cyclist.activeSelf && cyclist.transform.localPosition.x > -3000)
                    Despawn(cyclist);
            }

            Debug.Log("cleared cyclists");
        }
        ///Manually despawn pedestrians
        if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Alpha0))
        {
            spawnedPedestrians = 0;
            foreach (GameObject pedestrian in GameObject.FindGameObjectsWithTag("pedestrian"))
            {
                if (pedestrian.activeSelf && pedestrian.transform.localPosition.x > -3000)
                    Despawn(pedestrian);
            }

            Debug.Log("cleared pedestrians");
        }
        ///Manually despawn boats
        if (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Alpha0))
        {
            spawnedBoats = 0;
            foreach (GameObject boat in GameObject.FindGameObjectsWithTag("boat"))
            {
                if (boat.activeSelf && boat.transform.localPosition.x > -3000)
                    Despawn(boat);
            }

            Debug.Log("cleared boats");
        }

    }

    /// <summary>
    /// Spawn a car and give it a random sprite, route and wayPoints to follow
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnCar()
    {

        spawningCars = true;
        int rndPathSelector = 0;
        int rnd = Random.Range(0, carRouteIds.Length);
        int waypointAmount = 0;
        rndmSprite = Random.Range(0, carSprites.Length);

        GameObject spawnedcar = Instantiate(car);
        spawnedcar.GetComponent<CarScript>().routeId = carRouteIds[rnd];
        spawnedcar.GetComponent<CarScript>().number = spawnedCars;
        spawnedcar.GetComponent<SpriteRenderer>().sprite = carSprites[rndmSprite];

        ///check if the car route has multiple paths, if so then select a random path
        foreach (GameObject waypoint in wayPoints)
        {
            if (waypoint.GetComponent<RouteScript>().routeId == carRouteIds[rnd]) ///get a random  car route
            {
                if (waypoint.GetComponent<RouteScript>().multiplePaths)
                {
                    int pathAmount = waypoint.GetComponent<RouteScript>().pathAmount;
                    rndPathSelector = Random.Range(1, pathAmount + 1);
                    break;
                }
            }
        }

        ///get the amount of wayPoints the route has
        foreach (GameObject waypoint in wayPoints)
        {
            if (waypoint.GetComponent<RouteScript>().routeId == carRouteIds[rnd])
            {
                if (waypoint.GetComponent<RouteScript>().pathId == rndPathSelector)
                {
                    waypointAmount = waypoint.GetComponent<RouteScript>().wayPointAmount;
                    break;
                }
            }
        }

        ///array of wayPoints of the right size
        GameObject[] wayPointsToFollow = new GameObject[waypointAmount];

        ///Give the car wayPoints to follow in the correct order by inserting them in an array at the right index
        foreach (GameObject waypoint in wayPoints)
        {
            if (waypoint.GetComponent<RouteScript>().routeId == carRouteIds[rnd])
            {
                if (waypoint.GetComponent<RouteScript>().pathId == rndPathSelector)
                    wayPointsToFollow[waypoint.GetComponent<RouteScript>().waypointId - 1] = waypoint;
            }
        }

        spawnedcar.GetComponent<CarScript>().hasSpawned = true;
        spawnedcar.GetComponent<CarScript>().wayPointsToFollow = wayPointsToFollow.ToList();
        spawnedCars += 1;
        ///Debug.Log("Spawned car: " + spawnedCars + "/" + carAmount);

        yield return new WaitForSeconds(carSpawnDelay);

        spawningCars = false;

        yield return null;
    }

    /// <summary>
    /// Spawn a set amount of cyclists at a certain interval
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnCyclist()
    {
        spawningCyclists = true;
        int rnd = Random.Range(0, cyclistRouteIds.Length);
        int waypointAmount = 0;

        GameObject spawnedcyclist = Instantiate(cyclist);
        spawnedcyclist.GetComponent<CyclistScript>().routeId = cyclistRouteIds[rnd]; ///get a random  cyclist route
        spawnedcyclist.GetComponent<CyclistScript>().number = spawnedCyclists;

        ///get the amount of wayPoints the route has
        foreach (GameObject waypoint in wayPoints)
        {
            if (waypoint.GetComponent<RouteScript>().routeId == cyclistRouteIds[rnd])
            {
                waypointAmount = waypoint.GetComponent<RouteScript>().wayPointAmount;
                break;
            }
        }

        ///array of wayPoints of the right size
        GameObject[] wayPointsToFollow = new GameObject[waypointAmount];

        ///Give the cyclist wayPoints to follow in the correct order by inserting them in an array at the right index
        foreach (GameObject waypoint in wayPoints)
        {
            if (waypoint.GetComponent<RouteScript>().routeId == cyclistRouteIds[rnd])
            {

                wayPointsToFollow[waypoint.GetComponent<RouteScript>().waypointId - 1] = waypoint;
            }
        }

        spawnedcyclist.GetComponent<CyclistScript>().hasSpawned = true;
        spawnedcyclist.GetComponent<CyclistScript>().wayPointsToFollow = wayPointsToFollow.ToList();
        spawnedCyclists += 1;
        ///Debug.Log("Spawned cyclist: " + spawnedCyclists + "/" + cyclistAmount);

        yield return new WaitForSeconds(cyclistSpawnDelay);
        spawningCyclists = false;
        yield return null;

    }

    /// <summary>
    /// Spawn a set amount of pedestrians at a certain interval
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnPedestrian()
    {
        spawningPedestrians = true;
        int rnd = Random.Range(0, pedestrianRouteIds.Length);
        int waypointAmount = 0;

        GameObject spawnedpedestrian = Instantiate(pedestrian);
        spawnedpedestrian.GetComponent<PedestrianScript>().routeId = pedestrianRouteIds[rnd]; ///get a random pedestrian route
        spawnedpedestrian.GetComponent<PedestrianScript>().number = spawnedPedestrians;

        ///get the amount of wayPoints the route has
        foreach (GameObject waypoint in wayPoints)
        {
            if (waypoint.GetComponent<RouteScript>().routeId == pedestrianRouteIds[rnd])
            {
                waypointAmount = waypoint.GetComponent<RouteScript>().wayPointAmount;
                break;
            }
        }

        ///array of wayPoints to follow of the right size
        GameObject[] wayPointsToFollow = new GameObject[waypointAmount];

        ///Give the pedestrian wayPoints to follow in the correct order by inserting them in an array at the right index
        foreach (GameObject waypoint in wayPoints)
        {
            if (waypoint.GetComponent<RouteScript>().routeId == pedestrianRouteIds[rnd])
            {

                wayPointsToFollow[waypoint.GetComponent<RouteScript>().waypointId - 1] = waypoint;
            }
        }

        spawnedpedestrian.GetComponent<PedestrianScript>().hasSpawned = true;
        spawnedpedestrian.GetComponent<PedestrianScript>().wayPointsToFollow = wayPointsToFollow.ToList();
        spawnedPedestrians += 1;
        ///Debug.Log("Spawned pedestrian: " + spawnedPedestrians + "/" + pedestrianAmount);

        yield return new WaitForSeconds(pedestrianSpawnDelay);
        spawningPedestrians = false;
        yield return null;

    }

    /// <summary>
    /// Spawn a set amount of boats at a certain interval
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnBoat()
    {
        spawningBoats = true;
        int rnd = Random.Range(0, boatRouteIds.Length);
        int waypointAmount = 0;

        GameObject spawnedboat = Instantiate(boat);
        spawnedboat.GetComponent<BoatScript>().routeId = boatRouteIds[rnd];
        spawnedboat.GetComponent<BoatScript>().number = spawnedBoats;


        ///get the amount of wayPoints the route has
        foreach (GameObject waypoint in wayPoints)
        {
            if (waypoint.GetComponent<RouteScript>().routeId == boatRouteIds[rnd])
            {
                waypointAmount = waypoint.GetComponent<RouteScript>().wayPointAmount;
                break;
            }
        }

        ///array of wayPoints of the right size
        GameObject[] wayPointsToFollow = new GameObject[waypointAmount];

        ///Give the boat wayPoints to follow in the correct order by inserting them in an array at the right index
        foreach (GameObject waypoint in wayPoints)
        {
            if (waypoint.GetComponent<RouteScript>().routeId == boatRouteIds[rnd])
            {
                wayPointsToFollow[waypoint.GetComponent<RouteScript>().waypointId - 1] = waypoint;
            }
        }

        spawnedboat.GetComponent<BoatScript>().hasSpawned = true;
        spawnedboat.GetComponent<BoatScript>().wayPointsToFollow = wayPointsToFollow.ToList();
        spawnedBoats += 1;
        ///Debug.Log("Spawned boat: " + spawnedBoats + "/" + boatAmount);

        yield return new WaitForSeconds(boatSpawnDelay);
        spawningBoats = false;
        yield return null;

    }
    /// <summary>
    /// Spawn a set amount of busses at a certain interval
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnBus()
    {
        spawningBusses = true;
        int rnd = Random.Range(0, busRouteIds.Length);
        int waypointAmount = 0;
        int rndPathSelector = 0;

        GameObject spawnedBus = Instantiate(bus);
        spawnedBus.GetComponent<CarScript>().routeId = busRouteIds[rnd];
        spawnedBus.GetComponent<CarScript>().number = spawnedBusses;

        ///check if the car route has multiple paths, if so then select a random path
        foreach (GameObject waypoint in wayPoints)
        {
            if (waypoint.GetComponent<RouteScript>().routeId == busRouteIds[rnd]) ///get a random  car route
            {
                if (waypoint.GetComponent<RouteScript>().multiplePaths)
                {
                    int pathAmount = waypoint.GetComponent<RouteScript>().pathAmount;
                    rndPathSelector = Random.Range(1, pathAmount + 1);
                    break;
                }
            }
        }

        ///get the amount of wayPoints of the route to follow
        foreach (GameObject waypoint in wayPoints)
        {
            if (waypoint.GetComponent<RouteScript>().routeId == busRouteIds[rnd])
            {
                if (waypoint.GetComponent<RouteScript>().pathId == rndPathSelector)
                {
                    waypointAmount = waypoint.GetComponent<RouteScript>().wayPointAmount;
                    break;
                }
            }
        }
        ///array of wayPoints of the right size
        GameObject[] wayPointsToFollow = new GameObject[waypointAmount];

        ///Give the bus wayPoints to follow in the correct order by inserting them in an array at the right index
        foreach (GameObject waypoint in wayPoints)
        {
            if (waypoint.GetComponent<RouteScript>().routeId == busRouteIds[rnd])
            {
                if (waypoint.GetComponent<RouteScript>().pathId == rndPathSelector)
                    wayPointsToFollow[waypoint.GetComponent<RouteScript>().waypointId - 1] = waypoint;
            }
        }
        spawnedBus.GetComponent<CarScript>().hasSpawned = true;
        spawnedBus.GetComponent<CarScript>().wayPointsToFollow = wayPointsToFollow.ToList();
        spawnedBusses += 1;
        ///Debug.Log("Spawned bus: " + spawnedBusses + "/" + busAmount);

        yield return new WaitForSeconds(busSpawnDelay);
        spawningBusses = false;
        yield return null;
    }

    /// <summary>
    /// Despawn a gameobject by destroying it and its children
    /// </summary>
    /// <param name="obj">game object</param>
    void Despawn(GameObject obj)
    {
        obj.SetActive(false);
        foreach (Transform child in obj.transform)
        {
            Destroy(child.gameObject);
        }
        Destroy(obj.gameObject);
    }
}
