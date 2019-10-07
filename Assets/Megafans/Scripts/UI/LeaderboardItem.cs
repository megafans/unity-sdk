#pragma warning disable 649

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace MegafansSDK.UI {
	
    public class LeaderboardItem : MonoBehaviour, IEventSystemHandler, IPointerClickHandler
    {

		[SerializeField] private Image keyBG;
		[SerializeField] private Text keyTxt;
		[SerializeField] private Image valueBG;
		[SerializeField] private Text valueTxt;
        [SerializeField] private string userCode;

        public void SetValues(string key, string value, string userCode) {
			keyTxt.text = key;
			valueTxt.text = value;
            this.userCode = userCode;
		}

		public void SetKeyColor(Color bgClr, Color txtClr) {
            keyBG.color = bgClr;
			keyTxt.color = txtClr;
		}

		public void SetValueColor(Color bgClr, Color txtClr) {
            valueBG.color = bgClr;
			valueTxt.color = txtClr;
		}


        public void SelectLeaderBoardRow()
        {
            //ShowWindow(viewProfileWindow);
            Debug.Log("Button clicked = " + valueTxt);
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