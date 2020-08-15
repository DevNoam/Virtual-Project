using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class OpenChatLog : MonoBehaviour, IEndDragHandler, IDragHandler, IBeginDragHandler
{
    bool isOpened = false;
    bool isDragging = false;
    bool closePanel = false;
    public RectTransform Panel;
    public Canvas canvas;

    public float Open;
    public float Closed;
    public GameObject ClosePanel;
    public TMP_Text ButtonText;
    Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);

    public void OpenButton()
    {
        if (isDragging != true)
        {
            if (isOpened == false)
            {
                Panel.offsetMin = new Vector2(Panel.offsetMin.x, Open);
                isOpened = true;
                ButtonText.text = "CLOSE";
            }
            else if (isOpened == true)
            {
                Panel.offsetMin = new Vector2(Panel.offsetMin.x, Closed);
                isOpened = false;
                ButtonText.text = "CHAT LOG";
            }
        }
    
    }


    public void OnDrag(PointerEventData eventData)
    {
        Panel.offsetMin = new Vector2(Panel.offsetMin.x, eventData.position.y / canvas.scaleFactor);
        if (!screenRect.Contains(Input.mousePosition))
        {
            ClosePanel.SetActive(true);
            closePanel = true;
        }
        if (screenRect.Contains(Input.mousePosition) && closePanel == true)
        {
            ClosePanel.SetActive(false);
            closePanel = false;
        }

    }

    public void OnEndDrag(PointerEventData eventData)
    {

        isDragging = false;
        if (isOpened == false)
        {
            isOpened = true;
            ButtonText.text = "CLOSE";
        }
        if (!screenRect.Contains(Input.mousePosition))
        {
            Panel.offsetMin = new Vector2(Panel.offsetMin.x, 767);
            ButtonText.text = "CHAT LOG";
            isOpened = false;

        }
        ClosePanel.SetActive(false);

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
    }
}
