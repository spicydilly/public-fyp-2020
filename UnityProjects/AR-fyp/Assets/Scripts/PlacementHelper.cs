using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this script is attached to the placement inidicator objects where a collider exists
// will check if there are any collisions during placement
// will be called upon by the gamecontroller

//also will change the outline shader color to indicate placement is invalid

public class PlacementHelper : MonoBehaviour
{

    public bool isSpaceFree;

    //controls the setting of objects color
    private bool materialChanged = false;

    //the gameobjects renderer components which will have materials changed
    public List<Renderer> boundaries;

    public Material outlineValid;
    public Material outlineInvalid;

    //using fixed update as it is called before physics functions like on collision stay
    private void FixedUpdate()
    {
        isSpaceFree = true;
    }

    //called last
    private void LateUpdate()
    {
        if (!isSpaceFree)
        {
            if (!materialChanged)
            {
                materialChanged = true;
                //change material of objects
                foreach (Renderer obj in boundaries)
                {
                    obj.material = outlineInvalid;
                }
            }

        }
        else
        {
            if (materialChanged)
            {
                materialChanged = false;
                //change material of objects
                foreach (Renderer obj in boundaries)
                {
                    obj.material = outlineValid;
                }
            }
        }
    }

    private void OnTriggerStay(Collider col)
    {
        // if something is colliding
        isSpaceFree = false;

    }
}
