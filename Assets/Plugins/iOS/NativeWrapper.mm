# import <Foundation/Foundation.h>
# import "Intercom/Intercom.h"

extern "C" {
void _registerUserWithUserId(const char* emailOrPhoneNumber, const char* gameId, const char* gameName);
void _showIntercom();
void _hideIntercom();
void _updateUsername(const char* name);
void _didFinishTournamentWithScore(int tournamentId, float score, const char* gameId, const char* gameName);
void _showIntercomIfUnreadMessages();
void _logOut();
void _InitIntercom(const char* appId,const char* appKey);
void _openSettings();
}

void _registerUserWithUserId(const char* emailOrPhoneNumber, const char* gameId, const char* gameName)
{
	// Register the current user with their userId
	NSString* userIdString = [NSString stringWithUTF8String: emailOrPhoneNumber];

	[Intercom registerUserWithUserId: userIdString];
    
    // Create the current game name and game id as a new company being played by the user
    ICMCompany* company = [ICMCompany new];
company.name = [NSString stringWithUTF8String: gameName];
    company.companyId = [NSString stringWithUTF8String: gameId];
    
    // Add the current game as company attribute on Intercom
    ICMUserAttributes* userAttributes = [ICMUserAttributes new];
userAttributes.companies = @[company];
    if ([userIdString containsString: @"@"]) {
        userAttributes.email = userIdString;
    } else {
        userAttributes.phone = userIdString;
    }
    
    [Intercom updateUser:userAttributes];
}

// Set the Intercom launcher to visible
void _showIntercom()
{

	[Intercom presentMessenger];
}

// Hide the Intercom launcher
void _hideIntercom()
{

	[Intercom hideIntercom];
    [Intercom setLauncherVisible:NO];
}

// Show Intercom if unread messages
void _showIntercomIfUnreadMessages()
{
	if ([Intercom unreadConversationCount] > 0)
	{

		[Intercom setLauncherVisible:YES];
    } else {
        [Intercom setLauncherVisible:NO];
    }
}

// Update username for user
void _updateUsername(const char* username)
{
	ICMUserAttributes* userAttributes = [ICMUserAttributes new];
	userAttributes.name = [NSString stringWithUTF8String: username];

	[Intercom updateUser:userAttributes];
}

// Log tournament event with score
void _didFinishTournamentWithScore(int tournamentId, float score, const char* gameId, const char* gameName)
{

	[Intercom logEventWithName:@"tournament_with_score"

					  metaData: @{
                                  @"tournament_id": [NSString stringWithFormat:@"%d", tournamentId],
                                  @"score": [NSString stringWithFormat:@"%f", score],
                                  @"game_id": [NSString stringWithUTF8String: gameId],
                                  @"game_name": [NSString stringWithUTF8String: gameName]
                                  }
     ];
}

void _InitIntercom(const char* _appId,const char* _appKey)
{
  [Intercom setApiKey:[NSString stringWithUTF8String: _appId] forAppId:[NSString stringWithUTF8String: _appKey]];
}

// Log out of Intercom
void _logOut()
{
	[Intercom logout];
}

void _openSettings () {
    NSURL * url = [NSURL URLWithString: UIApplicationOpenSettingsURLString];
    [[UIApplication sharedApplication] openURL:url options: {} completionHandler: NULL];
}
