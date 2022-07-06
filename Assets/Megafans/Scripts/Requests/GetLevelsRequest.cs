#pragma warning disable 618
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System;
using Newtonsoft.Json;

namespace MegafansSDK.Utils
{
    [System.Serializable]
    public class GetLevelsRequest : Request
    {
        public int game_type_id;
        public string app_game_uid;

        public override WWW GetWWW(string url)
        {
            string queryUrl = url + "?appGameUid=" + app_game_uid;

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("MegaFansSDKVersion", MegafansConstants.MegafansSDKVersion);

            string authorization = MegafansWebService.GetBearerToken();
            headers.Add("Authorization", authorization);

            WWW www = new WWW(queryUrl, null, headers);

            return www;
        }
    }
}