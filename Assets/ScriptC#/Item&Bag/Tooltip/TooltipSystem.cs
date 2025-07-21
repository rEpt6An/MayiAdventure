// TooltipSystem.cs

using UnityEngine;
using System.Collections;

public class TooltipSystem : MonoBehaviour
{
    public static TooltipSystem Instance { get; private set; }

    [Header("UI 引用")]
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

    // 这个方法由 TooltipTrigger 调用
    public void Show(ITooltipDataProvider dataProvider)
    {
        // 停止任何正在进行的显示或隐藏协程
        if (showCoroutine != null) StopCoroutine(showCoroutine);

        // 启动一个新的协程，延迟一小段时间再显示
        showCoroutine = StartCoroutine(ShowAfterDelay(dataProvider, 0.1f));
    }

    private IEnumerator ShowAfterDelay(ITooltipDataProvider dataProvider, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        // 在显示之前，再次确认Tooltip是否应该被显示（由Trigger的状态决定）
        tooltip.SetText(dataProvider.GetTooltipContent(), dataProvider.GetTooltipHeader());
        tooltip.Show();
    }

    // 这个方法由 TooltipTrigger 调用
    public void Hide()
    {
        // 停止任何正在进行的显示协程
        if (showCoroutine != null) StopCoroutine(showCoroutine);

        tooltip.Hide();
    }
}