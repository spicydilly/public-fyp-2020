using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
//using UnityEngine.Experimental.XR;
using UnityEngine.XR.ARSubsystems;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject consoleObject;
    public GameObject consoleObjectIndicator;

    // the parent gameobject which will have the placement indicator as a child
    public GameObject placementObjectHolder;
    //this is the object that displays
    public GameObject placementObject;

    //holds the transform that contains the physical objects in the starting room
    public Transform startingRoom;

    Ray ray;
    RaycastHit hit;

    //variables for helping player to find the floor
    private bool floorFoundYet;

    private Pose PlacementPose;
    private ARRaycastManager aRRaycastManager;
    private bool placementPoseIsValid;

    // bool to determine if we have something to place
    public bool allowedPlace;

    //variable storing the UI gameobject, for use displaying messages
    private GameObject theUI;

    //the portal that is being spawned
    private GameObject thePortal;
    //the map that is being spawned 
    private GameObject theMap;

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

    //gameobjects holidng the up&down movement buttons for moving the placement object
    public GameObject upDownBtnsHolder;

    void Start()
    {
        aRRaycastManager = FindObjectOfType<ARRaycastManager>();

        allowedPlace = true;
        placementPoseIsValid = false;

        floorFoundYet = false;

        theUI = GameObject.FindGameObjectWithTag("MainUI");

        //display prompt to user
        DisplayAMessage("Move Phone around to scan floor to begin!", 5f);
    }

    void Update()
    {

        if ( allowedPlace )
        {
            if (!floorFoundYet)
            {
                floorFoundYet = true;
                //make sure movement buttons are enabled
                upDownBtnsHolder.SetActive(true);
            }
            
            UpdatePlacementPose();
            UpdatePlacementIndicator();

        }

        if (Input.touchCount > 0 ) 
        {
            Touch touch = Input.GetTouch(0); //store touch data

            if (touch.phase == TouchPhase.Began) //press begun
            {
                //we now need to check what is being interacted with

                //first checking if the main UI was clicked, would return true if it was
                if (!EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId))
                {
                    //use raycast to detect what is clicked
                    ray = Camera.main.ScreenPointToRay(touch.position);

                    if (theConsoleSpawned && theMapSpawned && Physics.Raycast(ray, out hit))
                    {
                        if (hit.collider.gameObject.CompareTag("MapObject"))
                        {
                            hit.collider.gameObject.GetComponentInParent<MapController>().InteractedWith(hit.collider.gameObject);
                        }

                    }
                    else if (allowedPlace && placementPoseIsValid) // nothing was hit 
                    {
                        PlaceObject();
                    }
                }
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
            //disable movement buttons
            upDownBtnsHolder.SetActive(false);

            GameObject temp = null;
            //check if the object that is been placed has already been loaded 
            if (objToPlace.activeInHierarchy)
            {
                //move the object to the correct location
                objToPlace.transform.position = new Vector3(placementObj.transform.position.x, placementObj.transform.position.y, placementObj.transform.position.z);
                objToPlace.transform.rotation = placementObj.transform.rotation;
                temp = objToPlace;
            }
            else
            {
                temp = Instantiate(objToPlace, placementObj.transform.position, placementObj.transform.rotation);
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
        theMap = (GameObject)Instantiate(map, new Vector3(0, -2000f, 0), Quaternion.identity);

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
        //make sure movement buttons are enabled
        upDownBtnsHolder.SetActive(true);

        //notify the user
        DisplayAMessage("You can now place the map!", 3f);

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
        //make sure movement buttons are enabled
        upDownBtnsHolder.SetActive(true);

        //notify the user
        DisplayAMessage("You can now place the Portal!", 3f);

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
        var screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        aRRaycastManager.Raycast(screenCenter, hits, TrackableType.Planes);

        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid)
        {
            PlacementPose = hits[0].pose;

            var cameraForward = Camera.main.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            PlacementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }

    //this function is called when the clear button has been pressed via the interfacecontroller.cs script
    public void ClearTheScene()
    {

        // check player location, they cannot reset scene if inside portal
        if (currentLoc != 1)
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

        //make sure movement buttons are enabled for placing the console
        upDownBtnsHolder.SetActive(true);

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

    public void DisplayAMessage ( string theMessage, float timeToShow )
    {
        //call the function from the UI gameobject 
        StartCoroutine(theUI.GetComponent<InterfaceController>().DisplayAMessage(theMessage, timeToShow));
    }

    //function called by console controller to inform a map has been selected
    public void MapChosen ( GameObject map )
    {
        //store selected map
        theMap = map;

        //destroy old placement if any
        Destroy(placementObject);

        //set the placement indicator, use prefabs own rotation
        placementObject = (GameObject)Instantiate(map, placementObjectHolder.transform.position, map.transform.rotation);
        // place as child of the placementobjectholder
        placementObject.transform.parent = placementObjectHolder.transform;

        //allow placement
        placementPoseIsValid = false;
        allowedPlace = true;
        
    }

    //functions which control the movement of the placementobject via buttons (up & down)
    public void MoveUpBtn ()
    {
        placementObject.transform.position = new Vector3(placementObject.transform.position.x, placementObject.transform.position.y + 0.1f, placementObject.transform.position.z);
    }
    
    public void MoveDownBtn ()
    {
        placementObject.transform.position = new Vector3(placementObject.transform.position.x, placementObject.transform.position.y - 0.1f, placementObject.transform.position.z);
    }

}
