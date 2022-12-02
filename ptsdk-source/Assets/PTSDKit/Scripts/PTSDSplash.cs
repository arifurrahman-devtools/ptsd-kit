using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PTSDKit
{
    public class PTSDSplash : MonoBehaviour
    {
        public static PTSDSplash Instance { get; private set; }
        public Image progressImage;
        public void SetProgress(float progress)
        {
            if (progressImage) progressImage.fillAmount = progress;
        }
        private void Awake()
        {
            Instance = this;
        }
    }


}
