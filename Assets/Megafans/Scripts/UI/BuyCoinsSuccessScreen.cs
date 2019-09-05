using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace MegafansSDK.UI
{

    public class BuyCoinsSuccessScreen : MonoBehaviour
    {   
        [SerializeField] private Text descriptionText;
        [SerializeField] private Button continueBtn;

        public void ShowBuyCoinsSuccessScreen(int tokenQuantity)
        {        
            descriptionText.text = tokenQuantity + " Tokens have been added to your account";
            this.gameObject.SetActive(true);
        }

        public void HideBuyCoinsSuccessScreen()
        {
            this.gameObject.SetActive(false);
        }

    }

}
