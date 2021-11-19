using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GGFPSIntegrationTool.Systems
{
    [RequireComponent(typeof(Animator), typeof(NavMeshAgent), typeof(NonPlayerHealth))]
    public class NonPlayerController : MonoBehaviour
    {
        [SerializeField] Transform _TargetPlayerTransform;
        [SerializeField] NonPlayerDamageInfliction _NonPlayerDamageInfliction;

        Animator AnimatorProperty { get; set; }
        NavMeshAgent NavMeshAgentProperty { get; set; }
        NonPlayerHealth NonPlayerHealthProperty { get; set; }
        Collider ColliderProperty { get; set; }
        Vector3 LastPosition { get; set; }

        void Awake()
        {
            AnimatorProperty = GetComponent<Animator>();
            NavMeshAgentProperty = GetComponent<NavMeshAgent>();
            NonPlayerHealthProperty = GetComponent<NonPlayerHealth>();
            ColliderProperty = GetComponent<Collider>();
        }

        void Start()
        {
            AnimatorProperty.SetBool("IsWalking", true);
        }

        void Update()
        {
            if (!NonPlayerHealthProperty.IsDead)
            {
                if (_NonPlayerDamageInfliction.IsDamaging)
                {
                    transform.position = LastPosition;
                }

                NavMeshAgentProperty.SetDestination(_TargetPlayerTransform.position);
                LastPosition = transform.position;
            }
            else
            {
                AnimatorProperty.SetBool("IsDead", true);

                NavMeshAgentProperty.enabled = false;
                ColliderProperty.enabled = false;
            }
        }
    }
}


