using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using Mirror;

public class CommandsManager : NetworkBehaviour
{
    private bool lookingForFunction = false;

    private string _Parameter;

    [SerializeField]
    private PlayerManager playerManager;

    [Client]
    public void ReceivedCommand(string CommandName)
    {
        _Parameter = null; //Clear the Parameter Variable.
        string command = CommandName; //Making a new Variable for easy coding.
        Debug.Log("Command sent!, The command: " + command);  //Telling the Client that his command has been sent.

        command = command.Substring(1);
        command = command.ToUpper(); //Make the whole command string with CAPS.

        lookingForFunction = true;

        for (int i = 0; i < 2; i++)
        {
            if (lookingForFunction == true && i == 0) // THIS IS LOCAL COMMAND
            {
                if (command.Contains(" ")) //Check if there is a Parameter to the command for Example "/Warp "Disco""
                {
                    _Parameter = command.Split(' ')[1];
                }
                Invoke(command.Split(' ')[0], 0);
            }

            if (lookingForFunction == true && i > 0) // THIS IS PLAYFAB COMMAND
            {
                string commanditself = command.Split(' ')[0];
                //Searching for command in PlayFab CloudScript & Execute if existed
                PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
                {
                    FunctionName = commanditself,
                    FunctionParameter = new { parameter = _Parameter },
                    GeneratePlayStreamEvent = true
                }, CloudResults => {
                    Debug.Log("Executed PlayFab CloudScript!");
                    lookingForFunction = false;
                }, error => { });
                break;
            }
        }
    }

    //////////
    /// MAKE SURE TO WRITE THE VOID's WITH CAPS!
    ///////////

    [Client]
    private void PING()
    {
        lookingForFunction = false;
        Debug.Log("PONG! " + _Parameter);
        Debug.Log("Your ping status: " + (int)(NetworkTime.rtt * 1000));

        //playerspeed = Parameter; //EXAMPLE FOR USING LOCAL COMMAND USING PARAMETER
        // Here you can execute custom PlayFab request, or change local value like the speed of the Local Player.
    }


    //Change room
    [Client]
    private void WARP()
    {
        if(_Parameter == "Disco")
        lookingForFunction = false;
        playerManager.roomsManager.ChangeRoom(_Parameter, new Vector3(0, 0, 0));
        Debug.Log("Teleported to: " + _Parameter);
    }

    [Client]
    private void RESPAWN()
    {
        lookingForFunction = false;
        playerManager.roomsManager.ChangeRoom(gameObject.scene.name, new Vector3(0, 0, 0));
        Debug.Log("RESPAWNED!");
    }

    [Client]
    private void WAVE()
    {
        playerManager.animationsManager.AnimationWaving();
    }
}
