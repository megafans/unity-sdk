using UnityEngine;

namespace MegafansSDK
{

    public enum GameType
    {
        OFFLINE = 0,
        TOURNAMENT = 1,
        PRACTICE = 2
    }

    public enum RegistationStatus
    {
        UNREGISTERED = 7,
        EMAILCONFIRMED = 1,
        PHONEREGISTERED = 11,
        FBREGISTERED = 13
    }

    public enum Deployment
    {
        Development,
        Staging,
        Production,
        Custom
    }

}

namespace MegafansSDK.Utils
{

    public enum LeaderboardType
    {
        LEADERBOARD
    }

    public enum RankingType
    {
        LEADERBOARD,
        HISTORY
    }

}