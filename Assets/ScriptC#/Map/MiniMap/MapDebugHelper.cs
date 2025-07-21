// MapDebugHelper.cs

using UnityEngine;

public class MapDebugHelper : MonoBehaviour
{
    [Header("核心引用")]
    [Tooltip("场景中的小地图摄像机")]
    public Camera minimapCamera;
    [Tooltip("存放所有地块的父对象 (NodesContainer)")]
    public Transform mapContainer;

    [Header("图层设置")]
    [Tooltip("请确保你的地块预制件设置在这个图层上")]
    public LayerMask mapNodeLayer; // 我们使用LayerMask，比字符串更安全

    [Header("调试选项")]
    [Tooltip("在场景视图中绘制小地图摄像机的视野范围")]
    public bool drawCameraFrustum = true;

    void Start()
    {
        if (minimapCamera == null || mapContainer == null)
        {
            Debug.LogError("MapDebugHelper: 缺少小地图摄像机或地图容器的引用！");
            return;
        }

        ForceSetupMinimapCamera();
    }

    /// <summary>
    /// 强制设置小地图摄像机的所有关键参数
    /// </summary>
    [ContextMenu("Force Setup Minimap Camera Now")] // 这会让你可以在Inspector里右键点击脚本，手动执行此方法
    public void ForceSetupMinimapCamera()
    {
        Debug.Log("--- 开始强制设置小地图摄像机 ---");

        // 1. 强制设置Culling Mask
        minimapCamera.cullingMask = mapNodeLayer;
        Debug.Log($"摄像机Culling Mask已强制设置为: {LayerMask.LayerToName(Mathf.RoundToInt(Mathf.Log(mapNodeLayer.value, 2)))}");

        // 2. 计算地图内容的边界
        if (mapContainer.childCount == 0)
        {
            Debug.LogWarning("地图容器中没有子对象（地块），无法计算边界。请确保地图已生成。");
            return;
        }
        Bounds mapBounds = new Bounds(mapContainer.GetChild(0).position, Vector3.zero);
        foreach (Transform child in mapContainer)
        {
            mapBounds.Encapsulate(child.position);
        }
        Debug.Log($"计算出的地图边界 中心点: {mapBounds.center}, 大小: {mapBounds.size}");

        // 3. 强制设置摄像机位置
        minimapCamera.transform.position = new Vector3(mapBounds.center.x, mapBounds.center.y, -10f);
        Debug.Log($"摄像机位置已强制设置为: {minimapCamera.transform.position}");

        // 4. 强制设置摄像机视野大小
        // 为了确保能看到整个地图，我们将视野大小设置为边界宽度和高度中较大的一个，并增加一点边距
        float requiredSize = Mathf.Max(mapBounds.size.x, mapBounds.size.y) / 2f;
        requiredSize += 2f; // 增加2个单位的边距
        minimapCamera.orthographicSize = requiredSize;
        Debug.Log($"摄像机Orthographic Size已强制设置为: {requiredSize}");

        Debug.Log("--- 小地图摄像机强制设置完成 ---");
    }

    // 在Scene视图中绘制调试信息
    void OnDrawGizmos()
    {
        if (drawCameraFrustum && minimapCamera != null)
        {
            Gizmos.color = Color.yellow;
            // 获取摄像机的视锥角点
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