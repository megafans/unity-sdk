using System.Collections.Generic;
using UnityEngine;

namespace MegafansSDK.Utils {

    [System.Serializable]
    public class JoinMatchResponse : Response
    {

        public JoinMatchData data;

    }

    [System.Serializable]
    public class JoinMatchData
    {
        public string token;
        public Dictionary<string, string> metaData;
        public string level;
    }
}
