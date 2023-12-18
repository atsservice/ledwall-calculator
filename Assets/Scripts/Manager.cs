using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Manager : MonoBehaviour
{
    public GameObject screenPrefab;

    public GameObject screenMenu;

    public GameObject selectedScreen { get; private set; }

    Vector2 mouseOldPosition, startClick;

    public void Start()
    {
        CloseScreenMenu();
    }

    public void Update()
    {
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
                    OpenScreenMenu();
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
                    CloseScreenMenu();
                }
            }
        }
    }

    public void NewScreen()
    {
        GameObject screen = Instantiate(screenPrefab);
        screen.transform.localPosition = Vector3.zero;
    }

    void OpenScreenMenu()
    {
        screenMenu.SetActive(true);
        screenMenu.transform.position = Camera.main.WorldToScreenPoint(selectedScreen.transform.position+selectedScreen.transform.localScale);
    }

    void CloseScreenMenu()
    {
        screenMenu.SetActive(false);
    }
}
