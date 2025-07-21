using UnityEngine;
using System.Collections;

public class AttackMovement : MonoBehaviour
{
    [Header("位移设置")]
    public float forwardDistance = 1.5f;  // 向前移动距离
    public float forwardDuration = 0.15f; // 向前移动时间（更短更迅速）
    public float returnDuration = 0.1f;   // 返回原位时间（更短）
    public float attackDelay = 0.1f;      // 攻击动作延迟时间

    private Vector3 originalPosition;     // 原始位置
    private Transform myTransform;        // 缓存Transform组件
    private bool isAttacking = false;     // 是否正在攻击

    void Awake()
    {
        // 缓存Transform组件提高性能
        myTransform = transform;

        // 保存原始位置
        originalPosition = myTransform.position;
    }

    // 执行攻击位移
    public IEnumerator PerformAttackMovement()
    {
        if (isAttacking) yield break;

        isAttacking = true;

        // 保存当前准确位置
        Vector3 currentPosition = myTransform.position;
        originalPosition = currentPosition;

        // 1. 向前冲刺
        Vector3 targetPosition = currentPosition + myTransform.forward * forwardDistance;
        float elapsed = 0f;

        while (elapsed < forwardDuration)
        {
            float t = elapsed / forwardDuration;
            myTransform.position = Vector3.Lerp(currentPosition, targetPosition, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 确保到达目标位置
        myTransform.position = targetPosition;

        // 2. 短暂停留（攻击动作发生点）
        yield return new WaitForSeconds(attackDelay);

        // 3. 快速返回原位
        elapsed = 0f;
        Vector3 startPosition = myTransform.position;

        while (elapsed < returnDuration)
        {
            float t = elapsed / returnDuration;
            myTransform.position = Vector3.Lerp(startPosition, originalPosition, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 确保返回原位
        myTransform.position = originalPosition;

        isAttacking = false;
    }
}