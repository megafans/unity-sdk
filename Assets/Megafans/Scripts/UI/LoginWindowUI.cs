#pragma warning disable 649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using MegafansSDK.Utils;

namespace MegafansSDK.UI {

	public class LoginWindowUI : MonoBehaviour {

		[SerializeField] private GameObject emailSection;
		[SerializeField] private GameObject phoneSection;

        public bool IsEmail = true;

        void Awake()
        {
            if (IsEmail)
            {
                phoneSection.SetActive(false);
                emailSection.SetActive(true);
            }
            else
            {
                phoneSection.SetActive(true);
                emailSection.SetActive(false);
            }
        }

        private void OnEnable()
        {
            if (IsEmail)
            {
                phoneSection.SetActive(false);
                emailSection.SetActive(true);
            }
            else
            {
                phoneSection.SetActive(true);
                emailSection.SetActive(false);
            }
        }

		//public void FBBtn_OnClick() {
		//	MegafansWebService.Instance.LoginFB (OnLoginFBResponse, OnLoginFBFailure);
		//}

		public void PhoneBtn_OnClick() {
			emailSection.SetActive (false);
			phoneSection.SetActive (true);
		}

		public void EmailBtn_OnClick() {
			phoneSection.SetActive (false);
			emailSection.SetActive (true);
		}

		public void BackBtn_OnClick() {
			MegafansUI.Instance.ShowLandingWindow (true);
		}

		private void OnLoginFBResponse(LoginResponse response) {
			if (response.success.Equals (MegafansConstants.SUCCESS_CODE)) {
                Debug.Log("***************FB LOGIN******************");
                Debug.Log(response);
                Debug.Log("*********************************");
                MegafansPrefs.UserId = response.data.id;
				MegafansPrefs.ProfilePicUrl = response.data.image;
                MegafansPrefs.Email = response.data.email;
                MegafansPrefs.AccessToken = response.data.token;
                MegafansPrefs.RefreshToken = response.data.refresh;
                Megafans.Instance.ReportUserLoggedIn (MegafansPrefs.UserId.ToString());
			}
			else {
				Debug.LogError (response.message);
				string error = "There was an error: " + response.message;
				MegafansUI.Instance.ShowPopup ("ERROR", error);
			}
		}

		private void OnLoginFBFailure(string error) {
			Debug.LogError (error);

			if (error.ToLower().Contains (MegafansConstants.UNAUTHORIZED_CODE.ToLower())) {
				string msg = "You need to register first.";
				MegafansUI.Instance.ShowPopup ("ERROR", msg);
			}
		}

	}

}