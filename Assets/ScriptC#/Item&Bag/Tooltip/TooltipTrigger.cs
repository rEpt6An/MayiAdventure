// TooltipTrigger.cs

using UnityEngine;
using UnityEngine.EventSystems;

// *** �����޸�: ����ʵ�� IPointerExitHandler ***
public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private ITooltipDataProvider dataProvider;

    public void SetDataProvider(ITooltipDataProvider provider)
    {
        dataProvider = provider;
    }

    // �����ָ��������UIԪ�ص�����ʱ����
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (dataProvider == null) return;

        TooltipSystem.Instance.Show(dataProvider);
    }

    // �����ָ���뿪���UIԪ�ص�����ʱ����
    public void OnPointerExit(PointerEventData eventData)
    {
        // ������Σ�ֻҪ����뿪�ˣ�������ϵͳ����Tooltip
        TooltipSystem.Instance.Hide();
    }

    // �����󱻽��û�����ʱ��Ҳȷ��Tooltip������
    private void OnDisable()
    {
        TooltipSystem.Instance.Hide();
    }
}