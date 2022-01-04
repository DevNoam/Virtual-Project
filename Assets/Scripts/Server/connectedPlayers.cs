using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class connectedPlayers : NetworkBehaviour
{
    private string[] activePlayers;

    [Server]
    public bool CheckIfActive(string usrName)
    {
        for (int i = 0; i < activePlayers.Length; i++)
        {
            if (activePlayers[i] == usrName)
                return true;
                //Player is active.
        }
        return false;
        //Player not active.
    }

    [Server]
    public void RemovePlayer(string usrName)
    {
        for (int i = 0; i < activePlayers.Length; i++)
        {
            if (activePlayers[i] == usrName)
                activePlayers[i] = null;
        }
    }
    [Server]
    public void AddPlayer(string usrName)
    {
        activePlayers[activePlayers.Length + 1] = usrName;
    }

}
