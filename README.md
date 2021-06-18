# UnitySDK
Unity C# SDK for AppFeedback

Go to Unity's inbuilt package manager ( Window -> Package Manager )

and add ( + Button top left of window -> add package from git url )

https://github.com/AppFeedback/UnitySDK.git

or download the latest source doe release at

https://github.com/AppFeedback/UnitySDK/releases

_Please note, not all features are available on basic plans. Uploading images, binary files & logs require Plus or Pro plans._

## .NET Settings
This SDK only supports .NET 4.x Upwards due to usage of async/await logic

Edit -> Project Settings -> Player | Other Settings | Configuration ->  Api Compatibility Level -> .NET 4.x

For IL2CPP Support, we would recommend using latest 2019 LTS

If you are experiencing issues using other Unity versions, there is a known bug in the IL2CPP compiler which can be manually patched:

[Unity Forum Link](https://forum.unity.com/threads/il2cpp-failing-in-windows-machine.891436/#post-5944052)

## Samples
The package manager ( Window -> Package Manager ) will also show an option to import samples like so
![image](https://user-images.githubusercontent.com/8695457/120478663-01136300-c3a5-11eb-8603-fb86ea14b11b.png)

You will have an example prefab form AF_FormExample. 

There is also an example Scene where you can give this a go, AF_Preview

Make sure to update the project key in AppFeedbackFormExample.cs before you try and run it.

Try out the scene to give it ago in action

![image](https://user-images.githubusercontent.com/8695457/121954526-c0c5c480-cd56-11eb-858b-0ecfcf36c05e.png)


## Setup Guide
Once imported do the following

Initilise the SDK:
```c#
AppFeedback.Configure("YOUR PROJECT KEY HERE!!");
```

Prepare the request like so:
```c#
AppFeedback.SendFeedbackRequest request = new AppFeedback.SendFeedbackRequest();
request.feedback = "This is your feedback string";
request.email = "youremail@email.com";
request.happiness = 10;
request.version = "1.0.0";
request.additionalData =
new Dictionary<string, string>
{
    {"cpu", "AMD" },
    {"ram", 2024.ToString() }
};
```

You can also add these to upload images, binary files and logs ( on supported plans ):
```c#
// E.g. read in a screenshot or image to send
byte[] imageBytes = System.IO.File.ReadAllBytes(@"screenshot.png");

request.imageBytes = imageBytes;

// You can also do a generic byte array for a binary file, say a save file
request.binaryBytes = null;

// Log is just the string and will be saved as a text file
request.log = "This is the log contents if you want to add something";
```

Prepare some callbacks:
```c#
static private void OnAppFeedbackSuccess()
{
    Debug.Log("Success");
}

static private void OnAppFeedbackFailure(AppFeedback.Error error)
{
    Debug.LogError(error.m_RawErrorString);
}
```
Send the data     
```
AppFeedback.Send(request, OnAppFeedbackSuccess, OnAppFeedbackFailure);
```

**Please note:**

The send will happen async to not block main thread. The Success and Failure callbacks will be called back on main thread.
If you are going to close/end the application immediately afterwards, it may not complete sending the feedback so use the:

```c#
AppFeedback.Flush();
```
function to force wait for all current send tasks. Remember, this will block the main thread until it is finished sending the data.
