using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenMenu : MonoBehaviour
{
    Manager manager;
    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.FindFirstObjectByType<Manager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void UpdateSizeX(string value)
    {        
        manager.selectedScreen.GetComponent<Screen>().size.x = float.Parse(value);
    }
    public void UpdateSizeY(string value)
    {
        manager.selectedScreen.GetComponent<Screen>().size.y = float.Parse(value);
    }
    public void UpdatePitch(string value)
    {
        manager.selectedScreen.GetComponent<Screen>().pitch = float.Parse(value);
    }
    public void UpdatePower(string value)
    {
        manager.selectedScreen.GetComponent<Screen>().tilePowerConsumption = float.Parse(value);
    }

     public void UpdatetileSizeX(string value)
    {
        manager.selectedScreen.GetComponent<Screen>().tileSize.x = float.Parse(value)/1000.0f;
    }

     public void UpdatetileSizeY(string value)
    {
        manager.selectedScreen.GetComponent<Screen>().tileSize.y = float.Parse(value)/1000.0f;
    }
}
