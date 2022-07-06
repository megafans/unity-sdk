#pragma warning disable 618
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace MegafansSDK.Utils
{

    [System.Serializable]
    public class OfferwallTokenRequest : Request
    {

        public string token;
        public string userID;

        public override WWW GetWWW(string url)
        {
            Dictionary<string, string> body = new Dictionary<string, string>();
            body.Add("rewards", token);
            body.Add("appuserid", userID);
            string jsonString = JsonConvert.SerializeObject(body);

            Dictionary<string, string> headers = new Dictionary<string, string>();
            string authorization = MegafansWebService.GetBearerToken();

            headers.Add("Authorization", authorization);
            headers.Add("Content-Type", "application/json");
            headers.Add("Accept", "application/json");

            byte[] pData = Encoding.ASCII.GetBytes(jsonString.ToCharArray());

            WWW www = new WWW(url, pData, headers);

            return www;
        }

    }

}