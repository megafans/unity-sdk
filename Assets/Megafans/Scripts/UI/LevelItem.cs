using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace MegafansSDK.UI {
	
	public class LevelItem : MonoBehaviour {

		[SerializeField] private Text titleTxt;
		[SerializeField] private Text tokensTxt;
		[SerializeField] private Button selectBtn;

		public void SetValues(string title, float tokensRequired, UnityAction selectBtnAction) {
			titleTxt.text = title;
			tokensTxt.text = tokensRequired.ToString ();

			selectBtn.onClick.AddListener (selectBtnAction);
		}

	}

}
