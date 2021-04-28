using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalingController : MonoBehaviour
{

    //this script is attached to each scaling object on the placed buildings
    // the script has a variable for setting which axis the scaling will be done on
    public bool axisX; //true means scaling is on X axis, false is Z

    //scaling
    float startX;
    float startY;
    Vector3 startScaleBtns; //reference to the scale of the scaling objects
    Vector3 startScaleMain; //reference to the main game objects starting scale 

    //minimum limits on scaling size
    public float minScaleBtns;
    public float minScaleMain;

    //game controller
    GameController gameController;
    //camera controller
    CameraController cameraController;

    //reference the the cube placement object itself
    public Transform theMainObject;

    //a workaround for forcing localscaling to check for collisions
    private bool moveLast;

    private void Start()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        cameraController = Camera.main.GetComponent<CameraController>();

        moveLast = false;
    }

    void OnMouseDown()
    {
        
        startX = Input.mousePosition.x;
        startY = Input.mousePosition.y;
        startScaleBtns = transform.localScale;
        startScaleMain = theMainObject.localScale;
        //when scaling, camera should be unable to move
        cameraController.moveAllowed = false;
        
    }

    void OnMouseDrag()
    {
        Vector3 sizeBtns = transform.localScale;
        Vector3 sizeMain = theMainObject.localScale;
        //check with axis to scale on
        if (axisX)
        {
            sizeBtns.x = startScaleBtns.x + (Input.mousePosition.x - startX) * gameController.scale_sensitivity;
            sizeMain.x = startScaleMain.x + (Input.mousePosition.x - startX) * gameController.scale_sensitivity;
            if (sizeBtns.x < minScaleBtns)
                sizeBtns.x = minScaleBtns;
            if (sizeMain.x < minScaleMain)
                sizeMain.x = minScaleMain;
        }
        else
        {
            sizeBtns.z = startScaleBtns.z + (Input.mousePosition.y - startY) * gameController.scale_sensitivity;
            sizeMain.z = startScaleMain.z + (Input.mousePosition.y - startY) * gameController.scale_sensitivity;
            if (sizeBtns.z < minScaleBtns)
                sizeBtns.z = minScaleBtns;
            if (sizeMain.z < minScaleMain)
                sizeMain.z = minScaleMain;
        }

        transform.localScale = sizeBtns;
        theMainObject.localScale = sizeMain;

        //scaling on object in Unity does not force collsion updates, a way around this is to do a small movement
        // this is the cheapest way, instead of forcing a physics update
        if (moveLast)
        {
            theMainObject.position = new Vector3(theMainObject.position.x, 0f, theMainObject.position.z);
            moveLast = false;
        }
        else
        {
            theMainObject.position = new Vector3(theMainObject.position.x, 0.001f, theMainObject.position.z);
            moveLast = true;
        }
            

    }

    private void OnMouseUp()
    {
        //scaling complete so allow movement again
        cameraController.moveAllowed = true;

    }

}
