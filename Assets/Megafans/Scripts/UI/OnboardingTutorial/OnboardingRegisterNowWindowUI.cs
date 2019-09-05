using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using MegaFans.Unity.iOS;
using MegaFans.Unity.Android;

namespace MegafansSDK.UI
{
    public class OnboardingRegisterNowWindowUI : MonoBehaviour
    {

        [SerializeField] private Text gameDescriptionLabel;

        private void Awake()
        {
            string gameName = Application.productName;
            gameDescriptionLabel.text = "You can now play " + gameName + " and win prizes powered by MegaFans!";
        }

        private void OnEnable()
        {
#if UNITY_EDITOR
            Debug.Log("Unity Editor");

            ////if (tex != null)
            ////{
            //    //isPhotoRemoved = false;
            //    //profilePicImg.texture = tex;               
            //    StartCoroutine(MegafansWebService.Instance.UploadProfilePic("/Users/markhoyt/Downloads/IMG_3956.PNG", (obj) =>
            //    {
            //        string msg = "Successfully uploaded profile picture.";
            //        MegafansUI.Instance.ShowPopup("SUCCESS", msg);
            //    }, (err) =>
            //    {
            //        string msg = "Error uploading profile image.  Please try again.";
            //        MegafansUI.Instance.ShowPopup("ERROR", msg);
            //    }));
            //return; 
#elif UNITY_IOS
            Debug.Log("IOS");
            IntercomWrapperiOS.HideIntercom();
#elif UNITY_ANDROID
            Debug.Log("ANDROID");
            IntercomWrapperAndroid.HideIntercom();
#endif
        }

        public void SignUpBtn_OnClick()
        {
            MegafansUI.Instance.ShowLandingWindow(false);
        }
    }
}