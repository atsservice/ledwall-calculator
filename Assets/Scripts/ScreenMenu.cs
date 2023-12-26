using UnityEngine;
using TMPro;
using System.Globalization;
using StringMath;

public class ScreenMenu : MonoBehaviour
{
    public Manager manager;
    public TMP_InputField inputFieldSizeX, inputFieldSizeY, inputFieldPitch, inputFieldPower, inputFieldTileSizeX, inputFieldTileSizeY, inputFieldStartX, inputFieldStartY;
    public TMP_Text textMeshPro;

    public TMP_Dropdown dropdownSelectScreen;

    public GameObject contentFrame;

    public void LoadData(Screen screen)
    {
        inputFieldSizeX.text = screen.size.x.ToString();
        inputFieldSizeY.text = screen.size.y.ToString();
        inputFieldPitch.text = screen.pitch.ToString();
        inputFieldPower.text = screen.tilePowerConsumption.ToString();
        inputFieldTileSizeX.text = screen.tileSize.x.ToString();
        inputFieldTileSizeY.text = screen.tileSize.y.ToString();
        inputFieldStartX.text = screen.startPosition.x.ToString();
        inputFieldStartY.text = screen.startPosition.y.ToString();
        UpdateInfo(screen);
        int i = manager.screens.IndexOf(screen) + 1;
        dropdownSelectScreen.value=i;
        contentFrame.SetActive(true);
    }

    public void UnloadData()
    {
        inputFieldSizeX.text = "";
        inputFieldSizeY.text = "";
        inputFieldPitch.text = "";
        inputFieldPower.text = "";
        inputFieldTileSizeX.text = "";
        inputFieldTileSizeY.text = "";
        inputFieldStartX.text = "";
        inputFieldStartY.text = "";
        textMeshPro.text = "";
        dropdownSelectScreen.value=0;
        contentFrame.SetActive(false);
    }

