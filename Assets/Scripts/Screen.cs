using System.Collections.Generic;
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
    void Awake()
    {   
        manager = FindObjectOfType<Manager>();     
        UpdateLedwall();
    }

    void Start(){
        
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

    char[] charset = {'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'};
    string LetterEncoding(int i){
        string output="";        
        do{
            int r = i%26;
            i/=26;
            output=charset[r]+output;            
        }while(i>0);

        return output;
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
                    tiles[i,j].GetComponent<SpriteRenderer>().color = manager.pixelmapPalette[(i + j) % manager.pixelmapPalette.Length];
                    tiles[i,j].GetComponentInChildren<TMP_Text>().text = i+1 + "," + (j+1);
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
                tiles[actualX, actualY].GetComponent<SpriteRenderer>().color = manager.powerPalette[actualLine%manager.powerPalette.Length];
                tiles[actualX, actualY].GetComponentInChildren<TMP_Text>().text = LetterEncoding(actualLine)+(actualTile+1);

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
            DrawSignalNewMethod();
        }
    }

    List<Vector2> GenerateShapes(int _tileNumber, Vector2 _tileResolution, Vector2 _sendingCardMaxResolution){        
        int maxHeight=_tileNumber;
        if ((int)(_sendingCardMaxResolution.y/_tileResolution.y)<maxHeight){
            maxHeight=(int)(_sendingCardMaxResolution.y/_tileResolution.y);
        }

        List<Vector2> shapes = new List<Vector2>();
        int i = 1;
        while (i<=maxHeight){            
            int width=_tileNumber/i;
            if ((int)(_sendingCardMaxResolution.x/_tileResolution.x)<width){
                width=(int)(_sendingCardMaxResolution.x/_tileResolution.x);
            }
            Vector2 shape = new Vector2(width,i);
            shapes.Add(shape);
            i+=1;
        }
        return shapes;
    }
    
    int CompareByArea(Vector2 a, Vector2 b)
    {
        float areaA = a.x*a.y;
        float areaB = b.x*b.y;
        if (areaA < areaB)
        {
            return -1;
        }
        else if (areaA > areaB)
        {
            return 1;
        }
        //a parità di area prendi quello con larghezza maggiore
        if (a.x < b.x) {
            return -1;
        }
        else if (a.x > b.x)
        {
            return 1;
        }
        return 0;
    }

    //area in numero di tile
    int[,] FillArea(Vector2 area, List<Vector2> validShapes, int startingLineNumber){
        int[,] output = new int[(int)area.x,(int)area.y];
        for (int a=0;a<area.x;a++){
            for (int b=0;b<area.y;b++){
                output[a,b]=-1;
            }
        }
        int actualSignalLine=startingLineNumber;
        //qui dovrebbe essere while true, ma per motivi di debug abbiamo limitato a 1000,
        //così se sbagliamo qualcosa non entra in un loop infinito. in futuro bisogna scriverlo meglio,
        //ma difficilmente si arriverà mai ad avere una sending card a cui colleghiamo 1000 cavi di rete fisici
        while(actualSignalLine<1000){
            //trova il primo punto libero
            int x=0;
            int y=0;
            while(output[x,y]!=-1){
                y++;
                if (y>=area.y){
                    y=0;
                    x++;
                    if (x>=area.x){
                        return output;
                    }
                }
            }
            //trova la forma che cabli più tiles            
            Vector2 bestShape = new Vector2(0,0);
            int bestResult=int.MinValue;
            foreach(Vector2 actualShape in validShapes){
                int result=0;
                for (int a=0;a<actualShape.x;a++){
                    for (int b=0;b<actualShape.y;b++){
                        if (x+a>=area.x){
                            continue;
                        }
                        if (y+b>=area.y){
                            continue;
                        }
                        if (output[x+a,y+b]==-1){
                            result++;
                        }
                    }
                }
                if (result>bestResult){
                    bestShape=actualShape;
                    bestResult=result;
                }
            }
            //riempi la forma
            for (int a=0;a<bestShape.x;a++){
                for (int b=0;b<bestShape.y;b++){
                    if (x+a>=area.x){
                        continue;
                    }
                    if (y+b>=area.y){
                        continue;
                    }
                    if (output[x+a,y+b]!=-1){
                        continue;
                    }
                    output[x+a,y+b]=actualSignalLine;
                }
            }
            actualSignalLine++;
        }
        return output;
    }

    void DrawSignalNewMethod(){        
        
        Vector2 sendingMaxResolution= new Vector2(3840,2160);

        maxTilesPerSignalLine = (int) (655360/(tileResolution.x * tileResolution.y));
        List<Vector2> validShapes = GenerateShapes(maxTilesPerSignalLine,tileResolution,sendingMaxResolution);
        validShapes.Sort(CompareByArea);
        validShapes.Reverse();
        //conta quante sending card dovrai usare per rispettare la risoluzione massima
        List<Vector4> areas = new List<Vector4>();
        int horizontalSendingNumber=(int)Mathf.Ceil(resolution.x/sendingMaxResolution.x);
        int verticalSendingNumber=(int)Mathf.Ceil(resolution.y/sendingMaxResolution.y);
        for (int i=0;i<horizontalSendingNumber;i++){
            for (int j=0;j<verticalSendingNumber;j++){
                float xStart=i*sendingMaxResolution.x;
                float yStart=j*sendingMaxResolution.y;
                float xSize=sendingMaxResolution.x;
                if ((xStart+xSize)>resolution.x){
                    xSize=resolution.x-xStart;
                }
                float ySize=sendingMaxResolution.y;
                if ((yStart+ySize)>resolution.y){
                    ySize=resolution.y-yStart;
                }
                
                areas.Add( new Vector4(xStart/tileResolution.x,yStart/tileResolution.y,xSize/tileResolution.x,ySize/tileResolution.y));
            }
        }
        int [,] signals = new int[horizontalTilenumber,verticalTilenumber];
        int startingSignalLine=0;
        foreach (Vector4 area in areas){
            int[,] areaSignals = FillArea(new Vector2(area.z,area.w),validShapes, startingSignalLine);
            for (int i=0;i<area.z;i++){
                for (int j=0;j<area.w;j++){
                    Debug.Log($"area: {area},i: {i},j: {j}");
                    signals[(int)(area.x+i),(int)(area.y+j)]=areaSignals[i,j];
                    if (areaSignals[i,j]>startingSignalLine){
                        startingSignalLine=areaSignals[i,j];
                    }
                }
            }
            startingSignalLine++;
        }
        
        // colora il ledwall
        for (int i=0; i< horizontalTilenumber; i++)
        {
            for (int j = 0; j < verticalTilenumber; j++)
            {                    
                tiles[i,j].GetComponent<SpriteRenderer>().color = manager.signalPalette[signals[i,j]%manager.signalPalette.Length];
                tiles[i,j].GetComponentInChildren<TMP_Text>().text = LetterEncoding(signals[i,j]);
            }
        }
    }
    void DrawSignalOldMethod(){
        maxTilesPerSignalLine = (int) (655360/(tileResolution.x * tileResolution.y));
        int horizontallines = maxTilesPerSignalLine / horizontalTilenumber;
        if (horizontallines==0){
            horizontallines=int.MaxValue;
        }
        else{
            horizontallines = (int)Mathf.Ceil(verticalTilenumber / (float)horizontallines);
        }
        
        int verticallines = maxTilesPerSignalLine / verticalTilenumber;        
        if (verticallines==0){
            verticallines=int.MaxValue;
            if (horizontallines==int.MaxValue){
                Debug.LogError("Nessuna soluzione");
            }
        }
        else{
            verticallines = (int)Mathf.Ceil(horizontalTilenumber / (float)verticallines);
        }

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
            tiles[actualX, actualY].GetComponent<SpriteRenderer>().color = manager.signalPalette[actualLine%manager.signalPalette.Length];
            tiles[actualX, actualY].GetComponentInChildren<TMP_Text>().text = LetterEncoding(actualLine)+(actualTile+1);

            if (horizontalsignal)
            {
                if (actualTile/horizontalTilenumber % 2 == 0)
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
                if (actualTile / verticalTilenumber % 2 == 0)
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
