using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingSplashScreenNewsManager : MonoBehaviour
{
    public Sprite[] imagesDB;
    public Image imageSection;
    public int newNewsIn = 5;

    private void Awake()
    {
        int i = Random.Range(0, imagesDB.Length);
        imageSection.sprite = imagesDB[i];

        Invoke("Awake", newNewsIn);
    }

    public void disableItself()
    {
        //Disable the splash screen when Animation ends.
        this.gameObject.SetActive(false);
    }
}
