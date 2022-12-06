using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Refresh : MonoBehaviour
{
    public GameObject RefreshCanvas;
    public string bannercode, interstialcode;
    public string adUrl;
    public Button redirectionButton;
    public Text touchCount;
    public static Refresh Instance;


    public UniGifImage m_uniGifImage, m_uniGifVideo;
    public RawImage adImage, advideo;
    public GameObject crossButtonState, background;
    public Image adImage1, adVideo1;
    public GameObject canvasObj;
    public Button redirectionButtonBanner;



    int i = 1;
    public void Awake()
    {
        redirectionButton.interactable = false;
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        RefreshCanvas = gameObject;
    }
    public void DestroyCanvas()
    {
        gameObject.SetActive(false);
    }
    public void BannerOpenUrl()
    {
        Debug.Log("this is code" + bannercode);
        if (bannercode != null)
        {
            Application.OpenURL(bannercode);
        }
    }
    public void InterstialAdUrl()
    {        
        if (interstialcode != null)
        { 
            Application.OpenURL(interstialcode);
        }
    }
}
