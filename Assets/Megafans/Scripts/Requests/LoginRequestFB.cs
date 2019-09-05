using System.Collections.Generic;
using UnityEngine;
using System.Text;
using Newtonsoft.Json;

namespace MegafansSDK.Utils {

	[System.Serializable]
	public class LoginRequestFB : LoginRequest {

		public override WWW GetWWW(string url) {
            Dictionary<string, string> body = new Dictionary<string, string>();
            body.Add("facebook_login_id", facebook_login_id);
            string queryUrl = url + "?appGameUid=" + appGameUid;
            string jsonString = JsonConvert.SerializeObject(body);

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Content-Type", "application/json");
            headers.Add("MegaFansSDKVersion", MegafansConstants.MegafansSDKVersion);

            byte[] pData = Encoding.ASCII.GetBytes(jsonString.ToCharArray());
            Debug.Log("Making facebook request");
            Debug.Log(jsonString);
            WWW www = new WWW(queryUrl, pData, headers);

            return www;
        }

	}

}
