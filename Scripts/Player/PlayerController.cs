using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float footstepDelay = 0.5f;
    [SerializeField] private float groundDrag = 15f;
    [SerializeField] private float airMultiplier = 0.1f;
    [SerializeField] private float airDrag = 0f;

    [Header("Ground Check")]
    [SerializeField] private float playerHeight = 2f;
    [SerializeField] private LayerMask whatIsGround;
    public bool grounded;

    [Header("Bobbing")]
    [SerializeField] private Animator bobbingAnimator;
    [SerializeField] private float bobbingMinimumSpeed = 2f;

    [Header("References")]
    [SerializeField] private Transform orientation;
    //[SerializeField] private GameObject crosshair;
    //[SerializeField] private GameObject weaponHolder;

    [Header("Audio")]
    [SerializeField] private GameObject[] footstepSounds;

    [Header("Portals")]
    [SerializeField] private int portalSceneIndex = 0;

    private Rigidbody rb;
    private bool canPlayFootstep;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        ResetFootstep();
    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        float speed = new Vector3(rb.velocity.x, 0f, rb.velocity.z).magnitude;
        bobbingAnimator.SetBool("Bobbing", speed >= bobbingMinimumSpeed);

        // stuff for itch.io page screenshots
        /*
        DateTime dateTime = DateTime.Now;
        if (Input.GetKeyDown(KeyCode.F12)) ScreenCapture.CaptureScreenshot("Screenshots/Screenshot_" + dateTime.Day
            + "-" + dateTime.Month + "-" + dateTime.Year + "_" + dateTime.Hour + "-" + dateTime.Minute + "-" + dateTime.Second + ".png");

        if (Input.GetKeyDown(KeyCode.P))
        {
            if (crosshair.activeSelf)
            {
                crosshair.SetActive(false);
                weaponHolder.SetActive(false);
            }
            else
            {
                crosshair.SetActive(true);
                weaponHolder.SetActive(true);
            }
        }
        */
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * movementSpeed, ForceMode.Force);
            rb.drag = groundDrag;
        }
        else
        {
            rb.AddForce(moveDirection.normalized * movementSpeed * airMultiplier, ForceMode.Force);
            rb.drag = airDrag;
        }

        // Footstep sound
        if (canPlayFootstep && moveDirection.sqrMagnitude > 0.2f)
        {
            PlayFootstep();
        }
    }

    private void PlayFootstep()
    {
        canPlayFootstep = false;
        Invoke(nameof(ResetFootstep), footstepDelay);

        int i = UnityEngine.Random.Range(0, footstepSounds.Length);
        Instantiate(footstepSounds[i], transform.position, Quaternion.identity, transform);
    }

    private void ResetFootstep() { canPlayFootstep = true; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Portal"))
        {
            Debug.Log("Entered portal");
            if (LevelLoader.instance != null) LevelLoader.instance.LoadLevel(portalSceneIndex);
            else Debug.LogError("Level Loader instance is null!");

        }
    }
}
