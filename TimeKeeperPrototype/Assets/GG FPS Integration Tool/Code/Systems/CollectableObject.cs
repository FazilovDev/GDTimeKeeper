using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGFPSIntegrationTool.Systems
{
    public class CollectableObject : MonoBehaviour
    {
        public enum CollectionType { Weapon, Ammo }

        [SerializeField] CollectionType m_CollectionType = CollectionType.Weapon;

        [SerializeField] Weapon m_Weapon;
        [SerializeField] Ammo m_Ammo;

        [SerializeField] bool m_Enable = true;
        [SerializeField] int m_AmmoInWeapon;
        [SerializeField] int m_AddToAmmoTotal;

        [SerializeField] GameObject m_AfterCollectionObject;
        [SerializeField] float m_AfterCollectionDespawnTime;


        WeaponSpace m_CurrentWeaponSpace;

        void OnTriggerEnter(Collider other)
        {
            m_CurrentWeaponSpace = other.GetComponentInChildren<WeaponSpace>();

            if (m_CurrentWeaponSpace != null)
            {
                if (m_CollectionType == CollectionType.Weapon)
                {
                    m_CurrentWeaponSpace.WeaponSpaceManager.AmmoManager.WeaponAmmoCount(
                        m_Weapon,
                        m_AmmoInWeapon,
                        m_Weapon.ammo
                        );

                    m_CurrentWeaponSpace.WeaponSpaceManager.AmmoManager.IncreaseTotalAmmoCount(
                        m_Weapon.ammo,
                        m_AddToAmmoTotal
                        );

                    // Enabling/disabling weapons after asigning ammo prevents issue with
                    // how ammo is applied to weapon or total ammo counts
                    if (m_Enable)
                    {
                        m_CurrentWeaponSpace.WeaponSpaceManager.EnableManager.EnableWeapon(m_Weapon);
                    }
                    else
                    {
                        m_CurrentWeaponSpace.WeaponSpaceManager.EnableManager.DisableWeapon(m_Weapon);
                    }
                }
                else if (m_CollectionType == CollectionType.Ammo)
                {
                    m_CurrentWeaponSpace.WeaponSpaceManager.AmmoManager.IncreaseTotalAmmoCount(
                        m_Ammo,
                        m_AddToAmmoTotal
                        );
                }

                if (m_AfterCollectionObject != null)
                {
                    GameObject afterObjectInstance = Instantiate(m_AfterCollectionObject, transform.position, transform.rotation);
                    Destroy(afterObjectInstance, m_AfterCollectionDespawnTime);
                }

                Destroy(gameObject);
            }
        }
    }
}