using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MegafansSDK.AdsManagerAPI;
using UnityEngine.UI;
using MegafansSDK.Utils;

public class InGameUIHelper : MonoBehaviour
{
    public GameObject m_BoostsPanel;
    public GameObject m_GameFailedPanel;
    public GameObject m_GameWinPanel;
    public GameObject m_GameStartPanel;
    public GameObject[] m_PreGameBoosters;

    void Awake()
    {
        if (MegafansHelper.m_ShoudlOpenMegFans)
        {
            //Helper.m_ShoudlOpenMegFans = false;
            MegafansHelper.m_Instance.ShowMegafans();
        }
    }

    public void HideAllShityPanelForTournament()
    {
        m_GameFailedPanel.SetActive(false);
        //m_GameStartPanel.SetActive(false);
        m_GameWinPanel.SetActive(false);
    }

    public void OpenMegaFunOnButtonClick()
    {
        MegafansHelper.m_Instance.ShowMegafans();
    }

    public void Settings()
    {
        //SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);
        //GameObject.Find("CanvasGlobal").transform.Find("Settings").gameObject.SetActive(true);
    }

    public void Pause()
    {
        //SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.click);

        //if (LevelManager.THIS.gameStatus == GameState.Playing)
            //GameObject.Find("CanvasGlobal").transform.Find("MenuPause").gameObject.SetActive(true);
    }

    internal void ShowBoostsPanel(bool _Show)
    {
        m_BoostsPanel.SetActive(_Show);
    }

    public void ShowOfferWall()
    {
        MegafansSDK.Megafans.Instance.m_AdsManager.ShowOfferwall();
    }

    internal void CleanPreGameBooster()
    {
        foreach (GameObject obj in m_PreGameBoosters)
        {
            obj.transform.GetChild(0).gameObject.SetActive(false);
            obj.GetComponent<Button>().interactable = true;
            obj.transform.GetChild(1).gameObject.SetActive(true);
        }
    }

    public void ShowVideoForBooster(int _ID)
    {
        MegafansSDK.Megafans.Instance.m_AdsManager.m_TenginInstance.SendEvent("VideoAdForBooster");

        MegafansSDK.AdsManagerAPI.AdsManager.instance.ApiCall_FullScreen(needtoShowThirdPartyAds => {

            if (needtoShowThirdPartyAds)
            {
                MegafansSDK.Megafans.Instance.m_AdsManager.ShowRewardedVideo(() =>
                {
                    if (_ID == 0)//Rainbow
                    {
                        //LevelManager.THIS.BoostColorfullBomb = 1;
                    }
                    else if (_ID == 1)
                    {
                        //LevelManager.THIS.BoostPackage = 5;
                    }
                    else
                    {
                        //LevelManager.THIS.BoostStriped = 5;
                    }

                    m_PreGameBoosters[_ID].transform.GetChild(0).gameObject.SetActive(true);
                    m_PreGameBoosters[_ID].GetComponent<Button>().interactable = false;
                    m_PreGameBoosters[_ID].transform.GetChild(1).gameObject.SetActive(false);

                }, false);
            }
            else
            {
                MegafansSDK.Megafans.Instance.m_AdsManager.ShowRewardedVideo(() =>
                {
                    /*if (_ID == 0)//Rainbow
                    {
                        LevelManager.THIS.BoostColorfullBomb = 1;
                    }
                    else if (_ID == 1)
                    {
                        LevelManager.THIS.BoostPackage = 5;
                    }
                    else
                    {
                        LevelManager.THIS.BoostStriped = 5;
                    }*/

                    m_PreGameBoosters[_ID].transform.GetChild(0).gameObject.SetActive(true);
                    m_PreGameBoosters[_ID].GetComponent<Button>().interactable = false;
                    m_PreGameBoosters[_ID].transform.GetChild(1).gameObject.SetActive(false);

                }, false);
            }
        });
       
    }
}
