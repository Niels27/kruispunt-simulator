using UnityEngine;
using System.Collections;


/// <summary>
/// A debugger to displaying in-game
/// </summary>
public class Debugger : MonoBehaviour
{
  
    bool doShow = false;
    uint qsize = 25;  /// number of messages to keep
    Queue myLogQueue = new Queue();

    /// Start is called before the first frame update
    void Start()
    {
        Debug.Log("Started up logging.");
    }

    /// Update is called once per frame
    void Update() { if (Input.GetKeyDown(KeyCode.L)) { doShow = !doShow; } }

    ///on application open
    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    ///on application close
    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    /// <summary>
    /// Queues the right debug logs up to a limit
    /// </summary>
    /// <param name="logString"></param>
    /// <param name="stackTrace"></param>
    /// <param name="type"></param>
    void HandleLog(string logString, string stackTrace, LogType type)
    {
        myLogQueue.Enqueue("[" + type + "] : " + logString);
        if (type == LogType.Exception)
            myLogQueue.Enqueue(stackTrace);
        while (myLogQueue.Count > qsize)
            myLogQueue.Dequeue();
    }
    ///show the debugger in game
    void OnGUI()
    {
        if (!doShow) { return; }
        GUILayout.BeginArea(new Rect(Screen.width - 1400, 0, 600, Screen.height));
        GUILayout.Label("\n" + string.Join("\n", myLogQueue.ToArray()));
        GUILayout.EndArea();
    }
}



