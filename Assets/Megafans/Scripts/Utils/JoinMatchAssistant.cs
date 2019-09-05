using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MegafansSDK.UI;

namespace MegafansSDK.Utils
{

    public class JoinMatchAssistant : MonoBehaviour
    {

        private string tournamentToken = "";
        private Dictionary<string, string> metaData;

        public void JoinPracticeMatch()
        {
            MegafansWebService.Instance.PracticeTournament(Megafans.Instance.GameUID, OnJoinPracticeMatchSuccess, OnJoinPracticeMatchFailure);
        }

        private void OnJoinPracticeMatchSuccess(JoinMatchResponse response)
        {
            if (response.success.Equals(MegafansConstants.SUCCESS_CODE))
            {
                MegafansUI.Instance.EnableUI(false);
                tournamentToken = response.data.token;
                if (response.data.metaData != null)
                {
                    metaData = response.data.metaData;
                } else {
                    metaData = new Dictionary<string, string>();
                }
                //metaData.Add("level", "12");
                Megafans.Instance.ReportStartGame(tournamentToken, 0, GameType.PRACTICE, metaData);
            }
        }

        private void OnJoinPracticeMatchFailure(string error)
        {
            Debug.LogError(error);
        }

        public void JoinTournamentMatch(int tournamentID)
        {
            Megafans.Instance.CurrentTournamentId = tournamentID;
            MegafansWebService.Instance.JoinTournament(tournamentID, OnJoinTournamentMatchSuccess, OnJoinTournamentMatchFailure);
        }

        public void ReplayTournamentMatch() {
            MegafansWebService.Instance.JoinTournament(Megafans.Instance.CurrentTournamentId, OnJoinTournamentMatchSuccess, OnJoinTournamentMatchFailure);
        }

        private void OnJoinTournamentMatchSuccess(JoinMatchResponse response)
        {
            if (response.success.Equals(MegafansConstants.SUCCESS_CODE))
            {
                MegafansUI.Instance.EnableUI(false);
                tournamentToken = response.data.token;
                if (response.data.metaData != null)
                {
                    metaData = response.data.metaData;
                } else if (!string.IsNullOrEmpty(response.data.level)) {
                    metaData = new Dictionary<string, string>();
                    metaData.Add("level", response.data.level);
                }
                else
                {
                    metaData = new Dictionary<string, string>();
                }
                Megafans.Instance.ReportStartGame(tournamentToken, Megafans.Instance.CurrentTournamentId, GameType.TOURNAMENT, metaData);
            }
        }

        private void OnJoinTournamentMatchFailure(string error)
        {
            Debug.LogError(error);
        }

    }

}
