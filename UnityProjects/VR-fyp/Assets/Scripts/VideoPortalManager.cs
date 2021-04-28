using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class VideoPortalManager : MonoBehaviour
{

    public GameObject MainCamera;

    public GameObject arObject;

    private Material[] arObjectMaterials;

    private Material portalPlaneMaterial;

    // Start is called before the first frame update
    void Start()
    {
        MainCamera = GameObject.FindWithTag("MainCamera");

        arObjectMaterials = arObject.GetComponent<Renderer>().sharedMaterials;
        portalPlaneMaterial = GetComponent<Renderer>().sharedMaterial;
    }

    // Update is called once per frame
    void OnTriggerStay(Collider collider)
    {
        Vector3 camPositionInPortalSpace = transform.InverseTransformPoint(MainCamera.transform.position);

        if (camPositionInPortalSpace.y <= 0.0f)
        {
            for (int i = 0; i < arObjectMaterials.Length; ++i)
            {
                arObjectMaterials[i].SetInt("_StencilComp", (int)CompareFunction.NotEqual);
            }

            portalPlaneMaterial.SetInt("_CullMode", (int)CullMode.Front);
        }

        if (camPositionInPortalSpace.y < 0.5f)
        {
            //disable stencil
            for (int i = 0; i < arObjectMaterials.Length; ++i)
            {
                arObjectMaterials[i].SetInt("_StencilComp", (int)CompareFunction.Always);
            }

            portalPlaneMaterial.SetInt("_CullMode", (int)CullMode.Off);
        }
        else
        {
            //enable stencil
            for (int i = 0; i < arObjectMaterials.Length; ++i)
            {
                arObjectMaterials[i].SetInt("_StencilComp", (int)CompareFunction.Equal);
            }

            portalPlaneMaterial.SetInt("_CullMode", (int)CullMode.Back);
        }
    }
}

