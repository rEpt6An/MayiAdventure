// PlayerMarker.cs

using System.Collections;
using UnityEngine;

public class PlayerMarker : MonoBehaviour
{
    [Header("�ƶ�����")]
    [Tooltip("��Ҵ�һ���ؿ��ƶ�����һ���ؿ������ʱ��")]
    public float moveDuration = 0.5f;

    // һ��״̬������ֹ���ƶ������н����µ��ƶ�ָ��
    private bool isMoving = false;

    /// <summary>
    /// �������������ⲿ�����������ƶ�
    /// </summary>
    /// <param name="targetPosition">Ҫ�ƶ�����Ŀ����������</param>
    public IEnumerator MoveTo(Vector3 targetPosition)
    {
        if (isMoving)
        {
            yield break; // ��������ƶ���������µ�ָ��
        }

        isMoving = true;

        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            // ʹ�� Mathf.SmoothStep ����ʵ��һ��ƽ���ġ����л��뻺�����ƶ�Ч��
            float t = elapsedTime / moveDuration;
            transform.position = Vector3.Lerp(startPosition, targetPosition, Mathf.SmoothStep(0.0f, 1.0f, t));

            elapsedTime += Time.deltaTime;
            yield return null; // �ȴ���һ֡
        }

        // ȷ�����վ�ȷ����Ŀ��λ��
        transform.position = targetPosition;

        isMoving = false;
    }
}