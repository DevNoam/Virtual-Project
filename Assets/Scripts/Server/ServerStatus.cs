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
    public int timeFromLastPlayer = 0; //The time passed after the last player has been disconnected.
    private string applicationVersion;
    [SerializeField]
    private NetworkManager networkManager;


    [Tooltip("ConnectedPlayers.txt server info should go here.")]
    public string postUrl;


    void Start()
    {
        StartCoroutine(checkServer());
        if (!IsHeadlessMode())
        {
            Destroy(this);
        }
        else
        {
            applicationVersion = Application.version;
            InvokeRepeating("Invoking", 10, 120);
            StartCoroutine(time());
            StartCoroutine(timePlayer());
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
    IEnumerator timePlayer()
    {
        while (true)
        {
            if (data > 0 && timeFromLastPlayer > 0)
            {
                timeFromLastPlayer = 0;
            }
            else if (data == 0 && timeFromLastPlayer >= 3600)
            {
                Debug.Log("Quit");
                Application.Quit();
            }
            if (data == 0)
            {
                timeFromLastPlayer++;
            }
            yield return new WaitForSeconds(1);
        }
    }
    IEnumerator checkServer()
    {
        string uri = "https://virtualproject.noamsapir.me/serverstatus/checkPort.php";
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();
            string line = webRequest.downloadHandler.text;
            if (line == "Open")
            {
                Application.Quit();
                Debug.Log("Quit");
            }
            else if (line == "Closed")
            {
                networkManager.StartServer();
            }
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
        wwwForm.Add(new MultipartFormDataSection("serverVersion", "ServerVersion: " + applicationVersion));

        UnityWebRequest www = UnityWebRequest.Post(postUrl, wwwForm);

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log($"{System.DateTime.Now} Server status sent.");
        }
    }
    public static bool IsHeadlessMode()
    {
        return UnityEngine.SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null;
    }
}
