using System.Collections.Generic;

namespace MegafansSDK.Utils {
    [System.Serializable]
    public class TournamentRulesResponse : Response {
        public List<TournamentRulesResponseData> data;
    }

    [System.Serializable]
    public class TournamentRulesResponseData {
        public string sequence;
        public string description;
    }
}

//namespace MegafansSDK.Utils
//{

//    [System.Serializable]
//    public class TournamentRulesResponse : Response
//    {   
//        public TournamentRulesResponseData data;

//    }

//    [System.Serializable]
//    public class TournamentRulesResponseData
//    {
//        public List<string> rules;
//    }
//}