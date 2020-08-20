using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.GroupsModels;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour
{
    public PlayerManager LocalPlayer;
    private string PlayerNameText;

    public Material[] skins;
    public int playerSkin;

    public PlayFabLogin playfabLogin;

    public int targetFrameRate = 60;
    public string playerCurrentToken;
    public string playerNewToken;

    public GameObject ChatLog;


    private void Start()
    {
        Application.targetFrameRate = targetFrameRate;
    }


    void LateUpdate()
    {
        if (NetworkManager.singleton.isNetworkActive)
        {
            if (LocalPlayer == null)
            {
                if (ClientScene.localPlayer == null)
                    return;

                LocalPlayer = ClientScene.localPlayer.GetComponent<PlayerManager>();
                if (LocalPlayer != null)
                {
                    ThisPlayerJoined();
                    Invoke("ThisPlayerJoined", 0.1f);
                }
            }
            else
            {

            }
        }
        else
        {
        }
    }


    public void SendNameToClients()
    {
        LocalPlayer.SendName();
    }

    void ThisPlayerJoined()
    {
        GetPlayerProfileRequest request = new GetPlayerProfileRequest();
        PlayFabClientAPI.GetPlayerProfile(request, Successs, fail);
    }

    public void Successs(GetPlayerProfileResult result)
    {
        string getName = result.PlayerProfile.DisplayName;
        PlayFabClientAPI.GetUserPublisherData(new GetUserDataRequest()
        {
        }, result2 =>
        {
            if (result2.Data.ContainsKey("PlayerCurrentColor"))
            {
                playerSkin = int.Parse(result2.Data["PlayerCurrentColor"].Value);
                Debug.Log(playerSkin);
                LocalPlayer.CmdReady(getName);
            }
            else if (result2.Data.ContainsKey("PlayerOwnedColors"))
            {
                playerSkin = 1;
            }
            else if (!result2.Data.ContainsKey("PlayerOwnedColors"))
            {
                PlayFabClientAPI.UpdateUserPublisherData(new UpdateUserDataRequest()
                {
                    Data = new Dictionary<string, string>() {
                        {"PlayerOwnedColors", "1".ToString()},
                        {"PlayerCurrentColor", "1".ToString()}
                        }
                }, result3 => {
                    Destroy(LocalPlayer);
                    SceneManager.LoadScene(0);
                }, error =>
                 {
                     Destroy(LocalPlayer);
                     SceneManager.LoadScene(0);
                 });
            }
            if (result2.Data.ContainsKey("SessionToken"))
            {
                playerCurrentToken = result2.Data["SessionToken"].Value.ToString();
                //InvokeRepeating("CheckForNewConnection", 10, 10);
                
            }
            if (result2.Data.ContainsKey("Modderator"))
            {
                if (int.Parse(result2.Data["Modderator"].Value) == 1)
                {
                    ChatLog.transform.localPosition = new Vector2(0, 0);
                }
            }
            else
            {
                ChatLog.SetActive(false);
            }
        }, (error) =>
        {
            Destroy(LocalPlayer);
            SceneManager.LoadScene(0);
        });


    }

    /*
    void CheckForNewConnection()
    {
        PlayFabClientAPI.GetUserPublisherData(new GetUserDataRequest(), results =>
        {
            if (results.Data.ContainsKey("SessionToken"))
            {
                playerNewToken = results.Data["SessionToken"].Value.ToString();
                if (playerCurrentToken != playerNewToken)
                {
                    Debug.Log("Logged in from other location");
                    SceneManager.LoadScene(2);
                    PlayFabAuthenticationAPI.ForgetAllCredentials();
                    NetworkManager.singleton.StopClient();

                }
            }
        }, failures => { });
    }*/


    public void fail(PlayFabError error)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
