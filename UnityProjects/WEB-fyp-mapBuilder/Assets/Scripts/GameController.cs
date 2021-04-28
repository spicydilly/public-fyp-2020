using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    // for movement 
    public bool isMoving;
    //rotation
    public bool isRotating;
    public float rot_sensitivity;
    //scaling
    public float scale_sensitivity;
    public bool isScaling;

    //for placing objects into
    public GameObject mapObj;

    //the prefab used as the "add" object
    public GameObject defaultObj;

    //for storing the selected object
    public GameObject cur_selected;

    //variable for controlling the "starting" screen
    public GameObject startUI; //the ui of the "start" screen
    public GameObject theSceneHolder; //the object holding the whole scene

    //variables for minimising UI's
    public GameObject allObjectsPanel; //holds ui that lists all the objects placed
    public GameObject allBtn;
    public GameObject allMinBtn;
    public GameObject thisInfoPanel; //holds ui that displays info on selected obj
    public GameObject infoBtn;
    public GameObject infoBtnMin;

    //list of the buttons for editing an object ( move, rotate, scale )
    public List<Button> editingButtons;

    //for the UI that shows selected objects info
    public InputField nameInputUI;
    public InputField descriptionInputUI;

    //UI that shows all objects
    public GameObject objectsScrollView;
    public GameObject entryPrefab; //entry prefab

    //url to the map image
    public string theMapImageUrl;

    //the map base, the image will be applied here
    public GameObject mapBase;

    //reference to cameracontroller
    CameraController cameraController;

    private SaveMapController saveMapController;

    //if the app is paused
    private bool isPaused = false;

    //references whether the map is a loaded map
    public bool mapIsLoaded = false; //default is false

    //limits on the number of placed objects allowed in the scene
    public int maxPlacedObjects;
    public int currentPlacedObjects; // reference to the current amount of placed objects
    public Button addBtn; //the add button

    //called once user clicks the "begin" button
    public void StartTheApp ()
    {
        startUI.SetActive(false);
        theSceneHolder.SetActive(true);

        saveMapController = GameObject.FindGameObjectWithTag("GameController").GetComponent<SaveMapController>();

        cameraController = Camera.main.GetComponent<CameraController>();
        //let cameracontroller know that movement can commence
        cameraController.moveAllowed = true;

        //if loaded map, load the image background if url isn't empty        
        if ( mapIsLoaded )
        {
            if ( theMapImageUrl != "" )
            {
                SetMapImage(theMapImageUrl);
            }
        }
        else // add the first cube to the scene
        {
            AddBtnPressed();
        }
        
    }

    void ResetActions ()
    {
        isMoving = isRotating = isScaling = false;

        //if a object is selected, disable the scaling objects if needed
        if (cur_selected)
            cur_selected.GetComponentInChildren<ObjectMovementController>().ScalingObjects.SetActive(false);
        
        //reset the buttons by looping through and allowing them to be interactable
        foreach(Button btn in editingButtons)
        {
            btn.interactable = true;
        }
        
    }

    public void MoveBtnPressed (Button btn)
    {
        //ignore press if paused
        if (!isPaused)
        {
            ResetActions();
            btn.interactable = false;
            isMoving = true;
            Debug.Log("Now Moving Objects!");
        }
        
    }

    public void RotateBtnPressed (Button btn)
    {
        //ignore press if paused
        if (!isPaused)
        {
            ResetActions();
            btn.interactable = false;
            isRotating = true;
            Debug.Log("Now Rotating Objects!");
        }
            
    }

    public void ScaleBtnPressed (Button btn)
    {
        //ignore press if paused
        if (!isPaused)
        {
            ResetActions();
            btn.interactable = false;
            isScaling = true;
            Debug.Log("Now Scaling Objects!");

            //if object is selected, tell it to display the scale options
            if (cur_selected)
                cur_selected.GetComponentInChildren<ObjectMovementController>().ScalingObjects.SetActive(true);
        }
    }

    //adds a new cube to the scene
    public void AddBtnPressed ()
    {
        //ignore press if paused
        if (!isPaused)
        {

            // instatiate the object directly under the cameras center
            GameObject obj = (GameObject)Instantiate(defaultObj, new Vector3(cameraController.transform.position.x, 0, cameraController.transform.position.z), Quaternion.identity);
            //set parent
            obj.transform.parent = mapObj.transform;

            //set the default info
            obj.GetComponentInChildren<ObjectMovementController>().LoadedIn();

            //set the object as the selected object
            SelectedAObject(obj);

            //if the current number of placed objects is zero, renable the UI for showing object details
            if ( currentPlacedObjects == 0 )
            {
                nameInputUI.interactable = true;
                descriptionInputUI.interactable = true;
            }

            //increment the number of placed objects
            currentPlacedObjects++;

            //check if max limit hit, if so disable the button
            CheckPlacedObjectsLimit();
        }   

    }

    //function which checks if the max limit of objects have been placed, if so the add button is disabled
    private void CheckPlacedObjectsLimit ()
    {
        if ( currentPlacedObjects >= maxPlacedObjects)
        {
            //disable the add button and change text to display why
            addBtn.interactable = false;
            addBtn.GetComponentInChildren<Text>().text = "Limit Reached";
        } else
        {
            addBtn.interactable = true;
            addBtn.GetComponentInChildren<Text>().text = "Add";
        }
    }

    //this function is called when an object is deleted from the scene
    public void CheckPlacedObjectsLimitAfterDelete()
    {
        currentPlacedObjects--; // decrease value
        CheckPlacedObjectsLimit(); //check if button needs to be updated

        //if value is zero, reset variables for current selected object
        if ( currentPlacedObjects == 0)
        {
            cur_selected = null;
            //disable the UI for showing current selection
            nameInputUI.interactable = false;
            descriptionInputUI.interactable = false;
        }
    }

    //set the selected object
    public void SelectedAObject ( GameObject obj )
    {
        //reset the last selected objects material, if it exists
        if ( cur_selected != null )
            cur_selected.GetComponentInChildren<ObjectMovementController>().UserSelected(false);

        //store reference
        cur_selected = obj;

        //set the material to selected
        cur_selected.GetComponentInChildren<ObjectMovementController>().UserSelected(true);

        //update displayed UI
        nameInputUI.text = obj.GetComponentInChildren<ObjectMovementController>().obj_name;
        descriptionInputUI.text = obj.GetComponentInChildren<ObjectMovementController>().obj_description;
    }

    //called by name input UI whenever there is a change, storing this to the selected object
    public void NameInputUpdate (InputField obj)
    {
        //only if a object is selected
        if (cur_selected)
            cur_selected.GetComponentInChildren<ObjectMovementController>().NameUpdate(obj.text);
    }

    //called by description input UI whenever there is a change, storing this to the selected object
    public void DescriptionInputUpdate(InputField obj)
    {
        //only if a object is selected
        if (cur_selected)
            cur_selected.GetComponentInChildren<ObjectMovementController>().obj_description = obj.text;
    }

    // this function will add info about our gameobjects to the scrollview
    public GameObject AddToScrollView (GameObject obj)
    {
        //create entry
        GameObject newEntry = Instantiate(entryPrefab);
        newEntry.GetComponentInChildren<Text>().text = obj.GetComponent<ObjectMovementController>().obj_name;
        //add onclick event to the main button so we can change the selected object
        newEntry.GetComponent<Button>().onClick.AddListener(() => SelectedAObject(obj));
        //add onclick event to the "delete" button which is the second child (1 in array)
        newEntry.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => DestroyBuilding(obj));
        

        //find the content view, and add text to this
        Transform[] Temp = objectsScrollView.transform.GetComponentsInChildren<Transform>();
        foreach (Transform Child in Temp)
        {
            if (Child.name == "Content") 
            { 
                newEntry.transform.SetParent(Child.gameObject.GetComponent<RectTransform>());
            }
        }

        return newEntry;

    }

    //function which controls the deletion of objects, this is needed to make sure
    // collisions correctly register before an object is removed
    public void DestroyBuilding(GameObject obj)
    {
        //move the object out of the scene, this ensures the collisionexit is queued 
        obj.GetComponent<Rigidbody>().isKinematic = true;
        obj.transform.position = new Vector3(100, 100, 100);
        //now destory the object using a helper, to ensure the destroy function is called before destroy
        StartCoroutine(DestroyBuildingHelper(obj));
    }

    IEnumerator DestroyBuildingHelper (GameObject toDelete)
    {
        //wait for physic calculations to complete
        yield return new WaitForFixedUpdate();

        Destroy(toDelete);
    }

    //this is called by the minimise buttons and the maximise buttons to control the UI
    public void MinBtnPressed ( GameObject obj )
    {
        if ( obj == allBtn ) //display the ui that shows all objects placed
        {
            allObjectsPanel.SetActive(true);
            allBtn.SetActive(false);
        } else if ( obj == allMinBtn )
        {
            allObjectsPanel.SetActive(false);
            allBtn.SetActive(true);
        } else if ( obj == infoBtn ) //display the ui that shows info on selected object
        {
            thisInfoPanel.SetActive(true);
            infoBtn.SetActive(false);
        } else if ( obj == infoBtnMin )
        {
            thisInfoPanel.SetActive(false);
            infoBtn.SetActive(true);
        }
    }

    //this function is called by the web browser or when loading a map, to set the texture of the map object
    //input string is the URL to the saved image
    public void SetMapImage( string theUrl )
    {
        theMapImageUrl = theUrl;
        StartCoroutine(LoadImageCoroutine());
    }

    private IEnumerator LoadImageCoroutine()
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(theMapImageUrl);

        //check if running in unity editor
        if (Application.isEditor)
        {
            //setting a default user-agent in the header, without this the webshost will return 403 forbidden error, web build doesn't have this issue over custom set .htaccess file
            request.SetRequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:69.0) Gecko/20100101 Firefox/69.0");
        }

        Debug.Log("Loading image at URL : " + theMapImageUrl);
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
            float x = mapBase.transform.parent.transform.localScale.x; //starting scale
            float z = x * ratio;
            mapBase.transform.parent.transform.localScale = new Vector3(x, 1, z); // set new scale
            mapBase.GetComponent<Renderer>().material.mainTexture = theTexture;
            //now reset the min and max positions for the camera as the map size has changed
            cameraController.SetMinMaxPositions();
        }
    }

    //function called when the scene needs to be paused or unpaused
    public void PauseScene (bool paused)
    {
        isPaused = paused;

        //disable or enable movement depending on if paused
        cameraController.moveAllowed = !isPaused;

        //reset the actions
        ResetActions();

        //minimize UI elements, and stop them from been enabled if paused
        allObjectsPanel.SetActive(false);
        allBtn.SetActive(true);
        allBtn.GetComponent<Button>().interactable = !isPaused;
        thisInfoPanel.SetActive(false);
        infoBtn.SetActive(true);
        infoBtn.GetComponent<Button>().interactable = !isPaused;

    }

    //function that removes all placed objects from the scene
    public void ClearTheScene (bool isAfterSave = false)
    {
        //loop through all maps objects, and remove if the objectMovementController is present on the object

        foreach ( Transform obj in mapObj.transform )
        {
            if (obj.gameObject.GetComponentInChildren<ObjectMovementController>() )
            {
                //destroy the object as the component exists
                Destroy(obj.gameObject);
            }

        }

        //if it's not a clear after a save was completed
        if ( !isAfterSave )
        {
            //now remove the loaded image on the map base
            Destroy(mapBase.GetComponent<Renderer>().material.mainTexture);

            //reset the maps id, name and description
            saveMapController.SavedMapLoadedIn();
        }
        

    }

}
