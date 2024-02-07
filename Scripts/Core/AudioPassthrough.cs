using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dirtywave.M8.Core
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioPassthrough : MonoBehaviour
    {
        private string _trackerMicName;
        private AudioSource _audio;

        [SerializeField, Min(0)]
        private int numSamplesLatency;
        
        void Start()
        {
            _audio = GetComponent<AudioSource>();
            foreach (var mic in Microphone.devices)
            {
                if (!mic.Contains("M8"))
                    continue;
                Debug.Log($"M8 found: {mic}");
                StartCoroutine(SetUpAudio(mic));
                break;
            }

        }

        private IEnumerator SetUpAudio(string micName)
        {
            const int loopBreaker = 100;
            _trackerMicName = micName;
            _audio = GetComponent<AudioSource>();
            _audio.clip = Microphone.Start(micName, true, 10, 44100);
            _audio.loop = true;
            var framesWaited = 0;
            while ((Microphone.GetPosition(_trackerMicName) <= numSamplesLatency) 
                   && framesWaited++ < loopBreaker)
                yield return new WaitForFixedUpdate();
            if (framesWaited == loopBreaker)
                Debug.LogError("Failed to start Microphone");
            else
                _audio.Play();
        }

        private void OnValidate()
        {
            var mics = Microphone.devices;
            Debug.Log($"Microphones: {String.Join(", ", mics)}");
        }

        private void OnDisable()
        {
            Microphone.End(_trackerMicName);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
