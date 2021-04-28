using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WristMenuController : MonoBehaviour
{

    // the menu game objects
    public GameObject menu; // this is the menus placeholder, clicking this will open mainmenu
    public GameObject mainMenu; // this is always the first menu
    //public GameObject secondaryMenu;


    //the buttons on the menu that trigger events
    public GameObject clearOption; // sits under secondary menu


    //booleans for tracking what is open
    private bool mainMenuOpen;
    //private bool secondaryMenuOpen;

    //game manager
    private GameObject gameManager;

    private void Start()
    {

        gameManager = GameObject.FindGameObjectWithTag("GameController");

    }

    public void MenuClickedOn(GameObject userClickedOn)
    {

        if (userClickedOn == menu)
        {

            // check it isn't already open
            if (!mainMenuOpen)
            {
                mainMenuOpen = true;

                //display the mainmenu
                mainMenu.SetActive(true);

                //stop displaying the first menu
                menu.SetActive(false);
            }

        }
        else if (userClickedOn == clearOption)
        {
            // does not clear the scene right now, just sets the portals to active=false
            gameManager.GetComponent<ConsoleController>().ClearTheScene();
        }

    }

    public void NoLongerHovering ()
    {
        // player stopped hovering on the wirst menu so disable after 5 secs
        StartCoroutine(ResetMenuTimer(5f));
    }

    public IEnumerator ResetMenuTimer(float t)
    {
        //wait 5 seconds
        yield return new WaitForSeconds(5f);

        //now reset the menus
        ResetTheMenus();
        

    }

    //this function resets the menus
    public void ResetTheMenus ()
    {
        menu.SetActive(true);
        mainMenu.SetActive(false);
        mainMenuOpen = false;
    }

}
