using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Luna_ThirdPersonController : MonoBehaviour
{
    [Header("Open the script for details")]

    [Header("Movement")] //A Header helps ORGANISE your fields
    [SerializeField] [Range(0.1f, 10f)] public float moveSpeed = 5f; //This is your moveSpeed, measured in floats to ensure we can use decimal numbers. The range attribute will show this in the inspector as a slider.
    [SerializeField] [Range(0.1f, 1080f)] public float rotationSpeed = 720f;
    [SerializeField] public float jumpForce = 5f;
    private Vector3 lastWorldMovementDirection; // Stores the player's movement direction in world space
    private bool maintainWorldDirection = false; // Flag to maintain movement direction after snapping

    [Header("Camera")]
    [SerializeField] public Transform cameraTransform;
    public Vector3 cameraOffset = new Vector3(0, 1.05f, -10); // Offset for the camera
    public Vector3 cameraLookOffset = new Vector3(0, 1, 0); // Offset for where the camera looks
    [Range(0.1f, 10f)] public float cameraFollowSpeed = 10f; // Speed of the camera following the player
    [SerializeField] private bool cameraFollowPlayerBack = false; // Determines if the camera should lock behind the player
    private bool playerOutOfView = false; // Tracks if the player is out of the camera's view
    private bool shouldSnapCamera = false; // Ensures snap happens only once
    private Camera mainCamera;
    private bool invertSAfterSnap = false; // Tracks whether S input is inverted after snapping


    [Header("Jump Check (can you jump or not)")]
    private Rigidbody rb; //reference to the rigidbody on the player
    private bool isGrounded; //if grounded, able to jump. If not, don't jump. Unless you want to jump forever, maybe simulate an octopus swimming in the water? Ask me for that particular project if interested.

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>(); //gets rigidbody component from the playerobject
        if (cameraTransform == null)
        {
            Debug.LogWarning("Camera Transform is not assigned."); //reminds you to assign camera in the debug log. Assign your camera!!
        }

        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found. Ensure there is a Main Camera in the scene.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        Move(); //calls a Move Event Function. This will be below the Update
        Rotate(); //calls a Rotate Event Function. This will be below the Update
        HandleCameraInput(); // Check for camera-related input
        UpdateCameraPosition();

        if (Input.GetButtonDown("Jump") && isGrounded) //Jump is bound to space by default. The isGrounded boolean is checked here to ensure you can jump.
        {
            Jump(); //calls a Jump Event Function.This will be below the Update
        }
    }

    void Move() //we're in the thick of it now, the Move Event function is called here. I'm sure everybody knows.
    {
        float horizontal = Input.GetAxis("Horizontal"); //A and D = left and right movement, make sure "Horizontal" is capitalised properly so that it works!
        float vertical = Input.GetAxis("Vertical"); //W and S = forwards and backwards movement, capitalise the V in Vertical.

        if (maintainWorldDirection)
        {
            // Use the stored world direction after snapping
            Vector3 moveDirection = lastWorldMovementDirection;
            Vector3 newPosition = transform.position + moveDirection * moveSpeed * Time.deltaTime;
            rb.MovePosition(newPosition);

            // Stop maintaining direction when the player releases the movement keys
            if (Mathf.Abs(horizontal) < 0.1f && Mathf.Abs(vertical) < 0.1f)
            {
                maintainWorldDirection = false; // Reset flag
            }

            return; // Skip normal movement handling
        }

        if (invertSAfterSnap)
        {
            vertical = -vertical; // Invert vertical input only if snapping occurred
        }

        Vector3 movementDirection = new Vector3(horizontal, 0, vertical).normalized; // this uses the above floats to actually move.

        if (movementDirection.magnitude >= 0.1f) //if the above movement direction is greater than 0.1, move the character. Otherwise, don't move.
        {

            if (vertical < 0 && !invertSAfterSnap) // Check if the player is pressing the 'S' key and vertical input isn't inverted yet
            {
                // Rotate the player 180 degrees to face backward
                transform.rotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y + 180, 0);
            }


            Vector3 moveDirection = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0) * movementDirection;
            Vector3 newPosition = transform.position + moveDirection * moveSpeed * Time.deltaTime;

            rb.MovePosition(newPosition);

            // Store the player's movement direction in world space before snapping
            lastWorldMovementDirection = moveDirection.normalized;
        }

        // Reset inversion when the player lets go of the 'S' key
        if (invertSAfterSnap && Mathf.Abs(Input.GetAxis("Vertical")) < 0.1f)
        {
            invertSAfterSnap = false; // Revert controls to normal
            if (shouldSnapCamera) // Snap the camera only once after letting go of S
            {
                SnapCameraToPlayer();
                shouldSnapCamera = false; // Reset snap flag
            }
        }
    }

    void Rotate() //rotate the character when turning.
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movementDirection = new Vector3(horizontal, 0, vertical).normalized;

        if (movementDirection.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(movementDirection.x, movementDirection.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float smoothedAngle = Mathf.LerpAngle(transform.eulerAngles.y, targetAngle, rotationSpeed * Time.deltaTime);

            transform.rotation = Quaternion.Euler(0, smoothedAngle, 0);
        }
    }

    void Jump() //Jump when on the ground.
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false; //sets isGrounded to false so we don't jump in the air again. Would be worth adjust for a double jump.
    }

    void HandleCameraInput()
    {
        // Toggle camera follow player back mode when 'P' is pressed
        if (Input.GetKeyDown(KeyCode.P))
        {
            cameraFollowPlayerBack = !cameraFollowPlayerBack; // Toggle the follow player back state
        }

        // Snap the camera behind the player when 'O' is pressed
        if (Input.GetKeyDown(KeyCode.O))
        {
            SnapCameraToPlayer(); // Snap without toggling the camera follow state
        }
    }

    void UpdateCameraPosition()
    {
        if (cameraTransform == null) return;

        if (cameraFollowPlayerBack)
        {
            // Check if the player is out of the camera's view
            CheckPlayerOutOfView();

            if (playerOutOfView)
            {
                shouldSnapCamera = true; // Mark that a snap should happen
            }

            if (Input.GetAxis("Vertical") >= 0 || playerOutOfView)
            {
                Vector3 targetPosition = transform.position + transform.rotation * cameraOffset;
                cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetPosition, cameraFollowSpeed * Time.deltaTime);

                Vector3 lookTarget = transform.position + cameraLookOffset;
                cameraTransform.rotation = Quaternion.Lerp(cameraTransform.rotation, Quaternion.LookRotation(lookTarget - cameraTransform.position), cameraFollowSpeed * Time.deltaTime);
            }
        }
        else
        {
            if (shouldSnapCamera && Mathf.Abs(Input.GetAxis("Vertical")) < 0.1f)
            {
                SnapCameraToPlayer();
                shouldSnapCamera = false; // Ensure the snap only happens once
            }
            else
            {
                Vector3 targetPosition = transform.position + cameraOffset;
                cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetPosition, cameraFollowSpeed * Time.deltaTime);

                Vector3 lookTarget = transform.position + cameraLookOffset;
                cameraTransform.LookAt(lookTarget);
            }
        }
    }

    void SnapCameraToPlayer()
    {
        if (cameraTransform == null) return;

        // Snap the camera directly behind the player
        Vector3 snapPosition = transform.position + transform.rotation * cameraOffset;
        cameraTransform.position = snapPosition;

        // Make the camera look slightly above the player
        Vector3 lookTarget = transform.position + cameraLookOffset;
        cameraTransform.rotation = Quaternion.LookRotation(lookTarget - cameraTransform.position);

        // Rotate the player 180 degrees
        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y + 180, 0);

        // Store the last movement direction in world space
        lastWorldMovementDirection = transform.forward * -1 * moveSpeed * Time.deltaTime;
        maintainWorldDirection = true; // Enable world-space movement maintenance
    }

    void CheckPlayerOutOfView()
    {
        if (mainCamera == null) return;

        // Convert the player's position to viewport coordinates
        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(transform.position);

        // Check if the player is out of the viewport (completely out of screen)
        if (viewportPosition.x < 0 || viewportPosition.x > 1 || viewportPosition.y < 0 || viewportPosition.y > 1 || viewportPosition.z < 0)
        {
            playerOutOfView = true;
        }
        else
        {
            playerOutOfView = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true; //if touching the ground, set isGrounded to true, allowing you to jump again.
        }
    }
}