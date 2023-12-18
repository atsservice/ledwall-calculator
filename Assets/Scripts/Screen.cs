using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Screen : MonoBehaviour
{
    public Vector2 size = new Vector2 (.5f, .5f);
    public Vector2 tileSize = new Vector2 (.5f, .5f);
    public Vector2 tileResolution;
    public float pitch, tilePowerConsumption;
    public TMP_Text textMeshPro;
    public Vector2 resolution;
    public float totalPowerConsumption;
    int horizontalTilenumber, verticalTilenumber;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = size;        
        horizontalTilenumber = (int)(size.x / tileSize.x);
        verticalTilenumber = (int)(size.y / tileSize.y);
        tileResolution = new Vector2((int)(tileSize.x*1000 / pitch), (int)(tileSize.y*1000 / pitch));
        
        //forza il ledwall ad una dimensione corretta
        size= new Vector2(horizontalTilenumber * tileSize.x, verticalTilenumber * tileSize.y);
        resolution = new Vector2(horizontalTilenumber * tileResolution.x, verticalTilenumber * tileResolution.y);
        totalPowerConsumption=horizontalTilenumber*verticalTilenumber * tilePowerConsumption;
        UpdateInfo();
    }

    void UpdateInfo()
    {
        string s = "";
        s += "Size:   " + size.x + " x " + size.y + " m\n";
        s += "Pixel Pitch: " + pitch + " mm\n\n";
        s += "Tile Size: " + tileSize.x + " x " + tileSize.y + " m\n";
        s += "Tile Resolution: " + tileResolution.x + " x " + tileResolution.y + " px\n";
        s += "Tile Number: " + horizontalTilenumber + " x " + verticalTilenumber + "\n";
        s += "Tile Power Consumption: " + tilePowerConsumption + " W\n\n";
        s += "Total Pixel Resolution: " + resolution.x + " x " + resolution.y + " px\n";
        s += "Total Power Consumption: " + totalPowerConsumption+" W";
        textMeshPro.text = s;    
        textMeshPro.transform.localScale = new Vector3(1 / transform.localScale.x, 1 / transform.localScale.y, 1)/100;
    }
}
