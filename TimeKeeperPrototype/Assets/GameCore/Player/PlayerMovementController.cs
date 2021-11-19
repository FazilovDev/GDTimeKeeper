using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class PlayerMovementController : TimeObject
{
    [SerializeField] private Camera characterCamera;

    [Header("Movement Input Axes")]
    [SerializeField] private string movementXName;
    [SerializeField] private string movementYName;

    [Header("Movement Input Keys")]
    [SerializeField] private KeyCode jumpKey;
    [SerializeField] private KeyCode runKey;

    [Header("Mouse Input Axes")]
    [SerializeField] private string mouseXName;
    [SerializeField] private string mouseYName;

    [Header("Movement Speeds")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;

    [Header("Mouse Sensitivities")]
    [SerializeField] private float mouseXSensitivity = 1f;
    [SerializeField] private float mouseYSensitivity = 1f;

    [Header("Aero Mobility")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float aeroMobilityMultiplier = 1f;
    [SerializeField] private float gravityMultiplier = 2f;

    [Header("Camera Bobbing Effects")]
    [SerializeField] private float walkCycleRate = 2f;
    [SerializeField] private bool isCameraBobbingEnabled = true;
    [SerializeField] private float cameraBobIntensityMoving = 0.05f;
    [SerializeField] private float cameraBobIntensityLanding = 0.2f;

    [Header("Audio")]
    [SerializeField] private AudioClip firstFootstepAudio;
    [SerializeField] private AudioClip secondFootstepAudio;
    [SerializeField] private AudioClip jumpAudio;
    [SerializeField] private AudioClip landAudio;

    private CharacterController characterController;
    private AudioSource audioSource;

    private Vector3 groundMovementDirection;

    private bool wasGrounded = false;

    private bool isCursorLocked = true;
    private Vector2 mouseDifference = Vector2.zero;
    private Vector3 movementDirection = Vector3.zero;

    private float currentWalkCycleTime = 0f;
    private bool isSinePeakPassed = false;
    private bool isSecondFootstepNext = false;
    private Vector3 cameraLocalPositionStart;

    private Coroutine landBobCoroutine;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();

        cameraLocalPositionStart = characterCamera.transform.localPosition;
    }

    private void Update()
    {
        Movement();
        Rotation();
        WalkCycleTiming();
    }

    private void Movement()
    {
        groundMovementDirection = (transform.forward * Input.GetAxis(movementYName)) + (transform.right * Input.GetAxis(movementXName));

        RaycastHit hit;
        Physics.SphereCast(transform.position, characterController.radius, Vector3.down, out hit, characterController.height / 2f);

        groundMovementDirection = Vector3.ProjectOnPlane(groundMovementDirection, hit.normal).normalized;

        float speedMultiplyer = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        speedMultiplyer *= characterController.isGrounded ? 1f : aeroMobilityMultiplier;

        movementDirection.x = groundMovementDirection.x * speedMultiplyer;
        movementDirection.z = groundMovementDirection.z * speedMultiplyer;

        if (characterController.isGrounded && !wasGrounded)
        {
            audioSource.clip = landAudio;
            audioSource.Play();

            landBobCoroutine = StartCoroutine(LandBobOverTime());

            wasGrounded = true;
        }

        if (characterController.isGrounded)
        {
            movementDirection.y = -10f;

            if (Input.GetKey(jumpKey))
            {
                movementDirection.y = jumpForce;

                audioSource.clip = jumpAudio;
                audioSource.Play();
            }
        }
        else
        {
            movementDirection += Physics.gravity * gravityMultiplier * Time.deltaTime;
            wasGrounded = false;
        }

        if (groundMovementDirection.magnitude > 0f)
        {
            if (landBobCoroutine != null)
            {
                StopCoroutine(landBobCoroutine);
            }
        }

        characterController.Move(movementDirection * Time.deltaTime * speedTime);
    }

    private void Rotation()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            isCursorLocked = false;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isCursorLocked = true;
        }

        if (isCursorLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            mouseDifference = new Vector2(Input.GetAxis(mouseXName), Input.GetAxis(mouseYName));
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            mouseDifference = Vector2.zero;
        }

        mouseDifference.x *= mouseXSensitivity;
        mouseDifference.y *= mouseYSensitivity;

        Quaternion xAxisRotation = Quaternion.AngleAxis(mouseDifference.y, -Vector3.right);

        Quaternion cameraRotation = characterCamera.transform.localRotation;
        cameraRotation *= xAxisRotation;

        cameraRotation.x = Mathf.Clamp(cameraRotation.x, -0.707f, 0.707f);
        cameraRotation.w = Mathf.Clamp(cameraRotation.w, 0.707f, 2f);

        characterCamera.transform.localRotation = cameraRotation;

        Quaternion yAxisRotation = Quaternion.AngleAxis(mouseDifference.x, Vector3.up);

        Quaternion characterRotation = characterController.transform.localRotation;
        characterRotation *= yAxisRotation;

        characterController.transform.localRotation = characterRotation;
    }

    private IEnumerator LandBobOverTime()
    {
        float currentTime = 0f;
        const float endTime = 0.4f;

        float currentSine = 0f;

        while (currentTime < endTime)
        {
            currentTime += Time.deltaTime * speedTime;

            currentSine = Mathf.Sin(currentTime);

            if (isCameraBobbingEnabled)
            {
                characterCamera.transform.localPosition = cameraLocalPositionStart + new Vector3(0f, currentSine * cameraBobIntensityLanding, 0f);
            }

            yield return null;
        }
    }

    private void WalkCycleTiming()
    {
        if (groundMovementDirection.magnitude <= 0f)
        {
            return;
        }

        if (!characterController.isGrounded)
        {
            return;
        }

        const float pi = Mathf.PI;
        float currentWalkCycleRate = Input.GetKey(KeyCode.LeftShift) ? (walkCycleRate * (runSpeed / walkSpeed)) : walkCycleRate;

        currentWalkCycleTime += Time.deltaTime * pi * 2f * walkCycleRate * speedTime;

        if (currentWalkCycleTime > pi * 2f)
        {
            currentWalkCycleTime -= (pi * 2f);
        }

        if (currentWalkCycleTime > pi * 0.5f && currentWalkCycleTime <= pi * 1.5f)
        {
            isSinePeakPassed = true;
        }

        float currentSine = Mathf.Sin(currentWalkCycleTime);

        if (isCameraBobbingEnabled)
        {
            characterCamera.transform.localPosition = cameraLocalPositionStart + new Vector3(0f, currentSine * cameraBobIntensityMoving, 0f);
        }

        if (isSinePeakPassed && !(currentWalkCycleTime > pi * 0.5f && currentWalkCycleTime <= pi * 1.5f))
        {
            audioSource.clip = isSecondFootstepNext ? secondFootstepAudio : firstFootstepAudio;
            audioSource.Play();

            isSinePeakPassed = false;
            isSecondFootstepNext = !isSecondFootstepNext;
        }
    }
}
