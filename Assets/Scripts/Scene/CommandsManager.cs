using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class CommandsManager : MonoBehaviour
{
    private bool lookingForFunction = false;

    private string _Parameter;
    public void ReceivedCommand(string CommandName)
    {
        string command = CommandName;
        Debug.Log("Command received!, The command: " + command);

        command = command.Substring(1);
        command = command.ToUpper();


        lookingForFunction = true;
        
        for (int i = 0; i < 2; i++)
        {
            if (lookingForFunction == true && i == 0) // THIS IS LOCAL COMMAND
            {
                if (command.Contains(" "))
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
                }, CloudResults => { Debug.Log("Executed PlayFab CloudScript!");
                    lookingForFunction = false;
                }, error => { });
                _Parameter = null;
                break;
            }
        }
    }

    private void FREEHAT()
    {
        Debug.Log("FREE HAT!, You didn't received anything actually. " + _Parameter);


        //playerspeed = Parameter; //EXAMPLE FOR USING LOCAL COMMAND USING PARAMETER
        _Parameter = null; // CLEAR THE PARAMETER!!!

        // Here you can execute custom PlayFab request, or change local value like the speed of the Local Player.
    }
}
