// CameraFollow.cs

using UnityEngine;
using UnityEngine.UI; // ����UI�����ռ���ʹ��Button

public class CameraFollow : MonoBehaviour
{
    [Header("����Ŀ��")]
    public Transform target;

    [Header("�������")]
    [Range(0.01f, 1.0f)]
    public float smoothSpeed = 0.125f;
    public Vector3 offset = new Vector3(0, 0, -10);

    [Header("��������")]
    public float zoomSpeed = 4f;
    public float minZoom = 2f;
    public float maxZoom = 8f;

    [Header("��ק����")]
    public float dragSpeed = 2f;

    [Header("UI ����")]
    [Tooltip("�������Ǹ����������ӽǵ�UI��ť")]
    public Button resetViewButton;

    private Camera cam;
    private Vector3 dragOrigin;
    private bool isFollowingPlayer = true;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("CameraFollow�ű���Ҫ�����ڴ���Camera����Ķ����ϣ�");
            this.enabled = false;
            return;
        }

        if (resetViewButton != null)
        {
            resetViewButton.onClick.AddListener(ResetToPlayerView);
        }

        // ��ʼ����ť״̬
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
            Debug.Log("�����������Ϊ�������ģʽ��");
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
                Debug.Log("������л����ֶ���קģʽ��");
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
    /// ���������ӽǰ�ť����ʾ״̬
    /// </summary>
    private void UpdateResetButtonState()
    {
        if (resetViewButton != null)
        {
            // *** ���ĸĶ������� ***
            // ����������������ʱ (isFollowingPlayerΪfalse)����ʾ��ť��
            // ����������ڸ������ʱ (isFollowingPlayerΪtrue)�����ذ�ť��
            resetViewButton.gameObject.SetActive(!isFollowingPlayer);
        }
    }
}