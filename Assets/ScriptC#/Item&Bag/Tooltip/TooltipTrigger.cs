// TooltipTrigger.cs

using UnityEngine;
using UnityEngine.EventSystems;

// *** 核心修复: 重新实现 IPointerExitHandler ***
public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private ITooltipDataProvider dataProvider;

    public void SetDataProvider(ITooltipDataProvider provider)
    {
        dataProvider = provider;
    }

    // 当鼠标指针进入这个UI元素的区域时调用
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (dataProvider == null) return;

        TooltipSystem.Instance.Show(dataProvider);
    }

    // 当鼠标指针离开这个UI元素的区域时调用
    public void OnPointerExit(PointerEventData eventData)
    {
        // 无论如何，只要鼠标离开了，就命令系统隐藏Tooltip
        TooltipSystem.Instance.Hide();
    }

    // 当对象被禁用或销毁时，也确保Tooltip被隐藏
    private void OnDisable()
    {
        TooltipSystem.Instance.Hide();
    }
}