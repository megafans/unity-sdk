//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Newtonsoft.Json;

//using Facebook.Unity;

//namespace MegafansSDK.Utils
//{

//    public class MegafansFBHelper : MonoBehaviour
//    {

//        private string UserId
//        {
//            get;
//            set;
//        }

//        private string Username
//        {
//            get;
//            set;
//        }

//        private string Email
//        {
//            get;
//            set;
//        }

//        private string image = "";
//        private string Image
//        {
//            get
//            {
//                return image;
//            }

//            set
//            {
//                image = value;
//            }
//        }

//        private const string usernameQuery = "/me?fields=name,email";
//        private const string profilePicQuery = "/me/picture?type=square&width=750&height=750";

//        private Action<string, string, string, Texture2D> RegisterSuccessCallback;
//        private Action<string, string, string> LoginSuccessCallback;
//        private Action<string> FailureCallback;

//        private bool isRegistering = false;

//        void Awake()
//        {
//            if (!FB.IsInitialized)
//            {
//                FB.Init(InitCallback, OnHideUnity);
//            }
//            else
//            {
//                FB.ActivateApp();
//            }
//        }

//        public void RegisterRequest(Action<string, string, string, Texture2D> successCallback, Action<string> failureCallback)
//        {
//            RegisterSuccessCallback = successCallback;
//            FailureCallback = failureCallback;

//            if (!FB.IsInitialized)
//            {
//                return;
//            }

//            isRegistering = true;

//            List<string> permissions = new List<string>() { "public_profile", "email" };
//            FB.LogInWithReadPermissions(permissions, AuthCallback);
//        }

//        public void LoginRequest(Action<string, string, string> successCallback, Action<string> failureCallback)
//        {
//            LoginSuccessCallback = successCallback;
//            FailureCallback = failureCallback;

//            if (!FB.IsInitialized)
//            {
//                return;
//            }

//            isRegistering = false;

//            List<string> permissions = new List<string>() { "public_profile", "email" };
//            FB.LogInWithReadPermissions(permissions, AuthCallback);
//        }

//        public void Logout()
//        {
//            if (!FB.IsInitialized)
//            {
//                return;
//            }

//            if (FB.IsLoggedIn)
//            {
//                FB.LogOut();
//            }
//        }

//        private void InitCallback()
//        {
//            if (FB.IsInitialized)
//            {
//                FB.ActivateApp();
//            }
//            else
//            {
//                Debug.Log("Failed to Initialize the Facebook SDK");
//            }
//        }

//        private void OnHideUnity(bool isGameShown)
//        {
//            if (!isGameShown)
//            {
//                Time.timeScale = 0;
//            }
//            else
//            {
//                Time.timeScale = 1;
//            }
//        }

//        private void AuthCallback(ILoginResult result)
//        {
//            if (String.IsNullOrEmpty(result.Error) && !result.Cancelled)
//            {
//                AccessToken accessToken = AccessToken.CurrentAccessToken;
//                UserId = accessToken.UserId;
//                Debug.Log(UserId);
//                Debug.Log("Logged into facebook");
//                if (FB.IsLoggedIn)
//                {
//                    FB.API(usernameQuery, HttpMethod.GET, (IGraphResult graphResult) =>
//                    {
//                        FB.API(profilePicQuery, HttpMethod.GET, (IGraphResult imageGraphResult) =>
//                        {
//                            Debug.Log("FACEBOOK GET PICTURE RESULT");
//                            Debug.Log(imageGraphResult.RawResult);
//                            OnUsernameResponse(graphResult, imageGraphResult.Texture);
//                        });
//                    });

//                }
//            }
//            else
//            {
//                string error = "Facebook login failed/cancelled.";
//                Debug.Log(error);

//                if (FailureCallback != null)
//                {
//                    FailureCallback(error);
//                }
//            }
//        }

//        private void OnUsernameResponse(IResult result, Texture2D newImageToUpload)
//        {
//            if (String.IsNullOrEmpty(result.Error) && !result.Cancelled)
//            {
//                Debug.Log("FACEBOOK RESULTS DICTIONARY");

//                if (!result.ResultDictionary.ContainsKey("email") ||
//                    !result.ResultDictionary.ContainsKey("name"))
//                {
//                    if (FailureCallback != null)
//                        FailureCallback(result.Error);
//                }
//                else
//                {
//                    Username = result.ResultDictionary["name"] as string;
//                    Email = result.ResultDictionary["email"] as string;

//                    if (isRegistering)
//                    {
//                        if (RegisterSuccessCallback != null)
//                        {
//                            RegisterSuccessCallback(UserId, Email, Username, newImageToUpload);
//                        }
//                    }
//                    else
//                    {
//                        if (LoginSuccessCallback != null)
//                        {
//                            LoginSuccessCallback(UserId, Email, Username);
//                        }
//                    }
//                }
//            }
//            else
//            {
//                if (FailureCallback != null)
//                {
//                    FailureCallback(result.Error);
//                }
//            }
//        }

//    }

//}