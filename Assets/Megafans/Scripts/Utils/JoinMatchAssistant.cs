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
        private bool isReplay = false;

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
                }
                else
                {
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
            isReplay = false;
            Megafans.Instance.CurrentTournamentId = tournamentID;
            Megafans.Instance.CurrentTounamentFreeEntries = Megafans.Instance.GetCurrentTournamentData().freeEntriesRemaining;
#if !UNITY_EDITOR

            if (
                Megafans.Instance.lastLocationData != null
                )
            {
                var lastLocation = Megafans.Instance.lastLocationData.Value.latitude.ToString() + ',' + Megafans.Instance.lastLocationData.Value.longitude.ToString();
                MegafansWebService.Instance.JoinTournament(tournamentID, lastLocation, OnJoinTournamentMatchSuccess, OnJoinTournamentMatchFailure);
            } else
            {
                // Force user to give us location            
                Megafans.Instance.CheckForLocationPermissions();

            }
#else
            MegafansWebService.Instance.JoinTournament(tournamentID, "latitude,longitude", OnJoinTournamentMatchSuccess, OnJoinTournamentMatchFailure);
#endif            
        }

        public void ReplayTournamentMatch()
        {
            //MegafansHelper.m_Instance.m_WasPlayingTournament = false;
            isReplay = true;
#if !UNITY_EDITOR
            if (Megafans.Instance.lastLocationData != null)
            {
                var lastLocation = Megafans.Instance.lastLocationData.Value.latitude.ToString() + ',' + Megafans.Instance.lastLocationData.Value.longitude.ToString();
                MegafansWebService.Instance.JoinTournament(Megafans.Instance.CurrentTournamentId, lastLocation, OnJoinTournamentMatchSuccess, OnJoinTournamentMatchFailure);
            }
            else
            {
                // Force user to give us location            
                Megafans.Instance.CheckForLocationPermissions();
            }
#else
            MegafansWebService.Instance.JoinTournament(Megafans.Instance.CurrentTournamentId, "latitude,longitude", OnJoinTournamentMatchSuccess, OnJoinTournamentMatchFailure);
#endif
        }

        private void OnJoinTournamentMatchSuccess(JoinMatchResponse response)
        {
            if (response.success.Equals(MegafansConstants.SUCCESS_CODE))
            {
                Megafans.Instance.m_AdsManager.m_TenginInstance.SendEvent("play tournament", Megafans.Instance.CurrentTournamentId.ToString());

                MegafansUI.Instance.EnableUI(false);
                tournamentToken = response.data.token;

                if (response.data.metaData != null)
                {
                    metaData = response.data.metaData;
                }
                else if (!string.IsNullOrEmpty(response.data.level))
                {
                    metaData = new Dictionary<string, string>();
                    metaData.Add("level", response.data.level);
                }
                else
                {
                    metaData = new Dictionary<string, string>();
                }

                if (Megafans.Instance.GetCurrentTournamentData().entryFee == 0f)
                {
                    if (isReplay)
                    {
                        MegafansSDK.AdsManagerAPI.AdsManager.instance.ApiCall_FullScreen(needtoShowThirdPartyAds => {

                            if (needtoShowThirdPartyAds)
                            {
                                MegafansSDK.Megafans.Instance.m_AdsManager.ShowInterstitial();
                            }


                        });
                    }

                    Megafans.Instance.ReportStartGame(tournamentToken, Megafans.Instance.CurrentTournamentId, GameType.PRACTICE, metaData);
                }
                else
                {
                    if (Megafans.Instance.CurrentTounamentFreeEntries > 0)
                    {
                        Megafans.Instance.CurrentTounamentFreeEntries -= 1;
                        Megafans.Instance.CurrentUsingFreeEntry = true;
                    }
                    else
                    {
                        Megafans.Instance.CurrentUsingFreeEntry = false;
                    }
                    Megafans.Instance.ReportStartGame(tournamentToken, Megafans.Instance.CurrentTournamentId, GameType.TOURNAMENT, metaData);
                }
            }
            else
            {
                if (response.message.Contains("Not enough credits"))
                {
                    if (MegafansPrefs.IsRegisteredMegaFansUser)
                    {
                        MegafansUI.Instance.ShowCreditsWarning();
                    }
                    else
                    {
                        MegafansUI.Instance.ShowUnregisteredUserWarning();
                    }
                }
            }
        }

        private void OnJoinTournamentMatchFailure(string error)
        {

        }
    }
}
