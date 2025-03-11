using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using MyGame.Loot;
public class MonsterAI : MonoBehaviour
{
    [Header("Enemy Parameters")]
    public int maxHealth = 50;
    public int currentHealth;

    [Header("Detection Settings")]
    public float playerDetectionRadius = 10f;
    public float fieldOfViewAngle = 60f;

    [Header("Movement Settings")]
    public float moveSpeed = 3.5f;
    public float patrolSpeed = 2f;

    [Header("Attack Settings")]
    public float attackRange = 2f;
    public int attackDamage = 5;
    public float attackCooldown = 1.5f;
    public int minAttackDamage = 3;  // Минимальный урон
    public int maxAttackDamage = 7;  // Максимальный урон
    [Header("Patrolling")]
    public bool canPatrol = true;
    public float patrolRadius = 5f;
    public float patrolWaitTimeMin = 3f;
    public float patrolWaitTimeMax = 4f;

    [Header("Floating Damage Text")]
    public GameObject floatingDamageTextPrefab;
    public Vector3 floatingTextOffset = new Vector3(0, 2f, 0);

    [Header("Animation Layers")]
    public int upperBodyLayerIndex = 1;
    public float upperLayerLerpSpeed = 5f;
    private float currentUpperLayerWeight = 0f;

    [Header("Rotation Settings")]
    public float rotationSpeed = 10f;

    [Header("AI State")]
    private bool isPlayerDetected = false;
    private bool isWaiting = false;
    private bool isDead = false;
    private bool isPlayerInAttackZone = false;
    private bool isAttacking = false;
    private float lastAttackTime;
    private bool hasSeenPlayer = false;

