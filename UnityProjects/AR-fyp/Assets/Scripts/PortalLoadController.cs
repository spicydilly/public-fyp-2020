using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


// This script will load the respective requested Image or Video as called by the game manager
// TODO : Implement gamemanager to call specific URL's, for testing I will use a single URL
public class PortalLoadController : MonoBehaviour
{

    public string urlToLoad = "https://cs1.ucc.ie/~dcc3/resources/UCC_Image_Quad_1";
    public Texture2D texture;

    //the skydome which we will apply the image to
    public GameObject skydome;

    // Start is called before the first frame update
    void Start()
    {

        StartCoroutine(LoadImageCoroutine());
        //currently the material will be white until the image/video has loaded

    }

    private IEnumerator LoadImageCoroutine()
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(urlToLoad);
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
}
