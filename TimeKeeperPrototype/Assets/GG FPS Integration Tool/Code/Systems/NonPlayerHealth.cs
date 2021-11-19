using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGFPSIntegrationTool.Systems
{
    // ? Use inheritance; Abstract Health class?
    [RequireComponent(typeof(NonPlayerAudio))]
    public class NonPlayerHealth : MonoBehaviour
    {
        [SerializeField] float _MaximumHealth = 100;

        public float DespawnAfterDeathDelay { get; set; } = 3f;

        public bool IsDead { get; private set; } = false;

        float CurrentHealth { get; set; }
        NonPlayerAudio NonPlayerAudioProperty { get; set; }

        void Awake()
        {
            CurrentHealth = _MaximumHealth;
            NonPlayerAudioProperty = GetComponent<NonPlayerAudio>();
        }
        
        public void InflictDamage(float healthLoss)
        {
            CurrentHealth -= healthLoss;

            if (CurrentHealth > 0f)
            {
                NonPlayerAudioProperty.PlayDamageAudio();
            }
            else
            {
                NonPlayerAudioProperty.PlayDeathAudio();

                IsDead = true;
                Destroy(gameObject, DespawnAfterDeathDelay);
            }
        }
    }
}