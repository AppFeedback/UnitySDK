# UnitySDK
Unity C# SDK for AppFeedback

Unity's package manager and add
https://github.com/AppFeedback/UnitySDK.git
or download manually to get started.

The package manager will also show an option to import samples if you want to get started quickly.

Make sure to update the project key in AppFeedbackFormExample.cs before you try and run it.

Please note, not all features are available on basic plans. Uploading images, binary files & logs require Plus or Pro plans.

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
