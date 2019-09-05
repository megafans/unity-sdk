using System.Collections.Generic;
using UnityEngine;
using System.Text;
using Newtonsoft.Json;

namespace MegafansSDK.Utils
{
    [System.Serializable]
    public class RegisterFirstTimeUserRequest : RegisterRequest
    {

        public override WWW GetWWW(string url)
        {
            string queryUrl = url + "?appGameUid=" + appGameUid;
            Dictionary<string, string> body = new Dictionary<string, string>();
            body.Add("device_token", device_token);
            body.Add("device_type", device_type);
            string jsonString = JsonConvert.SerializeObject(body);

            Dictionary<string, string> headers = new Dictionary<string, string>();
            //string authorization = MegafansWebService.Authenticate(email, password);
            //headers.Add("Authorization", authorization);
            headers.Add("Content-Type", "application/json");
            headers.Add("MegaFansSDKVersion", MegafansConstants.MegafansSDKVersion);

            byte[] pData = Encoding.ASCII.GetBytes(jsonString.ToCharArray());

            WWW www = new WWW(queryUrl, pData, headers);

            return www;
        }
    }
}
