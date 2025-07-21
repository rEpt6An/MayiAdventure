// UI_MapManager.cs

using UnityEngine;
using UnityEngine.UI;

public class UI_MapManager : MonoBehaviour
{
    [Header("面板引用")]
    public GameObject bagPanelObject;
    public GameObject shopPanelObject;
    public GameObject statsPanelObject; // *** 新增: 属性面板引用 ***

    [Header("全局遮罩")]
    public GameObject blockerCanvasObject;

    [Header("控制按钮")]
    public Button openBagButton;
    public Button openShopButton;
    public Button openStatsButton; // *** 新增: 属性面板按钮引用 ***

    void Start()
    {
        // 初始时确保所有面板和遮罩都是关闭的
        bagPanelObject?.SetActive(false);
        shopPanelObject?.SetActive(false);
        statsPanelObject?.SetActive(false); // *** 新增 ***
        blockerCanvasObject?.SetActive(false);

        openBagButton?.onClick.AddListener(ToggleBagPanel);
        openShopButton?.onClick.AddListener(ToggleShopPanel);
        openStatsButton?.onClick.AddListener(ToggleStatsPanel); // *** 新增 ***
    }

    // 我们需要一个统一的关闭方法，供所有面板的关闭按钮调用
    public void CloseAllPopups()
    {
        bagPanelObject?.SetActive(false);
        shopPanelObject?.SetActive(false);
        statsPanelObject?.SetActive(false); // *** 新增 ***
        blockerCanvasObject?.SetActive(false);
    }

    public void ToggleBagPanel()
    {
        // 先关闭所有其他面板
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

    // *** 新增: 切换属性面板的方法 ***
    public void ToggleStatsPanel()
    {
        bagPanelObject?.SetActive(false);
        shopPanelObject?.SetActive(false);

        bool isActive = statsPanelObject.activeSelf;
        statsPanelObject.SetActive(!isActive);
        blockerCanvasObject?.SetActive(!isActive);
    }
}