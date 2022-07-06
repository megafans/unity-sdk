using System.Collections.Generic;
using UnityEngine;

namespace MegafansSDK.Utils {

	[System.Serializable]
	public class ViewScoreboardResponse : Response {

		public int total_pages;
		public List<ScoreboardResponseData> data;
	}

    [System.Serializable]
    public class ScoreboardResponseData
    {
        public long score;
        public string created_at;
        public string name;
    }
}