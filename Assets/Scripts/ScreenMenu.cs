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
    public TMP_InputField inputFieldSizeX, inputFieldSizeY, inputFieldPitch, inputFieldPower, inputFieldTileSizeX, inputFieldTileSizeY;
    public TMP_Text textMeshPro;

    private void OnEnable()
    {
        Screen screen = manager.selectedScreen.GetComponent<Screen>();
        inputFieldSizeX.text = screen.size.x.ToString();
        inputFieldSizeY.text = screen.size.y.ToString();
        inputFieldPitch.text = screen.pitch.ToString();
        inputFieldPower.text = screen.tilePowerConsumption.ToString();
        inputFieldTileSizeX.text = screen.tileSize.x.ToString();
        inputFieldTileSizeY.text = screen.tileSize.y.ToString();

        UpdateInfo();
    }

    void UpdateInfo()
    {
        Screen screen = manager.selectedScreen.GetComponent<Screen>();
        string s = "";
        s += "Size:   " + screen.size.x + " x " + screen.size.y + " m\n";
        s += "Pixel Pitch: " + screen.pitch + " mm\n\n";
        s += "Tile Size: " + screen.tileSize.x + " x " + screen.tileSize.y + " m\n";
        s += "Tile Resolution: " + screen.tileResolution.x + " x " + screen.tileResolution.y + " px\n";
        s += "Tile Number: " + screen.horizontalTilenumber + " x " + screen.verticalTilenumber + "\n";
        s += "Tile Power Consumption: " + screen.tilePowerConsumption + " W\n\n";
        s += "Total Pixel Resolution: " + screen.resolution.x + " x " + screen.resolution.y + " px\n";
        s += "Total Power Consumption: " + screen.totalPowerConsumption + " W";
        textMeshPro.text = s;        
    }

    public void UpdateSizeX(string value)
    {
        Screen screen = manager.selectedScreen.GetComponent<Screen>();
        float sizeX = float.Parse(value);
        sizeX = (int)(sizeX / (screen.tileSize.x)) * (screen.tileSize.x);
        screen.size.x = sizeX;
        screen.UpdateLedwall();
    }
    public void UpdateSizeY(string value)
    {
        Screen screen = manager.selectedScreen.GetComponent<Screen>();
        float sizeY = float.Parse(value);
        sizeY = (int)(sizeY / (screen.tileSize.y)) * (screen.tileSize.y);
        screen.size.y = sizeY;
        screen.UpdateLedwall();
    }
    public void UpdatePitch(string value)
    {
        manager.selectedScreen.GetComponent<Screen>().pitch = float.Parse(value);
        Screen screen = manager.selectedScreen.GetComponent<Screen>();
        screen.tileResolution = new Vector2((int)(screen.tileSize.x * 1000 / screen.pitch), (int)(screen.tileSize.y * 1000 / screen.pitch));
        screen.UpdateLedwall();
    }
    public void UpdatePower(string value)
    {
        Screen screen = manager.selectedScreen.GetComponent<Screen>();
        screen.tilePowerConsumption = float.Parse(value);
        screen.UpdateLedwall();
    }

     public void UpdateTileSizeX(string value)
    {
        Screen screen = manager.selectedScreen.GetComponent<Screen>();
        screen.tileSize.x = float.Parse(value);
        screen.tileResolution.x = screen.tileSize.x * 1000 / screen.pitch;
        screen.UpdateLedwall();
    }

     public void UpdateTileSizeY(string value)
    {
        Screen screen = manager.selectedScreen.GetComponent<Screen>();
        screen.tileSize.y = float.Parse(value);
        screen.tileResolution.y = screen.tileSize.y * 1000 / screen.pitch;
        screen.UpdateLedwall();
    }
}
