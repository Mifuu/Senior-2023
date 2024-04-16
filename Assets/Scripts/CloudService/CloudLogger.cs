using System.Collections.Generic;
using UnityEngine;
using System;

namespace CloudService
{
    public class CloudLogger : MonoBehaviour
    {
        public class CloudLoggerSingular
        {
            private string moduleName;

            public CloudLoggerSingular(string moduleName)
            {
                this.moduleName = moduleName;
            }

            public void Log(string text) => CloudLogger.Singleton.Log(moduleName, text);
            public void LogError(string text, bool InformPlayer = false) => CloudLogger.Singleton.LogError(moduleName, text, InformPlayer);
        }

        public static CloudLogger Singleton;
        private Queue<string> _log;
        public event Action OnLog;
        public event Action OnLogError;

        [Header("Setup")]
        [SerializeField] private ModalController modalPanelPrefab;
        [SerializeField] private Transform canvas;

        [Header("Options")]
        [Tooltip("Use Debug.Log() when calling Log()")]
        [SerializeField] private bool useDebugLog;
        [Tooltip("Maximum number of log stored in memory")]
        [Range(1, 100)]
        [SerializeField] private int maxLogNumber;
        [SerializeField] private bool useUppercase;

        public void Awake()
        {
            if (Singleton == null)
                Singleton = this;
            else
                Destroy(this);

            _log = new Queue<string>();
            Log("test", "Hello");
        }

        public CloudLoggerSingular Get(string moduleName) => new CloudLoggerSingular(moduleName);

        public void Log(string moduleName, string text)
        {
            var finalText = FormText(moduleName, text);
            if (_log.Count >= maxLogNumber)
                _log.Dequeue();
            _log.Enqueue(finalText);
            if (useDebugLog)
                Debug.Log(finalText);
            OnLog?.Invoke();
        }

        public void LogError(string moduleName, string text, bool informPlayer = false)
        {
            var finalText = FormText(moduleName, text);
            if (_log.Count >= maxLogNumber)
                _log.Dequeue();
            _log.Enqueue(finalText);
            if (useDebugLog)
                Debug.LogError(finalText);
            OnLogError?.Invoke();
#if !DEDICATED_SERVER
            if (informPlayer)
                ShowErrorModal(moduleName, text);
#endif
        }

        private string FormText(string moduleName, string text)
        {
#if DEDICATED_SERVER
            var finalText = "[SERVER] " + moduleName + ": " + text;
#else
            var finalText = moduleName + ": " + text;
#endif
            if (useUppercase) return finalText.ToUpper();
            return finalText;
        }

        private void ShowErrorModal(string moduleName, string text)
        {
            var extramodal = Instantiate(modalPanelPrefab);
            extramodal.transform.SetParent(canvas);
            extramodal.transform.localScale = new Vector3(1, 1, 1);
            ModalController.ModalSetting setting = new ModalController.ModalSetting();
            setting.header_Text = "Error: " + moduleName;
            setting.content_Text = text;
            setting.footer_RemoveCancelButton = true;
            setting.footer_RemoveAlternateButton = true;
            modalPanelPrefab.ShowModal(setting);

            void CloseModal()
            {
                modalPanelPrefab.OnConfirmButtonPressed -= CloseModal;
                Destroy(modalPanelPrefab);
            }

            modalPanelPrefab.OnConfirmButtonPressed += CloseModal;
        }
    }
}
