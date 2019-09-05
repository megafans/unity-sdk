using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MegafansSDK.UI {
	
	public class PopupHandler : MonoBehaviour {

		[SerializeField] private Text headingTxt;
		[SerializeField] private Text messageTxt;

		public void ShowPopup(string heading, string msg) {
			headingTxt.text = heading;
			messageTxt.text = msg;

			this.gameObject.SetActive (true);
		}

		public void OkBtn_OnClick() {
			HidePopup ();
		}

		private void HidePopup() {
			this.gameObject.SetActive (false);
		}

	}

}
