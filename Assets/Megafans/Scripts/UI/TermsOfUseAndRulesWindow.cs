using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using MegafansSDK.Utils;
using MegafansSDK.UI;

namespace MegafansSDK.UI
{

    public interface WebViewLoadingTarget
    {
        void OnStartLoading();
        void OnFinishedLoading();

    }

    public class TermsOfUseAndRulesWindow : MonoBehaviour
    {

        [SerializeField] private Text headerLabelTxt;
        [SerializeField] private Text userTokensTxt;
        [SerializeField] private ListBox listBox;
        [SerializeField] private GameObject expandableListItemPrefab;
        [SerializeField] private Button helpBtn;
        [SerializeField] private Button backToGameBtn;
        [SerializeField] private Button userTokenHeaderBtn;


        List<TournamentRulesResponseData> rulesOrTermsData;
        LevelsResponseData levelInfo;
        WebView webViewScreen;
        string informationType;

        public void Init(LevelsResponseData levelInfo)
        {
            this.informationType = null;
            this.levelInfo = levelInfo;
            if (this.levelInfo != null)
            {
                headerLabelTxt.text = "How To Play";
            }
        }

        public void Init(string informationType, bool isSignUp)
        {
            this.levelInfo = null;
            this.informationType = informationType;
            if (informationType == MegafansConstants.TERMS_OF_USE)
            {
                headerLabelTxt.text = "Terms of Use";
            }
            else if (informationType == MegafansConstants.PRIVACY_POLICY)
            {
                headerLabelTxt.text = "Privacy Policy";
            }

            if (isSignUp)
            {
                helpBtn.gameObject.SetActive(false);
                backToGameBtn.gameObject.SetActive(false);
                userTokenHeaderBtn.gameObject.SetActive(false);
            }

            //if (webViewScreen == null) {
            //    webViewScreen = (new GameObject("WebView")).AddComponent<WebView>();
            //} else {
            //    Destroy(webViewScreen);
            //    webViewScreen = (new GameObject("WebView")).AddComponent<WebView>();
            //}
            //webViewScreen.Url = MegafansConstants.PRIVACY_POLICY_URL;
        }

        private void OnEnable()
        {
            userTokensTxt.text = MegafansPrefs.CurrentTokenBalance.ToString();
            listBox.ClearList();
            Vector3 scale = this.transform.localScale;
            //Resolution res = Screen.currentResolution;
            if (this.levelInfo == null) {
                Debug.Log("**********************************");
                Debug.Log("No Level Info");
                Debug.Log("**********************************");
                ShowWebView();
            } else {
                MegafansWebService.Instance.GetTournamentRules(this.levelInfo.id, OnGetTournamentRulesResponse, OnGetTournamentRulesError);
            }
        }

        private void OnDisable()
        {
            HideWebView();
        }

        public void ShowWebView()
        {
            if (!string.IsNullOrEmpty(this.informationType))
            {
                if (webViewScreen == null)
                {
                    webViewScreen = (new GameObject("WebView")).AddComponent<WebView>();
                }
                else
                {
                    Destroy(webViewScreen);
                    webViewScreen = (new GameObject("WebView")).AddComponent<WebView>();
                }
                if (informationType == MegafansConstants.TERMS_OF_USE)
                {
                    webViewScreen.Url = MegafansConstants.TERMS_OF_USE_URL;
                }
                else if (informationType == MegafansConstants.PRIVACY_POLICY)
                {
                    webViewScreen.Url = MegafansConstants.PRIVACY_POLICY_URL;
                }
            }
        }


        public void HideWebView()
        {
            if (webViewScreen)
            {
                webViewScreen.CloseWebView();
            }
        }

        public void CloseTermsAndConditions()
        {
            //if (webViewScreen)
            //{
            //    webViewScreen.CloseWebView();
            //}
            MegafansUI.Instance.BackFromTermsOfUseOrRulesWindow();
        }

        private void FillRulesOrTermsData(List<TournamentRulesResponseData> data)
        {
            for (int i = 0; i < data.Count; i++)
            {
                TournamentRulesResponseData info = data[i];
                GameObject textItem = Instantiate(expandableListItemPrefab);
                ExpandableTextItem textHandler = textItem.GetComponent<ExpandableTextItem>();
                if (textHandler != null)
                {
                    textHandler.SetValues("Step " + info.sequence + ":", info.description);
                    textItem.SetActive(true);
                    listBox.AddItem(textItem);
                }
            }
        }

        private void OnGetTournamentRulesResponse(TournamentRulesResponse response)
        {
            if (response.success.Equals(MegafansConstants.SUCCESS_CODE))
            {
                List<TournamentRulesResponseData> rulesData = response.data;
                if (rulesData.Count == 0 || rulesData == null)
                {
                    //messageTxt.gameObject.SetActive(true);
                    //messageTxt.text = "No tournaments running right now";
                }
                else
                {
                    //messageTxt.gameObject.SetActive(false);
                    rulesOrTermsData = rulesData;
                    FillRulesOrTermsData(rulesData);
                }
            }
        }

        private void OnGetTournamentRulesError(string error)
        {
            Debug.LogError(error.ToString());
        }
    }
}