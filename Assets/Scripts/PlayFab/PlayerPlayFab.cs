using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;
public class PlayerPlayFab : NetworkBehaviour
{
    public PlayerManager localPlayer;

    private void Start()
    {
        if (!hasAuthority) { return; }
        ProfileRequest();
    }

    [ClientCallback]
    void ProfileRequest()
    {
        GetPlayerProfileRequest request = new GetPlayerProfileRequest();
        PlayFabClientAPI.GetPlayerProfile(request, Successs => {

            //localPlayer.playerName = Successs.PlayerProfile.DisplayName;
            
            PlayFabClientAPI.GetUserPublisherData(new GetUserDataRequest()
            {
            }, Successs2 =>
            {
                if (Successs2.Data.ContainsKey("PlayerCurrentColor"))
                {
                    localPlayer.CmdUpdatePlayerName(Successs.PlayerProfile.DisplayName.ToString(), int.Parse(Successs2.Data["PlayerCurrentColor"].Value));
                    //localPlayer.CmdSetName(localPlayer.playerName, localPlayer.SkinColor, localPlayer.player.transform.position);

                    /////////////////////////

                    if (PlayerPrefs.HasKey("USERSAVED"))
                    {
                        if (PlayerPrefs.GetString("USERNAME").ToUpper() == Successs.PlayerProfile.DisplayName.ToUpper())
                        {
                            PlayerPrefs.SetInt("PLAYERCOLOR", int.Parse(Successs2.Data["PlayerCurrentColor"].Value));
                        }
                        else if (PlayerPrefs.GetString("USERNAME2").ToUpper() == Successs.PlayerProfile.DisplayName.ToUpper())
                        {
                            PlayerPrefs.SetInt("PLAYERCOLOR2", int.Parse(Successs2.Data["PlayerCurrentColor"].Value));
                        }
                        else if (PlayerPrefs.GetString("USERNAME3").ToUpper() == Successs.PlayerProfile.DisplayName.ToUpper())
                        {
                            PlayerPrefs.SetInt("PLAYERCOLOR3", int.Parse(Successs2.Data["PlayerCurrentColor"].Value));
                        }
                    }
                }
                else if (Successs2.Data.ContainsKey("PlayerOwnedColors"))
                {
                    //localPlayer.CmdUpdatePlayerName(Successs.PlayerProfile.DisplayName, 1);
                }
                else if (!Successs2.Data.ContainsKey("PlayerOwnedColors"))
                {
                    PlayFabClientAPI.UpdateUserPublisherData(new UpdateUserDataRequest()
                    {
                        Data = new Dictionary<string, string>() {
                        {"PlayerOwnedColors", "1".ToString()},
                        {"PlayerCurrentColor", "1".ToString()}
                        }
                    }, result3 => {
                        Destroy(localPlayer);
                        SceneManager.LoadScene(0);
                    }, error =>
                    {
                        Destroy(localPlayer);
                        SceneManager.LoadScene(0);
                    });
                }
                if (Successs2.Data.ContainsKey("Modderator"))
                {
                    if (int.Parse(Successs2.Data["Modderator"].Value) == 1)
                    {
                        localPlayer.chatSystem.chatLogParent.gameObject.GetComponent<Transform>().localPosition = new Vector2(0, 1);
                        localPlayer.isModderator = true;
                    }
                }
            }, (error) =>
            {
                Destroy(localPlayer);
                SceneManager.LoadScene(0);
            });
        }, fail => {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        });
    }
}
