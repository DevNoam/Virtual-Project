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
    private string resetEmail;
    public GameObject RegisterPanel;
    public Toggle RememberMeLogin;

    public GameObject LoadingPanel;
    public GameObject Registred;


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
        else
        {
            LoadingGUI();
            SceneManager.LoadSceneAsync(1);
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
                    Debug.Log("Registered");
                    Registred.SetActive(true);
                    LoadingPanel.SetActive(false);

                }, OnRegisterFailure => { Register(); });
            }, OnRegisterFailure => { Register(); });
        }, OnRegisterFailure);
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
    public void ResetEmailField(string emailIn)
    {
        resetEmail = emailIn;
    }


    public void SendRecoveryEmail()
    {
        var request = new SendAccountRecoveryEmailRequest { Email = resetEmail, TitleId = PlayFabSettings.staticSettings.TitleId, EmailTemplateId = "9D363D121634B726" };
        PlayFabClientAPI.SendAccountRecoveryEmail(request, result =>
        {
            Debug.Log("The player's account now has username and password");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }, OnLoginFailure);
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