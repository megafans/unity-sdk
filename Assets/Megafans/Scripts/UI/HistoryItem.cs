using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace MegafansSDK.UI
{

    public class HistoryItem : MonoBehaviour, IEventSystemHandler, IPointerClickHandler
    {

        [SerializeField] private Image scoreBG;
        [SerializeField] private Text scoreTxt;
        [SerializeField] private Image positionBG;
        [SerializeField] private Text positionTxt;
        [SerializeField] private Image dateBG;
        [SerializeField] private Text dateTxt;
        [SerializeField] private string userCode;

        public void SetValues(string score, string position, string date, string userCode)
        {
            scoreTxt.text = score;
            positionTxt.text = position;
            dateTxt.text = date;
            this.userCode = userCode;
        }

        public void SetScoreColor(Color bgClr, Color txtClr)
        {
            scoreBG.color = bgClr;
            scoreTxt.color = txtClr;
        }

        public void SetPositionColor(Color bgClr, Color txtClr)
        {
            positionBG.color = bgClr;
            positionTxt.color = txtClr;
        }

        public void SetDateColor(Color bgClr, Color txtClr)
        {
            dateBG.color = bgClr;
            dateTxt.color = txtClr;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("Button clicked = " + eventData);
            //PointerEventData pData = (PointerEventData)eventData;
            GameObject rankingAndHistoryWindowUI = this.transform.parent.gameObject.transform.parent.gameObject.transform.parent.gameObject.transform.parent.gameObject;
            ExecuteEvents.Execute<ICustomMessageTarget>(rankingAndHistoryWindowUI, null, (x, y) => x.ViewUserProfile(userCode));
        }
    }

}