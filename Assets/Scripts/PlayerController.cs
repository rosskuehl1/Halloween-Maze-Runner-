using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float dashSpeed = 12f;
    public float dashDuration = 0.25f;
    public float gravity = -25f;
    public Transform cameraPivot; // Assign FollowCam transform

    [Header("UI")]
    public SimpleJoystick joystick; // Assign from Canvas
    public Button dashButton;       // Assign from Canvas

    private CharacterController cc;
    private Vector3 velocity;
    private bool dashing = false;
    private float dashTimer = 0f;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        if (dashButton) dashButton.onClick.AddListener(DoDash);
    }

    void Update()
    {
        Vector2 input = joystick ? joystick.Value : new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector3 dir = new Vector3(input.x, 0, input.y);
        dir = Quaternion.Euler(0, cameraPivot ? cameraPivot.eulerAngles.y : 0f, 0) * dir; // camera-relative
        dir = Vector3.ClampMagnitude(dir, 1f);

        float speed = dashing ? dashSpeed : moveSpeed;
        Vector3 horiz = dir * speed;

        if (!cc.isGrounded)
            velocity.y += gravity * Time.deltaTime;
        else
            velocity.y = -2f;

        Vector3 motion = horiz + new Vector3(0, velocity.y, 0);
        cc.Move(motion * Time.deltaTime);

        if (dir.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 12f * Time.deltaTime);

        if (dashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0) dashing = false;
        }

        // Camera follow (simple smooth)
        if (cameraPivot)
        {
            Vector3 target = transform.position + new Vector3(0, 20, -12);
            cameraPivot.position = Vector3.Lerp(cameraPivot.position, target, 6f * Time.deltaTime);
            cameraPivot.rotation = Quaternion.Lerp(cameraPivot.rotation, Quaternion.Euler(55, 0, 0), 4f * Time.deltaTime);
        }
    }

    public void DoDash()
    {
        if (dashing) return;
        dashing = true;
        dashTimer = dashDuration;
        // (Optional) trigger haptics or SFX here
    }
}
