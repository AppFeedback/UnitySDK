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
        // !!!! YOUR KEY HERE - Find your key in your project settings on https://my.appfeedback.io/project-settings
        const string PROJECT_KEY = "YOURKEYHERE";

        public enum Panel
        {
            eHappiness,
            eGiveMoreDetails,
            eFeedback,
            eEmail,
            eHardwarePermission,
            eFinal,
            COUNT
        }

        [SerializeField] private GameObject[] m_Panels = new GameObject[(int)Panel.COUNT];

        [Header("Panel Happiness")]
        [SerializeField] private UnityEngine.UI.Toggle[] m_HappinessToggles = null;
        [SerializeField] private UnityEngine.UI.Button m_HappinessNextButton = null;

        [Header("Panel Give More Details")]
        [SerializeField] private UnityEngine.UI.Button m_GiveMoreDetailsYesButton = null;
        [SerializeField] private UnityEngine.UI.Button m_GiveMoreDetailsNoButton = null;

        [Header("Panel Feedback")]
        [SerializeField] private UnityEngine.UI.InputField m_FeedbackInputField = null;
        [SerializeField] private UnityEngine.UI.Button m_FeedbackNextButton = null;

        [Header("Panel Email")]
        [SerializeField] private UnityEngine.UI.InputField m_EmailInputField = null;
        [SerializeField] private UnityEngine.UI.Button m_EmailNextButton = null;

        [Header("Panel Hardware Permissions")]
        [SerializeField] private UnityEngine.UI.Button m_HardwarePermissionYesButton = null;
        [SerializeField] private UnityEngine.UI.Button m_HardwarePermissionNoButton = null;

        [Header("Panel Final")]
        [SerializeField] private UnityEngine.UI.Button m_FinalCloseButton = null;

        [Space]
        [Header("Misc")]
        [SerializeField] private UnityEngine.UI.Button m_ExitButton = null;
        [SerializeField] private GameObject m_GenericPopupPrefab = null;


        private bool m_HasHardwarePermission = false;

        private string m_FeedbackString = "";   //< Inputted feedback
        private string m_EmailString = "";      //< Inputted email. NOTE: Remember that you may need to ask for additional marketing consent depending on what you plan to do with the email
        private string m_VersionCode = "0.0.1"; //< This can be whatever you want!
        private int m_HappinessToggleIndexSelected = -1;

        private AppFeedbackGenericPopup m_CurrentlyShowingPopup = null;

        private void Awake()
        {
            m_FeedbackInputField.onEndEdit.AddListener(FeedbackFieldEndEdit);
            m_EmailInputField.onEndEdit.AddListener(EmailFieldEndEdit);

            m_ExitButton.onClick.AddListener(ExitButtonPressedCB);

            m_HappinessNextButton.onClick.AddListener(ButtonHappinessNextCB);
            m_HappinessNextButton.interactable = false;

            m_GiveMoreDetailsYesButton.onClick.AddListener(ButtonGiveMoreDetailsYesCB);
            m_GiveMoreDetailsNoButton.onClick.AddListener(ButtonGiveMoreDetailsNoCB);

            m_HardwarePermissionYesButton.onClick.AddListener(ButtonHardwareYesCB);
            m_HardwarePermissionNoButton.onClick.AddListener(ButtonHardwareNoCB);

            m_FeedbackNextButton.onClick.AddListener(ButtonFeedbackNextCB);
            m_EmailNextButton.onClick.AddListener(ButtonEmailNextCB);

            m_FinalCloseButton.onClick.AddListener(ButtonFinalEndCB);

            for (int i = 0; i < m_HappinessToggles.Length; i++)
            {
                // Capture index so delegate works
                int index = i;
                m_HappinessToggles[i].isOn = false;
                m_HappinessToggles[i].onValueChanged.AddListener(delegate { HappinessTogglePressed(index); });
            }

            ShowPanel(Panel.eHappiness);
        }

        private void OnDestroy()
        {
            m_FeedbackInputField.onEndEdit.RemoveAllListeners();
            m_EmailInputField.onEndEdit.RemoveAllListeners();
            m_ExitButton.onClick.RemoveAllListeners();
            m_HappinessNextButton.onClick.RemoveAllListeners();

            m_GiveMoreDetailsYesButton.onClick.RemoveAllListeners();
            m_GiveMoreDetailsNoButton.onClick.RemoveAllListeners();

            m_HardwarePermissionYesButton.onClick.RemoveAllListeners();
            m_HardwarePermissionNoButton.onClick.RemoveAllListeners();

            m_FeedbackNextButton.onClick.RemoveAllListeners();
            m_EmailNextButton.onClick.RemoveAllListeners();

            m_FinalCloseButton.onClick.RemoveAllListeners();
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

            request.additionalData = null;

            if (m_HasHardwarePermission)
            {
                // You do not need to send this, but as useful example to show some fields you might want to send
                // to help with user diagnostics etc
                // You must make it clear to the end user what you intend to do with this data, otherwise you may breach GDPR regulations
                // We let them know in this sample that we want to use their device information purely for diagnostic/debugging reasons
                request.additionalData = new Dictionary<string, string>
                {
                    { "device_model", SystemInfo.deviceModel },
                    { "device_gpu_name", SystemInfo.graphicsDeviceName },
                    { "device_os", SystemInfo.operatingSystem },
                    { "device_cpu_name", string.Format("{0} : CORES({1})", SystemInfo.processorType, SystemInfo.processorCount) },
                    { "device_ram", SystemInfo.systemMemorySize.ToString() },
                    { "device_vram", SystemInfo.graphicsMemorySize.ToString() },
                    { "system_language", Application.systemLanguage.ToString() },
                };
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
            m_HappinessNextButton.interactable = true;
            if (m_HappinessToggles[index].isOn)
            {
                m_HappinessToggleIndexSelected = index;
            }
        }

        private void ExitButtonPressedCB()
        {
            Destroy(gameObject);
        }

        private void ShowPanel(Panel panel)
        {
            for (int i = 0; i < (int)Panel.COUNT; i++)
            {
                m_Panels[i].SetActive(i == (int)panel);
            }
        }

        private void ButtonHappinessNextCB()
        {
            ShowPanel(Panel.eGiveMoreDetails);
        }

        private void ButtonGiveMoreDetailsYesCB()
        {
            ShowPanel(Panel.eFeedback);
        }

        private void ButtonGiveMoreDetailsNoCB()
        {
            ShowPanel(Panel.eEmail);
        }

        private void ButtonFeedbackNextCB()
        {
            ShowPanel(Panel.eEmail);
        }

        private void ButtonEmailNextCB()
        {            
            ShowPanel(Panel.eHardwarePermission);
        }

        private void ButtonHardwareYesCB()
        {
            m_HasHardwarePermission = true;
            SendFeedback();
            ShowPanel(Panel.eFinal);
        }

        private void ButtonHardwareNoCB()
        {
            m_HasHardwarePermission = false;
            SendFeedback();
            ShowPanel(Panel.eFinal);
        }

        private void ButtonFinalEndCB()
        {
            Destroy(gameObject);
        }
    }
}
