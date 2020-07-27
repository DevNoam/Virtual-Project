using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;

public class PlayerExistedLogin : MonoBehaviour
{
    string playerName;
    public TMP_Text playerNametext;
    PlayFabLogin playfabLogin;

    // Start is called before the first frame update
    void Start()
    {
        playerName = PlayerPrefs.GetString("USERNAME");
        playerNametext.text = playerName.ToUpper();
        playfabLogin = GameObject.Find("NetworkManager").GetComponent<PlayFabLogin>();
    }


    
    public void ButtonOnClick()
    {
        playfabLogin.LoadingGUI();
        string UserPasswordLogin = PlayerPrefs.GetString("PASSWORD");

        var request = new LoginWithPlayFabRequest { Username = playerName, Password = UserPasswordLogin };
        PlayFabClientAPI.LoginWithPlayFab(request, LoginSuccess, LoginFaild);
    }

    void LoginSuccess(LoginResult result)
    {
        SceneManager.LoadSceneAsync(1);
    }

    void LoginFaild(PlayFabError error)
    {
        playfabLogin.ErrorGUI();
        Debug.LogError(error.GenerateErrorReport());
    }


    #region RemoveAccount

    public void RemoveAccount()
    {
        PlayerPrefs.DeleteKey("USERSAVED");
        PlayerPrefs.DeleteKey("USERNAME");
        PlayerPrefs.DeleteKey("PASSWORD");
        PlayerPrefs.Save();
        playfabLogin.LoadingGUI();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    #endregion
}
