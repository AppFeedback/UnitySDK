# UnitySDK
Unity C# SDK for AppFeedback

Download this repo manually, or use Unity's package manager and add
https://github.com/AppFeedback/UnitySDK.git

Once imported do the following

Initilise the SDK:
```
AppFeedback.Configure("YOUR PROJECT KEY HERE!!");
```

Prepare the request like so:
```
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

You can also add these:
```
// E.g. read in a screenshot or image to send
byte[] imageBytes = System.IO.File.ReadAllBytes(@"screenshot.png");

request.imageBytes = imageBytes;

// You can also do a generic byte array for a binary file, say a save file
request.binaryBytes = null;

// Log is just the string and will be saved as a text file
request.log = "This is the log contents if you want to add something";
```

Prepare some callbacks:
```
static private void OnAppFeedbackSuccess()
{
    Console.WriteLine("Success");
}

static private void OnAppFeedbackFailure(AppFeedback.Error error)
{
    Console.WriteLine(error.m_RawErrorString);
}
```
Send the data     
```
AppFeedback.Send(request, OnAppFeedbackSuccess, OnAppFeedbackFailure);
```
