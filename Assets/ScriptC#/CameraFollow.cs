// CameraFollow.cs

using UnityEngine;
using UnityEngine.UI; // 引入UI命名空间来使用Button

public class CameraFollow : MonoBehaviour
{
    [Header("跟随目标")]
    public Transform target;

    [Header("跟随参数")]
    [Range(0.01f, 1.0f)]
    public float smoothSpeed = 0.125f;
    public Vector3 offset = new Vector3(0, 0, -10);

    [Header("缩放设置")]
    public float zoomSpeed = 4f;
    public float minZoom = 2f;
    public float maxZoom = 8f;

    [Header("拖拽设置")]
    public float dragSpeed = 2f;

    [Header("UI 引用")]
    [Tooltip("场景中那个用于重置视角的UI按钮")]
    public Button resetViewButton;

    private Camera cam;
    private Vector3 dragOrigin;
    private bool isFollowingPlayer = true;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("CameraFollow脚本需要挂载在带有Camera组件的对象上！");
            this.enabled = false;
            return;
        }

        if (resetViewButton != null)
        {
            resetViewButton.onClick.AddListener(ResetToPlayerView);
        }

        // 初始化按钮状态
        UpdateResetButtonState();
    }

    void LateUpdate()
    {
        HandleZoom();
        HandleDrag();

        if (isFollowingPlayer)
        {
            FollowTarget();
        }
    }

    public void ResetToPlayerView()
    {
        if (!isFollowingPlayer)
        {
            isFollowingPlayer = true;
            UpdateResetButtonState();
            Debug.Log("摄像机已重置为跟随玩家模式。");
        }
    }

    private void FollowTarget()
    {
        if (target == null) return;
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }

    private void HandleZoom()
    {
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            float scroll = Input.mouseScrollDelta.y;
            if (scroll != 0f)
            {
                cam.orthographicSize -= scroll * zoomSpeed * Time.unscaledDeltaTime * 10f;
                cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
            }
        }
    }

    private void HandleDrag()
    {
        if (Input.GetMouseButtonDown(2))
        {
            if (isFollowingPlayer)
            {
                isFollowingPlayer = false;
                UpdateResetButtonState();
                Debug.Log("摄像机切换到手动拖拽模式。");
            }
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(2))
        {
            if (isFollowingPlayer) return;
            Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);
            transform.position += difference;
        }
    }

    /// <summary>
    /// 更新重置视角按钮的显示状态
    /// </summary>
    private void UpdateResetButtonState()
    {
        if (resetViewButton != null)
        {
            // *** 核心改动在这里 ***
            // 当摄像机不跟随玩家时 (isFollowingPlayer为false)，显示按钮。
            // 当摄像机正在跟随玩家时 (isFollowingPlayer为true)，隐藏按钮。
            resetViewButton.gameObject.SetActive(!isFollowingPlayer);
        }
    }
}