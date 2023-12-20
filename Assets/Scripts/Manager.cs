using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum View
{
    Pixelmap,
    Power,
    Signal
}

public class Manager : MonoBehaviour
{

    public Color[] pixelmapPalette;
    public Color[] powerPalette;
    public Color[] signalPalette;
    public GameObject screenPrefab;

    public ScreenMenu screenMenu;

    public GameObject selectedScreen { get; private set; }

    Vector2 mouseOldPosition, startClick;

    public View VIEW = View.Pixelmap;

    public List<Screen> screens = new List<Screen>();

    public void Start(){
        screenMenu.UnloadData();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            selectedScreen = null;
            screenMenu.UnloadData();
        }

        if (Input.GetMouseButtonDown(0))
        {
            startClick = Input.mousePosition;
            mouseOldPosition = startClick;
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            selectedScreen = null;
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);                        
            if (hit.transform!=null)
            {
                if (hit.transform.gameObject.GetComponent<Screen>() != null)
                {
                    selectedScreen = hit.transform.gameObject;
                    screenMenu.LoadData(selectedScreen.GetComponent<Screen>());                    
                }
            }
        }

        if (Input.GetMouseButton(0))
        {
            Vector2 mouseDelta = (Vector2)Input.mousePosition - mouseOldPosition;
            Camera.main.transform.Translate(-mouseDelta * Camera.main.orthographicSize / 1000);
            mouseOldPosition = Input.mousePosition;
        }

        if (Input.mouseScrollDelta.y != 0)
        {
            Camera.main.orthographicSize -= Input.mouseScrollDelta.y;
            if (Camera.main.orthographicSize < 1)
            {
                Camera.main.orthographicSize = 1;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (startClick == (Vector2)Input.mousePosition)
            {
                if (selectedScreen == null)
                {
                    screenMenu.UnloadData();
                }
            }
        }
    }

    public void NewScreen()
    {
        GameObject screen = Instantiate(screenPrefab);
        screen.transform.localPosition = Vector3.zero;

        screens.Add(screen.GetComponent<Screen>());
        selectedScreen = screen;
        screenMenu.AddScreen(screens.Count);
        screenMenu.LoadData(screen.GetComponent<Screen>());
    }

    public void DeleteScreen()
    {
        if (selectedScreen == null)
        {
            return;
        }
        Screen screen = selectedScreen.GetComponent<Screen>();
        screens.Remove(screen);
        screenMenu.RemoveScreen(screens.Count);
        Destroy(selectedScreen);
        SelectScreen(0);
    }

    public void ChangeView(int value)
    {
        Debug.Log(value);
        VIEW = (View)value;
        foreach (Screen screen in screens)
        {
            screen.RegenerateTiles();
        }
    }

    public void SelectScreen(int value)
    {
        if (value == 0)
        { selectedScreen = null;
        screenMenu.UnloadData();
        return;
        }
        value--;
        selectedScreen=screens[value].gameObject;
        screenMenu.LoadData(screens[value]);   
    }
}
