using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GGFPSIntegrationTool.Systems
{
    // ? Use inheritance; Abstract Health class?
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField] float _MaximumHealth = 100;

        [SerializeField] Text _HealthText;
        [SerializeField] Image _LowHealthOverlay;
        [SerializeField] Image _DamageOverlay;

        [SerializeField] [Range(0f, 1f)] float _MaximumDamageOverlayOpacity = 0.3f;
        [SerializeField] [Range(0f, 5f)] float _DamageOverlayFadeDuration = 0.3f;

        public int DeathCount { get; private set; } = 0;

        float CurrentHealth { get; set; }
        float CurrentDamageOverlayOpacity { get; set; }

        CharacterController CharacterControllerProperty { get; set; }

        Vector3 PlayerRespawnPosition { get; set; }
        Quaternion PlayerRespawnRotation { get; set; }

        void Awake()
        {
            CurrentHealth = _MaximumHealth;

            CharacterControllerProperty = GetComponent<CharacterController>();

            PlayerRespawnPosition = transform.position;
            PlayerRespawnRotation = transform.rotation;
        }

        void Update()
        {
            _HealthText.text = ((int)CurrentHealth).ToString();

            UpdateLowHealthOverlay();
            UpdateDamageOverlay();
        }


        public void InflictDamage(float healthLoss)
        {
            CurrentHealth -= healthLoss;

            CurrentDamageOverlayOpacity = _MaximumDamageOverlayOpacity;

            if (CurrentHealth <= 0f)
            {
                DeathCount++;
                RespawnPlayer();
            }
        }

        public void AddHealth(float healthToAdd)
        {
            CurrentHealth += healthToAdd;

            if (CurrentHealth > _MaximumHealth)
            {
                CurrentHealth = _MaximumHealth;
            }
        }

        void RespawnPlayer()
        {
            CurrentHealth = _MaximumHealth;

            // Prevents characterController from overriding transform just after respawning,
            // causing disruption to respawing
            if (CharacterControllerProperty != null)
            {
                CharacterControllerProperty.enabled = false;
            }

            // ? will eventually need to reset weapon variables as well?
            transform.position = PlayerRespawnPosition;
            transform.rotation = PlayerRespawnRotation;

            if (CharacterControllerProperty != null)
            {
                CharacterControllerProperty.enabled = true;
            }
        }

        void UpdateLowHealthOverlay()
        {
            float alpha = (_MaximumHealth - CurrentHealth) / _MaximumHealth;

            Color c = _LowHealthOverlay.color;
            _LowHealthOverlay.color = new Color(c.r, c.g, c.b, alpha);
        }

        void UpdateDamageOverlay()
        {
            if (CurrentDamageOverlayOpacity > 0f)
            {
                CurrentDamageOverlayOpacity -= Time.deltaTime / _DamageOverlayFadeDuration;
            }
            else
            {
                CurrentDamageOverlayOpacity = 0f;
            }

            Color c = _DamageOverlay.color;
            _DamageOverlay.color = new Color(c.r, c.g, c.b, CurrentDamageOverlayOpacity);
        }
    }
}