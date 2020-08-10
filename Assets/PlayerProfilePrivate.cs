using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using Mirror;
using UnityEngine.EventSystems;

public class PlayerProfilePrivate : MonoBehaviour, IDragHandler, IEndDragHandler
{
    // Start is called before the first frame update
    public TMP_Text PlayerName;
    public TMP_Text Coins;

    private RectTransform rectTransform;
    [SerializeField] private Canvas canvas;
    private bool isOverProfile;
    public Vector2 InstantiatePosition;
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

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



            }, fail2 => { Start(); });

        }, fail => { Start(); });
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);
        if (!screenRect.Contains(Input.mousePosition))
        {
            this.rectTransform.localPosition = InstantiatePosition;
            this.gameObject.SetActive(false);
        }
    }
}