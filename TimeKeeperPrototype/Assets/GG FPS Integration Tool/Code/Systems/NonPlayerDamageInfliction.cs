using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGFPSIntegrationTool.Systems
{
    [RequireComponent(typeof(Collider), typeof(AudioSource))]
    public class NonPlayerDamageInfliction : MonoBehaviour
    {
        [SerializeField] Animator _NonPlayerAnimator;
        [SerializeField] NonPlayerHealth _NonPlayerHealth;

        [SerializeField] Collider _TargetPlayerCollider;
        [SerializeField] PlayerHealth _TargetPlayerHealth;
        [SerializeField] float _DamageHealthLoss = 10f;
        [SerializeField] float _DamageCooldownDuration = 0.5f;
        [SerializeField] GameObject _BloodSplatter;

        public float BloodSplatterLifeTime { get; set; } = 1f;
        public float DamageInflictionCountDownStartOffset { get; set; } = -0.1f;

        public Utilities.General.Range PitchRange { get; set; } = new Utilities.General.Range(0.9f, 1.2f);

        public bool IsDamaging { get; private set; } = false;
        AudioSource AudioSourceProperty { get; set; }


        Utilities.General.Time.CountDown DamageInflictionCountDown { get; set; } 
            = new Utilities.General.Time.CountDown();

        void Awake()
        {
            AudioSourceProperty = GetComponent<AudioSource>();
        }


        void Update()
        {
            DamageInflictionCountDown.Update();

            if (IsDamaging && !_NonPlayerHealth.IsDead)
            {
                if (!DamageInflictionCountDown.HasStarted)
                {
                    DamageInflictionCountDown.Start(_DamageCooldownDuration + DamageInflictionCountDownStartOffset);
                }

                if (DamageInflictionCountDown.HasCountDownEnded)
                {
                    _TargetPlayerHealth.InflictDamage(_DamageHealthLoss);

                    AudioSourceProperty.pitch = PitchRange.RandomValue;
                    AudioSourceProperty.Play();

                    GameObject hitInstance = Instantiate(_BloodSplatter, transform.position, transform.rotation);
                    Destroy(hitInstance, BloodSplatterLifeTime);
                }
            }

            _NonPlayerAnimator.SetBool("IsHitting", IsDamaging);
        }

        void OnTriggerEnter(Collider collider)
        {
            if (_TargetPlayerCollider == collider)
            {
                IsDamaging = true;
            }
        }

        void OnTriggerExit(Collider collider)
        {
            if (_TargetPlayerCollider == collider)
            {
                IsDamaging = false;
            }
        }
    }
}