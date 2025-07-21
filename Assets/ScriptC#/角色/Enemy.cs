// Enemy.cs

using UnityEngine;

// 这个脚本确保当一个敌人在战斗场景中被加载时，它的生命值会被重置为满值。
[RequireComponent(typeof(Character))] // 确保这个对象一定有Character组件
public class Enemy : MonoBehaviour
{
    private Character character;

    void Awake()
    {
        character = GetComponent<Character>();
        if (character.characterData != null)
        {
            // 将敌人的生命值和魔法值恢复到满值，以准备本次战斗。
            // 这不会影响玩家的数据，因为玩家的游戏对象上没有挂这个脚本。
            character.characterData.currentHP = character.characterData.maxHP;
            character.characterData.currentMP = character.characterData.maxMP;
            Debug.Log(gameObject.name + " 的血量已在战斗开始时自动重置。");
        }
    }
}