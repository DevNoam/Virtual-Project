using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Networking;

public class ServerStatus : MonoBehaviour
{
    public int data = 0;
    public bool isServer = false;


    private string postUrl = "http://moominrewritten.000webhostapp.com/VirtualProject/PostConnectedPlayers.php";
    // Start is called before the first frame update
    void Start()
    {
            if (!Application.isBatchMode && isServer == false)
            {
                Destroy(this);
            }
            InvokeRepeating("Invoking", 25, 120);
    }

    void Invoking()
    {
        StartCoroutine(UploadData());
        
    }

    // Update is called once per frame
    IEnumerator UploadData()
    {
        List<IMultipartFormSection> wwwForm = new List<IMultipartFormSection>();
        wwwForm.Add(new MultipartFormDataSection("curActivePlayers", data.ToString()));

        UnityWebRequest www = UnityWebRequest.Post(postUrl, wwwForm);

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Form upload complete!");
        }
    }
}
