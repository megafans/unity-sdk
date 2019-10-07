#pragma warning disable 649
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace MegafansSDK.UI
{

    public class ExpandableTextItem : MonoBehaviour
    {
        [SerializeField] private Text headerLabel;                   
        [SerializeField] private Text textLabel;
        [SerializeField] private Image openOrClosedToggleLabel;
        [SerializeField] private Sprite openArrow;
        [SerializeField] private Sprite closeArrow;

        public void toggleOnButton(bool isOn)
        {
            if (isOn) {
                openOrClosedToggleLabel.sprite = closeArrow;
            } else {
                openOrClosedToggleLabel.sprite = openArrow;
            }
        }

        public void SetValues(string title, string text)
        {
            headerLabel.text = title;
            textLabel.text = text;
        } 
    }
}
