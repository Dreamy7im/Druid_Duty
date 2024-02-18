using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float jumpHeight = 2f;

    [Header("Movement Settings too heavy")]
    [SerializeField] private float movementSpeedTooHeavy = 5f;
    [SerializeField] private float sprintSpeedTooHeavy = 10f;
    [SerializeField] private float jumpHeightTooHeavy = 2f;

    [Header("Crouch")]
    [SerializeField] private float crouchSpeed = 0.5f;
    [SerializeField] private float crouchHeight = 0.5f;

    [Header("KeyCode")]
    [SerializeField] private KeyCode Sprint;
    [SerializeField] private KeyCode Crouch;
    [SerializeField] private KeyCode Jump;


    [Header("Mouse Settings")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private bool UseMenu;

    [Header("Stamina Settings")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaDecreaseRate = 5f;
    [SerializeField] private float staminaRegenRate = 2f;
    [SerializeField] private float staminaRegenDelay = 1f; // Initial delay before stamina starts regenerating
    [SerializeField] private float jumpStaminaCost = 15f;

    [Header("Headbob Settings")]
    [SerializeField] private float headbobFrequency = 1f;
    [SerializeField] private float headbobIntensity = 0.05f;

    [Header("UI")]
    [SerializeField] private Image StaminaUI;
    [SerializeField] private Volume GlobalVolume;
   

    private CharacterController characterController;
    private Camera playerCamera;
    private Vector3 moveDirection = Vector3.zero;
    private float verticalRotation = 0f;
    private float verticalVelocity = 0f;
    private float headbobTimer = 0f;
    private Vector3 headbobStartPosition;

    [SerializeField] private float stamina;
    [SerializeField] private float currentStaminaRegenDelay; // Current delay for stamina regeneration
    private bool isSprinting = false;
    private float originalMovementSpeed;
    private float originalSprintSpeed;
    private float originalJumpHight;

    private bool isCrouching = false;
    private float originalControllerHeight;

    private bool Grounded;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        stamina = maxStamina;
        currentStaminaRegenDelay = staminaRegenDelay;
        headbobStartPosition = playerCamera.transform.localPosition;

        originalMovementSpeed = movementSpeed;
        originalSprintSpeed = sprintSpeed;
        originalJumpHight = jumpHeight;
        originalControllerHeight = characterController.height;
    }

    public void UsingMenuRotate()
    {
        UseMenu = !UseMenu;
    }

    private void Update()
    {
        if (!UseMenu)
        {
            HandleRotation();
        }

        HandleMovement();
        HandleSprint();
        HandleJump();
        HandleGravity();
        HandleStaminaRegeneration();
        HandleHeadbob();
        UpdateVinieteSprint();
        HandleCrouch();

        StaminaUI.fillAmount = stamina / maxStamina; // Wywaliæ do osobnego skryptu
    }

    public void SetWeightSpeed(bool IfTooHeavy)
    {
        if (IfTooHeavy)
        {
            movementSpeed = movementSpeedTooHeavy;
            sprintSpeed = sprintSpeedTooHeavy;
            jumpHeight = jumpHeightTooHeavy;
        }
        else
        {
            movementSpeed = originalMovementSpeed;
            sprintSpeed = originalSprintSpeed;
            jumpHeight = originalJumpHight;
        }
    }


    public void SetWeightOver()
    {
        movementSpeed = 0;
        sprintSpeed = 0;
        jumpHeight = 0;
    }

    private void UpdateVinieteSprint()
    {
        if (GlobalVolume != null)
        {
            // Oblicz wartoœæ wignietowania na podstawie stanu staminy
            float vignetStrength = 1f - (stamina / maxStamina);

            // Ogranicz wartoœæ wignietowania do zakresu od 0 do 1
            vignetStrength = Mathf.Clamp01(vignetStrength);

            // Zaktualizuj parametr wolumetrycznego efektu wignietowania
            GlobalVolume.profile.TryGet<Vignette>(out var vignette);

            if (vignette != null)
            {
                vignette.intensity.value = vignetStrength / 2;
            }
            else
            {
                Debug.LogWarning("Vignette effect not found on the Global Volume.");
            }
        }
        else
        {
            Debug.LogWarning("Global Volume reference not set.");
        }
    }



    private void HandleRotation()
    {
        Grounded = characterController.isGrounded;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        transform.Rotate(Vector3.up * mouseX);
        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    private void HandleMovement()
    {
        float moveForward = Input.GetAxis("Vertical") * movementSpeed;
        float moveSideways = Input.GetAxis("Horizontal") * movementSpeed;

        moveDirection = (transform.forward * moveForward) + (transform.right * moveSideways);
        characterController.Move(moveDirection * Time.deltaTime);
    }

    private void HandleCrouch()
    {
        // Jeœli gracz naciœnie przycisk kucania (Ctrl) i nie jest w trakcie skoku ani sprintu
        if (Input.GetKeyDown(Crouch) && !isSprinting && Grounded)
        {
            isCrouching = !isCrouching; // Zmieñ stan skradania (w³¹cz/wy³¹cz)

            if (isCrouching)
            {
                movementSpeed = crouchSpeed;
                characterController.height = crouchHeight;
            }
            else
            {
                movementSpeed = originalMovementSpeed;
                characterController.height = originalControllerHeight;
            }
        }

    }

    private void HandleSprint()
    {
        if (!isCrouching)
        {
            if (Input.GetKeyDown(Sprint) && stamina > 0f)
            {
                isSprinting = true;
                currentStaminaRegenDelay = staminaRegenDelay; // Reset delay for stamina regeneration when starting a new sprint
            }

            if (!Input.GetKey(Sprint) && stamina < maxStamina)
            {
                if (currentStaminaRegenDelay <= 0)
                {
                    stamina += staminaRegenRate * Time.deltaTime;
                    stamina = Mathf.Clamp(stamina, 0f, maxStamina);
                }
                else
                {
                    currentStaminaRegenDelay -= Time.deltaTime;

                    if (currentStaminaRegenDelay < 0)
                    {
                        currentStaminaRegenDelay = 0;
                    }
                }
            }

            if (Input.GetKeyUp(Sprint) || stamina <= 0)
            {
                isSprinting = false;
            }

            if (isSprinting && (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0))
            {
                moveDirection *= sprintSpeed;
                stamina -= staminaDecreaseRate * Time.deltaTime;

                if (stamina <= 0)
                {
                    isSprinting = false;
                    stamina = 0f; // Stamina should not go below 0
                }
            }
        }
    }

    

    private void HandleJump()
    {
        if (Grounded && Input.GetKeyDown(Jump) && stamina >= jumpStaminaCost)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
            stamina -= jumpStaminaCost;
        }
    }

    private void HandleGravity()
    {
        if (!characterController.isGrounded)
        {
            verticalVelocity += Physics.gravity.y * Time.deltaTime;
        }
        else
        {
            verticalVelocity = -0.5f;
        }

        moveDirection.y = verticalVelocity;
        characterController.Move(moveDirection * Time.deltaTime);
    }

    private void HandleStaminaRegeneration()
    {
        if (!isSprinting && stamina < maxStamina)
        {
            if (currentStaminaRegenDelay <= 0)
            {
                stamina += staminaRegenRate * Time.deltaTime;
                stamina = Mathf.Clamp(stamina, 0f, maxStamina);
            }
            else
            {
                currentStaminaRegenDelay -= Time.deltaTime;

                if (currentStaminaRegenDelay < 0)
                {
                    currentStaminaRegenDelay = 0;
                }
            }
        }
    }

    private void HandleHeadbob()
    {
        if (characterController.velocity.magnitude > 0 && Grounded)
        {
            float moveFactor = characterController.velocity.magnitude * Time.deltaTime;
            headbobTimer += moveFactor * headbobFrequency;
            float headbobFactor = Mathf.Sin(headbobTimer) * headbobIntensity;

            // Apply headbob movement to the camera
            playerCamera.transform.localPosition = headbobStartPosition + new Vector3(0f, headbobFactor, 0f);
        }
        else
        {
            // Reset headbob position when not moving
            playerCamera.transform.localPosition = headbobStartPosition;
            headbobTimer = 0f;
        }
    }
}