#pragma warning disable 649

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using MegafansSDK.Utils;


namespace MegafansSDK.UI
{

    public class TermsOfUseAndRulesWindow : MonoBehaviour
    {

        [SerializeField] Text headerLabelTxt;
        [SerializeField] Button backToGameBtn;
        [SerializeField] Scrollbar scrollbarForText;
        [SerializeField] RectTransform contentHolderForText;
        [SerializeField] Text textPrefab;

        readonly List<Text> textBoxes = new List<Text>();

        LevelsResponseData levelInfo;

        public void Init(LevelsResponseData levelInfo)
        {
            this.levelInfo = levelInfo;
            if (this.levelInfo != null)
                headerLabelTxt.text = "Tournament Rules";
        }

        public void Init(string informationType, bool isSignUp)
        {
            this.levelInfo = null;

            if (informationType == MegafansConstants.TERMS_OF_USE)
            {
                headerLabelTxt.text = "Terms of Use";
                MegafansWebService.Instance.GetTermsOfUse(OnGetTournamentTermsOrPrivacyPolicyResponse, LogError);
            }
            else if (informationType == MegafansConstants.PRIVACY_POLICY)
            {
                headerLabelTxt.text = "Privacy Policy";
                MegafansWebService.Instance.GetPrivacyInfo(OnGetTournamentTermsOrPrivacyPolicyResponse, LogError);
            }

            //backToGameBtn.gameObject.SetActive(!isSignUp);
        }

        void OnEnable()
        {
            if (levelInfo != null)
            {
                MegafansWebService.Instance.GetTournamentRules(this.levelInfo.guid, OnGetTournamentRulesResponse, LogError);
            }
        }

        void OnDisable()
        {
            foreach (var text in textBoxes)
                text.text = "";
        }

        public void CloseTermsAndConditions()
        {
            MegafansUI.Instance.BackFromTermsOfUseOrRulesWindow();
        }

        void OnGetTournamentRulesResponse(TournamentRulesResponse response)
        {
            if (!response.success.Equals(MegafansConstants.SUCCESS_CODE) || response.data?.Count < 1)
                return;

            //TODO: request that they change the backend on this over to a better format
            string text = Environment.NewLine;
            foreach (var data in response.data)
                text += (data.description + Environment.NewLine);
            SetUnityTextObjects(text);
        }

        void OnGetTournamentTermsOrPrivacyPolicyResponse(TermsOrPrivacyPolicyResponse response)
        {
            if (!response.success.Equals(MegafansConstants.SUCCESS_CODE) || response.data?.description == null)
                return;

            //TODO: request that they change the backend on this over to a better format
            SetUnityTextObjects(HTMLDataToText(response.data.description));
        }

        void LogError(string error)
        {
            Debug.LogError(error);
        }

        string HTMLDataToText(string html)
        {
            var data = html;
            data = data.Insert(0, Environment.NewLine + Environment.NewLine); //For spacing
            data = data.Replace("<strong>", "<b>"); data = data.Replace("</strong>", "</b>");
            data = data.Replace("<p>", ""); data = data.Replace("</p>", "");
            data = data.Replace("<u>", ""); data = data.Replace("</u>", "");
            data = data.Replace("<ol>", ""); data = data.Replace("</ol>", "");
            data = data.Replace("<em>", ""); data = data.Replace("</em>", "");
            data = data.Replace("</span>", "");
            data = data.Replace("</li>", "");
            data = data.Replace("</a>", "");
            data = data.Replace("&nbsp;", "");
            data = data.Replace("&ndash;", "–");
            data = data.Replace("&ldquo;", "\"");
            data = data.Replace("&rdquo;", "\"");
            for (int i = 0; i < data.Length;)
            {
                if (data[i] == '<' && (data[i + 1] == 's' || data[i + 1] == 'l' || data[i + 1] == 'a' || data[i + 1] == 'o'))
                {
                    int c = 0;
                    while (data[i + c] != '>')
                        c++;
                    data = data.Remove(i, c + 1);
                }
                else
                {
                    i++;
                }
            }
            return data;
        }

        void SetUnityTextObjects(string text)
        {
            string[] spiltData = text.Split(new string[1] { Environment.NewLine }, StringSplitOptions.None);
            for (int i = 0; i < spiltData.Length; i++)
            {
                if (i < textBoxes.Count)
                {
                    textBoxes[i].text = spiltData[i];
                    textBoxes[i].transform.localScale = Vector3.one;
                }
                else
                {
                    //TODO: merge short paragraphs before creating new instantiated text prefabs
                    //Need to create multiple text objects because of a hard limit on number of char
                    var instText = Instantiate<Text>(textPrefab);
                    instText.transform.SetParent(contentHolderForText);
                    instText.transform.localScale = Vector3.one;
                    instText.fontSize = 32;
                    instText.text = spiltData[i];
                    textBoxes.Add(instText);
                }
            }

            //Clear up extra text boxes from the last use
            if (textBoxes.Count - spiltData.Length > 0)
            {
                for (int i = spiltData.Length; i < textBoxes.Count; i++)
                    Destroy(textBoxes[i].gameObject);
                textBoxes.RemoveRange(spiltData.Length, textBoxes.Count - spiltData.Length);
            }

            //Wait two frames then we set the the scroll bar back to the top
            StartCoroutine(WaitTwoFramesForScrollBar());
        }


        //We do this because
        IEnumerator WaitTwoFramesForScrollBar()
        {
            yield return null;
            yield return null;
            scrollbarForText.value = 1f;
        }
    }
}