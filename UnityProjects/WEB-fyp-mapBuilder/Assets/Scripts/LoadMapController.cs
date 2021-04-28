using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class LoadMapController : MonoBehaviour
{
    private AllMaps loadedMaps;

    private MapLocationsData theLoadedMap;

    //the prefab used when adding objects
    public GameObject defaultLoadObj;

    //dropdown showing maps available for loading
    public Dropdown mapsToLoadDropdown;

    //the load map button
    public GameObject loadMapButton;

    //the text that displays the maps description
    public Text mapDescriptionText;

    //map holding gameobject, where map will be instatiated to on load
    public GameObject mapObj;

    //stores the chosen maps data
    private int loadedMapID = 0;
    private string loadedMapName;
    private string loadedMapDesc;

    private GameController gameController;
    private SaveMapController saveMapController;

    //calls javascript function on browser
    [DllImport("__Internal")]
    private static extern void LoadMaps();

    [DllImport("__Internal")]
    private static extern void LoadThisMap(int id);

    public void Start()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        saveMapController = GameObject.FindGameObjectWithTag("GameController").GetComponent<SaveMapController>();

        //request browser to fetch the users created maps
        LoadMaps();

    }

    public void AllTheMaps ( string mapsData )
    {

        //load the data into the AllMaps class
        loadedMaps = JsonUtility.FromJson<AllMaps>(mapsData);

        List<string> mapNames = new List<string>();
        //add these locations to string array to later be added to the dropdown
        foreach ( MapDetails map in loadedMaps.maps )
        {
            mapNames.Add(map.name);
        }

        //if maps exist, allow user to load them
        if ( mapNames.Any() )
        {
            //add to dropdown
            mapsToLoadDropdown.AddOptions(mapNames);
            //enable dropdown and load button
            mapsToLoadDropdown.interactable = true;
            loadMapButton.GetComponent<Button>().interactable = true;
            //set the description as the selected entry
            int selectedIndex = mapsToLoadDropdown.value;
            mapDescriptionText.text = loadedMaps.maps.Find(x => x.name == mapsToLoadDropdown.options[selectedIndex].text).desc;
        }
        
        
    }

    //called when loadmap button pressed
    public void LoadThisMapDropdown (Dropdown loadMap)
    {
        //if a map isn't loaded already
        if ( !gameController.mapIsLoaded )
        {
            //disable the load map button
            loadMapButton.GetComponent<Button>().interactable = false;
            //disable the dropdown
            loadMap.interactable = false;

            //let the game controller know that a map is loaded
            gameController.mapIsLoaded = true;

            //get the chosen map name 
            int selectedIndex = loadMap.value;
            string selectedMap = loadMap.options[selectedIndex].text;

            //store the data of the selected map
            loadedMapID = loadedMaps.maps.Find(x => x.name == selectedMap).locations_json;
            loadedMapName = selectedMap;
            loadedMapDesc = loadedMaps.maps.Find(x => x.name == selectedMap).desc;

            //load map into scene
            LoadThisMap(loadedMapID);

            //edit the loadmapbutton to allow the user to unload a map
            loadMapButton.GetComponentInChildren<Text>().text = "Unload";
            //enable the load map button
            loadMapButton.GetComponent<Button>().interactable = true;
        } else //map is loaded already, so unload the current map
        {
            //disable the load map button
            loadMapButton.GetComponent<Button>().interactable = false;

            //clear the objects from the scene
            gameController.ClearTheScene();

            //edit the loadmapbuttons text
            loadMapButton.GetComponentInChildren<Text>().text = "Load";
            //enable the load map button
            loadMapButton.GetComponent<Button>().interactable = true;
            //enable the dropdown
            loadMap.interactable = true;

            //let the game controller know that a map is no longer loaded
            gameController.mapIsLoaded = false;

        }
        
    }

    public void MapJson ( string mapData )
    {
        //load the data into the MapLocationsData class
        theLoadedMap = JsonUtility.FromJson<MapLocationsData>(mapData);
        // save map data to the save map controller
        saveMapController.loadedMapData = theLoadedMap;

        //set the maps id, name and description
        saveMapController.SavedMapLoadedIn(loadedMapID, loadedMapName, loadedMapDesc);

        //set the map base image ( if any )
        if (theLoadedMap.map_base[0].image_url != "")
        {
            gameController.theMapImageUrl = theLoadedMap.map_base[0].image_url; //not fully loaded in until user clicks "begin"
        }

        GameObject obj = null;
        gameController.currentPlacedObjects = 0;
        //instatiate each object in the loaded map 
        foreach (MapLocations location in theLoadedMap.map_locations)
        {
            obj = (GameObject)Instantiate(defaultLoadObj, new Vector3(location.posx, 0, location.posz), Quaternion.Euler(new Vector3(0, location.roty, 0)));
            //set the scale
            obj.GetComponentInChildren<ObjectMovementController>().transform.localScale = new Vector3(location.scalex, 1, location.scalez);

            //set parent
            obj.transform.parent = mapObj.transform;

            //add name and description to the object
            obj.GetComponentInChildren<ObjectMovementController>().LoadedIn(location.id, location.name, location.desc, location.image_url);
            gameController.currentPlacedObjects++;
        }

        //set the last added object as the selected object
        if (obj)
            gameController.SelectedAObject(obj);
        

    }

    //function called when the load dropdown changes value, is used to update the description
    public void LoadDropdownChange ( Dropdown theDropdown )
    {
        //get the chosen map name 
        int selectedIndex = theDropdown.value;
        string selectedMap = theDropdown.options[selectedIndex].text;

        //geth the chosen maps description
        string theDesc = loadedMaps.maps.Find(x => x.name == selectedMap).desc;

        //update the description
        mapDescriptionText.text = theDesc;

    }

    //function called after a map has been saved, in order to fetch the full updated json data so user can resave without refreshing
    public void LoadMapAfterSaving ( int mapIDToLoad )
    {
        //clear the objects from the scene, special case after a save
        gameController.ClearTheScene(true);

        loadedMapID = mapIDToLoad;
        loadedMapName = saveMapController.mapNameInput.text;
        loadedMapDesc = saveMapController.mapDescInput.text;

        //load map json
        LoadThisMap(mapIDToLoad);

        //let the game controller know that a map is loaded
        gameController.mapIsLoaded = true;
    }

}

//for json parsing
[System.Serializable]
public class MapDetails
{
    //object descriptions
    public string name;
    public string desc;
    public int locations_json;

}

[System.Serializable]
public class AllMaps
{
    public List<MapDetails> maps = new List<MapDetails>(); // the locations
}
