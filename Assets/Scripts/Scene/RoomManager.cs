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
        LookForPlayer();
    }


    void LookForPlayer()
    {
        if (NetworkManager.singleton.isNetworkActive)
        {
            if (LocalPlayer == null)
            {
                if (ClientScene.localPlayer == null)
                {
                    Invoke("LookForPlayer", 0f);
                }
                else
                {
                    LocalPlayer = ClientScene.localPlayer.GetComponent<PlayerManager>();
                    if (LocalPlayer != null)
                    {
                        ThisPlayerJoined();
                    }
                }
            }
            else
            {
                Invoke("LookForPlayer", 0f);
            }
        }
        else if (!NetworkManager.singleton.isNetworkActive)
        {
            Invoke("LookForPlayer", 0f);
        }


    }


    public void SendNameToClients()
    {
        LocalPlayer.SendName(playerName, playerSkin);
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
                LocalPlayer.CmdReady(playerName);


                if (PlayerPrefs.HasKey("USERSAVED"))
                {
                    if (PlayerPrefs.GetString("USERNAME").ToUpper() == playerName.ToUpper())
                    {
                        PlayerPrefs.SetInt("PLAYERCOLOR", int.Parse(result2.Data["PlayerCurrentColor"].Value));
                    }
                    else if (PlayerPrefs.GetString("USERNAME2").ToUpper() == playerName.ToUpper())
                    {
                        PlayerPrefs.SetInt("PLAYERCOLOR2", int.Parse(result2.Data["PlayerCurrentColor"].Value));
                    }
                    else if (PlayerPrefs.GetString("USERNAME3").ToUpper() == playerName.ToUpper())
                    {
                        PlayerPrefs.SetInt("PLAYERCOLOR3", int.Parse(result2.Data["PlayerCurrentColor"].Value));
                    }
                }
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
                    LocalPlayer.chatSystem.chatLogParent.gameObject.GetComponent<Transform>().localPosition = new Vector2(0, 1);
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
        LocalPlayer.SendName(playerName, playerSkin); // In the future also send the server Name;
    }
}