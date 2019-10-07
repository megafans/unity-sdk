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
        [SerializeField] private Image tournamentPicImg;
        [SerializeField] private ListBox payoutListBox;
        [SerializeField] private GameObject payoutItemPrefab;

        private LevelsResponseData tournamentInfo;

        public void SetValues(LevelsResponseData tournamentInfo, bool displayPayouts = false)
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
                
            if (displayPayouts) {
                payoutListBox.ClearList();

                if (tournamentInfo.payouts != null)
                {
                    tournamentPicImg.gameObject.SetActive(false);
                    payoutListBox.gameObject.SetActive(true);
                    PayoutItem headerViewHandler = payoutListBox.Header.GetComponent<PayoutItem>();
                    headerViewHandler.SetText(tournamentInfo.payouts.name);

                    for (int j = 0; j < tournamentInfo.payouts.data.Count; j++)
                    {
                        GameObject item = Instantiate(payoutItemPrefab);
                        PayoutItem viewHandler = item.GetComponent<PayoutItem>();
                        if (viewHandler != null)
                        {
                            viewHandler.SetText("Position " + tournamentInfo.payouts.data[j].position + " - " + tournamentInfo.payouts.data[j].amount + " tokens");
                            payoutListBox.AddItem(item);
                        }
                    }
                }
                else
                {
                    tournamentPicImg.gameObject.SetActive(true);
                    payoutListBox.gameObject.SetActive(false);
                }
            } else {
                if (string.IsNullOrEmpty(tournamentInfo.imageUrl))
                {
                    tournamentPicImg.sprite = Megafans.Instance.GameIcon;
                } else {
                    MegafansWebService.Instance.FetchImage(tournamentInfo.imageUrl, OnFetchPicSuccess, OnFetchPicFailure);
                    tournamentPicImg.gameObject.SetActive(true);
                    payoutListBox.gameObject.SetActive(false);
                }
            }          
        }


        private void OnDestroy()
        {

        }

        private void OnDisable()
        {

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
            if (tex != null)
            {
                tournamentPicImg.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            }
        }

        void OnFetchPicFailure(string error)
        {
            Debug.LogError(error);
        }
    }

}
