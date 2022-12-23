using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Set the controllable car controls
/// </summary>
public class CarInputHandler : MonoBehaviour
{
    
    CarScript carScript;

    void Awake()
    {
        carScript = GetComponent<CarScript>();
    }

    /// Update is called once per frame
   void Update()
    {
        Vector2 inputVector = Vector2.zero;

        inputVector.x = Input.GetAxis("Horizontal");
        inputVector.y = Input.GetAxis("Vertical");

        carScript.SetInputVector(inputVector);
    }
}
