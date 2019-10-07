#pragma warning disable 618
using UnityEngine;
using System.Collections.Generic;

namespace MegafansSDK.Utils
{
    [System.Serializable]
    public class TermsOrPrivacyPolicyRequest : Request {
            public override WWW GetWWW(string url)
            {
                //string queryUrl = url + "?tournamentId=" + tournament_id;
                ////Dictionary<string, string> body = new Dictionary<string, string>();
                ////body.Add("appGameUid", app_game_id);
                ////string jsonString = JsonConvert.SerializeObject(body);
                ////byte[] pData = Encoding.ASCII.GetBytes(jsonString.ToCharArray());


                Dictionary<string, string> headers = new Dictionary<string, string>();
                //string authorization = MegafansWebService.GetBearerToken();
                //headers.Add("Authorization", authorization);
                //headers.Add("Content-Type", "application/json");
                //headers.Add("MegaFansSDKVersion", MegafansConstants.MegafansSDKVersion);
                WWW www = new WWW(url, null, headers);

                return www;
            }
    }
}