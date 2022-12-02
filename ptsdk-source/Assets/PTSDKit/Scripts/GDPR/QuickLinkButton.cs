using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace PTSDKit
{
    public class QuickLinkButton : MonoBehaviour
    {
        [SerializeField] Button button;
        [TextArea]
        public string link = "";

        private void Start()
        {
            Init();
        }
        void Init()
        {
            if (button == null || string.IsNullOrEmpty(link))
            {
                Debug.LogError("Please set link properly!");
            }
            else
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() =>
                {
                    Application.OpenURL(link);
                });
            }
        }
        public void SetLink(string newLink)
        {
            link = newLink;
            button = GetComponent<Button>();
            Text linkText = this.transform.GetComponentInChildren<Text>();
            linkText.text = link;
            Vector2 sd = (this.transform as RectTransform).sizeDelta;
            sd.y = link.Length >= 65 ? 95 : 65;
            (this.transform as RectTransform).sizeDelta = sd;
            string[] parts = link.Split('.');
            this.gameObject.name = parts[1];
            Init();
        }

    }
}
