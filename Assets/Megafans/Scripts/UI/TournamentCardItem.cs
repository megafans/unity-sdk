#pragma warning disable 649
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using MegafansSDK.Utils;
using System.Collections.Generic;

namespace MegafansSDK.UI
{

    public class TournamentCardItem : MonoBehaviour, IEventSystemHandler, IPointerClickHandler
    {

        [SerializeField] private Text titleTxt;
        [SerializeField] private GameObject countdownTimer;
        [SerializeField] private GameObject unlockCountdownTimer;
        [SerializeField] private GameObject lockPanel;
        [SerializeField] private RawImage tournamentPicImg;
        [SerializeField] private Text prizeValueTxt;
        [SerializeField] private Text winnersFeeValueTxt;
        [SerializeField] private Text entryFeeValueTxt;
        [SerializeField] private GameObject joinTournamentButton;
        [SerializeField] private GameObject passwordLock;

        private LevelsResponseData tournamentInfo;
        [SerializeField] private string tourneyname;
        [SerializeField] private bool passrequired;

        public void SetValues(LevelsResponseData tournamentInfo, bool displayPayouts = false)
        {
            if (tournamentInfo.secondsLeft <= 0)
            {
                MegafansWebService.Instance.FetchImage(tournamentInfo.imageUrl, OnFetchPicSuccess, OnFetchPicFailure);
                tournamentPicImg.gameObject.SetActive(true);
                titleTxt.text = tournamentInfo.name;
                prizeValueTxt.text = "N/A";
                
                for(int i = 0; i < prizeValueTxt.transform.childCount; i++)
                {
                    prizeValueTxt.transform.GetChild(i).gameObject.SetActive(false);
                }

                winnersFeeValueTxt.text = "N/A";
                entryFeeValueTxt.text = "N/A";

                for (int i = 0; i < entryFeeValueTxt.transform.childCount; i++)
                {
                    entryFeeValueTxt.transform.GetChild(i).gameObject.SetActive(false);
                }

                lockPanel.SetActive(true);
                joinTournamentButton.SetActive(false);

                CountdownTimer unlockTimer = unlockCountdownTimer.GetComponent<CountdownTimer>();

                if (unlockTimer != null)
                {
                    if (tournamentInfo.secondsToStart > 0)
                    {
                        unlockTimer.Init(tournamentInfo.secondsToStart, true);
                    }
                }
                else
                    unlockCountdownTimer.SetActive(false);
            }
            else
            {
                CountdownTimer timerViewHandler = countdownTimer.GetComponent<CountdownTimer>();

                if (timerViewHandler != null)
                {
                    if (tournamentInfo.secondsLeft > 0)
                    {
                        timerViewHandler.Init(tournamentInfo.secondsLeft, true);
                    }
                    else
                    {
                        timerViewHandler.Init(tournamentInfo.secondsToStart, true);
                    }
                }

                titleTxt.text = tournamentInfo.name;
                this.tournamentInfo = tournamentInfo;

                passwordLock.SetActive(tournamentInfo.askPassword);
                tourneyname = tournamentInfo.name;
                passrequired = tournamentInfo.askPassword;

                if (displayPayouts)
                {
                    if (tournamentInfo != null
                        && tournamentInfo.payouts != null
                        && tournamentInfo.payouts.data != null
                        && tournamentInfo.payouts.data.Count > 0)
                    {
                        titleTxt.text = tournamentInfo.name;

                        //Gunslinger : Double currency mod
                        if(tournamentInfo.cash_tournament)
                        {
                            prizeValueTxt.transform.GetChild(0).gameObject.SetActive(false);
                            prizeValueTxt.text = "$" + tournamentInfo.payout;
                        }
                        else
                        {
                            prizeValueTxt.transform.GetChild(0).gameObject.SetActive(true);
                            prizeValueTxt.text = tournamentInfo.payout;
                        }

                        if (tournamentInfo.entryFee <= 0f)
                        {
                            entryFeeValueTxt.text = "Free";
                            entryFeeValueTxt.transform.GetChild(0).gameObject.SetActive(false);
                        }
                        else if (tournamentInfo.freeEntryTournament && tournamentInfo.freeEntriesRemaining > 0)
                        {
                            entryFeeValueTxt.text = tournamentInfo.freeEntriesRemaining + " Free ";
                            if (tournamentInfo.freeEntriesRemaining == 1)
                            {
                                entryFeeValueTxt.text += "Entry";
                            }
                            else
                            {
                                entryFeeValueTxt.text += "Entries";
                            }
                            entryFeeValueTxt.transform.GetChild(0).gameObject.SetActive(false);
                        }
                        else
                        {
                            entryFeeValueTxt.text = tournamentInfo.entryFee.ToString();
                            entryFeeValueTxt.transform.GetChild(0).gameObject.SetActive(true);
                        }

                        winnersFeeValueTxt.text = "Top " + tournamentInfo.payouts.data.Count.ToString();

                        MegafansWebService.Instance.FetchImage(tournamentInfo.imageUrl, OnFetchPicSuccess, OnFetchPicFailure);
                        tournamentPicImg.gameObject.SetActive(true);
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(tournamentInfo.imageUrl))
                    {
                        tournamentPicImg.texture = Megafans.Instance.GameTexture;
                    }
                    else
                    {
                        MegafansWebService.Instance.FetchImage(tournamentInfo.imageUrl, OnFetchPicSuccess, OnFetchPicFailure);
                        tournamentPicImg.gameObject.SetActive(true);
                        titleTxt.text = "N/A";
                        prizeValueTxt.text = "N/A";
                        winnersFeeValueTxt.text = "N/A";
                        entryFeeValueTxt.text = "N/A";
                    }
                }
            }
        }

        private void OnDestroy()
        {

        }

        private void OnDisable()
        {

        }

        private void OnEnable()
        {
            SetJoinButtonAction();
        }

        void SetJoinButtonAction()
        {
            joinTournamentButton.GetComponent<Button>().onClick.AddListener( delegate { OnJoinButtonClick(); } );
        }

        void OnJoinButtonClick()
        {
            TournamentLobbyUI _TLUI = FindObjectOfType<TournamentLobbyUI>();

            if (_TLUI != null)
            {
                _TLUI.JoinNowBtn_OnClick();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("Button clicked = " + eventData);
            //PointerEventData pData = (PointerEventData)eventData;
            GameObject tournamentLobbyUI = this.transform.parent.gameObject.transform.parent.gameObject.transform.parent.gameObject.transform.parent.gameObject;
            ExecuteEvents.Execute<TournamentCardItemCustomMessageTarget>(tournamentLobbyUI, null, (x, y) => x.EnterTournamentBtn_OnClick(tournamentInfo));
        }

        private void OnFetchPicSuccess(Texture2D tex)
        {
            if (tex != null && tournamentPicImg != null)
            {
                tournamentPicImg.texture = tex;
            }
        }

        void OnFetchPicFailure(string error)
        {
            Debug.LogError(error);
        }

        internal Button GetPlayButton()
        {
            return joinTournamentButton.GetComponent<Button>();
        }
    }
}
