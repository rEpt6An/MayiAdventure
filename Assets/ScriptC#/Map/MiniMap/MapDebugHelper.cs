// MapDebugHelper.cs

using UnityEngine;

public class MapDebugHelper : MonoBehaviour
{
    [Header("��������")]
    [Tooltip("�����е�С��ͼ�����")]
    public Camera minimapCamera;
    [Tooltip("������еؿ�ĸ����� (NodesContainer)")]
    public Transform mapContainer;

    [Header("ͼ������")]
    [Tooltip("��ȷ����ĵؿ�Ԥ�Ƽ����������ͼ����")]
    public LayerMask mapNodeLayer; // ����ʹ��LayerMask�����ַ�������ȫ

    [Header("����ѡ��")]
    [Tooltip("�ڳ�����ͼ�л���С��ͼ���������Ұ��Χ")]
    public bool drawCameraFrustum = true;

    void Start()
    {
        if (minimapCamera == null || mapContainer == null)
        {
            Debug.LogError("MapDebugHelper: ȱ��С��ͼ��������ͼ���������ã�");
            return;
        }

        ForceSetupMinimapCamera();
    }

    /// <summary>
    /// ǿ������С��ͼ����������йؼ�����
    /// </summary>
    [ContextMenu("Force Setup Minimap Camera Now")] // ������������Inspector���Ҽ�����ű����ֶ�ִ�д˷���
    public void ForceSetupMinimapCamera()
    {
        Debug.Log("--- ��ʼǿ������С��ͼ����� ---");

        // 1. ǿ������Culling Mask
        minimapCamera.cullingMask = mapNodeLayer;
        Debug.Log($"�����Culling Mask��ǿ������Ϊ: {LayerMask.LayerToName(Mathf.RoundToInt(Mathf.Log(mapNodeLayer.value, 2)))}");

        // 2. �����ͼ���ݵı߽�
        if (mapContainer.childCount == 0)
        {
            Debug.LogWarning("��ͼ������û���Ӷ��󣨵ؿ飩���޷�����߽硣��ȷ����ͼ�����ɡ�");
            return;
        }
        Bounds mapBounds = new Bounds(mapContainer.GetChild(0).position, Vector3.zero);
        foreach (Transform child in mapContainer)
        {
            mapBounds.Encapsulate(child.position);
        }
        Debug.Log($"������ĵ�ͼ�߽� ���ĵ�: {mapBounds.center}, ��С: {mapBounds.size}");

        // 3. ǿ�����������λ��
        minimapCamera.transform.position = new Vector3(mapBounds.center.x, mapBounds.center.y, -10f);
        Debug.Log($"�����λ����ǿ������Ϊ: {minimapCamera.transform.position}");

        // 4. ǿ�������������Ұ��С
        // Ϊ��ȷ���ܿ���������ͼ�����ǽ���Ұ��С����Ϊ�߽��Ⱥ͸߶��нϴ��һ����������һ��߾�
        float requiredSize = Mathf.Max(mapBounds.size.x, mapBounds.size.y) / 2f;
        requiredSize += 2f; // ����2����λ�ı߾�
        minimapCamera.orthographicSize = requiredSize;
        Debug.Log($"�����Orthographic Size��ǿ������Ϊ: {requiredSize}");

        Debug.Log("--- С��ͼ�����ǿ��������� ---");
    }

    // ��Scene��ͼ�л��Ƶ�����Ϣ
    void OnDrawGizmos()
    {
        if (drawCameraFrustum && minimapCamera != null)
        {
            Gizmos.color = Color.yellow;
            // ��ȡ���������׶�ǵ�
            Vector3[] frustumCorners = new Vector3[4];
            minimapCamera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), minimapCamera.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, frustumCorners);

            for (int i = 0; i < 4; i++)
            {
                var worldSpaceCorner = minimapCamera.transform.TransformVector(frustumCorners[i]) + minimapCamera.transform.position;
                Gizmos.DrawLine(minimapCamera.transform.position, worldSpaceCorner);
            }
        }
    }
}