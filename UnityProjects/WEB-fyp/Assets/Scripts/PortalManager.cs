using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PortalManager : MonoBehaviour
{
    // this is the quad which we will render the environment through
    public Renderer cameraQuad;

    // the materials in the scene that are being used to mask through the camera quad
    public List<Material> materialStencils;

    //the portals own content will be stored as children of this
    public Transform portalsRoom;

    // the image to load, will be called by the console after initialising this object
    public string urlToLoad;
    public Texture2D texture;

    //the skydome which we will apply the image to
    public GameObject skydome;

    //the game controller
    private GameController gameController;

    //reference to the gameibjects in the starting room
    private Transform startingRoom;

    //list of the objects stored behind the portal, stored for quicker access
    private List<GameObject> objBehind;

    private void Start()
    {
        cameraQuad = GameObject.FindGameObjectWithTag("StencilQuad").GetComponent<Renderer>();

        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        startingRoom = gameController.startingRoom;

        CheckObjectsBehind();

    }

    void OnTriggerExit(Collider other)
    {
        // checking that it is the camera that triggers this event
        if (!other.gameObject.CompareTag("MainCamera"))
            return;

        // checking which side of portal was exited to enable the correct environment
        Vector3 exitSide = (other.transform.position - this.transform.position).normalized;
        bool isEntering = Vector3.Dot(exitSide, this.transform.forward) > 0;
        Debug.Log("Player is entering environment :" + isEntering);

        //check which new material to apply to the quad to render the right environment
        if (isEntering)
        {
            cameraQuad.material = materialStencils[1];
            gameController.currentLoc = 2;
        }
        else
        {
            cameraQuad.material = materialStencils[0];
            gameController.currentLoc = 1;
        }

        //now make sure to change the render queues and colliders of the starting room
        CorrectPortalRoom(isEntering);
        //now disable colliders of the starting room
        ChangeColliders(isEntering);
    }

    // loop through each child in the room game object and change their render queue
    private void CorrectPortalRoom (bool isEntering)
    {
        foreach (Transform obj in portalsRoom)
        {
            if (isEntering) 
                obj.gameObject.GetComponent<Renderer>().material.renderQueue = 1930;
            else
                obj.gameObject.GetComponent<Renderer>().material.renderQueue = 1960;

            //now enable/disable colliders for objects in the portals room
            obj.GetComponent<Collider>().enabled = isEntering;
            
        }
    }

    //disable or enable the starting room colliders depending on if the portal is being entered or not
    private void ChangeColliders ( bool isEntering )
    {

        //first lets disable/enable the objects located behind the portal, these were stored when the portal was loaded
        foreach ( GameObject obj in objBehind )
        {
            obj.SetActive(!isEntering);
        }
        
        
        foreach ( Transform obj in startingRoom )
        {
            //if not already disabled 
            if ( obj.gameObject.activeInHierarchy )
            {
                // disable colliders, some objects have child objects with colliders so disable those
                foreach (Collider c in obj.GetComponentsInChildren<Collider>())
                {
                    c.enabled = !isEntering;
                }
            }
            
        }
        
    }

    private IEnumerator LoadImageCoroutine()
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(urlToLoad);

        //check if running in unity editor
        if (Application.isEditor)
        {
            //setting a default user-agent in the header, without this the webshost will return 403 forbidden error, web build doesn't have this issue over custom set .htaccess file
            request.SetRequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:69.0) Gecko/20100101 Firefox/69.0");
        }
        
        Debug.Log("Loading image at URL : " + urlToLoad);
        yield return request.SendWebRequest(); // this can take time to load
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
        {
            Debug.Log("Loading completed");
            skydome.GetComponent<Renderer>().material.mainTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        }

    }

    // console controller will call this to begin the image loading after initilising 
    public void SetTheUrl ( string theUrl )
    {
        urlToLoad = theUrl;
        StartCoroutine(LoadImageCoroutine());
        //currently the material will be white until the image/video has loaded
    }

    //this function gets the objects in the starting room located behind the portal,and alters their render queue
    // this resolves shader issues where close objects flicker at different distances
    public void CheckObjectsBehind ()
    {
        //reset the list
        objBehind = new List<GameObject>();

        //for each object and each child object in the starting room that have transforms
        // checking child objects as a parent might be large enough for one half of them to be behind the object
        // where a child of that object may actually be in front of the portal 
        foreach (Transform obj in startingRoom.GetComponentsInChildren<Transform>())
        {
            // first check if there i
            if (Vector3.Dot((obj.transform.position - transform.position).normalized, transform.forward) > 0) //check if the object is behind the portal, 1 if true
            {
                if (obj.gameObject.TryGetComponent(out Renderer renderer)) // if object has a renderer
                    renderer.material.renderQueue = 1941;

                //add object to the list
                objBehind.Add(obj.gameObject);
            }
            
        }
        
    }

    // function that resets the objects in the scene when the portal is destroyed
    private void OnDestroy()
    {
        //loop through the objects behind the portal and enable and reset their render queues to normal
        foreach ( GameObject obj in objBehind )
        {
            //make sure object isn't alredy deleted
            if ( obj )
                if (obj.gameObject.TryGetComponent(out Renderer renderer)) // if object has a renderer
                    renderer.material.renderQueue = 1940;
        }

        Debug.Log("Portal removed, objects reset!");
    }

}
