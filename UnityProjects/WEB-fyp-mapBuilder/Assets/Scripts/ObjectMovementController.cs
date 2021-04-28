using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectMovementController : MonoBehaviour
{
    // for movement 
    Vector3 dist;
    Vector3 startPos;
    float posX;
    float posZ;
    float posY;

    //rotation
    bool doRotate;
    Vector3 mouseRef;
    Vector3 mouseOffset;
    Vector3 rot;

    //holds the scaling objects
    public GameObject ScalingObjects;

    //game controller
    GameController gameController;
    //camera controller
    CameraController cameraController;

    //references the scrollview entry for this object
    GameObject scrollViewEntry;

    //this objects information
    public int obj_id; //only used if he map was loaded by LoadMapController
    public string obj_name;
    public string obj_description;
    public string obj_imageurl;

    //stores the materials that will be used by the objects, 0 should be default, 1 is when selected, 
    //2 is for placement issues, and 3 is for both placement issues and when the object is selected
    public Material[] theMaterials = new Material[4];

    //boolean that stores whether the object is colliding with another or not
    public bool isColliding;
    public int numColliding = 0; // number of objects colliding

    //boolean for storing if object is selected or not
    private bool thisSelected;

    //min max poistions for object placement
    private float minX, maxX, minZ, maxZ;

    private void Start()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        cameraController = Camera.main.GetComponent<CameraController>();

        //after creation, add to the scrollview
        scrollViewEntry = gameController.AddToScrollView(gameObject);

        //show the scaling options, depending if scaling is active or not and if this is the selected object when Start() is called
        if (thisSelected && gameController.isScaling)
            ScalingObjects.SetActive(true);
        else
            ScalingObjects.SetActive(false);

    }

    private void Update()
    {
        if (doRotate)
        {
            mouseOffset = Input.mousePosition - mouseRef;
            rot.y = -(mouseOffset.x + mouseOffset.y) * gameController.rot_sensitivity;
            transform.parent.Rotate(rot);
            mouseRef = Input.mousePosition;
        }
    }

    void OnMouseDown()
    {
        //tell game controller this is the selected object, if this is not already seleted
        if (!thisSelected)
            gameController.SelectedAObject(gameObject);

        //get the min and max positions for object placement
        // this is the same as the cameras min max positions
        GetTheMinMaxPositons();

        if (gameController.isMoving)
        {
            startPos = transform.position;
            dist = Camera.main.WorldToScreenPoint(transform.position);
            posX = Input.mousePosition.x - dist.x;
            posZ = Input.mousePosition.z - dist.z;
        } else if (gameController.isRotating )
        {
            doRotate = true;
            mouseRef = Input.mousePosition;
        } 
    }

    void OnMouseDrag()
    {
        if (gameController.isMoving )
        {
            //get movement
            float disX = Input.mousePosition.x - posX;
            float disY = Input.mousePosition.y - posY;
            float disZ = Input.mousePosition.z - posZ; 
            
            //get new position
            Vector3 newPos = Camera.main.ScreenToWorldPoint(new Vector3(disX, disY, disZ));
            
            //make sure it isn't hitting x limits
            if (newPos.x >= maxX)
                newPos.x = maxX; //set to the max x position
            if (newPos.x <= minX)
                newPos.x = minX; //set to the min x position

            //make sure it isn't hitting z limits
            if (newPos.z >= maxZ)
                newPos.z = maxZ; //set to the max z position
            if (newPos.z <= minZ)
                newPos.z = minZ; //set to the min z position

            transform.parent.position = new Vector3(newPos.x, startPos.y, newPos.z);
        } 
        
    }

    private void OnMouseUp()
    {
        if (gameController.isRotating )
        {
            doRotate = false;
        } 
        
    }

    //called by gamecontroller to update name
    public void NameUpdate (string newName)
    {
        obj_name = newName;
        //update scrollview entry if it has already been added
        if ( scrollViewEntry )
            scrollViewEntry.GetComponentInChildren<Text>().text = obj_name;
    }

    //on destroy, also destory scroll view entry, and decrease the placedobjects value in the gamecontroller
    private void OnDestroy()
    {
        Destroy(scrollViewEntry);
        gameController.CheckPlacedObjectsLimitAfterDelete(); //gamecontroller will decrement value
    }

    //called when gamecontroller selects or deselects the object, used to change rendering and
    // to set the collision trigger
    public void UserSelected(bool isSelected)
    {
        thisSelected = isSelected;

        if (isSelected)
        {
            gameObject.GetComponent<Renderer>().material = theMaterials[1];
            //if scaling is selected, enable the scaling objects
            if (gameController && gameController.isScaling)
                ScalingObjects.SetActive(true);
        }
        else
        {
            gameObject.GetComponent<Renderer>().material = theMaterials[0];
            //when unselected make sure to disable the scaling objects
            ScalingObjects.SetActive(false);
        }

        gameObject.GetComponent<Rigidbody>().isKinematic = isSelected;
        
    }

    //when colliding with another building object, set material to show collision
    private void OnCollisionEnter(Collision collision)
    {
        //use correct material
        if (thisSelected)
            gameObject.GetComponent<Renderer>().material = theMaterials[3];
        else 
            gameObject.GetComponent<Renderer>().material = theMaterials[2];
        isColliding = true;
        numColliding += 1;
     
    }

    //when collision exits
    private void OnCollisionExit(Collision collision)
    {
        numColliding -= 1;

        //if all collsions exit
        if (numColliding <= 0)
        {
            //use correct material
            if (thisSelected)
                gameObject.GetComponent<Renderer>().material = theMaterials[1];
            else
                gameObject.GetComponent<Renderer>().material = theMaterials[0];
            isColliding = false;
        }
        
       
    }

    //function which fetches the min/max positions from the cameracontroller
    private void GetTheMinMaxPositons ()
    {
        minX = cameraController.minX;
        maxX = cameraController.maxX;
        minZ = cameraController.minZ;
        maxZ = cameraController.maxZ;
    }

    //function called when the object is instatiated to set the correct data
    public void LoadedIn(int loadedID = 0, string loadedName = "Unamed", string loadedDesc = "", string imgUrl = "")
    {
        //sets the objects details
        obj_id = loadedID;
        obj_name = loadedName;
        obj_description = loadedDesc;
        obj_imageurl = imgUrl;
    }

}
