#pragma warning disable 649
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

namespace MegafansSDK.UI
{

    public class CountdownTimer : MonoBehaviour
    {
        [SerializeField] private Text daysTextLabel;
        [SerializeField] private Text daysDisplayTextLabel;
        [SerializeField] private Text hoursTextLabel;
        [SerializeField] private Text hoursDisplayTextLabel;
        [SerializeField] private Text minutesTextLabel;
        [SerializeField] private Text minutesDisplayTextLabel;
        [SerializeField] private Text secondsTextLabel;
        [SerializeField] private Text secondsDisplayTextLabel;
        [SerializeField] private Image secondsTextLabelBG;

        protected Coroutine counter;

        public int secondsRemaining;

        public void Init(int secondsRemaining, bool purpleBackground = false) {
            this.secondsRemaining = secondsRemaining;
            daysTextLabel.text = "";
            hoursTextLabel.text = "";
            minutesTextLabel.text = "";
            secondsTextLabel.text = "";
            //CancelInvoke("Tick");
            Time.timeScale = 1;
            if (counter != null)
            {
                this.StopCoroutine(counter);
                counter = null;
            }
            counter = StartCoroutine(StartCountdown());
        }

        void Update()
        {
            int updatedValue = this.secondsRemaining;
            int hours = 24;
            int minutes = 60;
            int seconds = 60;

            int days = hours * minutes * seconds;
            int daysRemaining = updatedValue / days;
            if (daysRemaining > 0)
            {
                daysTextLabel.text = ToDoubleDigits(daysRemaining);
                updatedValue = updatedValue % days;
            } else {
                daysTextLabel.text = "00";
            }

            int hoursRemaining = updatedValue / (minutes * seconds);
            if (hoursRemaining > 0)
            {
                hoursTextLabel.text = ToDoubleDigits(hoursRemaining);
                updatedValue = updatedValue % (minutes * seconds);
            }
            else
            {
                hoursTextLabel.text = "00";
            }

            int minutesRemaining = updatedValue / minutes;
            if (minutesRemaining > 0)
            {
                minutesTextLabel.text = ToDoubleDigits(minutesRemaining);
                updatedValue = updatedValue % minutes;
            }
            else
            {
                minutesTextLabel.text = "00";
            }

            secondsTextLabel.text = updatedValue.ToString();

        }

        private string ToDoubleDigits(int numericalValue) {
            string valueAsString = numericalValue.ToString();
            if (valueAsString.Length == 1) {
                return valueAsString.Insert(0, "0");
            }
            return valueAsString;
        }

        public IEnumerator StartCountdown()
        {           
            while (this.secondsRemaining > 0)
            {
                yield return new WaitForSeconds(1.0f);
                this.secondsRemaining -= 1;
            }
            if (secondsRemaining == 0)
            {
                Megafans.Instance.CleanTokenWhenFinishedTournament();
                MegafansUI.Instance.EnableUI(false);
                MegafansHelper helper = GameObject.FindObjectOfType<MegafansHelper>();
                if (helper != null)
                {
                    helper.ShowMegafans();
                }
            }
        }
    }
}
 