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
    PlayFabLogin playFabLogin;

    // Start is called before the first frame update
    void Start()
    {
        playerName = PlayerPrefs.GetString("USERNAME");
        playerNametext.text = playerName.ToUpper();
        playfabLogin = GameObject.Find("Manager").GetComponent<PlayFabLogin>();
    }


    
    public void ButtonOnClick()
    {
        string UserPasswordLogin = PlayerPrefs.GetString("PASSWORD");

        playfabLogin.OnExistedLogin(playerName, UserPasswordLogin);
    }



    #region RemoveAccount

    public void RemoveAccount()
    {
        PlayerPrefs.DeleteKey("USERSAVED");
        PlayerPrefs.DeleteKey("USERNAME");
        PlayerPrefs.DeleteKey("PASSWORD");
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    #endregion
}
