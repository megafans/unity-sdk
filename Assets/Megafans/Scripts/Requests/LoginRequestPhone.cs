#pragma warning disable 618
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace MegafansSDK.Utils {

	[System.Serializable]
	public class LoginRequestPhone : LoginRequest {

		public override WWW GetWWW(string url) {	
            Dictionary<string, string> body = new Dictionary<string, string>();
            body.Add ("device_token", device_token);
            body.Add ("device_type", device_type.ToString());
            body.Add ("phone_number", phone_number);
            body.Add ("username", username);
            string queryUrl = url + "?appGameUid=" + appGameUid;
            string jsonString = JsonConvert.SerializeObject(body);

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Content-Type", "application/json");
            headers.Add("MegaFansSDKVersion", MegafansConstants.MegafansSDKVersion);

            byte[] pData = Encoding.ASCII.GetBytes(jsonString.ToCharArray());


            WWW www = new WWW (queryUrl, pData, headers);

			return www;
		}

	}

}
