using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Example usage of the AppFeedback Unity SDK
/// This provides a basic input form which sends to the AppFeedback online service
/// Please make sure you set your PROJECT_KEY in this class!
/// </summary>
/// 
namespace AF.Samples
{
    public class AppFeedbackFormExample : MonoBehaviour
    {
        // !!!! YOUR KEY HERE !!!!
        const string PROJECT_KEY = "YOURKEYHERE";

        [SerializeField] private UnityEngine.UI.InputField m_FeedbackInputField = null;
        [SerializeField] private UnityEngine.UI.InputField m_EmailInputField = null;
        [SerializeField] private UnityEngine.UI.Toggle[] m_HappinessToggles = null;
        [SerializeField] private UnityEngine.UI.Button m_SubmitButton = null;
        [SerializeField] private UnityEngine.UI.Button m_ExitButton = null;

        [SerializeField] private GameObject m_GenericPopupPrefab = null;

        private string m_FeedbackString = "";   //< Inputted feedback
        private string m_EmailString = "";      //< Inputted email. NOTE: Remember that you may need to ask for additional marketing consent depending on what you plan to do with the email
        private string m_VersionCode = "0.0.1"; //< This can be whatever you want!
        private int m_HappinessToggleIndexSelected = -1;

        private AppFeedbackGenericPopup m_CurrentlyShowingPopup = null;

        private void Awake()
        {
            m_FeedbackInputField.onEndEdit.AddListener(FeedbackFieldEndEdit);
            m_EmailInputField.onEndEdit.AddListener(EmailFieldEndEdit);
            m_SubmitButton.interactable = false;

            for (int i = 0; i < m_HappinessToggles.Length; i++)
            {
                // Capture index so delegate works
                int index = i;
                m_HappinessToggles[i].isOn = false;
                m_HappinessToggles[i].onValueChanged.AddListener(delegate { HappinessTogglePressed(index); });
            }

            m_SubmitButton.onClick.AddListener(SendFeedback);
            m_ExitButton.onClick.AddListener(ExitButtonPressedCB);
        }

        private void OnDestroy()
        {
            m_FeedbackInputField.onEndEdit.RemoveListener(FeedbackFieldEndEdit);
            m_EmailInputField.onEndEdit.RemoveListener(EmailFieldEndEdit); 
            m_SubmitButton.onClick.RemoveListener(SendFeedback);
            m_ExitButton.onClick.RemoveListener(ExitButtonPressedCB);
        }

        private void SendFeedback()
        { 
            // Set the project key
            AppFeedback.Configure(PROJECT_KEY);    

            // Build the request
            AppFeedback.SendFeedbackRequest request = new AppFeedback.SendFeedbackRequest();
            request.feedback = m_FeedbackString;
            request.email = m_EmailString;
            request.happiness = (m_HappinessToggleIndexSelected + 1) * 2;
            request.version = m_VersionCode;

            // You do not need to send this, but as useful example to show some fields you might want to send
            // to help with user diagnostics etc
            request.additionalData =
            new Dictionary<string, string>
            {
                { "device_model", SystemInfo.deviceModel },
                { "device_gpu_name", SystemInfo.graphicsDeviceName },
                { "device_os", SystemInfo.operatingSystem },
                { "device_cpu_name", string.Format("{0} : CORES({1})", SystemInfo.processorType, SystemInfo.processorCount) },
                { "device_ram", SystemInfo.systemMemorySize.ToString() },
                { "device_vram", SystemInfo.graphicsMemorySize.ToString() },
                { "system_language", Application.systemLanguage.ToString() },
            };

            // Show our generic popup
            m_CurrentlyShowingPopup = Instantiate(m_GenericPopupPrefab, this.transform).GetComponent<AppFeedbackGenericPopup>();
            m_CurrentlyShowingPopup.SetText("Sending Feedback...");
            m_CurrentlyShowingPopup.SetAllowDismissal(false);

            // This is where you send the request, but also setup any callbacks
            AppFeedback.Send(request, OnAppFeedbackSuccessCB, OnAppFeedbackFailureCB);
        }

        private void OnAppFeedbackSuccessCB()
        {
            m_CurrentlyShowingPopup.SetText("Success");
            m_CurrentlyShowingPopup.SetAllowDismissal(true);
            Debug.Log("Success");
        }

        private void OnAppFeedbackFailureCB(AppFeedback.Error error)
        {
            m_CurrentlyShowingPopup.SetText(error.m_RawErrorString);
            m_CurrentlyShowingPopup.SetAllowDismissal(true);
            Debug.LogError("AppFeedback Failure: " + error.m_RawErrorString);
        }

        private void EmailFieldEndEdit(string edit)
        { 
            m_EmailString = edit;
        }

        private void FeedbackFieldEndEdit(string edit)
        {
            m_FeedbackString = edit;
        }

        private void HappinessTogglePressed(int index)
        {
            m_SubmitButton.interactable = true;
            if (m_HappinessToggles[index].isOn)
            {
                m_HappinessToggleIndexSelected = index;
            }
        }

        private void ExitButtonPressedCB()
        { 
            Destroy(gameObject);
        }
    }
}
