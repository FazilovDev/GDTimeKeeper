using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GGFPSIntegrationTool.Systems
{
    [RequireComponent(typeof(AudioSource))]
    public class NonPlayerAudio : MonoBehaviour
    {
        [SerializeField] AudioClip _PeriodicAudioClip;
        [SerializeField] AudioClip _DamageAudioClip;
        [SerializeField] AudioClip _DeathAudioClip;

        public Utilities.General.Range PeriodicAudioIntervalRange { get; set; } = 
            new Utilities.General.Range(3f, 6f);
        public Utilities.General.Range DamageAudioIntervalRange { get; set; } = 
            new Utilities.General.Range(0.4f, 0.6f);
        public Utilities.General.Range PitchRange { get; set; } = 
            new Utilities.General.Range(0.9f, 1.2f);

        Utilities.General.Time.CountDown PeriodicAudioCountDown { get; set; } = 
            new Utilities.General.Time.CountDown();
        Utilities.General.Time.CountDown DamageAudioCountDown { get; set; } = 
            new Utilities.General.Time.CountDown();

        bool HasDeathAudioPlayed { get; set; } = false;

        AudioSource AudioSourceProperty { get; set; }

        void Awake()
        {
            AudioSourceProperty = GetComponent<AudioSource>();
        }

        void Start()
        {
            PeriodicAudioCountDown.Start(PeriodicAudioIntervalRange.RandomValue);
            DamageAudioCountDown.Start(DamageAudioIntervalRange.RandomValue);
        }

        void Update()
        {
            if (!HasDeathAudioPlayed)
            {
                PlayPeriodicAudio();

                PeriodicAudioCountDown.Update();
                DamageAudioCountDown.Update();
            }
        }

        
        public void PlayDamageAudio()
        {
            if (DamageAudioCountDown.HasCountDownEnded)
            {
                PlayAudio(_DamageAudioClip);

                PeriodicAudioCountDown.Start(PeriodicAudioIntervalRange.RandomValue);
                DamageAudioCountDown.Start(DamageAudioIntervalRange.RandomValue);
            }
        }

        public void PlayDeathAudio()
        {
            if (!HasDeathAudioPlayed)
            {
                PlayAudio(_DeathAudioClip);
                HasDeathAudioPlayed = true;
            }
        }

        void PlayPeriodicAudio()
        {
            if (PeriodicAudioCountDown.HasEnded)
            {
                PlayAudio(_PeriodicAudioClip);
                PeriodicAudioCountDown.Start(PeriodicAudioIntervalRange.RandomValue);
            }
        }

        void PlayAudio(AudioClip audioClip)
        {
            AudioSourceProperty.pitch = PitchRange.RandomValue;
            AudioSourceProperty.clip = audioClip;
            AudioSourceProperty.Play();
        }
    }
}