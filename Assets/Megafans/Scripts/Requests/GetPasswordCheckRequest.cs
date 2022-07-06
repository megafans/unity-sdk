#pragma warning disable 618
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using Newtonsoft.Json;

namespace MegafansSDK.Utils
{
    [System.Serializable]
    public class GetPasswordCheckRequest : Request
    {
        public string tournamentID;
        public string password;

        public override WWW GetWWW(string url)
        {
            Dictionary<string, string> body = new Dictionary<string, string>();
            body.Add("tournamentId", tournamentID);
            body.Add("password", password);
            string jsonString = JsonConvert.SerializeObject(body);
            byte[] pData = Encoding.ASCII.GetBytes(jsonString.ToCharArray());
            Dictionary<string, string> headers = new Dictionary<string, string>();
            string authorization = MegafansWebService.GetBearerToken();
            headers.Add("Authorization", authorization);
            headers.Add("Content-Type", "application/json");
            headers.Add("MegaFansSDKVersion", MegafansConstants.MegafansSDKVersion);
            headers.Add("Platform", DeviceInfo.Platform);
            headers.Add("NotificationsId", DeviceInfo.NotificationID);
            headers.Add("AdvertisingId", DeviceInfo.AdvertisingID);
            headers.Add("VendorId", DeviceInfo.DeviceToken);
            WWW www = new WWW(url, pData, headers);
            return www;
        }
    }
}