using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Screen : MonoBehaviour
{
    public Vector2 size = new Vector2 (.5f, .5f);
    public Vector2 tileSize = new Vector2 (.5f, .5f);
    public Vector2 tileResolution = new Vector2 (128, 128);
    public float pitch = 3.9f;
    public float tilePowerConsumption = 150;    
    public Vector2 resolution;
    public float totalPowerConsumption;
    public int horizontalTilenumber=1, verticalTilenumber=1;

    public GameObject tilePrefab;
    // Start is called before the first frame update
    void Start()
    {
        UpdateLedwall();
    }
    // Update is called once per frame
    public void UpdateLedwall()
    {               
        horizontalTilenumber = (int)Mathf.Ceil(size.x / tileSize.x);
        verticalTilenumber = (int)Mathf.Ceil(size.y / tileSize.y);
        tileResolution = new Vector2((int)(tileSize.x*1000 / pitch), (int)(tileSize.y*1000 / pitch));
        //risolve il problema di una mattonella 0,5 x 1.0 mt dividendo per 2 la risoluzione, arrotondando ad un ntero e moltiplicando di nuovo per 2
        tileResolution= new Vector2((int)(tileResolution.x/2)*2, (int)(tileResolution.y/2)*2);
        
        //forza il ledwall ad una dimensione corretta
        size= new Vector2(horizontalTilenumber * tileSize.x, verticalTilenumber * tileSize.y);
        resolution = new Vector2(horizontalTilenumber * tileResolution.x, verticalTilenumber * tileResolution.y);
        totalPowerConsumption=horizontalTilenumber*verticalTilenumber * tilePowerConsumption;

        //update boxCollider
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.size = size;
        boxCollider.offset = ((size-tileSize)/ 2.0f)*new Vector2(1,-1);
                
        RegenerateTiles();
    }

    

    public void RegenerateTiles()
    {
        Transform screenTiles = transform.Find("ScreenTiles");
        foreach (Transform child in screenTiles)
        {
            Destroy(child.gameObject);
        }        
        for (int i = 0; i < horizontalTilenumber; i++)
        {
            for (int j = 0; j < verticalTilenumber; j++)
            {
                GameObject tile = Instantiate(tilePrefab, screenTiles);
                tile.transform.localPosition = new Vector3(i * tileSize.x, -j * tileSize.y, 0);
                tile.transform.localScale = tileSize;
                tile.GetComponentInChildren<TMP_Text>().transform.localScale = (Vector3.one / tileSize) / 200.0f;
                float actualHue = ((i+j)%5)/5.0f;
                tile.GetComponent<SpriteRenderer>().color = Color.HSVToRGB(actualHue, 1, 1);
                tile.GetComponentInChildren<TMP_Text>().text = i + "," + j;
            }
        }
    }
}
