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
        public Text userTournamentTokensTxt;
        [SerializeField] GameObject Container;
        public Text userClientTokensTxt;

        private void Start()
        {
            if (Screen.orientation != ScreenOrientation.Portrait)
            {
                Container.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
                Container.transform.position = new Vector3(Container.transform.position.x, Container.transform.position.y + 34, Container.transform.position.z);
            }
        }

        void OnEnable()
        {
            userTournamentTokensTxt.text = MegafansPrefs.TournamentEntryTokens.ToString();
            userClientTokensTxt.text = MegafansPrefs.CurrentTokenBalance.ToString();
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
                        MegafansWebService.Instance.GetFreeTokensCount(OnGetCreditsSuccess, OnGetCreditsFailure);
                    }, true);
                }
                else
                {
                    MegafansWebService.Instance.GetFreeTokensCount(OnGetCreditsSuccess,OnGetCreditsFailure);
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
            userTournamentTokensTxt.text = MegafansPrefs.TournamentEntryTokens.ToString();
            userClientTokensTxt.text = MegafansPrefs.CurrentTokenBalance.ToString();
        }

        internal void UpdateTokensGotGet(string _TokensToGet)
        {
            m_TokensCountToAdd.text = "X " + _TokensToGet;
        }
    }
}
