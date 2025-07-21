// Battle.cs

using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class Battle : MonoBehaviour
{
    [Header("战斗角色引用")]
    public Character attacker; // 玩家
    public Character defender; // 敌人

    [Header("战斗参数")]
    public float speedMultiplier = 1f;
    public float attackMoveDistance = 30f;
    public float attackMoveDuration = 0.15f;
    public float returnDuration = 0.1f;
    public float damageHeightOffset = -20f;
    public float damageRandomRange = 15f;

    [Header("结束事件")]
    public UnityEvent<bool> OnBattleEnd = new UnityEvent<bool>();

    [Header("数据源 (运行时)")]
    [SerializeField] private PlayerData runtimePlayerData;
    [SerializeField] private GameSessionData runtimeSessionData;

    // --- 为战斗双方创建数据实例 ---
    private PlayerData attackerDataInstance;
    private PlayerData defenderDataInstance;

    private float attackerTimer = 0f;
    private float defenderTimer = 0f;
    private bool isBattleActive = false;

    void Awake()
    {
        // --- 1. 为玩家创建数据实例 ---
        if (attacker != null && runtimePlayerData != null)
        {
            attackerDataInstance = Instantiate(runtimePlayerData);
            attacker.characterData = attackerDataInstance;
        }

        // --- 2. 为敌人创建数据实例 ---
        PlayerData enemyTemplate = runtimeSessionData?.nextEnemy;
        if (defender != null && enemyTemplate != null)
        {
            defenderDataInstance = Instantiate(enemyTemplate);
            defender.characterData = defenderDataInstance;
        }
    }

    void Start()
    {
        if (attacker == null || defender == null || defender.characterData == null)
        {
            Debug.LogError("战斗无法开始，缺少关键角色或数据！");
            this.enabled = false;
            return;
        }
        // UI的初始刷新由 UIcontrol 的 SetUIState_PreBattle 负责
    }

    public void StartBattle()
    {
        if (isBattleActive) return;
        isBattleActive = true;
        attackerTimer = 0f;
        defenderTimer = 0f;
        Debug.Log("战斗正式开始！");
    }

    void Update()
    {
        if (!isBattleActive) return;

        // 检查回蓝是否导致了数据变化
        bool statsChanged = HandleManaRegeneration();

        // 玩家攻击逻辑
        if (attackerDataInstance != null && attackerDataInstance.currentHP > 0)
        {
            attackerTimer += Time.deltaTime * speedMultiplier;
            if (attackerTimer >= GetAttackInterval(attackerDataInstance) && !attacker.isMoving)
            {
                StartCoroutine(PerformAttackSequence(attacker, defender));
                attackerTimer = 0f;
            }
        }

        // 敌人攻击逻辑
        if (defenderDataInstance != null && defenderDataInstance.currentHP > 0)
        {
            defenderTimer += Time.deltaTime * speedMultiplier;
            if (defenderTimer >= GetAttackInterval(defenderDataInstance) && !defender.isMoving)
            {
                StartCoroutine(PerformAttackSequence(defender, attacker));
                defenderTimer = 0f;
            }
        }

        // 如果回蓝导致了MP整数位的变化，就刷新UI
        if (statsChanged)
        {
            attacker?.UpdateAllVisuals();
            defender?.UpdateAllVisuals();
        }

        // 检查战斗结束条件
        if (attackerDataInstance.currentHP <= 0 || defenderDataInstance.currentHP <= 0)
        {
            EndBattle();
        }
    }

    private bool HandleManaRegeneration()
    {
        bool changed = false;
        if (attackerDataInstance != null && attackerDataInstance.currentHP > 0 && attackerDataInstance.currentMP < attackerDataInstance.maxMP)
        {
            float oldMp = attackerDataInstance.currentMP;
            float mpToRegen = attackerDataInstance.MP_speed * Time.deltaTime;
            attackerDataInstance.currentMP = Mathf.Min(attackerDataInstance.maxMP, attackerDataInstance.currentMP + mpToRegen);
            if (Mathf.FloorToInt(oldMp) != Mathf.FloorToInt(attackerDataInstance.currentMP)) changed = true;
        }
        if (defenderDataInstance != null && defenderDataInstance.currentHP > 0 && defenderDataInstance.currentMP < defenderDataInstance.maxMP)
        {
            float oldMp = defenderDataInstance.currentMP;
            float mpToRegen = defenderDataInstance.MP_speed * Time.deltaTime;
            defenderDataInstance.currentMP = Mathf.Min(defenderDataInstance.maxMP, defenderDataInstance.currentMP + mpToRegen);
            if (Mathf.FloorToInt(oldMp) != Mathf.FloorToInt(defenderDataInstance.currentMP)) changed = true;
        }
        return changed;
    }

    private float GetAttackInterval(PlayerData data)
    {
        if (data == null) return float.MaxValue;
        return 1f / Mathf.Max(0.1f, data.Act_speed);
    }

    public void EndBattle()
    {
        if (!isBattleActive) return;
        isBattleActive = false;
        Debug.Log("战斗结束！");

        // *** 核心修复 3: 只同步战斗中会发生变化的核心状态 ***
        // 金币、装备等由战利品UI和地图UI直接修改全局数据，这里不再干涉
        runtimePlayerData.currentHP = attackerDataInstance.currentHP;
        runtimePlayerData.currentMP = 0; // 战斗结束蓝量清零

        bool playerWon = defenderDataInstance.currentHP <= 0;
        OnBattleEnd?.Invoke(playerWon);
    }

    private IEnumerator PerformAttackSequence(Character currentAttacker, Character currentDefender)
    {
        currentAttacker.isMoving = true;
        yield return StartCoroutine(PerformAttackMovement(currentAttacker.transform));

        if (currentDefender.characterData.currentHP > 0)
        {
            PerformAttack(currentAttacker, currentDefender);
            // 攻击后立即更新双方UI
            attacker?.UpdateAllVisuals();
            defender?.UpdateAllVisuals();
        }
        currentAttacker.isMoving = false;
    }

    private IEnumerator PerformAttackMovement(Transform attackerTransform)
    {
        RectTransform rectTransform = attackerTransform.GetComponent<RectTransform>();
        Vector2 originalPosition = (rectTransform != null) ? rectTransform.anchoredPosition : (Vector2)attackerTransform.position;
        Vector2 moveDirection = (attackerTransform.localScale.x > 0) ? Vector2.right : Vector2.left;
        Vector2 targetPosition = originalPosition + moveDirection * attackMoveDistance;
        float elapsed = 0f;
        while (elapsed < attackMoveDuration)
        {
            Vector2 newPos = Vector2.Lerp(originalPosition, targetPosition, elapsed / attackMoveDuration);
            if (rectTransform != null) rectTransform.anchoredPosition = newPos; else attackerTransform.position = newPos;
            elapsed += Time.deltaTime;
            yield return null;
        }
        if (rectTransform != null) rectTransform.anchoredPosition = targetPosition; else attackerTransform.position = targetPosition;
        yield return new WaitForSeconds(0.05f);
        elapsed = 0f;
        while (elapsed < returnDuration)
        {
            Vector2 newPos = Vector2.Lerp(targetPosition, originalPosition, elapsed / returnDuration);
            if (rectTransform != null) rectTransform.anchoredPosition = newPos; else attackerTransform.position = newPos;
            elapsed += Time.deltaTime;
            yield return null;
        }
        if (rectTransform != null) rectTransform.anchoredPosition = originalPosition; else attackerTransform.position = originalPosition;
    }

    private void PerformAttack(Character cAttacker, Character cDefender)
    {
        // ... (闪避和暴击检定逻辑不变) ...

        var attackerData = cAttacker.characterData;
        var defenderData = cDefender.characterData;

        if (Random.Range(0, 100) < defenderData.Miss)
        {
            ShowPopup(0, false, true, false, GetDefenderPosition(cDefender)); // 添加了 isHeal 参数
            return;
        }

        bool isCritical = Random.Range(0, 100) < attackerData.Critical_chance;
        int damage = CalculateDamage(attackerData, defenderData, isCritical);

        // 对敌人造成伤害
        cDefender.changeHP(-damage);
        // 显示红色的伤害数字
        ShowPopup(damage, isCritical, false, false, GetDefenderPosition(cDefender));

        // *** 核心改动: 应用吸血逻辑 ***
        if (attackerData.suckblood > 0)
        {
            // 计算吸血恢复量
            int healAmount = Mathf.FloorToInt(damage * (attackerData.suckblood / 100f));

            if (healAmount > 0)
            {
                Debug.Log($"{attackerData.characterName} 吸血恢复了 {healAmount} HP!");
                // 为攻击者恢复生命
                cAttacker.changeHP(healAmount);
                // 在攻击者头上，显示一个绿色的治疗数字
                ShowPopup(healAmount, false, false, true, GetAttackerPosition(cAttacker));
            }
        }
    }

    private int CalculateDamage(PlayerData cAttackerData, PlayerData cDefenderData, bool isCritical)
    {
        int baseDamage = cAttackerData.Act;
        int defense = cDefenderData.Def;
        int damage = Mathf.Max(1, baseDamage - defense);
        return isCritical ? Mathf.FloorToInt(damage * 1.5f) : damage;
    }

    // *** 核心改动: 修改 ShowPopup 以接收新的参数 ***
    private void ShowPopup(int amount, bool isCritical, bool isMiss, bool isHeal, Vector3 position)
    {
        if (DamagePopupPool.Instance == null) return;
        DamagePopup popup = DamagePopupPool.Instance.GetFromPool();
        if (popup != null)
        {
            popup.verticalOffset = damageHeightOffset;
            popup.horizontalRange = damageRandomRange;
            // 将新的 isHeal 参数传递给 ShowDamage 方法
            popup.ShowDamage(amount, isCritical, isMiss, isHeal, position);
        }
    }
    private Vector3 GetDefenderPosition(Character defender)
    {
        return defender.transform.position;
    }

    // *** 新增: 一个辅助方法来获取攻击者的位置 ***
    private Vector3 GetAttackerPosition(Character attacker)
    {
        // 通常和防御者的位置获取方式一样
        return attacker.transform.position;
    }
}