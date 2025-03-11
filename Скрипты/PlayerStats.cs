using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    [Header("Base Stats (Set in Inspector)")]
    public int baseMaxHealth = 1100;
    public int baseDefense = 0;
    public float baseStamina = 1100f;

    [Header("Health Settings")]
    public int health = 1100;
    public int maxHealth = 1100;
    public Image healthFill;
    public Text healthText;

    [Header("Stamina Settings")]
    public float stamina = 1100f;
    public float maxStamina = 1100f;

    [Header("UI Regen Texts")]
    public Text healthRegenText;
    public Text staminaRegenText;

    [Header("Potion Effect UI")]
    // UI изображения для отображения эффекта зелья (Filled)
    public Image healthPotionEffectImage;
    public Image staminaPotionEffectImage;

    // Вычисленные бонусные показатели
    public int computedDefense = 0;

    // Игрок может бегать, если выносливость > 0
    public bool CanRun { get { return stamina > 0f; } }

    // Константы восстановления (значения за секунду)
    private const float idleHealthRegen = 9f;
    private const float idleStaminaRegen = 35f;
    private const float movingHealthRegen = 5f;
    private const float movingStaminaRegen = 17f;
    private const float attackingHealthRegen = 2f;
    private const float attackingStaminaRegen = 9f;

    public enum PlayerState { Idle, Moving, Attacking }

    private Rigidbody rb;
    private PlayerController playerController;
    private CombatSystem combatSystem;

    // Таймер состояния атаки (5 сек после удара или получения урона)
    private float attackTimer = 0f;

    // Для эффекта зелья здоровья
    private float additionalHealthRegenFromPotion = 0f;
    private float healthPotionDurationRemaining = 0f;
    private float healthPotionTotalDuration = 0f;

    // Для эффекта зелья выносливости
    private float additionalStaminaRegenFromPotion = 0f;
    private float staminaPotionDurationRemaining = 0f;
    private float staminaPotionTotalDuration = 0f;

    // Переменные для отслеживания кулдауна зелья (15 сек)
    private float lastHealthPotionTime = -15f;
    private float lastStaminaPotionTime = -15f;

    void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody>();
        playerController = GetComponent<PlayerController>();
        combatSystem = GetComponent<CombatSystem>();
    }

    void Start()
    {
        RecalculateStats();
        health = maxHealth;
        stamina = maxStamina;
        UpdateHealthUI();
        if (healthPotionEffectImage != null)
            healthPotionEffectImage.gameObject.SetActive(false);
        if (staminaPotionEffectImage != null)
            staminaPotionEffectImage.gameObject.SetActive(false);
        StartCoroutine(RegenCoroutine());
    }

    void Update()
    {
        RecalculateStats();
        UpdateHealthUI();

        // Если персонаж атакует, сбрасываем таймер состояния атаки на 5 секунд
        if (combatSystem != null && combatSystem.IsAttacking)
        {
            attackTimer = 5f;
        }
        else if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer < 0f)
                attackTimer = 0f;
        }
    }

    public void RecalculateStats()
    {
        int prevMaxHealth = maxHealth;
        float prevMaxStamina = maxStamina;

        int bonusHealth = 0;
        int bonusDefense = 0;
        float bonusStamina = 0f;

        if (EquipmentManager.Instance != null)
        {
            if (EquipmentManager.Instance.equippedWeapon != null)
            {
                bonusHealth += EquipmentManager.Instance.equippedWeapon.bonusHealth;
                bonusDefense += EquipmentManager.Instance.equippedWeapon.bonusDefense;
                bonusStamina += EquipmentManager.Instance.equippedWeapon.bonusStamina;
            }
            if (EquipmentManager.Instance.equippedShield != null)
            {
                bonusHealth += EquipmentManager.Instance.equippedShield.bonusHealth;
                bonusDefense += EquipmentManager.Instance.equippedShield.bonusDefense;
                bonusStamina += EquipmentManager.Instance.equippedShield.bonusStamina;
            }
            if (EquipmentManager.Instance.equippedHelmet != null)
            {
                bonusHealth += EquipmentManager.Instance.equippedHelmet.bonusHealth;
                bonusDefense += EquipmentManager.Instance.equippedHelmet.bonusDefense;
                bonusStamina += EquipmentManager.Instance.equippedHelmet.bonusStamina;
            }
            if (EquipmentManager.Instance.equippedChest != null)
            {
                bonusHealth += EquipmentManager.Instance.equippedChest.bonusHealth;
                bonusDefense += EquipmentManager.Instance.equippedChest.bonusDefense;
                bonusStamina += EquipmentManager.Instance.equippedChest.bonusStamina;
            }
            if (EquipmentManager.Instance.equippedGloves != null)
            {
                bonusHealth += EquipmentManager.Instance.equippedGloves.bonusHealth;
                bonusDefense += EquipmentManager.Instance.equippedGloves.bonusDefense;
                bonusStamina += EquipmentManager.Instance.equippedGloves.bonusStamina;
            }
            if (EquipmentManager.Instance.equippedPants != null)
            {
                bonusHealth += EquipmentManager.Instance.equippedPants.bonusHealth;
                bonusDefense += EquipmentManager.Instance.equippedPants.bonusDefense;
                bonusStamina += EquipmentManager.Instance.equippedPants.bonusStamina;
            }
            if (EquipmentManager.Instance.equippedBoots != null)
            {
                bonusHealth += EquipmentManager.Instance.equippedBoots.bonusHealth;
                bonusDefense += EquipmentManager.Instance.equippedBoots.bonusDefense;
                bonusStamina += EquipmentManager.Instance.equippedBoots.bonusStamina;
            }
            if (EquipmentManager.Instance.equippedRing != null)
            {
                bonusHealth += EquipmentManager.Instance.equippedRing.bonusHealth;
                bonusDefense += EquipmentManager.Instance.equippedRing.bonusDefense;
                bonusStamina += EquipmentManager.Instance.equippedRing.bonusStamina;
            }
        }

        maxHealth = baseMaxHealth + bonusHealth;
        maxStamina = baseStamina + bonusStamina;
        computedDefense = baseDefense + bonusDefense;

        int healthDiff = maxHealth - prevMaxHealth;
        float staminaDiff = maxStamina - prevMaxStamina;
        health += healthDiff;
        stamina += staminaDiff;

        if (health > maxHealth) health = maxHealth;
        if (stamina > maxStamina) stamina = maxStamina;
    }

    // Определяем текущее состояние персонажа (приоритет состояния атаки)
    public PlayerState GetCurrentPlayerState()
    {
        if (attackTimer > 0f)
            return PlayerState.Attacking;

        Vector3 horizontalVelocity = rb.linearVelocity;
        horizontalVelocity.y = 0;
        if (horizontalVelocity.magnitude > 0.1f)
            return PlayerState.Moving;

        return PlayerState.Idle;
    }

    // Корутина, выполняющая обновление ресурсов каждую секунду
    private IEnumerator RegenCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            // Если персонаж мёртв, пропускаем обновление
            if (playerController != null && playerController.IsDead)
                continue;

            int healthRegenAmount = 0;
            float staminaChangeAmount = 0f;

            // Если игрок бежит, расход выносливости происходит моментально
            if (playerController != null && playerController.IsRunning)
            {
                healthRegenAmount = Mathf.RoundToInt(movingHealthRegen);
                staminaChangeAmount = -playerController.runStaminaCost;
            }
            else
            {
                PlayerState state = GetCurrentPlayerState();
                switch (state)
                {
                    case PlayerState.Idle:
                        healthRegenAmount = Mathf.RoundToInt(idleHealthRegen);
                        staminaChangeAmount = idleStaminaRegen;
                        break;
                    case PlayerState.Attacking:
                        healthRegenAmount = Mathf.RoundToInt(attackingHealthRegen);
                        staminaChangeAmount = attackingStaminaRegen;
                        break;
                    case PlayerState.Moving:
                        healthRegenAmount = Mathf.RoundToInt(movingHealthRegen);
                        staminaChangeAmount = movingStaminaRegen;
                        break;
                }
            }

            // Применяем эффект зелья здоровья (ежесекундное восстановление)
            if (healthPotionDurationRemaining > 0f)
            {
                healthRegenAmount += Mathf.RoundToInt(additionalHealthRegenFromPotion);
                healthPotionDurationRemaining -= 1f;
                if (healthPotionDurationRemaining <= 0f)
                {
                    healthPotionDurationRemaining = 0f;
                    additionalHealthRegenFromPotion = 0f;
                    if (healthPotionEffectImage != null)
                        healthPotionEffectImage.gameObject.SetActive(false);
                }
                else if (healthPotionEffectImage != null)
                {
                    healthPotionEffectImage.fillAmount = healthPotionDurationRemaining / healthPotionTotalDuration;
                }
            }

            // Применяем эффект зелья выносливости (ежесекундное восстановление)
            if (staminaPotionDurationRemaining > 0f)
            {
                staminaChangeAmount += additionalStaminaRegenFromPotion;
                staminaPotionDurationRemaining -= 1f;
                if (staminaPotionDurationRemaining <= 0f)
                {
                    staminaPotionDurationRemaining = 0f;
                    additionalStaminaRegenFromPotion = 0f;
                    if (staminaPotionEffectImage != null)
                        staminaPotionEffectImage.gameObject.SetActive(false);
                }
                else if (staminaPotionEffectImage != null)
                {
                    staminaPotionEffectImage.fillAmount = staminaPotionDurationRemaining / staminaPotionTotalDuration;
                }
            }

            // Применяем восстановление для здоровья
            if (health < maxHealth)
            {
                health += healthRegenAmount;
                if (health > maxHealth)
                    health = maxHealth;
            }

            // Применяем изменение выносливости
            if (staminaChangeAmount >= 0)
            {
                if (stamina < maxStamina)
                {
                    stamina += staminaChangeAmount;
                    if (stamina > maxStamina)
                        stamina = maxStamina;
                }
            }
            else
            {
                stamina += staminaChangeAmount;
                if (stamina < 0)
                    stamina = 0;
            }

            // Обновляем UI для здоровья
            if (health < maxHealth)
            {
                if (healthRegenText != null)
                {
                    healthRegenText.gameObject.SetActive(true);
                    healthRegenText.text = "+" + healthRegenAmount.ToString();
                }
            }
            else
            {
                if (healthRegenText != null)
                    healthRegenText.gameObject.SetActive(false);
            }

            // Обновляем UI для выносливости:
            // Если игрок бежит, всегда отображаем расход выносливости за секунду,
            // даже если текущий запас выносливости полон.
            if (playerController != null && playerController.IsRunning)
            {
                if (staminaRegenText != null)
                {
                    staminaRegenText.gameObject.SetActive(true);
                    // Отображаем расход выносливости (отрицательное значение) с указанием " / sec"
                    staminaRegenText.text = ((int)staminaChangeAmount).ToString();
                }
            }
            else if (stamina < maxStamina && staminaChangeAmount > 0)
            {
                if (staminaRegenText != null)
                {
                    staminaRegenText.gameObject.SetActive(true);
                    staminaRegenText.text = "+" + ((int)staminaChangeAmount).ToString();
                }
            }
            else
            {
                if (staminaRegenText != null)
                    staminaRegenText.gameObject.SetActive(false);
            }

            UpdateHealthUI();
        }
    }

    // Для мгновенного отображения изменения выносливости (например, при перекате)
    public void ShowInstantStaminaChange(float amount)
    {
        if (staminaRegenText != null)
        {
            StopCoroutine("ShowInstantStaminaChangeCoroutine");
            StartCoroutine(ShowInstantStaminaChangeCoroutine(amount));
        }
    }

    private IEnumerator ShowInstantStaminaChangeCoroutine(float amount)
    {
        staminaRegenText.gameObject.SetActive(true);
        staminaRegenText.text = (amount >= 0 ? "+" : "") + ((int)amount).ToString();
        yield return new WaitForSeconds(1f);
        staminaRegenText.gameObject.SetActive(false);
    }

    public bool HasStamina(float amount)
    {
        return stamina >= amount;
    }

    public void ConsumeStamina(float amount)
    {
        stamina = Mathf.Max(stamina - amount, 0f);
    }

    public void TakeDamage(int damage)
    {
        int finalDamage = Mathf.Max(damage - computedDefense, 0);
        health -= finalDamage;
        if (health < 0)
            health = 0;

        attackTimer = 5f;
        UpdateHealthUI();

        if (health <= 0)
            Die();
    }

    public void Heal(int amount)
    {
        health += amount;
        if (health > maxHealth)
            health = maxHealth;
        UpdateHealthUI();
    }

    public void RestoreHP(int amount)
    {
        Heal(amount);
    }

    public void RestoreSP(int amount)
    {
        stamina += amount;
        if (stamina > maxStamina)
            stamina = maxStamina;
    }

    private void UpdateHealthUI()
    {
        if (healthFill != null)
            healthFill.fillAmount = (float)health / maxHealth;
        if (healthText != null)
            healthText.text = health.ToString();
    }

    private void Die()
    {
        Debug.Log("Player died!");
        if (PlayerController.Instance != null)
            PlayerController.Instance.Die();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    // При возрождении устанавливаем ресурсы на 33% от максимума
    public void ResetResourcesAfterRespawn()
    {
        health = Mathf.RoundToInt(maxHealth * 0.33f);
        stamina = maxStamina * 0.33f;
        UpdateHealthUI();
    }

    // Обновлённый метод применения зелья:
    // Если здоровье (для HP) или мана (для SP) на максимуме, зелье не применяется.
    // Также проверяется кулдаун в 15 секунд.
    public bool UsePotion(Equipment potion)
    {
        if (potion.type != EquipmentType.Potion)
            return false;

        if (potion.potionEffect == PotionEffectType.HP)
        {
            if (health >= maxHealth)
                return false;
            if (Time.time < lastHealthPotionTime + 15f)
                return false;
            lastHealthPotionTime = Time.time;
            UseHealthPotion(potion.restoreAmount, potion.restoreDuration);
            return true;
        }
        else if (potion.potionEffect == PotionEffectType.SP)
        {
            if (stamina >= maxStamina)
                return false;
            if (Time.time < lastStaminaPotionTime + 15f)
                return false;
            lastStaminaPotionTime = Time.time;
            UseStaminaPotion(potion.restoreAmount, potion.restoreDuration);
            return true;
        }
        return false;
    }

    // Геттеры для времени последнего использования зелья (для UI панели быстрого доступа)
    public float GetLastHealthPotionTime()
    {
        return lastHealthPotionTime;
    }

    public float GetLastStaminaPotionTime()
    {
        return lastStaminaPotionTime;
    }

    // Метод для применения зелья здоровья: рассчитывает восстановление за тик и активирует UI эффекта
    public void UseHealthPotion(int restoreAmount, float restoreDuration)
    {
        additionalHealthRegenFromPotion = (float)restoreAmount / restoreDuration;
        healthPotionDurationRemaining = restoreDuration;
        healthPotionTotalDuration = restoreDuration;
        if (healthPotionEffectImage != null)
        {
            healthPotionEffectImage.gameObject.SetActive(true);
            healthPotionEffectImage.fillAmount = 1f;
        }
    }

    // Метод для применения зелья выносливости: рассчитывает восстановление за тик и активирует UI эффекта
    public void UseStaminaPotion(int restoreAmount, float restoreDuration)
    {
        additionalStaminaRegenFromPotion = (float)restoreAmount / restoreDuration;
        staminaPotionDurationRemaining = restoreDuration;
        staminaPotionTotalDuration = restoreDuration;
        if (staminaPotionEffectImage != null)
        {
            staminaPotionEffectImage.gameObject.SetActive(true);
            staminaPotionEffectImage.fillAmount = 1f;
        }
    }
}
