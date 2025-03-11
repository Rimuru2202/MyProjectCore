using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [Header("Movement Settings")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;

    [Header("Jump Settings")]
    public float jumpForce = 5f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    [Header("Stamina Settings")]
    // Используется для дискретного расхода выносливости при беге (расход раз в секунду)
    public float runStaminaCost = 50f;

    [Header("Camera Settings")]
    public Transform cameraHolder;
    public Transform cameraPivot;
    public float mouseSensitivity = 1.6f;
    public float minY = -90f;
    public float maxY = 90f;
    public float cameraDistance = 3f;
    public float cameraHeight = 1.5f;

    [Header("Camera Follow Settings")]
    public float dodgeCameraSmoothSpeed = 10f;
    public float normalCameraSmoothSpeed = 20f;

    [Header("Dodge Settings")]
    public float dodgeForce = 7f;
    public float dodgeDuration = 0.4f;

    [Header("UI Elements")]
    public Image staminaFill;
    public Text staminaText;

    [Header("UI Feedback")]
    public Image insufficientStaminaIndicator;
    public float indicatorDisplayTime = 0.5f;
    public float indicatorFadeInDuration = 0.2f;
    public float indicatorFadeOutDuration = 0.2f;
    public float indicatorTargetAlpha = 0.5f;

    [Header("Landing Settings")]
    [Tooltip("Задержка перед проигрыванием анимации Jump_Down после приземления")]
    public float landingDelay = 0.3f;

    private Rigidbody rb;
    private Animator animator;
    private PlayerStats playerStats;
    private CombatSystem combatSystem;
    private Vector3 cameraVelocity = Vector3.zero;
    private bool isDodging = false;

    // Свойство для состояния бега (используется в PlayerStats)
    public bool IsRunning { get; private set; }
    private bool isDead = false;
    public bool IsDead { get { return isDead; } }
    private Vector3 deathPosition;
    private float verticalLookRotation = 0f;

    private bool isJumpingState = false;
    private bool hasFallen = false;
    private bool isGrounded = false;
    private bool jumpRequested = false;

    void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody>();

        if (SpawnManager.Instance != null)
        {
            SpawnPoint spawn = SpawnManager.Instance.GetSpawnPointByIndex(0);
            if (spawn != null)
            {
                Debug.Log("Устанавливаем позицию игрока на точку спавна: " + spawn.transform.position);
                rb.position = spawn.transform.position;
            }
            else
            {
                Debug.LogWarning("SpawnPoint с индексом 0 не найден! Проверьте настройки SpawnPoint.");
            }
        }
        else
        {
            Debug.LogError("SpawnManager не найден в сцене!");
        }
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.applyRootMotion = false;
        combatSystem = GetComponent<CombatSystem>();
        playerStats = GetComponent<PlayerStats>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        UpdateStaminaUI();

        if (insufficientStaminaIndicator != null)
        {
            Color col = insufficientStaminaIndicator.color;
            col.a = 0f;
            insufficientStaminaIndicator.color = col;
        }

        if (cameraPivot == null)
            cameraPivot = transform;
    }

    void Update()
    {
        if (isDead)
            return;

        if (InventoryUI.IsOpen || (ShopUIManager.Instance != null && ShopUIManager.Instance.gameObject.activeSelf))
            jumpRequested = false;
        else
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded &&
                !stateInfo.IsName("Jump_Up") &&
                !stateInfo.IsName("Fall") &&
                !stateInfo.IsName("Jump_Down"))
            {
                jumpRequested = true;
            }
        }

        LookAround();
        UpdateStaminaUI();

        if (isJumpingState && !hasFallen)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Jump_Up") && stateInfo.normalizedTime >= 1f)
            {
                animator.Play("Fall", 0, 0f);
                hasFallen = true;
            }
        }
    }

    void FixedUpdate()
    {
        if (isDead)
            return;

        Move();

        if (!isGrounded)
        {
            if (rb.linearVelocity.y < 0)
                rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
            else if (rb.linearVelocity.y > 0 && !Input.GetKey(KeyCode.Space))
                rb.linearVelocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    void LateUpdate()
    {
        if (isDead)
        {
            transform.position = deathPosition;
            return;
        }
        FollowPlayerWithCamera();
    }

    void Move()
    {
        if (jumpRequested)
        {
            Vector3 vel = rb.linearVelocity;
            vel.y = jumpForce;
            rb.linearVelocity = vel;
            animator.Play("Jump_Up", 0, 0f);
            isJumpingState = true;
            hasFallen = false;
            jumpRequested = false;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        bool isBlocking = (combatSystem != null && combatSystem.IsBlocking);
        bool isAttacking = (combatSystem != null && combatSystem.IsAttacking);
        bool runningAllowed = !isBlocking && !isAttacking;
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && playerStats.CanRun && runningAllowed;

        if (isRunning && vertical == 0)
            vertical = 1f;

        IsRunning = isRunning;
        float speed = isRunning ? runSpeed : walkSpeed;
        if (!isGrounded) speed *= 0.5f;

        Vector3 forward = cameraHolder.forward;
        Vector3 right = cameraHolder.right;
        forward.y = 0; right.y = 0;
        forward.Normalize(); right.Normalize();
        Vector3 moveDirection = (forward * vertical + right * horizontal).normalized;

        Vector3 targetVelocity = moveDirection * speed;
        Vector3 currentVelocity = rb.linearVelocity;
        rb.linearVelocity = new Vector3(targetVelocity.x, currentVelocity.y, targetVelocity.z);

        if (moveDirection.magnitude > 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(forward);
            rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f));
        }

        float animSpeed = isRunning ? 2f : (moveDirection.magnitude > 0.1f ? 1f : 0f);
        animator.SetFloat("Speed", animSpeed);
        animator.SetFloat("Horizontal", horizontal);
        animator.SetFloat("Vertical", vertical);
        // Расход выносливости при беге производится дискретно в PlayerStats.RegenCoroutine()
    }

    void LookAround()
    {
        if (InventoryUI.IsOpen || (ShopUIManager.Instance != null && ShopUIManager.Instance.gameObject.activeSelf))
            return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        verticalLookRotation -= mouseY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, minY, maxY);

        cameraHolder.localRotation = Quaternion.Euler(verticalLookRotation, cameraHolder.localRotation.eulerAngles.y, 0);
        cameraHolder.Rotate(Vector3.up * mouseX);
    }

    void FollowPlayerWithCamera()
    {
        Vector3 targetPosition = cameraPivot.position - cameraHolder.forward * cameraDistance + Vector3.up * cameraHeight;
        float smoothSpeed = isDodging ? dodgeCameraSmoothSpeed : normalCameraSmoothSpeed;
        cameraHolder.position = Vector3.SmoothDamp(cameraHolder.position, targetPosition, ref cameraVelocity, 1f / smoothSpeed);
    }

    void UpdateStaminaUI()
    {
        if (staminaFill != null && playerStats != null)
            staminaFill.fillAmount = playerStats.stamina / playerStats.maxStamina;
        if (staminaText != null && playerStats != null)
            staminaText.text = Mathf.RoundToInt(playerStats.stamina).ToString();
    }

    public bool HasStamina(float amount)
    {
        return playerStats != null && playerStats.HasStamina(amount);
    }

    public void ConsumeStamina(float amount)
    {
        if (playerStats != null)
        {
            playerStats.ConsumeStamina(amount);
            UpdateStaminaUI();
        }
    }

    public void ShowInsufficientStaminaIndicator()
    {
        if (insufficientStaminaIndicator != null)
        {
            StopAllCoroutines();
            StartCoroutine(ShowIndicatorCoroutine());
        }
    }

    IEnumerator ShowIndicatorCoroutine()
    {
        Color col = insufficientStaminaIndicator.color;
        float time = 0f;
        while (time < indicatorFadeInDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, indicatorTargetAlpha, time / indicatorFadeInDuration);
            insufficientStaminaIndicator.color = new Color(col.r, col.g, col.b, alpha);
            yield return null;
        }
        insufficientStaminaIndicator.color = new Color(col.r, col.g, col.b, indicatorTargetAlpha);
        yield return new WaitForSeconds(indicatorDisplayTime);
        time = 0f;
        while (time < indicatorFadeOutDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(indicatorTargetAlpha, 0f, time / indicatorFadeOutDuration);
            insufficientStaminaIndicator.color = new Color(col.r, col.g, col.b, alpha);
            yield return null;
        }
        insufficientStaminaIndicator.color = new Color(col.r, col.g, col.b, 0f);
    }

    public void StartDodgeRollCoroutine(float totalDrain, Vector3 dodgeDirection)
    {
        if (!isDodging)
            StartCoroutine(DodgeRoll(totalDrain, dodgeDirection));
    }

    // Корутинa для переката: расход выносливости списывается моментально,
    // после чего происходит лишь движение. Также отображается мгновенное изменение в UI.
    IEnumerator DodgeRoll(float totalDrain, Vector3 dodgeDirection)
    {
        isDodging = true;
        // Мгновенно списываем выносливость и показываем в UI (например, "-100")
        ConsumeStamina(totalDrain);
        if (PlayerStats.Instance != null)
            PlayerStats.Instance.ShowInstantStaminaChange(-totalDrain);

        float elapsedTime = 0f;
        dodgeDirection = new Vector3(dodgeDirection.x, 0, dodgeDirection.z).normalized;
        while (elapsedTime < dodgeDuration)
        {
            rb.MovePosition(transform.position + dodgeDirection * dodgeForce * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        isDodging = false;
    }

    public void SetCameraControl(bool enable) { }

    public void Die()
    {
        isDead = true;
        deathPosition = transform.position;
        animator.SetTrigger("Death");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Respawn(Vector3 spawnPosition)
    {
        isDead = false;
        transform.position = spawnPosition;
        deathPosition = spawnPosition;
        animator.ResetTrigger("Death");
        animator.Play("Idle", 0, 0f);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (PlayerStats.Instance != null)
            PlayerStats.Instance.ResetResourcesAfterRespawn();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            if (isJumpingState)
                StartCoroutine(PlayLandingAnimation());
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }

    IEnumerator PlayLandingAnimation()
    {
        yield return new WaitForSeconds(landingDelay);
        animator.Play("Jump_Down", 0, 0f);
        isJumpingState = false;
        hasFallen = false;
    }

    public void OnJumpAnimationEnd()
    {
        Debug.Log("Jump animation ended.");
    }

    public bool CanAttack
    {
        get
        {
            if (InventoryUI.IsOpen || (ShopUIManager.Instance != null && ShopUIManager.Instance.gameObject.activeSelf))
                return false;
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Jump_Up") || stateInfo.IsName("Fall") || stateInfo.IsName("Jump_Down"))
                return false;
            return true;
        }
    }
}
