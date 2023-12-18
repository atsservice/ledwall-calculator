using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Tilemaps;

public class ScreenMenu : MonoBehaviour
{
    public Manager manager;
    public TMP_InputField inputFieldSizeX, inputFieldSizeY, inputFieldPitch, inputFieldPower, inputFieldTileSizeX, inputFieldTileSizeY, inputFieldTileResolutionX, inputFieldTileResolutionY;

    
    private void OnEnable()
    {
        Screen screen = manager.selectedScreen.GetComponent<Screen>();
        inputFieldSizeX.text = screen.size.x.ToString();
        inputFieldSizeY.text = screen.size.y.ToString();
        inputFieldPitch.text = screen.pitch.ToString();
        inputFieldPower.text = screen.tilePowerConsumption.ToString();
        inputFieldTileSizeX.text = screen.tileSize.x.ToString();
        inputFieldTileSizeY.text = screen.tileSize.y.ToString();
        inputFieldTileResolutionX.text = screen.tileResolution.x.ToString();
        inputFieldTileResolutionY.text = screen.tileResolution.y.ToString();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void UpdateSizeX(string value)
    {
        Screen screen = manager.selectedScreen.GetComponent<Screen>();
        float sizeX = float.Parse(value);
        sizeX = (int)(sizeX / (screen.tileSize.x)) * (screen.tileSize.x);
        manager.selectedScreen.GetComponent<Screen>().size.x = sizeX;
    }
    public void UpdateSizeY(string value)
    {
        Screen screen = manager.selectedScreen.GetComponent<Screen>();
        float sizeY = float.Parse(value);
        sizeY = (int)(sizeY / (screen.tileSize.y)) * (screen.tileSize.y);
        manager.selectedScreen.GetComponent<Screen>().size.y = sizeY;
    }
    public void UpdatePitch(string value)
    {
        manager.selectedScreen.GetComponent<Screen>().pitch = float.Parse(value);
        Screen screen = manager.selectedScreen.GetComponent<Screen>();
        screen.tileResolution = new Vector2((int)(screen.tileSize.x * 1000 / screen.pitch), (int)(screen.tileSize.y * 1000 / screen.pitch));
    }
    public void UpdatePower(string value)
    {
        manager.selectedScreen.GetComponent<Screen>().tilePowerConsumption = float.Parse(value);
    }

     public void UpdateTileSizeX(string value)
    {
        Screen screen = manager.selectedScreen.GetComponent<Screen>();
        manager.selectedScreen.GetComponent<Screen>().tileSize.x = float.Parse(value);
        screen.tileResolution.x = screen.tileSize.x * 1000 / screen.pitch;
    }

     public void UpdateTileSizeY(string value)
    {
        Screen screen = manager.selectedScreen.GetComponent<Screen>();
        manager.selectedScreen.GetComponent<Screen>().tileSize.y = float.Parse(value);
        screen.tileResolution.y = screen.tileSize.y * 1000 / screen.pitch;
    }
}
