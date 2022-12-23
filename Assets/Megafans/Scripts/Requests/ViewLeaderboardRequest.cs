#pragma warning disable 618
using UnityEngine;
using System.Collections.Generic;

namespace MegafansSDK.Utils
{

    [System.Serializable]
    public class ViewLeaderboardRequest : Request
    {

        public string app_game_uid;
        //public string tournament_id;
        public string tournamentId;//sath commeneing bi karni chay
        public string leaderboardsize;
        public string leaderboardpage;

        public override WWW GetWWW(string url)
        {
            if(string.IsNullOrEmpty(leaderboardsize) && string.IsNullOrEmpty(leaderboardpage))
                url += ("?tournamentid=" + tournamentId);
            else
                url += ("?tournamentid=" + tournamentId + "&pagenumber=" + leaderboardpage + "&pageSize=" + leaderboardsize);

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