using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Accessibility
{

    public class ScreenShakePrefs : MonoBehaviour
    {
        public const string ScreenShakePrefsKey = "_screenShakePrefs";

        Slider slider;
        Toggle toggle;
        GameObject sliderParent;

        bool toggleOn;

        private void Awake()
        {
            slider = GetComponentInChildren<Slider>();
            sliderParent = slider.transform.parent.gameObject;
            toggle = GetComponentInChildren<Toggle>();
        }
        private void Start()
        {
            toggleOn = toggle.isOn;
            SetShakeToSliderCurrentVal();
        }

        private void OnEnable()
        {
            ReInitialise();
            slider.onValueChanged.AddListener(SaveSliderValue);
        }


        private void OnDisable()
        {
            slider.onValueChanged.RemoveListener(SaveSliderValue);
        }
        private void ReInitialise()
        {
            slider.value = GetSliderValue();
            if(slider.value == 0)
            {
                toggle.isOn = false;
                TurnOffScreenShake();
            }
            else
            {
                toggle.isOn = transform;
                TurnOnScreenShake();
            }
        }
        private float GetSliderValue()
        {
            if (!PlayerPrefs.HasKey(ScreenShakePrefsKey))
            {
                return 1;
            }
            return PlayerPrefs.GetFloat(ScreenShakePrefsKey);
        }

        public void SaveSliderValue(float newVal) => SetShakePref(newVal);

        // Update is called once per frame
        void Update()
        { 
            if (!toggle.isOn) // toggle has been switched off
            {
                if (toggleOn) // and we don't know about this
                {
                    TurnOffScreenShake();
                }
            }
            else
            {
                if (!toggleOn)
                {
                    TurnOnScreenShake();
                }
            }
        }

        private void TurnOnScreenShake()
        {
            toggleOn = true;
            sliderParent.SetActive(true);
            SetShakeToSliderCurrentVal();
        }

        private void TurnOffScreenShake()
        {
            toggleOn = false;
            sliderParent.SetActive(false);
            SetShakePref(0);
        }

        private void SetShakeToSliderCurrentVal()
        {
            var sliderVal = slider.value;
            SetShakePref(sliderVal);
        }

        private static void SetShakePref(float v)
        {
            PlayerPrefs.SetFloat(ScreenShakePrefsKey, v);
            if(PlayerPrefs.GetFloat(ScreenShakePrefsKey) != v)
            {
                throw new System.Exception("Did not save player prefs");
            }
        }

   
    }
}