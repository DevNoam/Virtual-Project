using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;

public class PlayerExistedLogin : MonoBehaviour
{
    string playerName;
    public TMP_Text playerNametext;
    PlayFabLogin playfabLogin;
    public int slotNumber;
    //public GameObject playerImage;
    //public Sprite[] playerImageLlist;

    // Start is called before the first frame update
    void Start()
    {
        if (slotNumber <= 1)
        {
            playerName = PlayerPrefs.GetString("USERNAME");
            //playerImage.GetComponent<Image>().sprite = playerImageLlist[PlayerPrefs.GetInt("PLAYERCOLOR")];
        }
        else if (slotNumber >= 2)
        {
            playerName = PlayerPrefs.GetString("USERNAME" + slotNumber);
            //playerImage.GetComponent<Image>().sprite = playerImageLlist[PlayerPrefs.GetInt("PLAYERCOLOR" + slotNumber)];
        }
        playerNametext.text = playerName.ToUpper();
        playfabLogin = GameObject.Find("LoginManager").GetComponent<PlayFabLogin>();
    }


    
    public void ButtonOnClick()
    {
        if (slotNumber <= 1)
        {
            string UserPasswordLogin = PlayerPrefs.GetString("PASSWORD");
            playfabLogin.OnExistedLogin(playerName, UserPasswordLogin);
        } else if (slotNumber >= 2)
        {
            string UserPasswordLogin = PlayerPrefs.GetString("PASSWORD" + slotNumber);
            playfabLogin.OnExistedLogin(playerName, UserPasswordLogin);
        }
    }



    #region RemoveAccount

    public void RemoveAccount()
    {
            if (slotNumber == 1) // REMOVE PLAYER 1 FROM LOCAL PC.
            {
            //REMOVE FIRST PLAYER
            PlayerPrefs.DeleteKey("USERNAME");
            PlayerPrefs.DeleteKey("PASSWORD");
            PlayerPrefs.DeleteKey("USERSAVED");
            PlayerPrefs.DeleteKey("PLAYERCOLOR");
            
            //IF THERE IS SECOND PLAYER
            if (PlayerPrefs.GetInt("USERSAVED2") == 1)
            {
                PlayerPrefs.SetString("USERNAME", PlayerPrefs.GetString("USERNAME2"));
                PlayerPrefs.SetString("PASSWORD", PlayerPrefs.GetString("PASSWORD2"));
                PlayerPrefs.SetInt("PLAYERCOLOR", PlayerPrefs.GetInt("PLAYERCOLOR2"));
                PlayerPrefs.SetInt("USERSAVED", 1);
                PlayerPrefs.DeleteKey("USERNAME2");
                PlayerPrefs.DeleteKey("PASSWORD2");
                PlayerPrefs.DeleteKey("USERSAVED2");
                PlayerPrefs.DeleteKey("PLAYERCOLOR2");
            }
            //IF THERE IS THIRD PLAYER
            if (PlayerPrefs.GetInt("USERSAVED3") == 1) //IF THERE IS THIRD PLAYER SAVED ON THE PC.
            {
                PlayerPrefs.SetString("USERNAME2", PlayerPrefs.GetString("USERNAME3"));
                PlayerPrefs.SetString("PASSWORD2", PlayerPrefs.GetString("PASSWORD3"));
                PlayerPrefs.SetInt("PLAYERCOLOR2", PlayerPrefs.GetInt("PLAYERCOLOR3"));
                PlayerPrefs.SetInt("USERSAVED2", 1);
                PlayerPrefs.DeleteKey("USERNAME3");
                PlayerPrefs.DeleteKey("PASSWORD3");
                PlayerPrefs.DeleteKey("USERSAVED3");
                PlayerPrefs.DeleteKey("PLAYERCOLOR3");

            }
            PlayerPrefs.Save();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

            }
            else if (slotNumber == 2) //REMOVE PLAYER 2 FROM LOCAL PC.
            {
            //REMOVE THE SECOND PLAYER 
                PlayerPrefs.DeleteKey("USERSAVED2");
                PlayerPrefs.DeleteKey("USERNAME2");
                PlayerPrefs.DeleteKey("PASSWORD2");
                PlayerPrefs.DeleteKey("PLAYERCOLOR2");
                
                //IF THERE IS THIRD PLAYER
                if (PlayerPrefs.GetInt("USERSAVED3") == 1)
                {
                    PlayerPrefs.SetString("USERNAME2", PlayerPrefs.GetString("USERNAME3"));
                    PlayerPrefs.SetString("PASSWORD2", PlayerPrefs.GetString("PASSWORD3"));
                    PlayerPrefs.SetInt("PLAYERCOLOR2", PlayerPrefs.GetInt("PLAYERCOLOR3"));
                    PlayerPrefs.SetInt("USERSAVED2", 1);

                    PlayerPrefs.DeleteKey("USERSAVED3");
                    PlayerPrefs.DeleteKey("USERNAME3");
                    PlayerPrefs.DeleteKey("PASSWORD3");
                    PlayerPrefs.DeleteKey("PLAYERCOLOR3");
                }
                PlayerPrefs.Save();
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            else if (slotNumber == 3) //REMOVE PLAYER 3 FROM LOCAL PC.
            {
                PlayerPrefs.DeleteKey("USERSAVED3");
                PlayerPrefs.DeleteKey("USERNAME3");
                PlayerPrefs.DeleteKey("PASSWORD3");
                PlayerPrefs.DeleteKey("PLAYERCOLOR3");
                PlayerPrefs.Save();
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    #endregion
}
