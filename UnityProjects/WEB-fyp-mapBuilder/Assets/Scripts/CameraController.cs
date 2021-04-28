using UnityEngine;
using System.Collections;
using System.Security.Cryptography.X509Certificates;

public class CameraController : MonoBehaviour
{

    public bool moveAllowed;

    private Vector2 mousePos;
    private Vector3 moveVector;
    private float x, y, z; // movement positions

    //screen edge
    public float screenEdge = 15f;

    //max and min values
    public float minZoom, maxZoom;
    public float minX, maxX, minZ, maxZ; //these are set by SetMinMaxPositions, as it depends on size of map

    //move and scroll speed
    public float moveSpeed, zoomSpeed;

    //reference to the mapbase game object, used to determine max movement positions
    public GameObject mapBase;

    public void Start()
    {
        // at load, disable movement as it shouldn't begin until the user triggers the "start" button
        moveAllowed = false;

        // set the min/max camera positions
        SetMinMaxPositions();

    }

    private void Update()
    {
        //if allowed to move
        if (moveAllowed)
        {
            mousePos = Input.mousePosition;

            //make sure camera isn't beyound the min and max x Positions
            //move when at "x" edge, or when relevant key is pressed
            if ( gameObject.transform.position.x >= minX && (mousePos.x < screenEdge || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) ) {
                x = -1;
            }
            else if ( gameObject.transform.position.x <= maxX && (mousePos.x >= Screen.width - screenEdge || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) ) {
                x = 1; 
            }
            else
            {
                x = 0;
            }

            //move when at "y" edge, this correlates to move the camera on the 'z' axis
            //make sure camera isn't beyound the min and max z Positions
            //move when at "y" edge, or when relevant key is pressed
            if (gameObject.transform.position.z >= minZ && ( mousePos.y < screenEdge || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) )
            {
                z = -1;
            }
            else if (gameObject.transform.position.z <= maxZ && ( mousePos.y >= Screen.height - screenEdge || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) )
            {
                z = 1;
            }
            else
            {
                z = 0;
            }

            //now check for scrolling in and out
            if (transform.position.y <= maxZoom && Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                y = 1;
            }
            else if (transform.position.y >= minZoom && Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                y = -1;
            }
            else
            {
                y = 0;
            }


            //move camera
            moveVector = (new Vector3(x * moveSpeed, y * zoomSpeed, z * moveSpeed) * Time.deltaTime);
            transform.Translate(moveVector, Space.World);

        }        

    }

    //this function limits the cameras position to not pass beyond the mapbases border
    public void SetMinMaxPositions ()
    {
        // the mapbase scaling in the world space, found using lossyScale
        Vector3 theScale = mapBase.transform.lossyScale;

        //the min and max poisitions are found by dividing the scale of both x and y axis
        // by 2, this gives a position right over either the top or the right side edge of
        // the map base. The left and bottom edge will just be the inverse position
        maxX = theScale.x / 2;
        minX = -maxX;
        maxZ = theScale.y / 2;
        minZ = -maxZ;

        Debug.Log("Camera values -- Max X : " + maxX + " ; Min X : " + minX + " ; MaxZ : " + maxZ + " ; MinZ : " + minZ);
    } 
}