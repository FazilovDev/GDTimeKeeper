using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GGFPSIntegrationTool.Systems
{
    public class Explosion : MonoBehaviour
    {
        [SerializeField] float m_Force = 15f;
        [SerializeField] float m_Radius = 5f;
        [SerializeField] float m_UpwardsModifier = 1f;
        [SerializeField] float m_ParticleEffectSize = 1f;

        [SerializeField] float _MaximumDamage = 100f;

        Collider[] m_EffectedColliders;
        Rigidbody m_CurrentRigidbody;

        IEnumerator Start()
        {
            ParticleSystem();

            yield return null;

            // Apply explosion force on next frame if effecting spawned debris is needed
            ExplosionEffect();
        }

        void ParticleSystem()
        {
            ParticleSystem[] particleSystems = GetComponentsInChildren<ParticleSystem>();

            foreach (ParticleSystem s in particleSystems)
            {
                ParticleSystem.MainModule mainModule = s.main;

                mainModule.startSizeMultiplier *= m_ParticleEffectSize;
                mainModule.startSpeedMultiplier *= m_ParticleEffectSize;

                s.Play();
            }
        }

        void ExplosionEffect()
        {
            m_EffectedColliders = Physics.OverlapSphere(transform.position, m_Radius);

            foreach (Collider c in m_EffectedColliders)
            {
                m_CurrentRigidbody = c.attachedRigidbody;

                if (m_CurrentRigidbody != null)
                {
                    m_CurrentRigidbody.AddExplosionForce(m_Force, transform.position, m_Radius, m_UpwardsModifier, ForceMode.Impulse);
                }


                    // ? 
                PlayerHealth playerHealth = c.gameObject.GetComponent<PlayerHealth>();
                NonPlayerHealth nonPlayerHealth = c.gameObject.GetComponent<NonPlayerHealth>();

                if (playerHealth != null || nonPlayerHealth != null)
                {
                    
                    float distance = Vector3.Distance(transform.position, c.transform.position);

                    // Should be between 0 and 1, anything beyond radius will not be consider
                    float distanceInterpolation = distance / m_Radius;

                    // The closer to the explosion, the more damage
                    // distanceInterpolation acts like a multiplier, when its at 1 meaning max distance,
                    // thus no damage is applied
                    if (playerHealth != null)
                    {
                        playerHealth.InflictDamage(_MaximumDamage - (_MaximumDamage * distanceInterpolation));
                    }
                    else
                    {
                        nonPlayerHealth.InflictDamage(_MaximumDamage - (_MaximumDamage * distanceInterpolation));
                    }
                }

                // @@@@@@@@@@@@@@@@@@@@

            }
        }
    }
}