    private NavMeshAgent agent;
    private Animator animator;
    private Transform player;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        animator = GetComponent<Animator>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        currentHealth = maxHealth;
        agent.speed = patrolSpeed;
    }

    void Update()
    {
        if (isDead) return;

        if (player != null)
        {
            // Если игрок мёртв, сбрасываем обнаружение и возвращаемся к патрулированию
            if (PlayerController.Instance != null && PlayerController.Instance.IsDead)
            {
                isPlayerDetected = false;
                isPlayerInAttackZone = false;
                hasSeenPlayer = false;
                agent.speed = patrolSpeed;
                if (!isWaiting)
                    SetNewPatrolDestination();
            }
            else
            {
                DetectPlayer();
            }
        }
        
        MoveEnemy();
        UpdateAnimation();
        UpdateUpperBodyLayerWeight();
    }

    void DetectPlayer()
    {
        if (player == null) return;

        if (hasSeenPlayer)
        {
            isPlayerDetected = true;
            return;
        }

        Vector3 directionToPlayer = player.position - transform.position;
        float distance = directionToPlayer.magnitude;

        if (distance < playerDetectionRadius)
        {
            float angle = Vector3.Angle(transform.forward, directionToPlayer.normalized);
            if (angle < fieldOfViewAngle)
            {
                isPlayerDetected = true;
                hasSeenPlayer = true;
                if (!isPlayerInAttackZone)
                    agent.speed = moveSpeed;
                return;
            }
        }
        else
        {
            isPlayerDetected = false;
        }
    }

    void MoveEnemy()
    {
        if (isPlayerDetected && player != null && player.gameObject.activeSelf && (PlayerController.Instance == null || !PlayerController.Instance.IsDead))
        {
            if (isPlayerInAttackZone)
            {
                agent.isStopped = true;
                if (!isAttacking && Time.time > lastAttackTime + attackCooldown)
                {
                    Attack();
                }
            }
            else
            {
                agent.isStopped = false;
                agent.SetDestination(player.position);
            }
        }
        else
        {
            // Если игрок не обнаружен или мёртв, переходим в патрульный режим
            agent.speed = patrolSpeed;
            if (canPatrol && !isWaiting && agent.remainingDistance < 0.5f)
                StartCoroutine(PatrolPause());
        }
    }

    IEnumerator PatrolPause()
    {
        isWaiting = true;
        agent.isStopped = true;
        animator.SetFloat("Speed", 0f);
        float waitTime = Random.Range(patrolWaitTimeMin, patrolWaitTimeMax);
        yield return new WaitForSeconds(waitTime);
        agent.isStopped = false;
        SetNewPatrolDestination();
        isWaiting = false;
    }

    void SetNewPatrolDestination()
    {
        Vector3 randomPoint = transform.position + Random.insideUnitSphere * patrolRadius;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, patrolRadius, NavMesh.AllAreas))
            agent.SetDestination(hit.position);
    }

    void UpdateAnimation()
    {
        if (isDead) return;
        Vector3 velocity = agent.velocity;
        float speed = velocity.magnitude;
        animator.SetFloat("Speed", isAttacking ? 0f : speed);
        float horizontal = Vector3.Dot(transform.right, velocity.normalized);
        float vertical = Vector3.Dot(transform.forward, velocity.normalized);
        animator.SetFloat("Horizontal", horizontal);
        animator.SetFloat("Vertical", vertical);
    }

    void UpdateUpperBodyLayerWeight()
    {
        if (isAttacking)
            currentUpperLayerWeight = 1f;
        else
            currentUpperLayerWeight = Mathf.Lerp(currentUpperLayerWeight, 0f, Time.deltaTime * upperLayerLerpSpeed);
        animator.SetLayerWeight(upperBodyLayerIndex, currentUpperLayerWeight);
    }

    void Attack()
    {
        if (isDead) return;
        agent.isStopped = true;
        isAttacking = true;
        animator.SetTrigger("Attack");
        lastAttackTime = Time.time;
    }

    void AttackHit()
    {
        Debug.Log("AttackHit event triggered");
        if (player != null && player.gameObject.activeSelf && player.CompareTag("Player"))
        {
            PlayerStats playerStats = player.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                int damage = Random.Range(minAttackDamage, maxAttackDamage + 1); // Генерация урона в указанном диапазоне
                playerStats.TakeDamage(damage);
                Debug.Log($"Player took damage: {damage}");
            }
        }
    }
    public void OnAttackAnimationEnd()
    {
        Debug.Log("Attack animation ended");
        isAttacking = false;
        if (isPlayerDetected && player != null && player.gameObject.activeSelf && (PlayerController.Instance == null || !PlayerController.Instance.IsDead))
            agent.isStopped = false;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        currentHealth -= damage;
        if (currentHealth < 0)
            currentHealth = 0;
        if (floatingDamageTextPrefab != null)
        {
            Vector3 spawnPos = transform.position + floatingTextOffset;
            Quaternion spawnRotation = Camera.main != null ? Quaternion.LookRotation(Camera.main.transform.forward) : Quaternion.identity;
            GameObject dmgText = Instantiate(floatingDamageTextPrefab, spawnPos, spawnRotation);
            FloatingDamageText fdt = dmgText.GetComponent<FloatingDamageText>();
            if (fdt != null)
                fdt.Setup(damage);
        }
        if (currentHealth <= 0)
            Die();
    }

    public void Die()
{
    EnemyXP xpComponent = GetComponent<EnemyXP>();
if (xpComponent != null) {
    PlayerLevelManager.Instance.AddXP(xpComponent.xpReward);
}

    isDead = true;
    agent.isStopped = true;
    animator.SetTrigger("Die");

    // Если у этого врага есть EnemySpawnMarker, уведомляем спавнпоинт о его смерти
    EnemySpawnMarker marker = GetComponent<EnemySpawnMarker>();
    if (marker != null && marker.spawnPoint != null)
    {
        marker.spawnPoint.OnEnemyDeath();
    }
    
    LootDropper dropper = GetComponent<LootDropper>();
    if (dropper != null)
        dropper.DropLoot();
    Destroy(gameObject, 0.5f);
}


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInAttackZone = true;
            Debug.Log("Player entered attack zone");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInAttackZone = false;
            agent.isStopped = false;
            Debug.Log("Player exited attack zone");
        }
    }

    void LateUpdate()
    {
        if (!isAttacking && agent.velocity.sqrMagnitude > 0.1f)
        {
            Quaternion desiredRotation = Quaternion.LookRotation(agent.velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * rotationSpeed);
        }
    }
}
