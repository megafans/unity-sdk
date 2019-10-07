#pragma warning disable 618
using UnityEngine;
using System.Collections.Generic;

namespace MegafansSDK.Utils {

	[System.Serializable]
	public class ViewLeaderboardRequest : Request {

		public string app_game_uid;

		public override WWW GetWWW(string url) {
            url += ("?appGameUid=" + app_game_uid);

            Dictionary<string, string> headers = new Dictionary<string, string>();
            string authorization = MegafansWebService.GetBearerToken();
            headers.Add("Authorization", authorization);
            headers.Add("Content-Type", "application/json");
            headers.Add("MegaFansSDKVersion", MegafansConstants.MegafansSDKVersion);

            WWW www = new WWW (url, null, headers);

			return www;
		}

	}

}