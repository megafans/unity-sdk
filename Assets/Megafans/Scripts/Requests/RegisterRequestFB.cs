#pragma warning disable 618
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using Newtonsoft.Json;

namespace MegafansSDK.Utils {

	[System.Serializable]
	public class RegisterRequestFB : RegisterRequest {

		public override WWW GetWWW(string url) {		
            Dictionary<string, string> body = new Dictionary<string, string>();
            body.Add("device_token", device_token);
            body.Add("device_type", device_type.ToString());
            body.Add("facebook_login_id", facebook_login_id);
            body.Add("username", username);
            body.Add("image", image);
            body.Add("email", email);
            string jsonString = JsonConvert.SerializeObject(body);

            Dictionary<string, string> headers = new Dictionary<string, string>();
            string authorization = MegafansWebService.GetBearerToken();
            headers.Add("Authorization", authorization);
            headers.Add("Content-Type", "application/json");
            headers.Add("MegaFansSDKVersion", MegafansConstants.MegafansSDKVersion);

            byte[] pData = Encoding.ASCII.GetBytes(jsonString.ToCharArray());
            Debug.Log("Making facebook sign up request");
            Debug.Log(jsonString);
            WWW www = new WWW(url, pData, headers);

            return www;
        }

	}

}
