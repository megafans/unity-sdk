using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace MegafansSDK.Utils {

	[System.Serializable]
	public class LoginRequestEmail : LoginRequest {

		public override WWW GetWWW(string url) {
            Dictionary<string, string> body = new Dictionary<string, string>();
            string queryUrl = url + "?appGameUid=" + appGameUid;
            string jsonString = JsonUtility.ToJson(body);

            Dictionary<string, string> headers = new Dictionary<string, string> ();
			string authorization = MegafansWebService.Authenticate (email, password);
			headers.Add ("Authorization", authorization);
            headers.Add ("Content-Type", "application/json");
            headers.Add("MegaFansSDKVersion", MegafansConstants.MegafansSDKVersion);

            byte[] pData = Encoding.ASCII.GetBytes(jsonString.ToCharArray());

            WWW www = new WWW (queryUrl, pData, headers);

			return www;
		}

	}

}
