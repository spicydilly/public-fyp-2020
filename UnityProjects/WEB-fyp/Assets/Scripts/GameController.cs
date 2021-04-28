using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    public bool startProgram;

    // the console
    public GameObject consoleObject;
    public GameObject consoleObjectIndicator;

    //holds the entire scene, is disabled until user starts
    public GameObject theScene;

    //holds the transform that contains the physical objects in the starting room
    public Transform startingRoom;

    //players components
    public MouseLook mouseLook;
    public PlayerMovement playerMovement;
    public CharacterController controller;

    //evenr system, must be disabled on load
    public GameObject eventSystem;

    //the starting notifiation
    public GameObject pauseMenu;
    //a text area to notify the user of events
    public GameObject messageWindow;

    // the parent gameobject which will have the placement indicator as a child
    public GameObject placementObjectHolder;
    //this is the object that displays
    public GameObject placementObject;

    Ray ray;
    RaycastHit hit;

    private Pose PlacementPose;
    private bool placementPoseIsValid;

    //the portal that is being spawned
    private GameObject thePortal;
    //the map that is being spawned 
    private GameObject theMap;

    // bool to determine if we have something to place
    public bool allowedPlace;

    // the spawned console
    public GameObject theConsoleSpawned;
    // the spawned portal
    public GameObject thePortalSpawned;
    // the spawned Map
    public GameObject theMapSpawned;

    //track where user is
    public int currentLoc = 1; // 1 is default for starting room, will be set by portalmanager

    //temp, will fix later
    public GameObject portalIndicator;

    // Start is called before the first frame update
    void Start()
    {

        startProgram = false; //will not start until user presses space bar

        allowedPlace = true;
        placementPoseIsValid = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!startProgram)
        {
            if (Input.GetKeyDown("space"))
            {
                //unpause the app
                PauseApp(false);

                //if the console was never places, notify the user 
                if (!theConsoleSpawned)
                {
                    //notify the user
                    StartCoroutine(DisplayAMessage("Try and place the Console!", 3f));
                }
                
            }
        }
        else
        {
            //if user presses "Space" again, pause the app
            if (Input.GetKeyDown(KeyCode.Space))
            {
                PauseApp(true);
            }

            if (allowedPlace)
            {
                UpdatePlacementPose();
                UpdatePlacementIndicator();
            }

            if (Input.GetMouseButtonDown(0))
            {
                ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject.CompareTag("Ground") && allowedPlace && placementPoseIsValid) // nothing was hit 
                    {
                        PlaceObject();
                    }
                    else if (theConsoleSpawned && theMapSpawned && Physics.Raycast(ray, out hit))
                    {
                        if (hit.collider.gameObject.CompareTag("MapObject"))
                        {
                            hit.collider.gameObject.GetComponentInParent<MapController>().InteractedWith(hit.collider.gameObject);
                        }
                        
                    }

                }
            }
        }
        
    }

    private void PlaceObject()
    {
        if (!theConsoleSpawned)
        {
            //try to place the console object
            theConsoleSpawned = PlaceObjectHelper(placementObject, consoleObject);
            //place console into the starting scene gameobject
            theConsoleSpawned.transform.parent = startingRoom;

        }
        else if (!theMapSpawned && theMap)
        {
            //try place the map
            theMapSpawned = PlaceObjectHelper(placementObject, theMap);

            //if map placed
            if ( theMapSpawned )
            {
                //place map into the starting scene gameobject
                theMapSpawned.transform.parent = startingRoom;
                // let console know we have placed the portal
                theConsoleSpawned.GetComponent<ConsoleController>().MapPlaced(theMapSpawned);
            }
            
        }
        else if (!thePortalSpawned && thePortal) //make sure that a portal is selected
        {
            //try to place the portal
            thePortalSpawned = PlaceObjectHelper(placementObject, thePortal);

            //if portal placed
            if (thePortalSpawned)
            {
                // let console know we have placed the portal
                theConsoleSpawned.GetComponent<ConsoleController>().PortalPlaced(thePortalSpawned);
            }
            
        }

    }

    //function that places the object, and returns the placed object
    private GameObject PlaceObjectHelper ( GameObject placementObj, GameObject objToPlace )
    {
        //first check if the placement object is colliding with anything
        if (placementObj.GetComponentInChildren<PlacementHelper>().isSpaceFree)
        {
            allowedPlace = false;

            GameObject temp = null;
            //check if the object that is been placed has already been loaded 
            if ( objToPlace.activeInHierarchy )
            {
                //move the object to the correct location
                objToPlace.transform.position = new Vector3(placementObj.transform.position.x, objToPlace.transform.position.y + 20f , placementObj.transform.position.z);
                objToPlace.transform.rotation = placementObj.transform.rotation;
                temp = objToPlace;
            } else
            {
                temp = Instantiate(objToPlace, new Vector3(placementObj.transform.position.x, objToPlace.transform.position.y, placementObj.transform.position.z), placementObj.transform.rotation);
            }

            //removing "(clone)" from object name, it is used in UI
            temp.name = objToPlace.name;

            //reset the placement object
            Destroy(placementObj);
            //reset the rotation of the placement object holder
            placementObjectHolder.transform.rotation = Quaternion.identity;

            return temp;
        }

        //no placement so returning no object
        return null;
    
    }

    //function called by console controller to inform a map has been selected
    public void MapChosen(GameObject map, GameObject indicator)
    {
        //spawn the map, in a location not visible the user
        theMap = (GameObject) Instantiate(map, new Vector3(0, -20f + map.transform.position.y, 0), Quaternion.identity);

        //destroy old placement if any
        Destroy(placementObject);
        //reset the rotation of the placement object holder
        placementObjectHolder.transform.rotation = Quaternion.identity;

        //set the placement indicator, use prefabs own rotation
        placementObject = (GameObject)Instantiate(indicator, placementObjectHolder.transform.position, indicator.transform.rotation);
        // place as child of the placementobjectholder
        placementObject.transform.parent = placementObjectHolder.transform;

        //allow placement
        placementPoseIsValid = false;
        allowedPlace = true;

        //notify the user
        StartCoroutine(DisplayAMessage("You can now place the map!", 3f));

    }

    //function called by console controller to inform a portal has been selected
    public void PortalChosen(GameObject portal)
    {
        //store selected map
        thePortal = portal;

        //destroy old placement if any
        Destroy(placementObject);
        //reset the rotation of the placement object holder
        placementObjectHolder.transform.rotation = Quaternion.identity;

        //set the placement indicator
        placementObject = (GameObject)Instantiate(portalIndicator, placementObjectHolder.transform.position, portalIndicator.transform.rotation);
        // place as child of the placementobjectholder
        placementObject.transform.parent = placementObjectHolder.transform;

        //allow placement
        placementPoseIsValid = false;
        allowedPlace = true;

        //notify the user
        StartCoroutine(DisplayAMessage("You can now place the Portal!", 3f));

    }

    //this function clears objects from the scene
    public void ClearTheScene()
    {

        // check player location, they cannot reset scene if inside portal
        if (currentLoc == 1)
            return;

        //reset values to starting positions and delete gameobjects
        Destroy(theConsoleSpawned);

        // clear spawned portal and map
        CloseMap();
        ClosePortal();

        placementPoseIsValid = false;
        allowedPlace = true;

        //destroy old placement if any
        Destroy(placementObject);
        //reset the rotation of the placement object holder
        placementObjectHolder.transform.rotation = Quaternion.identity;

        //scene cleared so console must be placed, using prefabs rotation
        placementObject = (GameObject)Instantiate(consoleObjectIndicator, placementObjectHolder.transform.position, consoleObject.transform.rotation);
        // place as child of the placementobjectholder
        placementObject.transform.parent = placementObjectHolder.transform;

        Debug.Log("Scene Reset");

    }

    //function to remove the loaded portal
    public bool ClosePortal()
    {
        //check if portal exists
        if (thePortalSpawned)
        {
            Debug.Log("Removing Portal");
            Destroy(thePortalSpawned);

            return true;
        }
        else
        {
            Debug.Log("No portal exists");
            return false;
        }

    }

    //function to remove the loaded Map
    public bool CloseMap()
    {
        //check if map exists
        if (theMapSpawned)
        {
            Debug.Log("Removing Map");
            Destroy(theMapSpawned);

            return true;

        }
        else
        {
            Debug.Log("No map exists");
            return false;
        }

    }

    private void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid)
        {
            placementObjectHolder.SetActive(true);
            placementObjectHolder.transform.SetPositionAndRotation(PlacementPose.position, PlacementPose.rotation);
        }
        else
        {
            placementObjectHolder.SetActive(false);
        }
    }

    private void UpdatePlacementPose()
    {
        ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.CompareTag("Ground") && allowedPlace)
            {
                placementPoseIsValid = true;

                PlacementPose.position = hit.point;

                var cameraForward = Camera.main.transform.forward;
                var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
                PlacementPose.rotation = Quaternion.LookRotation(cameraBearing);
            }
        }
               
    }

    //function that flashes a brief message on center of screen
    public IEnumerator DisplayAMessage(string messageText, float t)
    {
        messageWindow.SetActive(true);
        messageWindow.GetComponent<Text>().text = messageText;

        //wait specified time
        yield return new WaitForSeconds(t);

        messageWindow.SetActive(false);
    }

    //function which handles pausing of the application
    // true means to pause the app, false means unpause the app
    public void PauseApp (bool pause)
    {
        if (pause)
        {
            //free cursor
            Cursor.lockState = CursorLockMode.None;
        } else
        {
            //lock the cursor
            Cursor.lockState = CursorLockMode.Locked;
        }

        //input module
        eventSystem.SetActive(!pause);

        //allows for detection of spacebar to unpause in update()
        startProgram = !pause;

        theScene.SetActive(!pause);

        //enable or disable the players components
        mouseLook.enabled = !pause;
        playerMovement.enabled = !pause;
        controller.enabled = !pause;

        //the pause menu
        pauseMenu.SetActive(pause);

    }

    //this function is called by the webpage, it is called when either input is needed on the html
    // page or not , input is an int as booleans are not supported by unity web cross scripting
    public void InputNeeded ( int status )
    {
        bool value = true; // default
        if ( status == 0 )
        {
            value = false;
            PauseApp(true); // pause the app as input is now directed to the html
        }

        //this is needed as there exists a "sign in" modal that appears on the website, and without 
        // this piece of code, input on html isn't captured as WebGL is capturing all input
        // regardless of whether it is in focus or not
        // false disables the webgl input
#if !UNITY_EDITOR && UNITY_WEBGL
        // disable WebGLInput.captureAllKeyboardInput so elements in web page can handle keabord inputs
        WebGLInput.captureAllKeyboardInput = value;
#endif
    }

}
