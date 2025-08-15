using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 4f;
    public float sprintSpeed = 7f;
    public float jumpHeight = 1.6f;
    public float gravity = -13f;

    [Header("Mouse")]
    public float mouseSensitivity = 1.8f;
    public float pitchMin = -89f;
    public float pitchMax = 89f;

    private CharacterController cc;
    private Transform camTransform;
    private float yaw;
    private float pitch;
    private Vector3 velocity;

    private void Start()
    {
        Application.targetFrameRate = 120;
    }

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        camTransform = Camera.main?.transform;
        if (camTransform == null) Debug.LogWarning("No main camera found. Assign a camera or tag the camera as MainCamera.");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        yaw = transform.eulerAngles.y;
        pitch = camTransform ? camTransform.eulerAngles.x : 0f;
    }

    void Update()
    {
        HandleMouse();
        HandleMovement();
    }

    void HandleMouse()
    {
        float mx = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        float my = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

        yaw += mx;
        pitch -= my;
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);

        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        if (camTransform) camTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    void HandleMovement()
    {
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        float speed = isSprinting ? sprintSpeed : walkSpeed;

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        input = input.normalized;

        Vector3 move = transform.right * input.x + transform.forward * input.y;
        cc.Move(move * (speed * Time.deltaTime));

        if (cc.isGrounded)
        {
            velocity.y = -1f;
            if (Input.GetButtonDown("Jump"))
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        cc.Move(velocity * Time.deltaTime);
    }
}