using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace MegafansSDK.Utils {

	[System.Serializable]
	public class LogoutRequest : Request {

        public string appGameUid;

        public override WWW GetWWW(string url) {
            Dictionary<string, string> body = new Dictionary<string, string>();
            string queryUrl = url + "?appGameUid=" + appGameUid;
            string jsonString = JsonUtility.ToJson(body);

            byte[] pData = Encoding.ASCII.GetBytes(jsonString.ToCharArray());

            Dictionary<string, string> headers = new Dictionary<string, string>();
            string authorization = MegafansWebService.GetBearerToken();
            headers.Add("Authorization", authorization);
            headers.Add("Content-Type", "application/json");
            headers.Add("MegaFansSDKVersion", MegafansConstants.MegafansSDKVersion);

            WWW www = new WWW(queryUrl, pData, headers);

            return www;
		}

	}

}
