﻿//
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
//
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;


namespace Microsoft.MixedReality.Toolkit.Experimental.UI


{
    /// <summary>
    /// Component that plays sounds to communicate the state of a pinch slider
    /// </summary>
    ///     

    [RequireComponent(typeof(PinchSlider))]
    [AddComponentMenu("Scripts/Slider/SliderSounds")]
    public class SliderSounds : MonoBehaviour
    {

        private HapticSound haptic;
        private float hapticPitch;
        public int hapticPattern = 0;
        //public ScrollingObjectCollection scrollObj = null; 


    [Header("Audio Clips")]
        [SerializeField]
        [Tooltip("Sound to play when interaction with slider starts")]
        private AudioClip interactionStartSound = null;
        [SerializeField]
        [Tooltip("Sound to play when interaction with slider ends")]
        private AudioClip interactionEndSound = null;

        [Header("Tick Notch Sounds")]

        [SerializeField]
        [Tooltip("Whether to play 'tick tick' sounds as the slider passes notches")]
        private bool playTickSounds = true;

        [SerializeField]
        [Tooltip("Sound to play when slider passes a notch")]
        private AudioClip passNotchSound = null;

        [Range(0, 1)]
        [SerializeField]
        private float tickEvery = 0.1f;

        [SerializeField]
        private float startPitch = 0.75f;

        [SerializeField]
        private float endPitch = 1.25f;

        [SerializeField]
        private float minSecondsBetweenTicks = 0.01f;



        #region Private members
        private PinchSlider slider;

        // Play sound when passing through slider notches
        private float accumulatedDeltaSliderValue = 0;
        private float lastSoundPlayTime;

        private AudioSource grabReleaseAudioSource = null;
        private AudioSource passNotchAudioSource = null;
        #endregion

        private void Start()
        {
            haptic = GameObject.FindGameObjectWithTag("SyntactsHub").GetComponent<HapticSound>();
            hapticPitch = haptic.customHapticDesign[hapticPattern].x;
            
            if (grabReleaseAudioSource == null)
            {
                grabReleaseAudioSource = gameObject.AddComponent<AudioSource>();
            }
            if (passNotchAudioSource == null)
            {
                passNotchAudioSource = gameObject.AddComponent<AudioSource>();
            }
            slider = GetComponent<PinchSlider>();
            slider.OnInteractionStarted.AddListener(OnInteractionStarted);
            slider.OnInteractionEnded.AddListener(OnInteractionEnded);
            slider.OnValueUpdated.AddListener(OnValueUpdated);
        }

        private void OnValueUpdated(SliderEventData eventData)
        {
            if (playTickSounds && passNotchAudioSource != null && passNotchSound != null)
            {
                float delta = eventData.NewValue - eventData.OldValue;
                accumulatedDeltaSliderValue += Mathf.Abs(delta);
                var now = Time.timeSinceLevelLoad;
                if (accumulatedDeltaSliderValue > tickEvery && now - lastSoundPlayTime > minSecondsBetweenTicks)
                {
                    passNotchAudioSource.pitch = Mathf.Lerp(startPitch, endPitch, eventData.NewValue);
                    //Debug.Log("passNotchAudioSource.pitch: " + passNotchAudioSource.pitch);

                    
                    //float normal = Mathf.InverseLerp(startPitch, endPitch, eventData.NewValue);
                    hapticPitch = Mathf.Lerp(haptic.scrollLowPitch, haptic.scrollHighPitch, eventData.NewValue);

                    //Debug.Log("hapticPitch: " + hapticPitch);

                    if (passNotchAudioSource.isActiveAndEnabled)
                    {                      
   
                        Debug.Log("accumulatedDeltaSliderValue: " + accumulatedDeltaSliderValue);
                        //hapticPattern = haptic.playSound; 
                        passNotchAudioSource.PlayOneShot(passNotchSound);
                        haptic.PressKeyHapticSample(hapticPattern);
                        haptic.UpdatePitch(hapticPattern, hapticPitch);
                    }

                    accumulatedDeltaSliderValue = 0;
                    lastSoundPlayTime = now;
                }
            }
        }

        private void OnInteractionEnded(SliderEventData arg0)
        {
            if (interactionEndSound != null && grabReleaseAudioSource != null && grabReleaseAudioSource.isActiveAndEnabled)
            {
                grabReleaseAudioSource.PlayOneShot(interactionEndSound);
            }
        }

        private void OnInteractionStarted(SliderEventData arg0)
        {
            if (interactionStartSound != null && grabReleaseAudioSource != null && grabReleaseAudioSource.isActiveAndEnabled)
            {
                grabReleaseAudioSource.PlayOneShot(interactionStartSound);
            }
        }
    }


}