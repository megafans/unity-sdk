using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MegafansSDK.UI {
	
	public class CustomButton : MonoBehaviour {

		[SerializeField] private Text btnTxt;
		[SerializeField] private Image underline;
		[SerializeField] private Color selectedClr;
		[SerializeField] private Color deselectedClr;

		public void SelectBtn() {
			btnTxt.color = selectedClr;
			underline.gameObject.SetActive (true);
		}

		public void DeselectBtn() {
			btnTxt.color = deselectedClr;
			underline.gameObject.SetActive (false);
		}

	}

}
