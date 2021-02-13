﻿using PlayFab;
using System.Collections;
using System.Collections.Generic;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayFabLogin : MonoBehaviour
{
    private string UserNameLogin;
    private string UserPasswordLogin;
    public GameObject loginPanel;
    private string UserNameRegister;
    private string UserEmailRegister;
    private string UserPasswordRegister;
    private string resetEmail;
    private int desierdColor;
    public GameObject RegisterPanel;
    public Toggle RememberMeLogin;
    public TMP_Text LoginMessage;
    public TMP_Text RegisterMessage;
    public GameObject LoadingPanel;
    public TMP_Text registerPlayerNameGUI;

    public GameObject AccountVerificationReminder;

    public Material[] skins;

    public NetworkHUD NetworkManagerHUD;


    public void Start()
    {
        NetworkManagerHUD.enabled = false;
        if (Application.isBatchMode == true) //If is server, Instantly load the Server.
        {
            //NetworkManager.SetActive(true);
        }

        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            PlayFabSettings.staticSettings.TitleId = "8D6DB";
        }
    }

    private void OnLoginSuccess(LoginResult result)
    {
        NetworkManagerHUD.enabled = true;
        {
            PlayFabClientAPI.GetPlayerStatistics(new GetPlayerStatisticsRequest(),
                results =>
                {
                    foreach (var eachStat in results.Statistics)
                        switch (eachStat.StatisticName)
                        {
                            case "VerifiedStatus":
                                {
                                    if (RememberMeLogin.isOn == true)
                                    {
                                        //SAVE PLAYER CREDENTIALS ON LOCAL PC.
                                        if (PlayerPrefs.GetInt("USERSAVED") == 0)
                                        {
                                            PlayerPrefs.SetInt("USERSAVED", 1);
                                            PlayerPrefs.SetString("USERNAME", UserNameLogin);
                                            PlayerPrefs.SetString("PASSWORD", UserPasswordLogin);
                                            PlayerPrefs.Save();

                                            //SceneManager.LoadSceneAsync(1);
                                            NetworkManagerHUD.enableHUD();
                                        }
                                        else if (PlayerPrefs.GetInt("USERSAVED") == 1) //THERE IS A PLAYER EXISTED ON THE PC, THIS FIELD WILL SAVE A NEW ACCOUNT.
                                        {
                                            int whichSlotIsOpen = 2;

                                            if (PlayerPrefs.GetInt("USERSAVED2") == 0)
                                            {
                                                whichSlotIsOpen = 2;
                                            }
                                            else if (PlayerPrefs.GetInt("USERSAVED3") == 0)
                                            {
                                                whichSlotIsOpen = 3;
                                            }

                                            PlayerPrefs.SetInt("USERSAVED" + whichSlotIsOpen, 1);
                                            PlayerPrefs.SetString("USERNAME" + whichSlotIsOpen, UserNameLogin);
                                            PlayerPrefs.SetString("PASSWORD" + whichSlotIsOpen, UserPasswordLogin);
                                            PlayerPrefs.Save();

                                            //SceneManager.LoadSceneAsync(1);
                                            NetworkManagerHUD.enableHUD();
                                        }
                                    }
                                    else
                                    {
                                        //IF PLAYER CREDENTIALS SAVING HAS BEEN SKIPPED.

                                        //SceneManager.LoadSceneAsync(1);
                                        NetworkManagerHUD.enableHUD();
                                    }
                                    if (eachStat.Value == 0)
                                    {
                                        Instantiate(AccountVerificationReminder);
                                    }
                                    break;
                                }
                        }
                }, error => Debug.LogError(error.GenerateErrorReport())
);
        }
    }

    private void OnLoginFailure(PlayFabError error)
    {
        ErrorGUI();
        Debug.LogError(error.GenerateErrorReport());
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

    public void Register()
    {
        LoadingGUI();
        var registerRequest = new RegisterPlayFabUserRequest { RequireBothUsernameAndEmail = true, Username = UserNameRegister, DisplayName = UserNameRegister, Email = UserEmailRegister, Password = UserPasswordRegister };
        PlayFabClientAPI.RegisterPlayFabUser(registerRequest, OnRegisterSuccess => {

            var request = new LoginWithPlayFabRequest { Username = UserNameRegister, Password = UserPasswordRegister };
            PlayFabClientAPI.LoginWithPlayFab(request, loginRes =>
            {
                var request2 = new AddOrUpdateContactEmailRequest
                {
                    EmailAddress = UserEmailRegister
                };
                PlayFabClientAPI.AddOrUpdateContactEmail(request2, result =>
                {
                    PlayFabClientAPI.UpdateUserPublisherData(new UpdateUserDataRequest()
                    {
                        Data = new Dictionary<string, string>() {
                        {"PlayerOwnedColors", desierdColor.ToString()},
                        {"PlayerCurrentColor", desierdColor.ToString()}
                        }
                    }, result2 => {
                        Debug.Log("Registered");
                        RegisterMessage.color = new Color32(26, 238, 0, 255);
                        RegisterMessage.text = "Registred";

                        LoadingPanel.GetComponent<Animator>().SetTrigger("End");//
                        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
                    {
                        FunctionName = "NewPlayer",
                        GeneratePlayStreamEvent = true,
                    }, results5 => {
                        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                    }, error5 => { });
                        },
                        error =>
                        {
                            Debug.Log(error.GenerateErrorReport());
                            RegisterMessage.color = new Color32(255, 0, 30, 255);
                            RegisterMessage.text = "Error";
                        });
                }, OnRegisterFailure2 => { Register(); });
            }, OnRegisterFailure3 => { Register(); });
        }, OnRegisterFailure);
    }

    private void OnRegisterFailure(PlayFabError error)
    {
        ErrorGUI();
        Debug.LogError(error.GenerateErrorReport());
        RegisterMessage.color = new Color32(255, 0, 30, 255);
        RegisterMessage.text = "Error";
    }





    #region Gui_InputFields


    public void getUserPasswordLogin(string passwordIn)
    {
        UserPasswordLogin = passwordIn;
    }

    public void GetUserNameLogin(string UsernameIn)
    {
        UserNameLogin = UsernameIn;
    }

    public void GetUserNameRegister(string UsernameIn)
    {
        UserNameRegister = UsernameIn;
        registerPlayerNameGUI.text = UsernameIn;
    }
    public void GetUserEmailRegister(string UsernameIn)
    {
        UserEmailRegister = UsernameIn;
    }
    public void getUserPasswordRegister(string passwordIn)
    {
        UserPasswordRegister = passwordIn;
    }
    public void ResetEmailField(string emailIn)
    {
        resetEmail = emailIn;
    }



    public void Color(int Color)
    {
        desierdColor = Color;
    }
    #endregion

    public void SendRecoveryEmail()
    {
        var request = new SendAccountRecoveryEmailRequest { Email = resetEmail, TitleId = PlayFabSettings.staticSettings.TitleId};
        PlayFabClientAPI.SendAccountRecoveryEmail(request, result =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }, OnLoginFailure);
    }

    public void OnclickLogin()
    {
        LoadingGUI();
        var request = new LoginWithPlayFabRequest { Username = UserNameLogin, Password = UserPasswordLogin };
        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnLoginFailure);
    }
    public void OnExistedLogin(string userName, string passWord)
    {
        LoadingGUI();
        var request = new LoginWithPlayFabRequest { Username = userName, Password = passWord };
        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnLoginFailure);
    }


    public void LoadingGUI()
    {
        LoadingPanel.SetActive(true);
        LoadingPanel.GetComponent<Animator>().SetTrigger("Start");
    }
    public void ErrorGUI()
    {
        LoadingPanel.GetComponent<Animator>().SetTrigger("End");
    }


    public void GuestLogin()
    {
        LoadingGUI();
        //SceneManager.LoadSceneAsync(1);
        NetworkManagerHUD.enableHUD();
    }
}