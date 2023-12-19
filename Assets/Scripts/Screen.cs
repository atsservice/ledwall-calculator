using TMPro;
using UnityEngine;

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
    public Vector2 startPosition;
    public GameObject tilePrefab;

    public Manager manager;

    public int maxTilesPerPowerLine = 16;
    public int maxTilesPerSignalLine = 40;

    public GameObject[,] tiles;

    int[][] powerLines;

    // Start is called before the first frame update
    void Start()
    {
        manager = FindObjectOfType<Manager>();        
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

        //update startPosition
        transform.position = new Vector3(startPosition.x / 1000.0f * pitch, -startPosition.y / 1000.0f * pitch, 0);
                
        RegenerateTiles();
    }



    public void RegenerateTiles()
    {
        Transform screenTiles = transform.Find("ScreenTiles");
        foreach (Transform child in screenTiles)
        {
            Destroy(child.gameObject);
        }
        tiles = new GameObject[horizontalTilenumber, verticalTilenumber];
        for (int i = 0; i < horizontalTilenumber; i++)
        {
            for (int j = 0; j < verticalTilenumber; j++)
            {
                GameObject tile = Instantiate(tilePrefab, screenTiles);
                tile.transform.localPosition = new Vector3(i * tileSize.x, -j * tileSize.y, 0);
                tile.transform.localScale = tileSize;
                tile.GetComponentInChildren<TMP_Text>().transform.localScale = (Vector3.one / tileSize) / 200.0f;                
                tiles[i, j] = tile;
            }
        }
        if (manager.VIEW == View.Pixelmap)
        {
            for (int i=0; i< horizontalTilenumber; i++)
            {
                for (int j = 0; j < verticalTilenumber; j++)
                {
                    float actualHue = ((i + j) % 5) / 5.0f;
                    tiles[i,j].GetComponent<SpriteRenderer>().color = Color.HSVToRGB(actualHue, 1, 1);
                    tiles[i,j].GetComponentInChildren<TMP_Text>().text = i + "," + j;
                }
            }
        }

        if (manager.VIEW == View.Power)
        {
            maxTilesPerPowerLine = 16/(int)(tileSize.x*tileSize.y*4);
            //calcolo linee di corrrente mettendo al massimo 'maxTilesPerPowerLine' per linea e arrotondando per eccesso
            int numberOfPowerLines = (int)Mathf.Ceil(horizontalTilenumber * verticalTilenumber / (float)maxTilesPerPowerLine);
            //arrotondo il numero di powerLines in modo che siano sempre un multiplo di 3
            numberOfPowerLines = (int)Mathf.Ceil(numberOfPowerLines / 3.0f) * 3;


            //inizializza l'array di powerLines
            powerLines = new int[numberOfPowerLines][];
            int tilesPerPowerLine = horizontalTilenumber * verticalTilenumber / numberOfPowerLines;
            int unEvenTiles = (horizontalTilenumber * verticalTilenumber) % numberOfPowerLines;

            for (int i = 0; i < numberOfPowerLines; i++)
            {
                if (i < unEvenTiles)
                {
                    powerLines[i] = new int[tilesPerPowerLine + 1];
                }
                else
                {
                    powerLines[i] = new int[tilesPerPowerLine];
                }
            }

            //cabla il ledwall partendo da in alto a sinistra e muovendosi in orizzontale
            int actualX = 0;
            int actualY = 0;
            int actualLine = 0;
            int actualTile = 0;
            for (int i = 0; i < horizontalTilenumber * verticalTilenumber; i++)
            {
                powerLines[actualLine][actualTile] = actualY * horizontalTilenumber + actualX;

                float actualHue = (float)actualLine/(float)numberOfPowerLines;
                tiles[actualX, actualY].GetComponent<SpriteRenderer>().color = Color.HSVToRGB(actualHue, 1, 1);
                tiles[actualX, actualY].GetComponentInChildren<TMP_Text>().text = ( actualTile+1).ToString();

                if (actualY % 2 == 0)
                {
                    actualX++;
                    if (actualX >= horizontalTilenumber)
                    {
                        actualX--;
                        actualY++;
                    }
                }
                else
                {
                    actualX--;
                    if (actualX < 0)
                    {
                        actualX++;
                        actualY++;
                    }
                }
                actualTile++;
                if (actualTile >= powerLines[actualLine].Length)
                {
                    actualTile = 0;
                    actualLine++;
                }
            }
        }

        if (manager.VIEW == View.Signal)
        {
            maxTilesPerSignalLine = (int) (655360/(tileResolution.x * tileResolution.y));
            int horizontallines = maxTilesPerSignalLine / horizontalTilenumber;
            horizontallines = (int)Mathf.Ceil(verticalTilenumber / (float)horizontallines);

            int verticallines = maxTilesPerSignalLine / verticalTilenumber;
            verticallines = (int)Mathf.Ceil(horizontalTilenumber / (float)verticallines);

            bool horizontalsignal = true;
            int numberOfLines = horizontallines;
            if (verticallines < horizontallines)
            {
                horizontalsignal = false;
                numberOfLines = verticallines;
            }

            int actualX = 0;
            int actualY = 0;
            int actualLine = 0;
            int actualTile = 0;

            for (int i = 0; i < horizontalTilenumber * verticalTilenumber; i++)
            {
                float actualHue = (float)actualLine / (float)numberOfLines;
                tiles[actualX, actualY].GetComponent<SpriteRenderer>().color = Color.HSVToRGB(actualHue, 1, 1);
                tiles[actualX, actualY].GetComponentInChildren<TMP_Text>().text = (actualTile + 1).ToString();

                if (horizontalsignal)
                {
                    if ((actualTile/horizontalTilenumber) % 2 == 0)
                    {
                        actualX++;
                        if (actualX >= horizontalTilenumber)
                        {
                            actualX--;
                            actualY++;
                        }
                    }
                    else
                    {
                        actualX--;
                        if (actualX < 0)
                        {
                            actualX++;
                            actualY++;
                        }
                    }
                    actualTile++;
                    if (actualY >= (actualLine+1) * (maxTilesPerSignalLine / horizontalTilenumber))
                    {
                        actualX = 0;
                        actualTile = 0;
                        actualLine++;
                    }
                }
                else
                {
                    if ((actualTile / verticalTilenumber) % 2 == 0)
                    {
                        actualY++;
                        if (actualY >= verticalTilenumber)
                        {
                            actualY--;
                            actualX++;
                        }
                    }
                    else
                    {
                        actualY--;
                        if (actualY < 0)
                        {
                            actualY++;
                            actualX++;
                        }
                    }
                    actualTile++;
                    if (actualX >= (actualLine + 1) * (maxTilesPerSignalLine / verticalTilenumber))
                    {
                        actualY = 0;
                        actualTile = 0;
                        actualLine++;
                    }
                }
            }
        }
    }
}
