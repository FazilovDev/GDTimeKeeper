using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UWS = GGFPSIntegrationTool.Utilities.WeaponSpace;

namespace GGFPSIntegrationTool.Systems
{
    static class WeaponSpaceTooltip
    {
        public const string
            m_inputFire = "Key or button used to fire weapon.",
            m_inputAutoFire = "Key or button that is held down for automatic firing. Often the same as Input Auto Fire.",
            m_inputReload = "Key or button used to reload weapon.",
            m_inputSwitch = "Key or button used to switch weapon.",
            m_inputAim = "Key or button used to aim weapon.",
            m_inputRun = "Key or button that is held down to run.",

            m_MouseXInfluenceName = "Name of axis specified in Input Manager for left and right mouse movements.",
            m_MouseYInfluenceName = "Name of axis specified in Input Manager for up and down mouse movements.",

            m_WeaponCollection = "WeaponCollection to use on this character.",

            m_CameraRaySpawn = "Camera used to project firing ray via its Z axis.",
            m_FPSPlayer = "FPS player character in the scene.",

            m_UICrosshairSpace = "UI image GameObject used to display crosshair sprites.",
            m_UIMagAmmoCount = "UI text GameObject used to display weapon ammo count.",
            m_UITotalAmmoCount = "UI text GameObject used to display total ammo count.",
            m_UIAmmoIconSpace = "UI image GameObject used to display ammo icon sprites.";
    }

    [RequireComponent(typeof(Animator), typeof(AudioSource))]
    public class WeaponSpace : MonoBehaviour
    {
        [SerializeField] CharacterController m_CharacterController;
        [SerializeField] [Tooltip(WeaponSpaceTooltip.m_WeaponCollection)] WeaponCollection m_WeaponCollection;
        [SerializeField] [Tooltip(WeaponSpaceTooltip.m_CameraRaySpawn)] Transform m_CameraRaySpawn;
        [SerializeField] [Tooltip(WeaponSpaceTooltip.m_FPSPlayer)] Transform m_FPSPlayer;

        // ? should be independent impact namespace?
        [SerializeField] GameObject _BloodSplatterImpact;

        [Header("Input Keys")]
        [SerializeField] [Tooltip(WeaponSpaceTooltip.m_inputFire)] KeyCode m_inputFire;
        [SerializeField] [Tooltip(WeaponSpaceTooltip.m_inputAutoFire)] KeyCode m_inputAutoFire;
        [SerializeField] [Tooltip(WeaponSpaceTooltip.m_inputReload)] KeyCode m_inputReload;
        [SerializeField] [Tooltip(WeaponSpaceTooltip.m_inputSwitch)] KeyCode m_inputSwitch;
        [SerializeField] [Tooltip(WeaponSpaceTooltip.m_inputAim)] KeyCode m_inputAim;
        [SerializeField] [Tooltip(WeaponSpaceTooltip.m_inputRun)] KeyCode m_inputRun;

        [Header("Mouse Influence Axes")]
        [SerializeField] [Tooltip(WeaponSpaceTooltip.m_MouseXInfluenceName)] string m_MouseXInfluenceName;
        [SerializeField] [Tooltip(WeaponSpaceTooltip.m_MouseYInfluenceName)] string m_MouseYInfluenceName;

        [Header("UI Objects")]
        [SerializeField] [Tooltip(WeaponSpaceTooltip.m_UICrosshairSpace)] Image m_CrosshairSpace;
        [SerializeField] [Tooltip(WeaponSpaceTooltip.m_UIMagAmmoCount)] Text m_MagAmmoCount;
        [SerializeField] [Tooltip(WeaponSpaceTooltip.m_UITotalAmmoCount)] Text m_TotalAmmoCount;
        [SerializeField] [Tooltip(WeaponSpaceTooltip.m_UIAmmoIconSpace)] Image m_AmmoIconSpace;

        [Header("Layer Names")]
        [SerializeField] string[] _FireRaycastIgnorableLayerNames;

        public UWS.Manager WeaponSpaceManager { get; set; }

        void Awake()
        {
            WeaponSpaceManager = new UWS.Manager(
                m_CharacterController,
                m_WeaponCollection,
                GetComponent<Animator>(),
                GetComponent<AudioSource>(),
                this,
                transform,
                m_CameraRaySpawn,
                m_inputFire,
                m_inputAutoFire,
                m_inputReload,
                m_inputSwitch,
                m_inputAim,
                m_inputRun,
                m_CrosshairSpace,
                m_AmmoIconSpace,
                m_MagAmmoCount,
                m_TotalAmmoCount,
                _BloodSplatterImpact,
                _FireRaycastIgnorableLayerNames
                );
        }

        void Start()
        {
            WeaponSpaceManager.Start(transform);
        }

        void Update()
        {
            WeaponSpaceManager.Update(transform, m_MouseXInfluenceName, m_MouseYInfluenceName);
        }
    }
}