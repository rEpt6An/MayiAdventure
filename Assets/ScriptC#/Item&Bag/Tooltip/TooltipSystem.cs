// TooltipSystem.cs

using UnityEngine;
using System.Collections;

public class TooltipSystem : MonoBehaviour
{
    public static TooltipSystem Instance { get; private set; }

    [Header("UI ����")]
    public Tooltip tooltip;

    private Coroutine showCoroutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ��������� TooltipTrigger ����
    public void Show(ITooltipDataProvider dataProvider)
    {
        // ֹͣ�κ����ڽ��е���ʾ������Э��
        if (showCoroutine != null) StopCoroutine(showCoroutine);

        // ����һ���µ�Э�̣��ӳ�һС��ʱ������ʾ
        showCoroutine = StartCoroutine(ShowAfterDelay(dataProvider, 0.1f));
    }

    private IEnumerator ShowAfterDelay(ITooltipDataProvider dataProvider, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        // ����ʾ֮ǰ���ٴ�ȷ��Tooltip�Ƿ�Ӧ�ñ���ʾ����Trigger��״̬������
        tooltip.SetText(dataProvider.GetTooltipContent(), dataProvider.GetTooltipHeader());
        tooltip.Show();
    }

    // ��������� TooltipTrigger ����
    public void Hide()
    {
        // ֹͣ�κ����ڽ��е���ʾЭ��
        if (showCoroutine != null) StopCoroutine(showCoroutine);

        tooltip.Hide();
    }
}