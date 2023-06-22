#import <UIKit/UIKit.h>
#import <AudioToolBox/AudioToolBox.h>

extern UIViewController *UnityGetGLViewController();

@interface MatchMatchIOSSDK : NSObject
@end

@implementation MatchMatchIOSSDK

+ (void) ShowToast:(NSString*)message
{
    UIAlertController * alert = [UIAlertController alertControllerWithTitle: @"MatchMatch" message:message preferredStyle:UIAlertControllerStyleAlert];
    
    UIAlertAction * defaultAction = [UIAlertAction actionWithTitle:@"OK" style:UIAlertActionStyleDefault handler:^(UIAlertAction * action) {}];
    
    [alert addAction:defaultAction];
    [UnityGetGLViewController() presentViewController:alert animated:YES completion:nil];
}

+ (void) Vibrate:(int)time
{
    AudioServicesPlaySystemSound(kSystemSoundID_Vibrate);
}

@end

extern "C"
{
    void _ShowToast(const char * message)
    {
        [MatchMatchIOSSDK ShowToast:[NSString stringWithUTF8String:message]];
    }

    void _Vibrate(int time)
    {
        [MatchMatchIOSSDK Vibrate:time];
    }
}
