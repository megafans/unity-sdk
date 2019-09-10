using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;
using Newtonsoft.Json;

namespace MegafansSDK.Utils {

	[System.Serializable]
	public class EditProfileRequest : Request {

		public string email_address;
        public string phone_number;
        public string username;
        public string facebook_login_id;

        public override WWW GetWWW(string url) {		
            Dictionary<string, string> body = new Dictionary<string, string>();
            if (!String.IsNullOrEmpty(username)) {
                body.Add("username", username);
            }

            if (!String.IsNullOrEmpty(phone_number)) {
                body.Add("phoneNumber", phone_number);
            }

            if (!String.IsNullOrEmpty(email_address))
            {
                body.Add("email", email_address);
            }

            if (!String.IsNullOrEmpty(facebook_login_id))
            {
                body.Add("facebook_login_id", facebook_login_id);
            }

            string jsonString = JsonConvert.SerializeObject(body);

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("MegaFansSDKVersion", MegafansConstants.MegafansSDKVersion);

            string authorization = MegafansWebService.GetBearerToken();
            headers.Add("Authorization", authorization);
            headers.Add("Content-Type", "application/json");

            byte[] pData = Encoding.ASCII.GetBytes(jsonString.ToCharArray());

            WWW www = new WWW(url, pData, headers);

            return www;
		}

	}
}
