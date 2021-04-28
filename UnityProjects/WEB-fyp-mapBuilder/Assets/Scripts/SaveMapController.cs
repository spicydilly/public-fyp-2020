using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SaveMapController : MonoBehaviour
{

    //reference to the main UI 'save' button
    public GameObject mainUISaveBtn;

    //holder of the save map menu
    public GameObject saveMenu;

    //this input holds the name of the map
    public InputField mapNameInput;
    private string mapName = "";
    //input for description
    public InputField mapDescInput;
    private string mapDesc = "";

    //the toggle for publishing the map after saving, 0 is no, 1 is yes
    public Toggle publishToggle;
    //this toggle only shows when the map is a loaded map, it allows the user to instead save a new map rather than overwrite the loaded map
    public GameObject newMapToggle;

    //this Text displays the messages the user needs to be aware of
    public Text messageTextObj;

    //map gameobject holds all of the placed objects as children
    public GameObject map;

    //reference to this mapsID, default is 0, is only used when the map already exists
    public int loadedMapID = 0;

    //the loaded maps data, used if the map was loaded into the scene
    public MapLocationsData loadedMapData;

    GameController gameController;
    CameraController cameraController;

    //referece to the loadmap controller
    LoadMapController loadMapController;

    //calls javascript function on browser
    [DllImport("__Internal")]
    private static extern void SendTheMapJson(string str);

    private void Start()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        cameraController = Camera.main.GetComponent<CameraController>();
        loadMapController = GameObject.FindGameObjectWithTag("GameController").GetComponent<LoadMapController>();
    }

    //function called when the main UI 'save' button has been pressed
    public void MainUISaveBtnPressed ()
    {
        //if map is a loaded map, allow option to save as new
        if (gameController.mapIsLoaded)
            newMapToggle.SetActive(true);

        //disable the button
        mainUISaveBtn.SetActive(false);

        //set the name and descrition fields
        mapNameInput.text = mapName;
        mapDescInput.text = mapDesc;

        //enable the menu
        saveMenu.SetActive(true);

        //tell the gamecontroller that we need to pause 
        gameController.PauseScene(true);
    }

    //function called when the "close" SavePanel Button is pressed
    public void CloseSavePanel()
    {
        //reset the message text
        messageTextObj.text = "Ready to Save Map...";
        //enable save button again
        mainUISaveBtn.SetActive(true);
        //close the save panel ui
        saveMenu.SetActive(false);
        //unpause the scene
        gameController.PauseScene(false);
    }

    //function that is called when the user clicks the "save map" button
    public void SaveMap ()
    {

        //first check if user actually placed objects before saving
        if ( gameController.currentPlacedObjects <= 0)
        {
            messageTextObj.text = "Make sure to place at least 1 location before saving!";
        } 
        //check to make sure that a map name is set
        else if (string.IsNullOrWhiteSpace(mapNameInput.text))
        {
            messageTextObj.text = "Make sure set a Map Name before saving!";
        }
        else
        {
            //save the map name
            mapName = mapNameInput.text;

            //create the json file
            string json = JsonUtility.ToJson(CreateTheJson());

            //if nothing was returned, notify user to fix collsion issues
            if (json == "")
                messageTextObj.text = "A Collsion is present on the Map, make sure to check all placed objects!";
            else
                SendTheMapJson(json); //send json to browser javascript function, what is returned is the message we display
        }

    }


    //this function returns the json file which includes the map locations data
    private MapLocationsData CreateTheJson ()
    {
        //create instance of the MapLocationsData class
        MapLocationsData theMap = new MapLocationsData();

        //set whether map should be published after saving
        if (publishToggle.isOn)
            theMap.publish = 1; //by deafault it is 0

        //create instance of map base
        MapLocations theMapBase = new MapLocations();

        //if the user did not opt to save the map as a new map rather than overwrite the existing loaded map
        // and if the map is a loaded in map, set the id as the loaded mapbase's id, and the maps id
        if ( !newMapToggle.GetComponent<Toggle>().isOn && gameController.mapIsLoaded )
        {
            theMap.map_id = loadedMapID;
            theMapBase.id = loadedMapData.map_base[0].id; //the map base objects id that was loaded, this wouldn't have changed
        }        

        theMapBase.name = mapNameInput.text; // the name of the map the user specified
        theMapBase.desc = mapDescInput.text; //description of the map
        theMapBase.image_url = gameController.theMapImageUrl; //url of the maps image texture
        theMapBase.scalex = theMapBase.scalez = theMapBase.roty = 0f;//set all to 0
        //set the pos x and z to the current max positions of the camera, serverside it is
        // checked for each building to ensure no edits were made to json that were illegal
        // since mapbase is always centered, the minimum is the inverse of the max
        theMapBase.posx = cameraController.maxX;
        theMapBase.posz = cameraController.maxZ;
        //add this isntance
        theMap.map_base.Add(theMapBase);

        //loop through each object inside the mapbase object, as that is where our placed objects reside
        // only looping on the "objectMovementController" components as that controller holds the buildings data
        foreach ( ObjectMovementController obj in map.GetComponentsInChildren<ObjectMovementController>() )
        {
            //if the object has a collsion, return null immediatly 
            if (obj.numColliding != 0)
                return null;

            //create instance of the MapLocations class
            MapLocations location = new MapLocations();

            location.id = obj.obj_id; //objects id, 0 is default
            location.name = obj.obj_name; //set the name
            location.desc = obj.obj_description; //set description
            location.image_url = obj.obj_imageurl; //set image url
            location.posx = obj.transform.parent.position.x;
            location.posz = obj.transform.parent.position.z;
            location.scalex = obj.transform.localScale.x;
            location.scalez = obj.transform.localScale.z;
            location.roty = obj.transform.parent.localRotation.eulerAngles.y;

            //add this instance to the maplocationsdata class 
            theMap.map_locations.Add(location);

        }

        //if the map is a loaded in map, check if objects were deleted from the loaded map
        if (gameController.mapIsLoaded)
            theMap = CheckUpdatedJson(theMap, loadedMapData);

        return theMap;
    }

    //function used for checking for deleted objects from the loaded map
    private MapLocationsData CheckUpdatedJson ( MapLocationsData updatedData, MapLocationsData originalData)
    {
        //loop through the original data, and check if the ID exists in the updated data
        // if not, set the scale to 0 the server will then handle these as deletions
        foreach (MapLocations location in originalData.map_locations)
        {
            if ( !updatedData.map_locations.Exists(x => x.id == location.id) )
            {
                location.scalex = location.scalez = 0f;
                // add this location to the updated data
                updatedData.map_locations.Add(location);
            }
        }

        return updatedData;
    }

    //function called by browser after successful save, the server returns the mapsID so this can be used for resaving the same map
    public void SuccessfulSave ( int savedID )
    {
        Debug.Log("Save Success!");
        //display response
        BrowserSaveResponse("Successfully saved!");

        loadedMapID = savedID;

        //now let application know the map should function as a loaded map, meaning if resaved it will be an update
        loadMapController.LoadMapAfterSaving(savedID);

    }

    //function which handles the response from the browser after saving
    public void BrowserSaveResponse (string message)
    {
        //display the message
        messageTextObj.text = message;
    }

    //function used when a map is loaded, is used to set the maps id, name and description
    // set default values
    public void SavedMapLoadedIn (int loadedID = 0, string loadedName = "", string loadedDesc = "")
    {
        loadedMapID = loadedID;
        mapName = loadedName;
        mapDesc = loadedDesc;
    }

}

//for json parsing
[System.Serializable]
public class MapLocations
{
    //location id, this by deafult is 0 as it is only used when updating existing map
    public int id = 0;
    //object descriptions
    public string name;
    public string desc;
    public string image_url;
    //the positions, scale, and rotation of object
    public float posx;
    public float posz;
    public float scalex;
    public float scalez;
    public float roty;
}

[System.Serializable]
public class MapLocationsData
{
    public List<MapLocations> map_locations = new List<MapLocations>(); // the locations
    public List<MapLocations> map_base = new List<MapLocations>(); // stores references to the mapbase
    public int map_id = 0; //map id is 0 by default, only used when updating an existing map
    public int publish = 0; //when saving map, should it be published, 0 is default
}
