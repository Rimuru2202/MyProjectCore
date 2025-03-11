using System.Collections.Generic;
using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    [Header("Attack Settings")]
    [Tooltip("Stamina cost for heavy attack")]
    public float heavyAttackStaminaCost = 20f;
    [Tooltip("Hold time for charge attack (in seconds)")]
    public float chargeAttackHoldTime = 1.5f;
    [Tooltip("Damage for unarmed attack")]
    public int unarmedAttackDamage = 1;
    [Tooltip("Damage for armed attack")]
    public int armedAttackDamage = 10;

    [Header("Hitbox Settings")]
    [Tooltip("Size of damage box (Box)")]
    public Vector3 damageBoxSize = new Vector3(1f, 1f, 2f);
    [Tooltip("Offset for damage box (relative to player)")]
    public Vector3 damageBoxOffset = new Vector3(0f, 0f, 1f);

    private float attackHoldTimer = 0f;
    private bool isAttacking = false;
    private bool isHeavyAttack = false;
    public bool IsAttacking { get { return isAttacking; } }

    [Header("Block Settings")]
    [Tooltip("Stamina drain rate when blocking")]
    public float blockStaminaDrainRate = 10f;
    private bool isBlocking = false;
    public bool IsBlocking { get { return isBlocking; } }

    [Header("Dodge Settings")]
    [Tooltip("Stamina cost for dodge")]
    public float dodgeStaminaCost = 25f;
    private bool isDodging = false;

    [Header("Layer Settings")]
    [Tooltip("UpperBody layer index (Override)")]
    public int upperBodyLayerIndex = 1;
    [Tooltip("Lerp speed for upper body layer weight")]
    public float upperLayerLerpSpeed = 5f;
    private float currentUpperLayerWeight = 0f;

    private Animator animator;
    private PlayerController playerController;

    void Start()
    {
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (InventoryUI.IsOpen ||
            (ShopUIManager.Instance != null && ShopUIManager.Instance.gameObject.activeSelf) ||
            playerController.IsDead)
            return;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Jump_Up") || stateInfo.IsName("Fall") || stateInfo.IsName("Jump_Down"))
            return;

        HandleAttackInput();
        HandleBlockInput();
        HandleDodgeInput();
        UpdateUpperBodyLayerWeight();
    }

    void HandleAttackInput()
    {
        if (isAttacking || isBlocking || isDodging)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            attackHoldTimer = 0f;
            isHeavyAttack = false;
        }
        if (Input.GetMouseButton(0))
            attackHoldTimer += Time.deltaTime;
        if (Input.GetMouseButtonUp(0))
        {
            if (attackHoldTimer >= chargeAttackHoldTime)
            {
                isHeavyAttack = true;
                if (EquipmentManager.Instance != null && EquipmentManager.Instance.HasWeapon)
                {
                    if (playerController.HasStamina(heavyAttackStaminaCost))
                    {
                        playerController.ConsumeStamina(heavyAttackStaminaCost);
                        animator.SetTrigger("SwordChargeAttack");
                        isAttacking = true;
                    }
                }
            }
            else
            {
                isHeavyAttack = false;
                if (EquipmentManager.Instance != null && EquipmentManager.Instance.HasWeapon)
                    animator.SetTrigger("SwordAttack");
                else
                {
                    int randomHand = Random.Range(0, 2);
                    if (randomHand == 0)
                        animator.SetTrigger("PunchLeft");
                    else
                        animator.SetTrigger("PunchRight");
                }
                isAttacking = true;
            }
            attackHoldTimer = 0f;
        }
    }

    public void AttackHit()
    {
        Debug.Log("Attack hit event received.");
        DealDamageInLine();
    }

    void DealDamageInLine()
    {
        int baseDamage = unarmedAttackDamage;
        if (EquipmentManager.Instance != null && EquipmentManager.Instance.HasWeapon)
        {
            Equipment weapon = EquipmentManager.Instance.equippedWeapon;
            baseDamage = (weapon != null) ? weapon.damage : armedAttackDamage;
        }
        int finalDamage = isHeavyAttack ? baseDamage * 2 : baseDamage;
        Vector3 center = playerController.transform.position + playerController.transform.TransformDirection(damageBoxOffset);
        Collider[] hits = Physics.OverlapBox(center, damageBoxSize * 0.5f, playerController.transform.rotation);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                MonsterAI enemy = hit.GetComponent<MonsterAI>();
                if (enemy != null)
                    enemy.TakeDamage(finalDamage);
            }
        }
    }

    public void OnAttackAnimationEnd()
    {
        isAttacking = false;
        isHeavyAttack = false;
    }

    void HandleBlockInput()
    {
        if (playerController.IsRunning || isAttacking || isDodging)
            return;

        if (Input.GetMouseButtonDown(1))
            StartBlocking();
        if (Input.GetMouseButtonUp(1))
            StopBlocking();

        if (isBlocking)
        {
            if (playerController.HasStamina(blockStaminaDrainRate * Time.deltaTime))
                playerController.ConsumeStamina(blockStaminaDrainRate * Time.deltaTime);
            else
                StopBlocking();
        }
    }

    void StartBlocking()
    {
        isBlocking = true;
        if (EquipmentManager.Instance != null && EquipmentManager.Instance.equippedShield != null)
            animator.SetBool("isBlockingWithShield", true);
        else
            animator.SetBool("isBlockingUnarmed", true);
    }

    void StopBlocking()
    {
        isBlocking = false;
        animator.SetBool("isBlockingWithShield", false);
        animator.SetBool("isBlockingUnarmed", false);
    }

    void HandleDodgeInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) && !isDodging && !isBlocking && !isAttacking && !playerController.IsRunning)
        {
            if (playerController.HasStamina(dodgeStaminaCost))
            {
                string dodgeTrigger = "";
                Vector3 dodgeDirection = Vector3.zero;
                if (Input.GetKey(KeyCode.W))
                {
                    dodgeTrigger = "RollW";
                    dodgeDirection = playerController.cameraHolder.forward;
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    dodgeTrigger = "RollS";
                    dodgeDirection = -playerController.cameraHolder.forward;
                }
                else if (Input.GetKey(KeyCode.A))
                {
                    dodgeTrigger = "RollA";
                    dodgeDirection = -playerController.cameraHolder.right;
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    dodgeTrigger = "RollD";
                    dodgeDirection = playerController.cameraHolder.right;
                }
                animator.SetTrigger(dodgeTrigger);
                isDodging = true;
                // ????????? ???????; ?????? ???????????? ?????????? ??????????? ?????? ????????
                playerController.StartDodgeRollCoroutine(dodgeStaminaCost, dodgeDirection);
            }
        }
    }

    public void EndDodge()
    {
        isDodging = false;
    }

    void UpdateUpperBodyLayerWeight()
    {
        if (isAttacking)
            currentUpperLayerWeight = 1f;
        else if (isBlocking)
            currentUpperLayerWeight = Mathf.Lerp(currentUpperLayerWeight, 1f, Time.deltaTime * upperLayerLerpSpeed);
        else
            currentUpperLayerWeight = Mathf.Lerp(currentUpperLayerWeight, 0f, Time.deltaTime * upperLayerLerpSpeed);
        animator.SetLayerWeight(upperBodyLayerIndex, currentUpperLayerWeight);
    }
}
