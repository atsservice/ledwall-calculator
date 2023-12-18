using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Manager : MonoBehaviour
{
    public GameObject screenPrefab;

    public GameObject screenMenu;

    public GameObject selectedScreen { get; private set; }

    public void Start()
    {
        CloseScreenMenu();
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
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
        if (selectedScreen == null)
        {
            CloseScreenMenu();
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
