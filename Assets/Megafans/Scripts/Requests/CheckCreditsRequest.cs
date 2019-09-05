﻿using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace MegafansSDK.Utils {

	[System.Serializable]
	public class CheckCreditsRequest : Request {

		public string userId;

		public override WWW GetWWW(string url) {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            string authorization = MegafansWebService.GetBearerToken();
            headers.Add("Authorization", authorization);
            headers.Add("Content-Type", "application/json");
            headers.Add("MegaFansSDKVersion", MegafansConstants.MegafansSDKVersion);
            WWW www = new WWW(url, null, headers);

            return www;
        }

	}

}