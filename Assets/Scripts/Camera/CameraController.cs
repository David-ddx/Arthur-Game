// Assets/Scripts/Camera/CameraController.cs
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("跟随目标 - 把Hero_Knight拖进来")]
    public Transform target;

    [Header("摄像机参数")]
    public float distance = 5f;
    public float minDistance = 2f;
    public float maxDistance = 10f;
    public float zoomSpeed = 3f;
    public float mouseSensitivity = 3f;
    public float minPitch = -20f;
    public float maxPitch = 60f;
    public Vector3 targetOffset = new Vector3(0f, 1.5f, 0f);

    // 外部可读取的摄像机水平朝向（PlayerController用这个）
    [HideInInspector] public Vector3 flatForward;
    [HideInInspector] public Vector3 flatRight;

    private float yaw = 0f;
    private float pitch = 20f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 读取鼠标
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // 滚轮缩放
        distance -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        // 计算摄像机位置
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 lookAtPoint = target.position + targetOffset;
        transform.position = lookAtPoint - rotation * Vector3.forward * distance;
        transform.LookAt(lookAtPoint);

        // 每帧更新水平朝向（供PlayerController使用）
        flatForward = rotation * Vector3.forward;
        flatForward.y = 0f;
        flatForward.Normalize();

        flatRight = rotation * Vector3.right;
        flatRight.y = 0f;
        flatRight.Normalize();
    }
}
