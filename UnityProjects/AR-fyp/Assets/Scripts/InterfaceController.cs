using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceController : MonoBehaviour
{

    public Button menuBtn;
    public Button clearBtn; // this is to clear current portals and controllers from scene
    public Button backBtn;

    public GameObject subMenu1;

    public bool menuOpen;
    public bool subMenuOpen;

    //general use message window
    public GameObject messageWindow;

    //game manager
    private GameController gameController;

    // Start is called before the first frame update
    void Start()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        //add listeners to the buttons to detect clicks
        menuBtn.onClick.AddListener(OpenMenu);
        backBtn.onClick.AddListener(OpenMenu);
        clearBtn.onClick.AddListener(ClearButtonClicked);
        menuOpen = subMenuOpen = false; // menus will always be closed on load
    }

    void OpenMenu ()
    {
        if (menuOpen)
        {
            //close the menu
            menuOpen = false;
            menuBtn.gameObject.SetActive(true);
            subMenu1.SetActive(false);
        } else
        {
            //open the menu
            menuOpen = true;
            menuBtn.gameObject.SetActive(false);
            subMenu1.SetActive(true);
        }
    }

    void ClearButtonClicked ()
    {
        //clear current portals and controllers from scene
        gameController.ClearTheScene();

    }

    //function that flashes a brief message on center of screen
    public IEnumerator DisplayAMessage (string messageText,  float t )
    {
        messageWindow.SetActive(true);
        messageWindow.GetComponent<Text>().text = messageText;

        //wait specified time
        yield return new WaitForSeconds(t);

        messageWindow.SetActive(false);
    }
}
