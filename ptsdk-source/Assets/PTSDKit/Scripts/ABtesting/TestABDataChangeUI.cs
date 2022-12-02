using System;
using UnityEngine;
using UnityEngine.UI;
namespace PTSDKit
{
    public class TestABDataChangeUI : MonoBehaviour
    {
        public Text typeTitleText;
        public Text valueText;
        public Button nextValueButton;
        public Button prevValueButton;

        ABKeep keep;

        int selectionIndex = -1;
        int N;
        public bool loaded = false;

        HardAB ab;
        public void Load(ABKeep keep)
        {
            loaded = true;
            this.keep = keep;
            ab = keep.hardAB;
            N = keep.displayDefinitions.Count;

            for (int i = 0; i < N; i++)
            {
                string s;
                ab.GetValue(out s);
                if (keep.displayDefinitions[i].value == s) selectionIndex = i;
            }
            typeTitleText.text = ab.GetKey();
            if (selectionIndex >= 0)
            {
                SetDisplayText();
            }
            else
            {
                valueText.text = "server was silent";
            }
            nextValueButton.onClick.RemoveAllListeners();
            nextValueButton.onClick.AddListener(OnNext);

            prevValueButton.onClick.RemoveAllListeners();
            prevValueButton.onClick.AddListener(OnPrev);
        }
        void SetDisplayText()
        {
            ABDisplayDefinitions dd = keep.displayDefinitions[selectionIndex];

            valueText.text =  string.IsNullOrEmpty(dd.significance)?dd.value:dd.significance;
        }


        void OnNext()
        {
            if (selectionIndex == -1) selectionIndex = 0;
            else
            {
                selectionIndex++;
                if (selectionIndex >= N) selectionIndex = 0;
            }
            SetDisplayText();
        }
        void OnPrev()
        {
            if (selectionIndex == -1) selectionIndex = 0;
            else
            {
                selectionIndex--;
                if (selectionIndex < 0) selectionIndex = N - 1;
            }

            SetDisplayText();
        }



        public void OnApply()
        {
            if (selectionIndex >= 0)
            {
                ab.ForceSetValue(keep.displayDefinitions[selectionIndex].value);
                if (keep.ab_type == ABMan.Instance.customDefinitionSetup.type)
                {
                    int value = 0;
                    keep.hardAB.GetValue(out value);
                    ABMan.Instance.customDefinitionSetup.CheckCustomDimensions(value);
                }
            }
        }
    }
}