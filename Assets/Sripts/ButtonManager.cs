using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    public MenuScript menu;

    /// Start is called before the first frame update
    public void Start()
    {
        menu= GameObject.Find("Panel").GetComponent<MenuScript>();
    }

    ///When the button is clicked load the main scene (the simulation) and set certain variables
    public void NewGameBtn(string level)
    {
        MessageHandlerScript.data = menu.data;
        MessageHandlerScript.connectBroker = menu.connectBroker;
        SpawnScript.hideSensors = menu.hideSensors;
        SpawnScript.fastMode = menu.fastMode;
        SpawnScript.infiniteSpawn = menu.infiniteSpawn;
        SpawnScript.spawnCars = menu.spawnCars;
        SpawnScript.spawnCyclists = menu.spawnCyclists;
        SpawnScript.spawnPedestrians = menu.spawnPedestrians;
        SpawnScript.spawnBoats = menu.spawnBoats;
        SpawnScript.spawnBusses = menu.spawnBusses;
        SpawnScript.busAmount = menu.busAmount;
        SpawnScript.carAmount = menu.carAmount;
        SpawnScript.cyclistAmount = menu.cyclistAmount;
        SpawnScript.pedestrianAmount = menu.pedestrianAmount;
        SpawnScript.boatAmount = menu.boatAmount;
        SpawnScript.carSpawnDelay = menu.carSpawnDelay;
        SpawnScript.cyclistSpawnDelay = menu.cyclistSpawnDelay;
        SpawnScript.pedestrianSpawnDelay = menu.pedestrianSpawnDelay;
        SpawnScript.boatSpawnDelay = menu.boatSpawnDelay;
        SpawnScript.busSpawnDelay = menu.busSpawnDelay;
        SceneManager.LoadScene(level);
    }

    ///Button to exit the game
    public void ExitGameBtn()
    {
        Application.Quit();
    }
}
