using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF.Samples
{
    public class AppFeedbackGenericPopup : MonoBehaviour
    {
        [SerializeField] private UnityEngine.UI.Text m_TextField;
        [SerializeField] private UnityEngine.UI.Button m_DismissButton;

        private void Awake()
        {
            m_DismissButton.onClick.AddListener(DimissButtonCB);
        }

        private void OnDestroy()
        {
            m_DismissButton.onClick.RemoveListener(DimissButtonCB);
        }

        private void DimissButtonCB()
        { 
            Destroy(gameObject);
        }

        public void SetText(string text)
        { 
            m_TextField.text = text;
        }

        public void SetAllowDismissal(bool flag)
        { 
            m_DismissButton.gameObject.SetActive(flag);
        }
    }
}