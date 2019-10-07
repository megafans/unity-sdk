#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MegafansSDK.UI
{

    public class CustomInput : MonoBehaviour
    {   
        [SerializeField] private Image highlightedImage;
        [SerializeField] private Image unhighlightedImage;
        [SerializeField] private Image isValidImage;
        [SerializeField] public InputField textInput;
        [SerializeField] private LoadingBar isVerifying;

        //public string defaultValue {
        //    get {
        //        return defaultValue;
        //    }

        //    set {
        //        defaultValue = value;
        //    }
        //}

        public bool valueVerified {
            get {
                return valueVerified;
            }

            set {
                valueVerified = value;
                if (valueVerified) {
                    isValidImage.gameObject.SetActive(true);
                    isVerifying.gameObject.SetActive(true);
                } else {
                    isValidImage.gameObject.SetActive(false);
                    isVerifying.gameObject.SetActive(false);
                }
            }
        }

        public void onTextInputChange()
        {
            if (string.IsNullOrEmpty(this.textInput.text)) {
                //valueVerified = false;
                highlightedImage.gameObject.SetActive(true);
                unhighlightedImage.gameObject.SetActive(false);
            } else {
                highlightedImage.gameObject.SetActive(true);
                unhighlightedImage.gameObject.SetActive(false);
            }
        }
    }
}
