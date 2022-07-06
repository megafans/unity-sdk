#pragma warning disable 618
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace MegafansSDK.Utils
{

    [System.Serializable]
    public class LastTournamentResultRequest : Request
    {
        public string leaderboardsize;
        public string leaderboardpage;

        public override WWW GetWWW(string url)
        {
            if (!string.IsNullOrEmpty(leaderboardsize) && !string.IsNullOrEmpty(leaderboardpage))
                url += ("?pagenumber=" + leaderboardpage + "&pageSize=" + leaderboardsize);

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