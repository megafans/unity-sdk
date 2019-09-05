package com.megafans.unitysdk;

import io.intercom.android.sdk.Company;
import io.intercom.android.sdk.Intercom;
import io.intercom.android.sdk.UserAttributes;
import io.intercom.android.sdk.identity.Registration;

public class IntercomWrapper {

    public static void RegisterIntercomUserWithId(String userId, String email, String gameId, String gameName) {
        Registration registration = Registration.create().withUserId(userId);
        Intercom.client().registerIdentifiedUser(registration);

        Company company = new Company.Builder()
                .withName(gameName)
                .withCompanyId(gameId)
                .build();

        UserAttributes userAttributes = new UserAttributes.Builder()
                .withCompany(company)
                .withEmail(email)
                .build();

        Intercom.client().updateUser(userAttributes);
    }

    public static void ShowIntercom() {
        Intercom.client().setLauncherVisibility(Intercom.Visibility.VISIBLE);
    }

    public static void HideIntercom() {
        Intercom.client().setLauncherVisibility(Intercom.Visibility.VISIBLE);
    }

    public static void UpdateIntercomUserName(String username) {
        UserAttributes userAttributes = new UserAttributes.Builder()
                .withName(username)
                .build();

        Intercom.client().updateUser(userAttributes);
    }

    public static void RegisterIntercomUserWithId(String userId) {
        Registration registration = Registration.create().withUserId(userId);
        Intercom.client().registerIdentifiedUser(registration);
        Intercom.client().displayMessenger();
    }

    public static void LogOut() {
        Intercom.client().logout();
    }

}