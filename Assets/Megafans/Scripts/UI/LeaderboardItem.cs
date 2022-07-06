#pragma warning disable 649

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace MegafansSDK.UI
{

    public class LeaderboardItem : MonoBehaviour, IEventSystemHandler, IPointerClickHandler
    {

        [SerializeField] Image keyBG;
        [SerializeField] Text rankTxt;
        [SerializeField] Image valueBG;
        [SerializeField] Text userNameTxt;
        [SerializeField] string userCode;
        [SerializeField] Text scoreTxt;
        [SerializeField] private RawImage avatarPic;
        [SerializeField] private RawImage flagPic;
        [SerializeField] Text prizeTxt;
        [SerializeField] GameObject cashIcon;
        [SerializeField] GameObject tokenIcon;
        [SerializeField] GameObject GiftIcon;

        public void SetValues(string _Rank,
                              string _Name,
                              string _Score,
                              string _UserCode,
                              int _isCash,
                              bool _ReqAvatarImage = false,
                              string _AvatarImgLink = "",
                              bool _ReqFlagmage = false,
                              string _FlagImgLink = "",
                              string _PrizeTxt = "")
        {
            rankTxt.text = _Rank;
            userNameTxt.text = _Name;
            scoreTxt.text = _Score;
            userCode = _UserCode;

            if (string.IsNullOrEmpty(_PrizeTxt))
            {
                prizeTxt.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                if (Megafans.Instance.GetCurrentTournamentData().cash_tournament)
                    cashIcon.SetActive(true);
                else
                    tokenIcon.SetActive(true);
                prizeTxt.gameObject.SetActive(true);
                cashIcon.SetActive(_isCash == 0);
                tokenIcon.SetActive(_isCash == 1);
                GiftIcon.SetActive(_isCash == 2);
                if (_isCash != 2)
                {
                    float _Prize = float.Parse(_PrizeTxt);

                    if (_Prize > 1000)
                    {
                        float _Div = Mathf.Floor(_Prize * 0.01f) * 0.1f;
                        _PrizeTxt = _Div.ToString() + "K";
                    }
                    else
                        _PrizeTxt = Mathf.CeilToInt(_Prize).ToString();
                }

                prizeTxt.text = _PrizeTxt;

            }

            if (_ReqAvatarImage)
                MegafansSDK.Utils.MegafansWebService.Instance.FetchImage(_AvatarImgLink, OnFetchAvatarSuccess, OnFetchAvatarFailure);

            if (_ReqFlagmage)
                MegafansSDK.Utils.MegafansWebService.Instance.FetchImage(_FlagImgLink, OnFetchFlagSuccess, OnFetchFlagFailure);

        }

        public void SetKeyColor(Color bgClr, Color txtClr)
        {

            // keyBG.color = bgClr;
            //rankTxt.color = txtClr;
        }

        public void SetValueColor(Color bgClr, Color txtClr)
        {
            // valueBG.color = bgClr;
            //userNameTxt.color = txtClr;
        }

        public void SelectLeaderBoardRow()
        {
            //ShowWindow(viewProfileWindow);
            //Debug.Log("Button clicked = " + valueTxt);
        }

        private void OnFetchAvatarSuccess(Texture2D tex)
        {
            if (tex != null &&
                avatarPic != null)
                avatarPic.texture = tex;
        }

        void OnFetchAvatarFailure(string error)
        {
            Debug.LogError(error);
        }

        private void OnFetchFlagSuccess(Texture2D tex)
        {
            if (tex != null && flagPic != null)
            {
                flagPic.texture = tex;
                flagPic.gameObject.SetActive(true);
            }
            else
            {
                flagPic.gameObject.SetActive(false);
            }
        }

        void OnFetchFlagFailure(string error)
        {
            Debug.LogError(error);
            flagPic.gameObject.SetActive(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("Button clicked = " + eventData);
            //PointerEventData pData = (PointerEventData)eventData;
            GameObject leaderBoardWindow = this.transform.parent.gameObject.transform.parent.gameObject.transform.parent.gameObject.transform.parent.gameObject;
            ExecuteEvents.Execute<ICustomMessageTarget>(leaderBoardWindow, null, (x, y) => x.ViewUserProfile(userCode));
        }
    }

}