using UnityEngine;

namespace MegafansSDK.UI {

	public class LoadingBar : MonoBehaviour {

		public void ShowLoadingBar() {
			this.gameObject.SetActive (true);
		}

		public void HideLoadingBar() {
			this.gameObject.SetActive (false);
		}

	}

}
