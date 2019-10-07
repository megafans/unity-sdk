#pragma warning disable 618
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace MegafansSDK.Utils {

	[System.Serializable]
	public class RegisterRequestPhone : RegisterRequest {

		public override WWW GetWWW(string url) {	
            Dictionary<string, string> body = new Dictionary<string, string>();
            body.Add("device_token", device_token);
            body.Add("device_type", device_type.ToString());
            body.Add("phone_number", phone_number);
            body.Add("registerType", registerType);
            body.Add("username", username);
            string jsonString = JsonConvert.SerializeObject(body);

            Dictionary<string, string> headers = new Dictionary<string, string>();
            //string authorization = MegafansWebService.Authenticate(email, password);
            string authorization = MegafansWebService.GetBearerToken();
            headers.Add("Authorization", authorization);
            headers.Add("Content-Type", "application/json");
            headers.Add("MegaFansSDKVersion", MegafansConstants.MegafansSDKVersion);

            byte[] pData = Encoding.ASCII.GetBytes(jsonString.ToCharArray());

            WWW www = new WWW (url, pData, headers);

			return www;
		}

	}

}