    private void Update()
    {
        //cicla tra gli TMP_InputField quando premo TAB, cambiando il focus da uno all'altro        
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            TMP_InputField[] selectableInputFields = { inputFieldSizeX, inputFieldSizeY, inputFieldTileSizeX, inputFieldTileSizeY, inputFieldPitch, inputFieldPower, inputFieldStartX, inputFieldStartY};
            int selected = -1;
            for (int i = 0; i < selectableInputFields.Length; i++)
            {
                if (selectableInputFields[i].isFocused)
                {
                    selected = i;
                    break;
                }
            }
            if (selected == -1)
            {
                return;
            }
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                selected--;
                if (selected < 0)
                {
                    selected = selectableInputFields.Length - 1;
                }
            }
            else
            {
                selected++;
                if (selected >= selectableInputFields.Length)
                {
                    selected = 0;
                }
            }
            selectableInputFields[selected].Select();
        }
        
    }

    void UpdateInfo(Screen screen)
    {
        string s = "";
        s += "Size:   " + screen.size.x + " x " + screen.size.y + " m\n";
        s += "Pixel Pitch: " + screen.pitch + " mm\n\n";
        s += "Tile Size: " + screen.tileSize.x + " x " + screen.tileSize.y + " m\n";
        s += "Tile Resolution: " + screen.tileResolution.x + " x " + screen.tileResolution.y + " px\n";
        s += "Start Position: " + screen.startPosition.x + " x " + screen.startPosition.y + " px\n";
        s += "Tile Number: " + screen.horizontalTilenumber + " x " + screen.verticalTilenumber + "\n";
        s += "Tile Power Consumption: " + screen.tilePowerConsumption + " W\n\n";
        s += "Total Pixel Resolution: " + screen.resolution.x + " x " + screen.resolution.y + " px\n";
        s += "Total Power Consumption: " + screen.totalPowerConsumption + " W\n\n";
        s += "Ledwall Materials:\n";
        s += "Total Tile Number: " + screen.verticalTilenumber*screen.horizontalTilenumber + " tiles\n";
       // s += "Total Power Line:  ;
       // s += "Total Signal Line: " ;
       // s += "Aspect Ratio: " ;
        textMeshPro.text = s;        
    }

    public string ValidateValue(string value, bool nonZeroPositive=false)
    {      
        value=value.Replace(',','.');
        try{     
            MathExpr expr = value;
            float f = (float) expr.Result;
            if (nonZeroPositive && f<=0){
                return null;
            }
            return f.ToString().Replace(',','.');
        }
        catch(MathException exception){
            Debug.Log(exception.Message);
            return null;
        }
    }

    public void UpdateSizeX(string value)
    {
        Screen screen = manager.selectedScreen.GetComponent<Screen>();
        value =ValidateValue(value,true);
        if (value == null)
        {
            inputFieldSizeX.text = screen.size.x.ToString();
            return;
        }
        
        float sizeX = float.Parse(value,CultureInfo.InvariantCulture);
        sizeX = (int)(sizeX / (screen.tileSize.x)) * (screen.tileSize.x);
        screen.size.x = sizeX;
        inputFieldSizeX.text=sizeX.ToString();
        screen.UpdateLedwall();
        UpdateInfo(screen);
    }
    public void UpdateSizeY(string value)
    {
        Screen screen = manager.selectedScreen.GetComponent<Screen>();
        value = ValidateValue(value,true);
        if (value == null)
        {
            inputFieldSizeY.text=screen.size.y.ToString();
            return;
        }

        float sizeY = float.Parse(value,CultureInfo.InvariantCulture);
        sizeY = (int)(sizeY / (screen.tileSize.y)) * (screen.tileSize.y);
        screen.size.y = sizeY;
        inputFieldSizeY.text=sizeY.ToString();
        screen.UpdateLedwall();
        UpdateInfo(screen);
    }
    public void UpdatePitch(string value)
    {
        Screen screen = manager.selectedScreen.GetComponent<Screen>();
        value = ValidateValue(value,true);
        if (value == null)
        {
            inputFieldPitch.text=screen.pitch.ToString();
            return;
        }

        float pitch = float.Parse(value,CultureInfo.InvariantCulture);   
        manager.selectedScreen.GetComponent<Screen>().pitch = pitch;
        screen.tileResolution = new Vector2((int)(screen.tileSize.x * 1000 / screen.pitch), (int)(screen.tileSize.y * 1000 / screen.pitch));
        inputFieldPitch.text=pitch.ToString();
        screen.UpdateLedwall();
        UpdateInfo(screen);
    }
    public void UpdatePower(string value)
    {
        Screen screen = manager.selectedScreen.GetComponent<Screen>();
        value = ValidateValue(value,true);
        if (value == null)
        {
            inputFieldPower.text=screen.tilePowerConsumption.ToString();
            return;
        }
        float power = float.Parse(value,CultureInfo.InvariantCulture);
        screen.tilePowerConsumption = power;
        inputFieldPower.text=power.ToString();
        screen.UpdateLedwall();
        UpdateInfo(screen);
    }

     public void UpdateTileSizeX(string value)
    {
        Screen screen = manager.selectedScreen.GetComponent<Screen>();
        value = ValidateValue(value,true);
        if (value == null)
        {
            inputFieldTileSizeX.text=screen.tileSize.x.ToString();
            return;
        }
        
        screen.tileSize.x = float.Parse(value,CultureInfo.InvariantCulture);
        screen.tileResolution.x = screen.tileSize.x * 1000 / screen.pitch;
        inputFieldTileSizeX.text=screen.tileSize.x.ToString();
        screen.UpdateLedwall();
        UpdateInfo(screen);
    }

     public void UpdateTileSizeY(string value)
    {
        Screen screen = manager.selectedScreen.GetComponent<Screen>();
        value = ValidateValue(value,true);
        if (value == null)
        {
            inputFieldTileSizeY.text = screen.tileSize.y.ToString();
            return;
        }
        
        screen.tileSize.y = float.Parse(value,CultureInfo.InvariantCulture);
        screen.tileResolution.y = screen.tileSize.y * 1000 / screen.pitch;
        inputFieldTileSizeY.text=screen.tileSize.y.ToString();
        screen.UpdateLedwall();
        UpdateInfo(screen);
    }

    public void UpdateStartX(string value)
    {
        Screen screen = manager.selectedScreen.GetComponent<Screen>();
        value = ValidateValue(value);
        if (value == null)
        {
            inputFieldStartX.text = screen.startPosition.x.ToString();
            return;
        }

        screen.startPosition.x = (int)float.Parse(value, CultureInfo.InvariantCulture);
        inputFieldStartX.text=screen.startPosition.x.ToString();
        screen.UpdateLedwall();
        UpdateInfo(screen);
    }

    public void UpdateStartY(string value)
    {
        Screen screen = manager.selectedScreen.GetComponent<Screen>();
        value = ValidateValue(value);
        if (value == null)
        {
            inputFieldStartY.text = screen.startPosition.y.ToString();
            return;
        }

        screen.startPosition.y = (int)float.Parse(value, CultureInfo.InvariantCulture);
        inputFieldStartY.text=screen.startPosition.y.ToString();
        screen.UpdateLedwall();
        UpdateInfo(screen);
    }

    public void AddScreen(int i)
    {
        dropdownSelectScreen.options.Add(new TMP_Dropdown.OptionData() { text = "screen " + i });
    }
    public void RemoveScreen(int newSize)
    {
        string elementZero = dropdownSelectScreen.options[0].text;
        dropdownSelectScreen.options.Clear();
        dropdownSelectScreen.options.Add(new TMP_Dropdown.OptionData() { text = elementZero });
        for (int i = 0; i < newSize; i++)
        {
            dropdownSelectScreen.options.Add(new TMP_Dropdown.OptionData() { text = "screen " + (i+1) });
        }
    }
}
