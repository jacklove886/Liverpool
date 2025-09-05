using Models;
using UnityEngine;

public class MainPlayerCamera : MonoSingleton<MainPlayerCamera>
{
    [Header("相机组件")]
    public Camera playerCamera;
    public Transform viewPoint;

    [Header("跟随目标")]
    public GameObject player;

    [Header("相机控制参数")]
    public float xSpeed = 120f;           // 水平旋转速度
    public float ySpeed = 120f;           // 垂直旋转速度
    public float yMinLimit = -20f;        // 垂直角度最小值
    public float yMaxLimit = 80f;         // 垂直角度最大值

    [Header("距离控制")]
    public float distance = 5f;           // 相机距离
    public float distanceMin = 2f;        // 最小距离
    public float distanceMax = 10f;       // 最大距离
    public float zoomSpeed = 2f;          // 缩放速度

    [Header("位置偏移")]
    public Vector3 offset = new Vector3(0, 2, 0);  // 基础偏移
    public Vector3 rotateOffset = new Vector3(0, 0, -1);  // 旋转偏移

    [Header("平滑参数")]
    public float smoothTime = 0.1f;       
    public bool enableMouseControl = true; // 是否启用鼠标控制

    // 私有变量
    private float x = 0f;
    private float y = 0f;
    private Vector3 velocity = Vector3.zero;

    protected override void OnStart()
    {
        // 初始化相机角度
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
    }

    void Update()
    {
        if (player == null)
        {
            player = User.Instance.CurrentCharacterObject;
        }

        if (!enableMouseControl)
        {
            return;
        }
            
        // 鼠标控制相机旋转
        x += Input.GetAxis("Mouse X") * xSpeed * Time.deltaTime;
        y -= Input.GetAxis("Mouse Y") * ySpeed * Time.deltaTime;
        y = ClampAngle(y, yMinLimit, yMaxLimit);

        // 鼠标滚轮控制距离
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            distance -= scroll * zoomSpeed;
            distance = Mathf.Clamp(distance, distanceMin, distanceMax);
        }
    }

    void LateUpdate()
    {
        if (player == null)
            return;

        // 计算相机旋转
        Quaternion rotation = Quaternion.Euler(y, x, 0);
        
        // 计算目标位置
        Vector3 targetPosition = player.transform.position + offset + rotation * rotateOffset * distance;
        
        // 移动到目标位置
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        transform.rotation = rotation;

        // 同步玩家Y轴旋转
        player.transform.rotation = Quaternion.Euler(0, x, 0);
    }

    // 限制角度范围
    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360f) angle += 360f;
        if (angle > 360f) angle -= 360f;
        return Mathf.Clamp(angle, min, max);
    }

    // 设置跟随目标
    public void SetTarget(GameObject target)
    {
        player = target;
    }

    // 重置相机位置
    public void ResetCamera()
    {
        if (player != null)
        {
            Vector3 angles = player.transform.eulerAngles;
            x = angles.y;
            y = 20f; // 默认俯视角度
            distance = 5f;
        }
    }

    // 启用/禁用鼠标控制
    public void SetMouseControl(bool enable)
    {
        enableMouseControl = enable;
    }

}