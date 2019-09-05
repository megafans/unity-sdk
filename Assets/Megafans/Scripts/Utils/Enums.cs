using UnityEngine;

namespace MegafansSDK {

	public enum GameType {
		TOURNAMENT = 1,
		PRACTICE = 2
	}

    public enum RegistationStatus {
        UNREGISTERED = 7,
        EMAILCONFIRMED = 1,
        PHONEREGISTERED = 11,
        FBREGISTERED = 13
    }

}

namespace MegafansSDK.Utils {

	public enum LeaderboardType {
		LEADERBOARD,
		USER_SCOREBOARD
	}

	public enum RankingType {
		RANKING,
		SCORE
	}

}