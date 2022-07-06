using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;

namespace MegafansSDK.Utils
{

    [System.Serializable]
    public class ViewLeaderboardResponse : Response
    {

        public int total_pages;
        public LeaderboardResponseData data;

    }

    [System.Serializable]
    public class LeaderboardResponseData
    {
        public List<LeaderboardData> players;
        public LeaderboardData me;
    }

    [System.Serializable]
    public class LeaderboardData
    {

        public long? score;
        public string username;
        public int position;
        public string code = null;
        public string image;
        public string flag;
        public int isCash;
        public string created_at;
        public string payoutAmount;
    }
}