using UnityEngine;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace MegafansSDK.Utils {

	[System.Serializable]
	public class OtpRequest : Request {

		public string otp;
		public string phone_number;
        public string app_game_uid;

        public override WWW GetWWW(string url) {
            Dictionary<string, string> body = new Dictionary<string, string>();
            body.Add("otp", otp);
            body.Add("phone_number", phone_number);
            string jsonString = JsonConvert.SerializeObject(body);
            string queryUrl = url + "?appGameUid=" + app_game_uid;

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Content-Type", "application/json");
            headers.Add("MegaFansSDKVersion", MegafansConstants.MegafansSDKVersion);

            byte[] pData = Encoding.ASCII.GetBytes(jsonString.ToCharArray());

            WWW www = new WWW (queryUrl, pData, headers);

			return www;
		}

	}

}
