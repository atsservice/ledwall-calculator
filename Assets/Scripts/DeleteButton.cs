using UnityEngine;
using UnityEngine.UI;

public class DeleteButton : MonoBehaviour
{
    bool confirm=false;
    bool clickedOnButton = false;
    Manager manager;

    // Start is called before the first frame update
    void Start()
    {
        manager=FindObjectOfType<Manager>(); 
    }        

    void OnEnable(){
        confirm=false;
        clickedOnButton=false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)|| (Input.GetMouseButtonUp(0) && !clickedOnButton)){
            confirm=false;            
        }
        clickedOnButton=false;

        if(confirm){
            GetComponent<Image>().color=Color.red;
            GetComponentInChildren<Text>().text="Click Again To Confirm";        
        }else{
            GetComponent<Image>().color=Color.white;
            GetComponentInChildren<Text>().text="Delete Screen";        
        }
    }
    
    public void DeleteScreen(){
        clickedOnButton=true;
        if(confirm){
            manager.DeleteScreen();
            confirm=false;
        }else{
            confirm=true;
        }
    }
}
