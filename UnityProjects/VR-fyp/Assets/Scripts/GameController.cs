using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class GameController : MonoBehaviour
{

    // the console
    public GameObject consoleObject;
    public GameObject consoleObjectIndicator;

    //holds the transform that contains the physical objects in the starting room
    public Transform startingRoom;

    //a text area to notify the user of events
    public GameObject messageWindow;

    // the parent gameobject which will have the placement indicator as a child
    public GameObject placementObjectHolder;
    //this is the object that displays
    public GameObject placementObject;

    //variables for left controller placement rays
    public XRController leftPlacementRay;
    public InputHelpers.Button placementActivationButton;
    public float activationThreshold = 0.1f;
    public XRRayInteractor leftRay;
    public bool EnableLeftPlacement { get; set; } = true;

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

    //variable for storing state of the trigger pressed when placing objects
    private bool selectState = false;

    //temp, will fix later
    public GameObject portalIndicator;

    // Start is called before the first frame update
    void Start()
    {
        //startProgram = false; //will not start until user presses space bar

        allowedPlace = true;
        placementPoseIsValid = false;
    }

    // Update is called once per frame
    void Update()
    {

        if (allowedPlace)
        {
            //if left trigger is being pressed
            if (CheckIfActivated(leftPlacementRay))
                UpdatePlacementPose();

            //is only true when a pose is valid, and the trigger is no longer pressed
            //meaning the user wants to place
            else if (placementPoseIsValid && selectState )
            {
                PlaceObject();
                placementPoseIsValid = false;
                selectState = false;
            }
        }

        

    }

    private void PlaceObject()
    {
        if (!theConsoleSpawned)
        {
            //place the console object
            theConsoleSpawned = PlaceObjectHelper(placementObject, consoleObject);
            //place console into the starting scene gameobject
            theConsoleSpawned.transform.parent = startingRoom;

        }
        else if (!theMapSpawned && theMap)
        {
            //place the map
            theMapSpawned = PlaceObjectHelper(placementObject, theMap);
            

            //if map placed
            if (theMapSpawned)
            {
                //place map into the starting scene gameobject
                theMapSpawned.transform.parent = startingRoom;
                // let console know we have placed the portal
                theConsoleSpawned.GetComponent<ConsoleController>().MapPlaced(theMapSpawned);
            }

        }
        else if (!thePortalSpawned && thePortal) //make sure that a portal is selected
        {
            //place the portal
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
    private GameObject PlaceObjectHelper(GameObject placementObj, GameObject objToPlace)
    {
        //first check if the placement object is colliding with anything
        if (placementObj.GetComponentInChildren<PlacementHelper>().isSpaceFree)
        {
            allowedPlace = false;

            GameObject temp = null;
            //check if the object that is been placed has already been loaded 
            if (objToPlace.activeInHierarchy)
            {
                //move the object to the correct location
                objToPlace.transform.position = new Vector3(placementObj.transform.position.x, objToPlace.transform.position.y + 20f, placementObj.transform.position.z);
                objToPlace.transform.rotation = placementObj.transform.rotation;
                temp = objToPlace;
            }
            else
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
        theMap = (GameObject)Instantiate(map, new Vector3(0, -20f + map.transform.position.y, 0), Quaternion.identity);

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

    private void UpdatePlacementPose()
    {
        Vector3 pos = new Vector3();
        Vector3 norm = new Vector3();
        int i = 0;
        bool validInteract = false;

        bool isLeftInteractRayHovering = leftRay.TryGetHitInfo(ref pos, ref norm, ref i, ref validInteract);
        if (!isLeftInteractRayHovering)
        {
            leftPlacementRay.gameObject.SetActive(true);
            var cameraForward = Camera.main.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            PlacementPose.rotation = Quaternion.LookRotation(cameraBearing);

                
            placementPoseIsValid = true;
            selectState = true;
        }
        
    }
    public bool CheckIfActivated(XRController controller)
    {
        InputHelpers.IsPressed(controller.inputDevice, placementActivationButton, out bool isActivated, activationThreshold);
        return isActivated;
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
}
