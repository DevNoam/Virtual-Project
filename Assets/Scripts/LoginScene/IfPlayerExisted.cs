using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IfPlayerExisted : MonoBehaviour
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
            AddplayersProfiles();
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
    void AddplayersProfiles()
    {
            GameObject playerProfile = Instantiate(PlayerProfilePrefab, PlayerProfilePrefab.transform.position, PlayerProfilePrefab.transform.rotation) as GameObject;
            playerProfile.transform.SetParent(contentPanel.transform, false);
    }
}
