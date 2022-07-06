using System.Collections.Generic;
using UnityEngine;

namespace MegafansSDK.Utils
{

    [System.Serializable]
    public class LevelsResponse : Response
    {

        public List<LevelsResponseData> data;

    }

    [System.Serializable]
    public class LevelsResponseData
    {
        public bool cash_tournament;
        public int id;
        public string name;
        public string message;
        public float entryFee;
        public string start;
        public string end;
        public int secondsLeft;
        public int secondsToStart;
        public bool f2p;
        public string imageUrl;
        public string payout;
        public LevelsPayoutData payouts;
        public bool askPassword;
        public bool freeEntryTournament;
        public int freeEntriesRemaining;
    }

    [System.Serializable]
    public class LevelsPayoutData
    {   
        public string name;
        public List<PayoutResponseData> data;
    }

    [System.Serializable]
    public class PayoutResponseData
    {
        public int position;
        public string amount;
    }
}