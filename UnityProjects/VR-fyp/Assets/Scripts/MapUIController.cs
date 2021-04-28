using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//this script just holds the function that makes the maps UI follow the user
public class MapUIController : MonoBehaviour
{

    //reference to the user gameobject
    private GameObject theUser;

    private void Start()
    {
        theUser = GameObject.FindGameObjectWithTag("MainCamera");
    }

    // Update is called once per frame
    void Update()
    {
        if ( theUser != null)
        {
            Vector3 targetPos = new Vector3(theUser.transform.position.x, transform.position.y, theUser.transform.position.z);
            transform.rotation = Quaternion.LookRotation(transform.position - targetPos);
        }
    }
}
