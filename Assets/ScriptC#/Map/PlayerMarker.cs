// PlayerMarker.cs

using System.Collections;
using UnityEngine;

public class PlayerMarker : MonoBehaviour
{
    [Header("移动设置")]
    [Tooltip("玩家从一个地块移动到另一个地块所需的时间")]
    public float moveDuration = 0.5f;

    // 一个状态锁，防止在移动过程中接收新的移动指令
    private bool isMoving = false;

    /// <summary>
    /// 公共方法，供外部调用以启动移动
    /// </summary>
    /// <param name="targetPosition">要移动到的目标世界坐标</param>
    public IEnumerator MoveTo(Vector3 targetPosition)
    {
        if (isMoving)
        {
            yield break; // 如果正在移动，则忽略新的指令
        }

        isMoving = true;

        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            // 使用 Mathf.SmoothStep 可以实现一个平滑的、带有缓入缓出的移动效果
            float t = elapsedTime / moveDuration;
            transform.position = Vector3.Lerp(startPosition, targetPosition, Mathf.SmoothStep(0.0f, 1.0f, t));

            elapsedTime += Time.deltaTime;
            yield return null; // 等待下一帧
        }

        // 确保最终精确到达目标位置
        transform.position = targetPosition;

        isMoving = false;
    }
}