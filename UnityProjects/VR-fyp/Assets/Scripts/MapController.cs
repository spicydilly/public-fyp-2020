using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.ExceptionServices;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MapController : MonoBehaviour
{

    private MapLocationsData mapLocationsJson;

    //the game controller
    private GameController gameController;
    //the console controller
    private ConsoleController consoleController;

    // variables for the UI of the map
    public Canvas theCanvas;
    public Text textName;
    public Text textDescription;
    public Button btn360;

    // stores the url for the image
    private string image360URL;

    //materials
    public Material selectedBuilding;
    public Material unselectedBuilding;

    //variable for storing the last selected building
    private GameObject lastSelected;

    //the prefab used as the buildings
    public GameObject defaultObj;
    //the map base, the image will be applied here
    public GameObject mapImageBase;
    //gameobject where the buildings will be instatiated under
    public GameObject mapBuildingHolder;

    // Start is called before the first frame update
    void Start()
    {

        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        consoleController = GameObject.FindGameObjectWithTag("ConsoleController").GetComponent<ConsoleController>();

        //set the event camera for the UI
        theCanvas.GetComponent<Canvas>().worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>() ;

        StartCoroutine(LoadMapJson(consoleController.mapLocationsID));

    }

    void UpdateDropdown ()
    {
        List<string> validLocations = new List<string>();
        //for storing the index of the option in the dropdown
        int i = 0;
        //iterate through locations and send to dropdown if a url exists
        foreach (MapLocations location in mapLocationsJson.map_locations)
        {

            if (location.image_url != "null" && location.image_url != "")
            {
                // store as acceptable for the dropdown
                validLocations.Add(location.name);

                location.dropdownIndex = i; //store index of the locaiton in the dropdown
                i++; //increment index location
            }
        }

        //once complete send to the console
        consoleController.PortalsDropdownLocationsUpdate(validLocations);
    }

    // pass in the object which was selected and decide what to do
    public void InteractedWith ( GameObject obj )
    {

        MapLocations result = mapLocationsJson.map_locations.Find(x => x.id == Int32.Parse(obj.name));
        if (result == null)
        {
            Debug.LogError(obj.name + " has no online reference - ID NOT IN JSON");
        }
        else 
        {

            // set the canvas
            textName.text = result.name;
            textDescription.text = result.desc;
            theCanvas.gameObject.SetActive(true);

            // change the material to show it is selected
            obj.GetComponent<Renderer>().material = selectedBuilding;
            
            if ( lastSelected && obj != lastSelected )
            {
                //change the previous selected building back to normal
                lastSelected.GetComponent<Renderer>().material = unselectedBuilding;
            }
            //set last selected to the current object
            lastSelected = obj;

            if (result.image_url == "null" || result.image_url == "")
            {
                Debug.Log(result.name + " has no 360 image");
                // no 360 image so disable button
                btn360.gameObject.SetActive(false);
                // reset stored url
                image360URL = "";
            } else
            {
                // save 360 image url which user can send to console in btn360Selected()
                image360URL = result.image_url;
                Debug.Log(result.name + " has an Image! Stored 360 Image URL : " + result.image_url);
                btn360.gameObject.SetActive(true);
            }
        }
                    
    }

    // called when 360 image btn selected
    public void btn360Selected ( )
    {
        // send url to gamecontroller for storing
        consoleController.SetTheURL(image360URL);
    }

    // this is called by the console if the user tries to load a URL via the console
    // returns the URL of the request location
    public string GetTheURL(int locationIndex)
    {

        string tempUrl = "";
        MapLocations result = mapLocationsJson.map_locations.Find(x => x.dropdownIndex == locationIndex);
        if (result == null)
        {
            Debug.LogError(locationIndex + " has no online reference - ID NOT IN JSON");
        }
        else
        {
            if (result.image_url == "null" || result.image_url == "")
            {
                Debug.Log(result.id + " has no 360 image");
            }
            else
            {
                // save 360 image url which user can send to console in btn360Selected()
                tempUrl = result.image_url;
                Debug.Log(result.id + " has an Image! Stored 360 Image URL : " + result.image_url);
            }
        }

        return tempUrl;

    }

    // A local json file is stored, but we can check the online resources to see if there is a more
    //up to date version and replace ours if neccessary
    private IEnumerator LoadMapJson(int mapJsonID)
    {
        // url of the online json file
        string theUrl = "https://thedylon.com/fyp2020/getjsonofmap.php?id=" + mapJsonID;

        UnityWebRequest request = UnityWebRequest.Get(theUrl);

        //setting a default user-agent in the header, without this the webhost will return 403 forbidden error
        request.SetRequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:69.0) Gecko/20100101 Firefox/69.0");

        Debug.Log("Loading Json file at URL : " + theUrl);
        yield return request.SendWebRequest(); // this can take time to load
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error); // log the error, default local json will be used
        else
        {
            Debug.Log("Loading Json file completed");
            // replacing the local variable to point to the updated json from web
            mapLocationsJson = JsonUtility.FromJson<MapLocationsData>(((DownloadHandler)request.downloadHandler).text);
            //populate the console dropdown
            UpdateDropdown();
            Debug.Log("Now using online Json file");
        }

        //special case is for ID 1, this map is locally stored
        if (mapJsonID != 1)
        {
            //after map data is loaded, build the map
            BuildTheMap();
        }

    }

    //function called to build the map from the loaded data
    public void BuildTheMap()
    {
        //set the map base image ( if any )
        if (mapLocationsJson.map_base[0].image_url != "")
        {
            StartCoroutine(LoadImageCoroutine(mapLocationsJson.map_base[0].image_url)); //load the image
        }

        GameObject obj = null;
        //instatiate each object in the loaded map 
        foreach (MapLocations location in mapLocationsJson.map_locations)
        {
            obj = (GameObject)Instantiate(defaultObj);
            //set parent
            obj.transform.parent = mapBuildingHolder.transform;
            //reset location so future movements are relative to the parent
            //set location
            obj.transform.localPosition = new Vector3(location.posx, 0, location.posz);
            obj.transform.localRotation = Quaternion.Euler(new Vector3(0, location.roty, 0));
            //set the scale
            obj.transform.localScale = new Vector3(location.scalex, 5, location.scalez);

            //set the objects name as the location ID as that is always unique
            obj.name = location.id.ToString();

        }

    }

    private IEnumerator LoadImageCoroutine(string mapImageUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture("https://www.thedylon.com/fyp2020/" + mapImageUrl);

        //setting a default user-agent in the header, without this the webshost will return 403 forbidden error
        request.SetRequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:69.0) Gecko/20100101 Firefox/69.0");

        Debug.Log("Loading image at URL : " + mapImageUrl);
        yield return request.SendWebRequest(); // this can take time to load
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
        {
            Debug.Log("Loading completed");
            Texture2D theTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            //lets scale the map base so the aspect ratio is correct on the quad
            //technically scaling the parent as we want all attached quads for the map
            // base to scale
            float ratio = (float)theTexture.height / (float)theTexture.width;
            float x = mapImageBase.transform.parent.transform.localScale.x; //starting scale
            float z = x * ratio;
            mapImageBase.transform.parent.transform.localScale = new Vector3(x, mapImageBase.transform.parent.localScale.y, z); // set new scale
            //set scale of the indicator object also
            gameController.placementObject.transform.GetChild(0).transform.localScale = new Vector3(x, mapImageBase.transform.parent.localScale.y, z);
            mapImageBase.GetComponent<Renderer>().material.mainTexture = theTexture;
        }

    }
}

//for json parsing
[System.Serializable]
public class MapLocations
{
    //location id
    public int id;
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

    //special variable which holds the index location of where this location is added in the consoles dropdown menu
    public int dropdownIndex = -1; //-1 as this isn't a valid dropdown location so is great as a default
}

[System.Serializable]
public class MapLocationsData
{
    public List<MapLocations> map_locations = new List<MapLocations>(); // the locations
    public List<MapLocations> map_base = new List<MapLocations>(); // stores references to the mapbase
}


