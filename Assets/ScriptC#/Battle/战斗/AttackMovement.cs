using UnityEngine;
using System.Collections;

public class AttackMovement : MonoBehaviour
{
    [Header("λ������")]
    public float forwardDistance = 1.5f;  // ��ǰ�ƶ�����
    public float forwardDuration = 0.15f; // ��ǰ�ƶ�ʱ�䣨���̸�Ѹ�٣�
    public float returnDuration = 0.1f;   // ����ԭλʱ�䣨���̣�
    public float attackDelay = 0.1f;      // ���������ӳ�ʱ��

    private Vector3 originalPosition;     // ԭʼλ��
    private Transform myTransform;        // ����Transform���
    private bool isAttacking = false;     // �Ƿ����ڹ���

    void Awake()
    {
        // ����Transform����������
        myTransform = transform;

        // ����ԭʼλ��
        originalPosition = myTransform.position;
    }

    // ִ�й���λ��
    public IEnumerator PerformAttackMovement()
    {
        if (isAttacking) yield break;

        isAttacking = true;

        // ���浱ǰ׼ȷλ��
        Vector3 currentPosition = myTransform.position;
        originalPosition = currentPosition;

        // 1. ��ǰ���
        Vector3 targetPosition = currentPosition + myTransform.forward * forwardDistance;
        float elapsed = 0f;

        while (elapsed < forwardDuration)
        {
            float t = elapsed / forwardDuration;
            myTransform.position = Vector3.Lerp(currentPosition, targetPosition, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // ȷ������Ŀ��λ��
        myTransform.position = targetPosition;

        // 2. ����ͣ�����������������㣩
        yield return new WaitForSeconds(attackDelay);

        // 3. ���ٷ���ԭλ
        elapsed = 0f;
        Vector3 startPosition = myTransform.position;

        while (elapsed < returnDuration)
        {
            float t = elapsed / returnDuration;
            myTransform.position = Vector3.Lerp(startPosition, originalPosition, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // ȷ������ԭλ
        myTransform.position = originalPosition;

        isAttacking = false;
    }
}