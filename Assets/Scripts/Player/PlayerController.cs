// Assets/Scripts/Player/PlayerController.cs
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerStats))]
public class PlayerController : MonoBehaviour
{
    [Header("移动参数")]
    public float walkSpeed = 4f;
    public float runSpeed = 7f;
    public float rotationSpeed = 10f;
    public float gravity = -20f;

    [Header("耐力消耗")]
    public float runStaminaCost = 10f;

    [Header("摄像机 - 把Main Camera拖进来")]
    public CameraController cameraController;

    private CharacterController cc;
    private PlayerStats stats;
    private Vector3 velocity;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        stats = GetComponent<PlayerStats>();

        if (cameraController == null)
            Debug.LogError("PlayerController：请把Main Camera拖进 Camera Controller 槽！");
    }

    void Update()
    {
        if (stats.IsDead()) return;
        HandleMovement();
    }

    void HandleMovement()
    {
        // 地面检测
        if (cc.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // 输入
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 input = new Vector3(h, 0f, v);

        if (input.magnitude > 0.1f && cameraController != null)
        {
            // 用摄像机脚本暴露的水平朝向计算移动方向
            Vector3 moveDir = (cameraController.flatForward * v
                             + cameraController.flatRight * h).normalized;

            // 跑步判断
            bool isRunning = Input.GetKey(KeyCode.LeftShift)
                          && stats.UseStamina(runStaminaCost * Time.deltaTime);
            float speed = isRunning ? runSpeed : walkSpeed;

            cc.Move(moveDir * speed * Time.deltaTime);

            // 转身
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(moveDir),
                rotationSpeed * Time.deltaTime
            );
        }

        // 重力
        velocity.y += gravity * Time.deltaTime;
        cc.Move(velocity * Time.deltaTime);
    }
    public float GetCurrentSpeed()
    {
        return cc.velocity.magnitude;
    }

    // 返回当前移动速度（给动画用）
    public float GetCurrentMoveSpeed()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 input = new Vector3(h, 0f, v);

        if (input.magnitude < 0.1f)
            return 0f;
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        return isRunning ? runSpeed : walkSpeed;
    }


}
