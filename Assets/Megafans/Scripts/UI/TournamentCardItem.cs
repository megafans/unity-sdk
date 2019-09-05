using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using MegafansSDK.Utils;

namespace MegafansSDK.UI
{

    public class TournamentCardItem : MonoBehaviour, IEventSystemHandler, IPointerClickHandler
    {

        [SerializeField] private Text titleTxt;
        [SerializeField] private GameObject countdownTimer;
        [SerializeField] private Image tournamentPicImg;

        private LevelsResponseData tournamentInfo;

        public void SetValues(LevelsResponseData tournamentInfo)
        {
            CountdownTimer timerViewHandler = countdownTimer.GetComponent<CountdownTimer>();
            if (timerViewHandler != null)
            {
                if (tournamentInfo.secondsLeft > 0) {
                    timerViewHandler.Init(tournamentInfo.secondsLeft, true);
                } else {
                    timerViewHandler.Init(tournamentInfo.secondsToStart, true);
                }
            }
            titleTxt.text = tournamentInfo.name;
            this.tournamentInfo = tournamentInfo;

            if (string.IsNullOrEmpty(tournamentInfo.tournamentDisplayImageURL)) {
                tournamentPicImg.sprite = Megafans.Instance.GameIcon;
            }
        }

        private void OnDestroy()
        {
            Debug.Log("On Destroy");
        }

        private void OnDisable()
        {
            Debug.Log("On Disable");
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("Button clicked = " + eventData);
            //PointerEventData pData = (PointerEventData)eventData;
            GameObject tournamentLobbyUI = this.transform.parent.gameObject.transform.parent.gameObject.transform.parent.gameObject.transform.parent.gameObject;
            ExecuteEvents.Execute<TournamentCardItemCustomMessageTarget>(tournamentLobbyUI, null, (x, y) => x.EnterTournamentBtn_OnClick(tournamentInfo));
        }

    }

}
