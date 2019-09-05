using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace MegafansSDK.UI
{

    public class ExpandableHeaderItem : MonoBehaviour
    {

        [SerializeField] private Text textLabel;
        [SerializeField] private Button toggleAccordianBtn;

        public void SetValues(string text)
        {
            textLabel.text = text;
        }
    }
}
