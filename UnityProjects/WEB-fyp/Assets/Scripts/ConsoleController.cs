using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ConsoleController : MonoBehaviour
{

    //reference to the gamecontroller
    GameController gameController;

    private Maps allMapsJson;
    //storing id of the chosen map, is referenced by the map controller
    public int mapLocationsID;

    // the spawned portal
    public GameObject theChosenPortal;
    // the chosen Map
    public GameObject theChosenMap;
    private GameObject theChosenMapIndicator; //indicator

    //name of selected location for portal
    public string selectedLocationName;

    //variables for loading the correct portal urls
    public string urlToLoad;

    //variables for interface
    public bool mainMenuOpen;
    public bool mapLoadMenuOpen;
    public bool portalsMenuOpen;

    //the UI status bar
    public Text infoMap;
    public Text infoPortal;
    public Button closePortal;
    public Button closeMap;

    //btns
    public Button backBtn; // this button is only active on specific pages and is shared
    public Button portalsMenuBtn; // this is only enables after a map has been chosen

    //dropdowns
    public Dropdown portalLocations;
    public Dropdown mapLocations;

    //variables holding the helper text
    public GameObject portalWarningText;
    public GameObject mapWarningText;

    //objects holding different menu states
    public GameObject mainMenuUI;
    public GameObject mapLoadUI;
    public GameObject portalsUI;

    //stores the dropdowns rect for manual scrolling
    private ScrollRect scrollRect;
    private bool scrollUp = false;
    private bool scrollDown = false;

    // variables for the UI of the map
    public Canvas theCanvas;

    //holds the prefab objects
    public GameObject mapPrefab;
    public GameObject mapIndicatorPrefab;


    public void Start()
    {

        //find the gamecontroller
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        //set the event camera for the UI
        theCanvas.GetComponent<Canvas>().worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        // set the menu variables on load
        mainMenuOpen = true;
        mapLoadMenuOpen = portalsMenuOpen = false;

        // fetch the maps data
        StartCoroutine(LoadMapJson());

    }

    // for scrolling the dropdowns
    private void Update()
    {
                
        if (scrollUp )
        {
            //Vector3 pos = new Vector3(scrollRect.content.localPosition.x, scrollRect.content.localPosition.y + (Mathf.Sin(Time.time * 10f) * 10f), 0f);
            //scrollRect.content.localPosition = pos;
            scrollRect.verticalNormalizedPosition += 0.1f * Time.deltaTime;
        } else if ( scrollDown )
        {
            //Vector3 pos = new Vector3(scrollRect.content.localPosition.x, scrollRect.content.localPosition.y + (Mathf.Sin(Time.time * 10f) * 10f), 0f);
            //scrollRect.content.localPosition = pos;
            scrollRect.verticalNormalizedPosition -= 0.1f * Time.deltaTime;
        }
    }

    // called when a portal is requested to be opened
    public void PlaceAPortalDropdown()
    {

        // check if portal already spawned
        if (gameController.thePortalSpawned)
        {
            // portal already spawned
            Debug.Log("Portal already spawned");
            //display message for 5 seconds
            StartCoroutine(DisplayMessage(portalWarningText, 5f));
        }
        else
        {
            //get the selected dropdown item index
            int selectedLocationIndex = portalLocations.value;

            PlacePortal(selectedLocationIndex);
        }

    }

    // function lets the gamecontroller know the chosen portal
    public void PlacePortal(int locationDropdownIndex)
    {
        // get the URL of the selected location
        urlToLoad = gameController.theMapSpawned.GetComponent<MapController>().GetTheURL(locationDropdownIndex);

        Debug.Log("Portal can now be placed");
        //send to game manager
        gameController.PortalChosen(theChosenPortal);

    }

    //this is called when the user clicks on the 360 button on the maps UI
    public void Btn360MapHelper ( string urlToUse )
    {
        urlToLoad = urlToUse;

        Debug.Log("Portal can now be placed");
        //send to game manager
        gameController.PortalChosen(theChosenPortal);
    }

    //function which controls UI of the map load menu
    public void MapLoadMenu ()
    {

        mainMenuOpen = false;
        mapLoadMenuOpen = true;

        //disable the mainmenu UI
        mainMenuUI.SetActive(false);

        //check if a map is already loaded 
        if (gameController.theMapSpawned)
        {
            portalsMenuBtn.gameObject.SetActive(true);
        }
        else
        {
            portalsMenuBtn.gameObject.SetActive(false);
        }

        //enable the correct UI
        mapLoadUI.SetActive(true);
        //enable the backbtn
        backBtn.gameObject.SetActive(true);

    }

    //fucntion which controls the UI of the portal load menu, loads via the maploadmenu
    public void PortalsMenu ()
    {
        mapLoadMenuOpen = false;
        portalsMenuOpen = true;

        //disable the mainmenu UI
        mapLoadUI.SetActive(false);

        //enable the correct UI
        portalsUI.SetActive(true);
        //enable the backbtn
        backBtn.gameObject.SetActive(true);

    }

    //fucntion which supports the use of the back button on the UI
    public void BackButtonClicked ()
    {
        //disable the menus which are active
        if ( mapLoadMenuOpen)
        {
            mapLoadMenuOpen = false;
            mapLoadUI.SetActive(false);
        } 
        else if ( portalsMenuOpen )
        {
            portalsMenuOpen = false;
            portalsUI.SetActive(false);
        }

        //disable the back btn as it shouldn't be active on main menu
        backBtn.gameObject.SetActive(false);

        //now enable main menu UI
        mainMenuOpen = true;
        mainMenuUI.SetActive(true);

    }

    //function for loading the chosen map
    public void PlaceChosenMap(GameObject selectedMap)
    {

        // check if a map is already loaded 
        if (gameController.theMapSpawned)
        {
            Debug.Log("Map already loaded");

            //display message for 5 seconds
            StartCoroutine(DisplayMessage(mapWarningText, 5f));
        }
        else
        {
            //get the chosen map name to reference the loaded maps, mapnames are unique
            int selectedMapIndex = selectedMap.GetComponent<Dropdown>().value;
            string selectedMapName = selectedMap.GetComponent<Dropdown>().options[selectedMapIndex].text;

            mapLocationsID = allMapsJson.maps.Find(x => x.name == selectedMapName).locations_json;

            Debug.Log("Loading Map : " + selectedMapName);
            //set the text on the consoles UI
            infoMap.text = selectedMapName;

            //special case, if map id is 1 it is locally stored
            if ( mapLocationsID == 1 )
            {
                //now load matching map from resources
                theChosenMap = Resources.Load(selectedMapName) as GameObject;
                //load the maps indicator for placement
                theChosenMapIndicator = Resources.Load(selectedMapName + " Indicator") as GameObject; ;
                //send to game manager
                gameController.MapChosen(theChosenMap, theChosenMapIndicator);
            }
            else
            {
                gameController.MapChosen(mapPrefab, mapIndicatorPrefab);
            }

        }

    }

    //gamecontroller will call this after placing a portal
    public void MapPlaced(GameObject theMap)
    {
        //map is now loaded, so enable the option to view portals
        portalsMenuBtn.gameObject.SetActive(true);

        //set the status UI button to allow deletion
        closeMap.gameObject.SetActive(true);
    }

    // this function resets the scene by disabling the portals
    public void ClearTheScene ()
    {
        //function is implemented in the gamemanager
        gameController.ClearTheScene();

    }

    //function for setting and getting the url to be loaded, redundant right now but will be customized
    public void SetTheURL ( string theUrl )
    {
        urlToLoad = theUrl;
        Debug.Log("URL : " + theUrl + " stored for loading.");
    }

    //function for updating the portal locations dropdown
    public void PortalsDropdownLocationsUpdate(List<string> validLocations)
    {
        //first clear the current options
        portalLocations.ClearOptions();

        //now lets add our new locations
        portalLocations.AddOptions(validLocations);
    }

    //function to remove the loaded portal
    public void ClosePortal()
    {

        //function is implemented in game controller, returns true if suceeded
        if (gameController.ClosePortal())
        {
            closePortal.gameObject.SetActive(false);//disable button
            infoPortal.text = "None"; //reset status text

            BackButtonClicked(); // back to main menu if not already there
        }

    }

    //function to remove the loaded Map
    public void CloseMap()
    {
        //function is implemented in game manager, returns true if suceeded
        if (gameController.CloseMap())
        {
            closeMap.gameObject.SetActive(false);//disable button
            infoMap.text = "None"; //reset status text

            BackButtonClicked(); // back to main menu if not already there
        }

    }

    //gamecontroller will call this after placing a portal
    public void PortalPlaced(GameObject placedPortal)
    {
        //now load url
        placedPortal.GetComponent<PortalManager>().SetTheUrl(urlToLoad);

        //set the status UI
        infoPortal.text = selectedLocationName;
        closePortal.gameObject.SetActive(true);
    }

    // function that will display a message on the console UI for t amount of seconds
    public IEnumerator DisplayMessage (GameObject objMessage, float t) 
    {
        objMessage.SetActive(true);

        //wait specified time
        yield return new WaitForSeconds(t);

        objMessage.SetActive(false);
    }

    //custom function for dealing with scrolling dropdowns
    public void ScrollDropdownUp ( GameObject obj )
    {
        Debug.Log("scroll up");

        scrollRect = obj.transform.Find("Dropdown List").GetComponent<ScrollRect>();

        //make sure it is not null
        if (scrollRect != null)
        {
            scrollUp = !scrollUp;
        }
    }

    //custom function for dealing with scrolling dropdowns
    public void ScrollDropdownDown(GameObject obj )
    {
        Debug.Log("scroll down");

        scrollRect = obj.transform.Find("Dropdown List").GetComponent<ScrollRect>();

        //make sure it is not null
        if (scrollRect != null)
        {
            scrollDown = !scrollDown;
        }
    }

    //function that updates the consoles list of maps
    public void UpdateMapsDropdown ()
    {
        //first clear the current options
        mapLocations.ClearOptions();

        List<string> validMaps = new List<string>();
        //now lets add our new locations
        foreach ( MapDetails m in allMapsJson.maps )
        {
            validMaps.Add(m.name);
        }

        mapLocations.AddOptions(validMaps);

    }

    // fetch the online data for loading published maps
    private IEnumerator LoadMapJson()
    {
        // url of site where maps are returned
        string theUrl = "https://thedylon.com/fyp2020/getjsonallmaps.php";

        UnityWebRequest request = UnityWebRequest.Get(theUrl);

        //check if running in unity editor
        if (Application.isEditor)
        {
            //setting a default user-agent in the header, without this the webshost will return 403 forbidden error, web build doesn't have this issue over custom set .htaccess file
            request.SetRequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:69.0) Gecko/20100101 Firefox/69.0");
        }

        Debug.Log("Loading Json file at URL : " + theUrl);
        yield return request.SendWebRequest(); // this can take time to load
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error); // log the error, default local json will be used
        else
        {
            Debug.Log("Loading Json file completed");
            // replacing the local variable to point to the updated json from web
            allMapsJson = JsonUtility.FromJson<Maps>(((DownloadHandler)request.downloadHandler).text);
            //populate the consoles dropdown
            UpdateMapsDropdown();
            Debug.Log("Now using online Json file");
        }

    }
}

//for json parsing
[System.Serializable]
public class MapDetails
{
    public string name;
    public string desc;
    public int locations_json; //this is the maps id for loading that specific maps locations
}

[System.Serializable]
public class Maps
{
    public List<MapDetails> maps;
}

