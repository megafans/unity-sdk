using System.Collections.Generic;
using UnityEngine;

namespace MegafansSDK.Utils {

	[System.Serializable]
	public class ViewScoreboardResponse : Response {

		public int total_pages;
		public ScoreboardResponseData data;
	}


    [System.Serializable]
    public class ScoreboardResponseData
    {
        public List<ScoreboardData> user;
        public LeaderboardData me;
    }

    [System.Serializable]
    public class ScoreboardData
    {

        public long score;
        public string username;
        public int rank;
        public string code;
        public string created_at;
    }
}