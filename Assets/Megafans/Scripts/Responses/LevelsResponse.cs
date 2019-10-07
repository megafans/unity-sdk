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
        public LevelsPayoutData payouts;
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