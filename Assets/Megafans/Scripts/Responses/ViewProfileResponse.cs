using System.Collections.Generic;
using UnityEngine;

namespace MegafansSDK.Utils {

	[System.Serializable]
	public class ViewProfileResponse : Response {
		
		public ViewProfileData data;

	}

	[System.Serializable]
	public class ViewProfileData {
		public string username;
		public string email;
		public string phoneNumber;
		public string deviceType;
		public string gender;
		public string dateOfBirth;
		public float clientBalance;	
		public string image;
        public string role;
        public int? status;
        public string countryCode;
        public string countryName;
        public string countryFlag;
		public int? tournamentsWon = 0;
        public int? tournamentsEntered = 0;
        public int? highestRank = 0;
        public int? practiceGames = 0;
        public double? highestPracticeScore = 0.0;
        public double? highestTournamentScore = 0.0;
	}
}
