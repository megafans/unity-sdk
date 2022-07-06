using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;

namespace MegafansSDK.Utils
{

    [System.Serializable]
    public class LastTournamentResultResponse : Response
    {

        public int total_pages;
        public LeaderboardResponseData data;

    }
}