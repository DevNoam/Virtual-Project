using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;
public class RoomManager : MonoBehaviour
{
    public PlayerManager LocalPlayer;
    private string PlayerNameText;
    public Material[] skins;
    public int playerSkin;

    public PlayFabLogin playfabLogin;

    public int targetFrameRate = 60;




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
            else if(!result2.Data.ContainsKey("PlayerOwnedColors"))
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
                },error =>
                {
                    Destroy(LocalPlayer);
                    SceneManager.LoadScene(0);
                });
            }
        }, (error) =>
        {
            Destroy(LocalPlayer);
            SceneManager.LoadScene(0);
        });
    }

    public void fail(PlayFabError error)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
