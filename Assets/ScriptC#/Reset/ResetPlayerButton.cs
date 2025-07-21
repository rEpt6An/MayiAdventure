// ResetPlayerButton.cs

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ResetPlayerButton : MonoBehaviour
{
    private Button resetButton;

    void Start()
    {
        resetButton = GetComponent<Button>();
        resetButton.onClick.AddListener(HandleResetClick);
    }

    private void HandleResetClick()
    {
        if (ResetPlayer.Instance != null)
        {
            Debug.Log("���԰�ť����������ڿ�ʼ����Ϸ...");

            // *** �����޸�: ������ȷ�ķ����� ***
            // ResetPlayer.Instance.ResetPlayerData(); // �ɵġ������ڵķ���
            ResetPlayer.Instance.StartNewGame();   // �µġ���ȷ�ķ���
        }
        else
        {
            Debug.LogError("�޷��ҵ�ResetPlayer��ʵ����");
        }
    }
}