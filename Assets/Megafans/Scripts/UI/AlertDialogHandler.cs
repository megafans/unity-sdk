#pragma warning disable 649

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace MegafansSDK.UI
{

    public class AlertDialogHandler : MonoBehaviour
    {

        [SerializeField] private Image icon;
        [SerializeField] private Text headingTxt;
        [SerializeField] private Text messageTxt;
        [SerializeField] private Button positiveBtn;
        [SerializeField] private Text positiveBtnTxt;
        [SerializeField] private Button negativeBtn;
        [SerializeField] private Text negativeBtnTxt;
        [SerializeField] private Button closeBtn;

        public void ShowAlertDialog(Sprite icon, string heading, string msg, string positiveBtnTxt,
            string negativeBtnTxt, UnityAction positiveBtnAction, UnityAction negativeBtnAction)
        {

            this.icon.sprite = icon;
            headingTxt.text = heading;
            messageTxt.text = msg;
            this.positiveBtnTxt.text = positiveBtnTxt;
            this.negativeBtnTxt.text = negativeBtnTxt;

            positiveBtn.onClick.RemoveAllListeners();
            positiveBtn.onClick.AddListener(positiveBtnAction);

            negativeBtn.gameObject.SetActive(false);
            negativeBtn.onClick.RemoveAllListeners();
            negativeBtn.onClick.AddListener(negativeBtnAction);

            closeBtn.gameObject.SetActive(true);
            closeBtn.onClick.RemoveAllListeners();
            closeBtn.onClick.AddListener(negativeBtnAction);

            this.gameObject.SetActive(true);
        }

        public void ShowAlertDialog(Sprite icon, string heading, string msg, string positiveBtnTxt,
    string negativeBtnTxt, UnityAction positiveBtnAction, UnityAction negativeBtnAction, UnityAction closeBtnAction, bool hideCloseBtn)
        {

            this.icon.sprite = icon;
            headingTxt.text = heading;
            messageTxt.text = msg;
            this.positiveBtnTxt.text = positiveBtnTxt;
            this.negativeBtnTxt.text = negativeBtnTxt;

            positiveBtn.onClick.RemoveAllListeners();
            positiveBtn.onClick.AddListener(positiveBtnAction);

            negativeBtn.gameObject.SetActive(false);
            negativeBtn.onClick.RemoveAllListeners();
            negativeBtn.onClick.AddListener(negativeBtnAction);

            if (hideCloseBtn)
            {
                closeBtn.gameObject.SetActive(false);
                closeBtn.onClick.RemoveAllListeners();
            }
            else
            {
                closeBtn.gameObject.SetActive(true);
                closeBtn.onClick.RemoveAllListeners();
                closeBtn.onClick.AddListener(negativeBtnAction);
            }

            this.gameObject.SetActive(true);
        }

        public void HideAlertDialog()
        {
            this.gameObject.SetActive(false);
        }

    }

}
