#pragma warning disable 618
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace MegafansSDK.Utils {

	[System.Serializable]
	public class JoinMatchRequest : Request {

		public int tournament_id;
        public string app_game_id;
        public string lastLocationValue;

        public override WWW GetWWW(string url) {
            string queryUrl;
            if (tournament_id != 0) {
                queryUrl = url + "?tournamentId=" + tournament_id;
            } else {
                queryUrl = url + "?appGameUid=" + app_game_id;
            }
            //Dictionary<string, string> body = new Dictionary<string, string>();
            //body.Add("appGameUid", app_game_id);
            //string jsonString = JsonConvert.SerializeObject(body);
            //byte[] pData = Encoding.ASCII.GetBytes(jsonString.ToCharArray());


            Dictionary<string, string> headers = new Dictionary<string, string>();
            string authorization = MegafansWebService.GetBearerToken();
            headers.Add("Authorization", authorization);
            headers.Add("Content-Type", "application/json");
            headers.Add("MegaFansSDKVersion", MegafansConstants.MegafansSDKVersion);
            headers.Add("gpslocation", lastLocationValue);

            WWW www = new WWW(queryUrl, null, headers);

            return www;
		}

	}

}
