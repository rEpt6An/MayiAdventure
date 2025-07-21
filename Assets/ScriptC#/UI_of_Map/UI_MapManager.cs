// UI_MapManager.cs

using UnityEngine;
using UnityEngine.UI;

public class UI_MapManager : MonoBehaviour
{
    [Header("�������")]
    public GameObject bagPanelObject;
    public GameObject shopPanelObject;
    public GameObject statsPanelObject; // *** ����: ����������� ***

    [Header("ȫ������")]
    public GameObject blockerCanvasObject;

    [Header("���ư�ť")]
    public Button openBagButton;
    public Button openShopButton;
    public Button openStatsButton; // *** ����: ������尴ť���� ***

    void Start()
    {
        // ��ʼʱȷ�������������ֶ��ǹرյ�
        bagPanelObject?.SetActive(false);
        shopPanelObject?.SetActive(false);
        statsPanelObject?.SetActive(false); // *** ���� ***
        blockerCanvasObject?.SetActive(false);

        openBagButton?.onClick.AddListener(ToggleBagPanel);
        openShopButton?.onClick.AddListener(ToggleShopPanel);
        openStatsButton?.onClick.AddListener(ToggleStatsPanel); // *** ���� ***
    }

    // ������Ҫһ��ͳһ�Ĺرշ��������������Ĺرհ�ť����
    public void CloseAllPopups()
    {
        bagPanelObject?.SetActive(false);
        shopPanelObject?.SetActive(false);
        statsPanelObject?.SetActive(false); // *** ���� ***
        blockerCanvasObject?.SetActive(false);
    }

    public void ToggleBagPanel()
    {
        // �ȹر������������
        shopPanelObject?.SetActive(false);
        statsPanelObject?.SetActive(false);

        bool isActive = bagPanelObject.activeSelf;
        bagPanelObject.SetActive(!isActive);
        blockerCanvasObject?.SetActive(!isActive);
    }

    public void ToggleShopPanel()
    {
        bagPanelObject?.SetActive(false);
        statsPanelObject?.SetActive(false);

        bool isActive = shopPanelObject.activeSelf;
        shopPanelObject.SetActive(!isActive);
        blockerCanvasObject?.SetActive(!isActive);
    }

    // *** ����: �л��������ķ��� ***
    public void ToggleStatsPanel()
    {
        bagPanelObject?.SetActive(false);
        shopPanelObject?.SetActive(false);

        bool isActive = statsPanelObject.activeSelf;
        statsPanelObject.SetActive(!isActive);
        blockerCanvasObject?.SetActive(!isActive);
    }
}