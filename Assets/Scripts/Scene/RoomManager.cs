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

    public Material[] skins;
    public int playerSkin;
    public string playerName;

    public int targetFrameRate = 60;

    public GameObject ChatLog;
    NetworkManager networkManager;
    public GameObject LoadingGUI;

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
                    //Invoke("ThisPlayerJoined", 0.1f);
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
        LocalPlayer.SendName(playerName);
    }

    void ThisPlayerJoined()
    {
        GetPlayerProfileRequest request = new GetPlayerProfileRequest();
        PlayFabClientAPI.GetPlayerProfile(request, Successs, fail);
    }

    public void Successs(GetPlayerProfileResult result)
    {
        playerName = result.PlayerProfile.DisplayName;
        PlayFabClientAPI.GetUserPublisherData(new GetUserDataRequest()
        {
        }, result2 =>
        {
            if (result2.Data.ContainsKey("PlayerCurrentColor"))
            {
                playerSkin = int.Parse(result2.Data["PlayerCurrentColor"].Value);
                //Debug.Log(playerSkin);
                LocalPlayer.CmdReady(playerName);
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
            if (result2.Data.ContainsKey("Modderator"))
            {
                if (int.Parse(result2.Data["Modderator"].Value) == 1)
                {
                    ChatLog.transform.localPosition = new Vector2(0, 0);
                }
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


    public void ChangeRoom(string Name)
    {
        LocalPlayer.SendName(playerName); // In the future also send the server Name;
    }
}