#pragma warning disable 649

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace MegafansSDK.UI
{

    public class PayoutItem : MonoBehaviour
    {
        [SerializeField] private Text valueTxt;

        public void SetText(string text)
        {
            valueTxt.text = text;
        }
    }
}