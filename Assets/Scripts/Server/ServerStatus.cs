using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Mirror;
using UnityEngine.Networking;
public class ServerStatus : MonoBehaviour
{
    public int data = 0; //amount of connected players from all the rooms.
    public bool isServer = false;
    public int runTime = 0; //how many seconds the server is running
    

    private string postUrl = "https://moominrewritten.000webhostapp.com/VirtualProject/PostConnectedPlayers.php";

    void Start()
    {
        if (!Application.isBatchMode && isServer == false)
        {
            Destroy(this);
        }
        else if (isServer == true)
        {
            InvokeRepeating("Invoking", 10, 120);
            StartCoroutine(time());
        }
    }

    IEnumerator time()
    {
        while (true)
        {
            runTime++;
            yield return new WaitForSeconds(1);
        }
    }

    void Invoking()
    {
        StartCoroutine(UploadData());    
    }

    // Update is called once per frame
    
    IEnumerator UploadData()
    {
        string uptime = "Hours: 0 Minutes: 0";
        data = NetworkServer.connections.Count;

        if (System.TimeSpan.FromSeconds(runTime).Days >= 1)
        { 
            uptime = "Days: " + System.TimeSpan.FromSeconds(runTime).Days + " Hours: " + System.TimeSpan.FromSeconds(runTime).Hours + " Minutes: " + System.TimeSpan.FromSeconds(runTime).Minutes;
        } else
        {
            uptime = "Hours: " + System.TimeSpan.FromSeconds(runTime).Hours + " Minutes: " + System.TimeSpan.FromSeconds(runTime).Minutes;
        }

        List<IMultipartFormSection> wwwForm = new List<IMultipartFormSection>();
        wwwForm.Add(new MultipartFormDataSection("curActivePlayers", data.ToString()));
        wwwForm.Add(new MultipartFormDataSection("upTime", uptime));

        UnityWebRequest www = UnityWebRequest.Post(postUrl, wwwForm);

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Server status sent.");
        }
    }
}
