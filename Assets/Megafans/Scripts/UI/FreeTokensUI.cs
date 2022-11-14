using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MegafansSDK;
using MegafansSDK.Utils;

namespace MegafansSDK.UI
{
    public class FreeTokensUI : MonoBehaviour
    {
        public Text m_TokensCountToAdd;
        public Text userTokensTxt;

        void OnEnable()
        {
            userTokensTxt.text = MegafansPrefs.CurrentTokenBalance.ToString();
        }

        public void ShowVideoAdForFreeTokens()
        {
            Megafans.Instance.m_AdsManager.m_TenginInstance.SendEvent("VideoAdForFreeTokens");
            MegafansSDK.AdsManagerAPI.AdsManager.instance.ApiCall_FullScreen(needtoShowThirdPartyAds =>
            {

                if (needtoShowThirdPartyAds)
                {
                    MegafansSDK.Megafans.Instance.m_AdsManager.ShowRewardedVideo(() =>
                    {
                        Debug.Log("---------------------- Fucking Update UI -------------------");
                        /*MegafansWebService.Instance.GetCredits(MegafansPrefs.UserId,
                                                               OnGetCreditsSuccess,
                                                               OnGetCreditsFailure);*/
                        MegafansWebService.Instance.GetFreeTokensCount(OnGetCreditsSuccess,
                                                               OnGetCreditsFailure);


                    }, true);
                }
                else
                {
                    /*MegafansWebService.Instance.GetCredits(MegafansPrefs.UserId,
                                                               OnGetCreditsSuccess,
                                                               OnGetCreditsFailure);*/
                    MegafansWebService.Instance.GetFreeTokensCount(OnGetCreditsSuccess,
                                                           OnGetCreditsFailure);
                }
            });
        }

        private void OnGetCreditsFailure(string obj)
        {
            Debug.Log("Getting credits FAILED");
        }
        private void OnGetCreditsSuccess(GetFreeTokensCountResponse response)
        {
            if (response.success.Equals(MegafansConstants.SUCCESS_CODE))
            {
                Debug.Log("trying to add free tokens");
                Debug.Log(response);
                //MegafansPrefs.CurrentTokenBalance = response.data.credits;
                //MegafansPrefs.CurrentTokenBalance = response.data.credits;

                MegafansUI.Instance.UpdateAllTokenTexts();
            }
        }

        private void OnGetCreditsSuccess(CheckCreditsResponse response)
        {
            if (response.success.Equals(MegafansConstants.SUCCESS_CODE))
            {
                MegafansPrefs.CurrentTokenBalance = response.data.credits;

                MegafansUI.Instance.UpdateAllTokenTexts();
            }
        }

        public void OpenOfferWall()
        {
            Megafans.Instance.m_AdsManager.OpenIronOfferWall();
        }

        public void ShowFreeTokensPanel(bool _Show)
        {
            gameObject.SetActive(_Show);
        }

        internal void UpdateCreditUI()
        {
            userTokensTxt.text = MegafansPrefs.CurrentTokenBalance.ToString();
        }

        internal void UpdateTokensGotGet(string _TokensToGet)
        {
            m_TokensCountToAdd.text = "X " + _TokensToGet;
        }
    }
}
