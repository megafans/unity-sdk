#pragma warning disable 649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace MegafansSDK.UI {
	
	public class PopupHandler : MonoBehaviour {

		[SerializeField] private Text headingTxt;
		[SerializeField] private Text messageTxt;
		UnityAction okBtnAction;

		public void ShowPopup(string heading, string msg, UnityAction okBtnAction = null) {
			headingTxt.text = heading;
			messageTxt.text = msg;

			this.gameObject.SetActive (true);
			this.okBtnAction = okBtnAction;
		}

		public void OkBtn_OnClick() {
			if (this.okBtnAction != null)
            {
				this.okBtnAction();
				this.okBtnAction = null;
			}
			HidePopup ();			
		}

		private void HidePopup() {
			this.gameObject.SetActive (false);
		}

	}

}
