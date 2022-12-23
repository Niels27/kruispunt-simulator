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
/// Process the different possible player inputs
/// </summary>
public class Controls : MonoBehaviour
{
    public Camera overviewCamera;
    public Camera crossroadCamera;
    public Camera roundaboutCamera;
    public List<Camera> cameras;
    private int cameraIndex;

    /// Start is called before the first frame update
    void Start()
    {
        ///Add cameras
        overviewCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        crossroadCamera = GameObject.FindGameObjectWithTag("crossroad camera").GetComponent<Camera>();
        roundaboutCamera = GameObject.FindGameObjectWithTag("roundabout camera").GetComponent<Camera>();
        cameras.Add(overviewCamera);
        cameras.Add(crossroadCamera);
        cameras.Add(roundaboutCamera);
        overviewCamera.enabled = true;
        crossroadCamera.enabled = false;
        roundaboutCamera.enabled = false;
    }

    /// Update is called once per frame
    void Update()
    {
        ///go back to the menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Menu");
        }
        ///pause the game
        if (Input.GetKeyDown(KeyCode.P))
        {
            Time.timeScale = 0.001f;
            GameObject.Find("pause").GetComponent<TextMeshProUGUI>().text = "PAUSED";
        }
        ///unpause the game
        if (Input.GetKeyDown(KeyCode.U))
        {
            GameObject.Find("pause").GetComponent<TextMeshProUGUI>().text = "";
            Time.timeScale = 1;
        }
        ///toggle cameras
        if (Input.GetKeyDown(KeyCode.Space))
        {
            cameraIndex += 1;
            if (cameraIndex >= cameras.Count)
                cameraIndex = 0;

            foreach (Camera c in cameras)
            {
                if (c.enabled)
                    c.enabled = false;
            }
            cameras[cameraIndex].enabled = true;
        }
    }
}
