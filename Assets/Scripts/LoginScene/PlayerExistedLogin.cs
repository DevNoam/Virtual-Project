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
    string randomToken;
    void LoginSuccess(LoginResult result)
    {

        randomToken = "TKN" + Random.Range(1, 1000) + "Sector2" + Random.Range(0, 1000);
        Debug.Log(randomToken);
        PlayFabClientAPI.UpdateUserPublisherData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>() {
                        {"SessionToken", randomToken.ToString() }
                        }
        }, result2 =>
        {
            SceneManager.LoadSceneAsync(1);
        }, LoginFaild);
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
