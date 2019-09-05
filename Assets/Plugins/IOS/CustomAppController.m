#import "CustomAppController.h"
@import Intercom;

@implementation CustomAppController
 
- (BOOL)application:(UIApplication*)application didFinishLaunchingWithOptions:(NSDictionary*)launchOptions
{
    [Intercom setApiKey:@"ios_sdk-ca12a8c72d94a3126c1943dbcc4097f2325bf645" forAppId:@"c2il6kq0"];
    NSLog(@"APP DID FINISH LAUNCHING WITH OPIONS.");
    return [super application:application didFinishLaunchingWithOptions:launchOptions ];
}

@end

IMPL_APP_CONTROLLER_SUBCLASS(CustomAppController)
