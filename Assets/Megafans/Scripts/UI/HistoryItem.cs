#pragma warning disable 649
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace MegafansSDK.UI
{

    public class HistoryItem : MonoBehaviour//, IEventSystemHandler, IPointerClickHandler
    {

        [SerializeField] private Text scoreTxt;
        [SerializeField] private Text positionTxt;
        [SerializeField] private Text dateTxt;
        [SerializeField] private Text trounamentNameTxt;

        public void SetValues(string score, string position, string date, string tournamentName)
        {
            scoreTxt.text = score;
            positionTxt.text = position;
            dateTxt.text = date;
            trounamentNameTxt.text = tournamentName;
        }
    }

}