using UnityEngine;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace MegafansSDK.Utils {
    [System.Serializable]
    public class RefreshAccessTokenRequest : Request
    {

        public string access_token;
        public string refresh_token;
        public string appGameUid;

        public override WWW GetWWW(string url) {
            string queryUrl = url + "?appGameUid=" + appGameUid;
            Dictionary<string, string> body = new Dictionary<string, string>();
            body.Add ("token", access_token);
            body.Add ("refresh", refresh_token);
            string jsonString = JsonConvert.SerializeObject(body);

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Content-Type", "application/json");
            headers.Add("Accept", "application/json");
            headers.Add("MegaFansSDKVersion", MegafansConstants.MegafansSDKVersion);

            byte[] pData = Encoding.ASCII.GetBytes(jsonString.ToCharArray());

            WWW www = new WWW(queryUrl, pData, headers);

            return www;
        }
    }
}