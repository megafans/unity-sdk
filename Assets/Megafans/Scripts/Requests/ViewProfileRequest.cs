using UnityEngine;
using System.Collections.Generic;

namespace MegafansSDK.Utils {

	[System.Serializable]
	public class ViewProfileRequest : Request {

		public string code;
        public string appGameUID;

        public override WWW GetWWW(string url) {
			Dictionary<string, string> headers = new Dictionary<string, string>();
            string queryURL = url + "?appGameUid=" + appGameUID;

            if (code != null) {
                queryURL += ("&code=" + code);
            }         
            string authorization = MegafansWebService.GetBearerToken();
            headers.Add("Authorization", authorization);
            headers.Add("MegaFansSDKVersion", MegafansConstants.MegafansSDKVersion);

            WWW www = new WWW (queryURL, null, headers);

			return www;
		}

	}

}