using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace MegafansSDK.Utils {

	public class MegafansUtils {

		private static Regex ValidEmailRegex = CreateValidEmailRegex();

		public static bool IsUsernameValid(string username) {
			if (username.Length <= 0) {
				return false;
			}

			return true;
		}

        public static bool IsPhoneNumberPrefixValid(string phoneNumber) {
            if (phoneNumber.Length <= 0)
            {
                return false;
            }

            return true;
        }

        public static bool IsPhoneNumberValid(string phoneNumber) {
			if (phoneNumber.Length < 10) {
				return false;
			}

			return true;
		}

		public static bool IsPasswordValid(string password) {
			if (password.Length < 8) {
				return false;
			}

			return true;
		}

		public static bool IsEmailValid(string email) {
			bool isValid = ValidEmailRegex.IsMatch(email);

			return isValid;
		}

		public static string TextureToString(Texture2D tex) {
			byte[] imageBytes = tex.EncodeToPNG ();
			string imageString = Convert.ToBase64String (imageBytes);

			return imageString;
		}

		public static Texture2D StringToTexture(string imageString) {
			byte[] imageBytes = Convert.FromBase64String (imageString);
			Texture2D tex = new Texture2D (2, 2);
			tex.LoadImage (imageBytes);

			return tex;
		}

        public static void LoadPNG(string filePath, Image imageToReturn) {
            Texture2D tex = null;
            byte[] fileData;

            if (File.Exists(filePath))
            {
                fileData = File.ReadAllBytes(filePath);
                tex = new Texture2D(2, 2);
                tex.LoadImage(fileData);
            }

            imageToReturn.sprite = Sprite.Create(tex, new Rect(0, 0, 128, 128), new Vector2());
        }


        public static string GetCurrentDateTime() {
			return DateTime.Now.ToString ("yyyy-MM-dd HH:mm:ss");
		}

		private static Regex CreateValidEmailRegex() {
			string validEmailPattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
            + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
            + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";

			return new Regex (validEmailPattern, RegexOptions.IgnoreCase);
		}

        public static string GetNameOfMonth(int month) {
            List<string> monthNames = new List<string>();
            monthNames.Add("January");
            monthNames.Add("February");
            monthNames.Add("March");
            monthNames.Add("April");
            monthNames.Add("May");
            monthNames.Add("June");
            monthNames.Add("July");
            monthNames.Add("August");
            monthNames.Add("September");
            monthNames.Add("October");
            monthNames.Add("November");
            monthNames.Add("December");
            return monthNames[month - 1];
        }
	}

}
