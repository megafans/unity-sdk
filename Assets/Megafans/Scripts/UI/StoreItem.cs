#pragma warning disable 649

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace MegafansSDK.UI {
	
	public class StoreItem : MonoBehaviour {

		[SerializeField] private Text qtyTxt;
		[SerializeField] private Text priceTxt;
		[SerializeField] private Button buyBtn;

		private int qty = 0;
		public int Quantity {
			get {
				return qty;
			}

			private set {
				qty = value;
				qtyTxt.text = qty.ToString () + " tokens";
			}
		}

		private string price = "$0.00";
		public string Price {
			get {
				return price;
			}

			set {
				price = value;
				priceTxt.text = price.ToString ();
			}
		}

        public string PriceString
        {
            set
            {
                priceTxt.text = value;
            }
        }

        void Awake() {
			//qty = int.Parse (qtyTxt.text);
			//price = priceTxt.text;
		}

		public void SetValues(int qty, string price, UnityAction buyBtnAction) {
			Quantity = qty;
			Price = price;

			buyBtn.onClick.RemoveAllListeners();
			buyBtn.onClick.AddListener (buyBtnAction);
		}

        //Free Tokens Button
        public void SetValues(UnityAction buyBtnAction)
        {
            buyBtn.onClick.RemoveAllListeners();
            buyBtn.onClick.AddListener(buyBtnAction);
        }

    }

}
