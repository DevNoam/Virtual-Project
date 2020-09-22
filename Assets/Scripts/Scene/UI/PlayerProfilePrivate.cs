using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using Mirror;

public class PlayerProfilePrivate : MonoBehaviour
{
    // Start is called before the first frame update
    public TMP_Text PlayerName;
    public TMP_Text Coins;

    public void RequestData()
    {
        GetPlayerProfileRequest request = new GetPlayerProfileRequest();
        PlayFabClientAPI.GetPlayerProfile(request, Success =>
        {
            PlayerName.text = Success.PlayerProfile.DisplayName;
            GetUserInventoryRequest request2 = new GetUserInventoryRequest();
            PlayFabClientAPI.GetUserInventory(request2, success =>
            {
                int Currency;
                success.VirtualCurrency.TryGetValue("CO", out Currency);
                Coins.text = "Balance: " + Currency.ToString();

            }, fail2 => { RequestData(); });

        }, fail => { RequestData(); });
    }
}