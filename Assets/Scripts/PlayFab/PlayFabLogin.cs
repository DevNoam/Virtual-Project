using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayFabLogin : MonoBehaviour
{
    private string UserNameLogin;
    private string UserPasswordLogin;
    public GameObject loginPanel;
    private string UserNameRegister;
    private string UserEmailRegister;
    private string UserPasswordRegister;
    public GameObject RegisterPanel;
    public Toggle RememberMeLogin;

    public GameObject LoadingPanel;


    public void Start()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            //Please change the titleId below to your own titleId from PlayFab Game Manager
            //If you have already set the value in the Editor Extensions, this can be skipped.
            

            PlayFabSettings.staticSettings.TitleId = "8D6DB";
        }
    }

    private void OnLoginSuccess(LoginResult result)
    {
        if (RememberMeLogin.isOn == true)
        {
            PlayerPrefs.SetInt("USERSAVED", 1);
            PlayerPrefs.SetString("USERNAME", UserNameLogin);
            PlayerPrefs.SetString("PASSWORD", UserPasswordLogin);
            PlayerPrefs.Save();

            LoadingGUI();
            SceneManager.LoadSceneAsync(1);
        }
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("Registered");

        RegisterPanel.SetActive(false);
        loginPanel.SetActive(true);
    }

    private void OnLoginFailure(PlayFabError error)
    {
        ErrorGUI();
        Debug.LogError(error.GenerateErrorReport());
    }

    public void Register()
    {
        LoadingGUI();
        var registerRequest = new RegisterPlayFabUserRequest { Username = UserNameRegister, DisplayName = UserNameRegister, Email = UserEmailRegister, Password = UserPasswordRegister };
        PlayFabClientAPI.RegisterPlayFabUser(registerRequest, OnRegisterSuccess, OnRegisterFailure);
    }

    private void OnRegisterFailure(PlayFabError error)
    {
        ErrorGUI();
        Debug.LogError(error.GenerateErrorReport());
    }

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
    }
    public void GetUserEmailRegister(string UsernameIn)
    {
        UserEmailRegister = UsernameIn;
    }

    public void getUserPasswordRegister(string passwordIn)
    {
        UserPasswordRegister = passwordIn;
    }

    public void OnclickLogin()
    {
        LoadingGUI();
        var request = new LoginWithPlayFabRequest { Username = UserNameLogin, Password = UserPasswordLogin };
        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnLoginFailure);
    }
    public void OnOpenRegisterMenu()
    {
        RegisterPanel.SetActive(true);
    }


    public void LoadingGUI()
    {
        LoadingPanel.SetActive(true);
    }
    public void ErrorGUI()
    {
        LoadingPanel.SetActive(false);
    }
}