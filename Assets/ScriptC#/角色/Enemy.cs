// Enemy.cs

using UnityEngine;

// ����ű�ȷ����һ��������ս�������б�����ʱ����������ֵ�ᱻ����Ϊ��ֵ��
[RequireComponent(typeof(Character))] // ȷ���������һ����Character���
public class Enemy : MonoBehaviour
{
    private Character character;

    void Awake()
    {
        character = GetComponent<Character>();
        if (character.characterData != null)
        {
            // �����˵�����ֵ��ħ��ֵ�ָ�����ֵ����׼������ս����
            // �ⲻ��Ӱ����ҵ����ݣ���Ϊ��ҵ���Ϸ������û�й�����ű���
            character.characterData.currentHP = character.characterData.maxHP;
            character.characterData.currentMP = character.characterData.maxMP;
            Debug.Log(gameObject.name + " ��Ѫ������ս����ʼʱ�Զ����á�");
        }
    }
}