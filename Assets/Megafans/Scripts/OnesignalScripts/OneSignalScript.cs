using OneSignalSDK;
using UnityEngine;
using MegafansSDK;
public class OneSignalScript : MonoBehaviour
{

    void Start()
    {
        OneSignal.Default.Initialize(Megafans.Instance._OneSignalAppID);
        OneSignal.Default.NotificationOpened += _notificationOpened;
        OneSignal.Default.NotificationWillShow += _notificationReceived;
        OneSignal.Default.NotificationPermissionChanged += _notificationPermissionChanged;
        OneSignal.Default.PushSubscriptionStateChanged += _pushStateChanged;

        PromptForPush();
    }
    async void PromptForPush()
    {  
     var result = await OneSignal.Default.PromptForPushNotificationsWithUserResponse();
    }
    private void _notificationPermissionChanged(NotificationPermission current, NotificationPermission previous)
    {
        Debug.Log($"Notification Permissions changed to: {current}");
    }
    private void _notificationOpened(NotificationOpenedResult result)
    {
        Debug.Log($"Notification was opened with result: {JsonUtility.ToJson(result)}");
    }

    private Notification _notificationReceived(Notification notification)
    {
        var additionalData = notification.additionalData != null
            ? Json.Serialize(notification.additionalData)
                : null;

        Debug.Log($"Notification was received in foreground: {JsonUtility.ToJson(notification)}\n{additionalData}");
        return notification; // show the notification
    }
    private void _pushStateChanged(PushSubscriptionState current, PushSubscriptionState previous)
    {
        Debug.Log($"Push state changed to: {JsonUtility.ToJson(current)}");
    }

}
