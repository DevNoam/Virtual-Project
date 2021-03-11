using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IfPlayerExist : MonoBehaviour
{
    public GameObject loginPanel;
    public GameObject ExistedPlayer;
    public GameObject AddAccount;


    public GameObject PlayerProfilePrefab;
    public Transform contentPanel;



    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("USERSAVED"))
        {
            int whomanyplayerssaved = 0;
            if (PlayerPrefs.HasKey("USERSAVED"))
            {
                whomanyplayerssaved++;
                if (PlayerPrefs.HasKey("USERSAVED2"))
                {
                    whomanyplayerssaved++;
                    if (PlayerPrefs.HasKey("USERSAVED3"))
                    {
                        whomanyplayerssaved++;
                    }
                }
                if (whomanyplayerssaved == 1)
                {
                    AddplayersProfiles(new Vector3(0, 0, 0), 1);
                }
                else if (whomanyplayerssaved == 2)
                {
                    //Set first 2 Players positions:
                    AddplayersProfiles(new Vector3(130, 25, 0), 2); //Player 1
                    AddplayersProfiles(new Vector3(-130, 25, 0), 1); //Player 2
                }
                else if (whomanyplayerssaved == 3)
                {
                    //Set the 3 Players card positions:
                    AddplayersProfiles(new Vector3(260, 25, 0), 3); //Player 1
                    AddplayersProfiles(new Vector3(0, 25, 0), 2); //Player 2
                    AddplayersProfiles(new Vector3(-260, 25, 0), 1); //Player 3
                    AddAccount.gameObject.SetActive(false);
                }
            }
        } 
        else 
        {
            loginPanel.SetActive(true);
            ExistedPlayer.SetActive(false);
        }
    }
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    void AddplayersProfiles(Vector3 position, int playerNumber)
    {
        GameObject playerProfile = Instantiate(PlayerProfilePrefab, position, PlayerProfilePrefab.transform.rotation) as GameObject;
        playerProfile.transform.SetParent(contentPanel.transform, false);
        playerProfile.GetComponent<PlayerExistedLogin>().slotNumber = playerNumber;
    }
}
