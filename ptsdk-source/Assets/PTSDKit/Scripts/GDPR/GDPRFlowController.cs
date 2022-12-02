using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
namespace PTSDKit
{
    public class GDPRFlowController : MonoBehaviour
    {
        public Button disruptiveButton;
        [Space]
        public GameObject root;
        public Transform panelRoot;
        public Button proceedButton;
        public Button paranoiaButton;
        public Toggle analyticsToggle;
        public Toggle adsToggle;
        public Text proceedText;
        public Text paranoiaText;
        [Space]
        public Transform linkRoot;
        public GameObject linkPrefab;

        public void Disable()
        {
            root.SetActive(false);
            disruptiveButton.gameObject.SetActive(false);
        }
        Action<bool, bool> onComplete_analytics_ads;
        public void StartFlow(Action<bool, bool> onComplete_analytics_ads)
        {
            analyticsToggle.isOn = GDPRMan.HasAnalyticsPermission;
            adsToggle.isOn = GDPRMan.HasAdPermission;
            Debug.Log("first time choice");
            this.onComplete_analytics_ads = onComplete_analytics_ads;
            disruptiveButton.gameObject.SetActive(false);
            root.SetActive(true);
            SetPage(GDPRpage.Intro);
        }
        public void ReturnToFlow(Action<bool, bool> onComplete_analytics_ads)
        {
            analyticsToggle.isOn = GDPRMan.HasAnalyticsPermission;
            adsToggle.isOn = GDPRMan.HasAdPermission;
            this.onComplete_analytics_ads = onComplete_analytics_ads;
            disruptiveButton.gameObject.SetActive(false);
            root.SetActive(true);
            SetPage(GDPRpage.Options);
        }

        GDPRpage currentPage;
        void Start()
        {
            proceedButton.onClick.AddListener(Proceed);
            paranoiaButton.onClick.AddListener(BeParanoid);
        }
        void SetPage(GDPRpage page)
        {
            currentPage = page;
            int i = 0;
            switch (currentPage)
            {
                case GDPRpage.Intro:
                    proceedText.text = "Awesome! I support that :)";
                    paranoiaText.text = "Manage Data Settings";
                    break;
                case GDPRpage.Benefits:
                    proceedText.text = "Awesome! I support that :)";
                    paranoiaText.text = "Next";
                    break;
                case GDPRpage.Options:

                    proceedText.text = "Accept";
                    paranoiaText.text = "Back";
                    break;
                case GDPRpage.Warning:
                    proceedText.text = "Let me fix my settings";
                    paranoiaText.text = "I understand";
                    break;
                default:
                    break;
            }
            foreach (Transform item in panelRoot)
            {
                item.gameObject.SetActive(i == (int)page);
                i++;
            }
            
        }
        void Action_Complete(bool analytics, bool ads)
        {
            root.SetActive(false);
            onComplete_analytics_ads?.Invoke(analytics, ads);
            onComplete_analytics_ads = null;

        }
        void Proceed()
        {
            switch (currentPage)
            {
                case GDPRpage.Intro:
                case GDPRpage.Benefits:
                    Action_Complete(true, true);
                    break;
                case GDPRpage.Options:
                    {
                        if (analyticsToggle.isOn && adsToggle.isOn)
                        {
                            Action_Complete(true, true);
                        }
                        else
                        {
                            SetPage(GDPRpage.Warning);
                        }
                    }
                    break;
                case GDPRpage.Warning:
                    SetPage(GDPRpage.Options);
                    break;
                default:
                    Debug.LogError("Unknown case!");
                    break;
            }
        }
        void BeParanoid()
        {
            switch (currentPage)
            {
                case GDPRpage.Intro:
                    SetPage(GDPRpage.Benefits);
                    break;
                case GDPRpage.Benefits:
                    SetPage(GDPRpage.Options);
                    break;
                case GDPRpage.Options:
                    SetPage(GDPRpage.Benefits);
                    break;
                case GDPRpage.Warning:
                    Action_Complete(analyticsToggle.isOn, adsToggle.isOn);
                    break;
                default:
                    Debug.LogError("Unknown case!");
                    break;
            }

        }

        public void PopulateLinks(List<string> links)
        {
            foreach (string l in links)
            {
                GameObject lg = Instantiate(linkPrefab);
                lg.transform.SetParent(linkRoot);
                lg.transform.localScale = Vector3.one;
                lg.GetComponent<QuickLinkButton>().SetLink(l);
            }
        }

        public enum GDPRpage
        {
            Intro = 0,
            Benefits = 1,
            Options = 2,
            Warning = 3,
        }

        #region renaming
        [Space]
        [Space]
        [Header("Replace Target Texts")]
        public Text introHeader;
        public Text introBody;
        public Text benefitsTitle;
        public Text warningBody;
        [HideInInspector] public string currentGameName;
        public void SetName(string newGDPRname)
        {
            currentGameName = newGDPRname;
            introHeader.text = string.Format("Make {0} better and stay free!",currentGameName);
            
            introBody.text = string.Format("Hey! We hope you're excited to try {0}. {1} {2} {3} {4}", currentGameName,
                "Before you get started though, our team wanted to let you know, upon getting your consent we're going to continue improving our game with your device data.",
                "Specifically, we will be using your device data to optimize your gameplay mechanics, application stability, and show relevant ads which will give you more in-game currency.",
                "As always, we thank you for playing our game and helping us in anyway possible.",
                "If you're ever interested in sharing your data, you can always adjust your settings at a later time as well.");
            
            benefitsTitle.text = string.Format("How your data makes {0} better!",currentGameName);
            
            warningBody.text = string.Format("If you don't give us consent to use your data, you will be making our ability to support {0} harder, {1}",currentGameName,
                "which may result in negatively affecting the user experience.");
        }
        #endregion
    }

}