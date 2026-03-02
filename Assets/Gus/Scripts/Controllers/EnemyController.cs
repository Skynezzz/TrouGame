using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("Movement")]
    public float lookRadius = 10f;
    private float updateTimer = 0f;

    [Header("Attack")]
    public float attackRange = 2;
    public float attackCooldown = 1.5f;
    public float attackDamage = 10f;
    private float attackTimer = 0f;

    [Header("Health")]
    public float maxHealth = 100f;
    private float currentHealth;
    private bool isDead = false;

    
    private Transform target;
    private NavMeshAgent agent;
    private Animator animator;

    

    private const float UPDATE_RATE = 0.1f;

    private static readonly int IsWalkingHash = Animator.StringToHash("IsWalking");
    private static readonly int AttackHash    = Animator.StringToHash("Attack");
    private static readonly int HitHash       = Animator.StringToHash("Hit");
    private static readonly int DeathHash     = Animator.StringToHash("Death");

    // -------------------------------------------------------------------------
    // UNITY CALLBACKS
    // -------------------------------------------------------------------------

    void Start()
    {
        InitComponents();
        InitPlayer();
        InitHealth();
    }

    void Update()
    {
        if (target == null || isDead) return;

        DebugInput();

        attackTimer -= Time.deltaTime;
        updateTimer -= Time.deltaTime;

        float sqrDistance     = (transform.position - target.position).sqrMagnitude;
        bool  isInRange       = sqrDistance <= lookRadius  * lookRadius;
        bool  isAtAttackRange = sqrDistance <= attackRange * attackRange;

        if (isInRange)
            HandleCombat(isAtAttackRange);
        else
            HandleIdle();
    }

    // -------------------------------------------------------------------------
    // INIT
    // -------------------------------------------------------------------------

    void InitComponents()
    {
        animator               = GetComponentInChildren<Animator>();
        agent                  = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = attackRange;
    }

    void InitPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
            target = playerObj.transform;
        else
            Debug.LogWarning($"[EnemyController] Aucun GameObject avec le tag 'Player' trouvé sur {gameObject.name}");
    }

    void InitHealth()
    {
        currentHealth = maxHealth;
    }

    // -------------------------------------------------------------------------
    // STATES
    // -------------------------------------------------------------------------

    void HandleCombat(bool isAtAttackRange)
    {
        if (isAtAttackRange)
            HandleAttack();
        else
            HandleChase();
    }

    void HandleChase()
    {
        if (updateTimer <= 0f)
        {
            agent.SetDestination(target.position);
            updateTimer = UPDATE_RATE;
        }

        animator.SetBool(IsWalkingHash, agent.velocity.sqrMagnitude > 0.01f);
    }

    void HandleAttack()
    {
        agent.ResetPath();
        animator.SetBool(IsWalkingHash, false);
        LookAtTarget();

        if (attackTimer <= 0f)
        {
            animator.SetTrigger(AttackHash);
            attackTimer = attackCooldown;
        }
    }

    void HandleIdle()
    {
        if (agent.hasPath)
        {
            agent.ResetPath();
            animator.SetBool(IsWalkingHash, false);
        }
    }

    // -------------------------------------------------------------------------
    // HEALTH
    // -------------------------------------------------------------------------

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth  = Mathf.Clamp(currentHealth, 0f, maxHealth);

        if (currentHealth <= 0f)
            HandleDeath();
        else
            HandleHit();
    }

    void HandleHit()
    {
        animator.SetTrigger(HitHash);
    }

    void HandleDeath()
    {
        isDead = true;

        agent.ResetPath();
        agent.enabled        = false;
        animator.applyRootMotion = false;

        animator.SetTrigger(DeathHash);

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
    }

    // Appelé via Animation Event à la fin du clip de mort
    public void OnDeathAnimationEnd()
    {
        Destroy(gameObject);
    }

    // -------------------------------------------------------------------------
    // COMBAT
    // -------------------------------------------------------------------------
    
    public void DealDamage()
    {
        if (target == null) return;

        float sqrDistance = (transform.position - target.position).sqrMagnitude;

        /*if (sqrDistance <= attackRange * attackRange)
        {
            PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();
            if (playerHealth != null)
                playerHealth.TakeDamage(attackDamage);
        }*/
    }

    // -------------------------------------------------------------------------
    // UTILS
    // -------------------------------------------------------------------------

    void LookAtTarget()
    {
        Vector3    direction    = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
        transform.rotation      = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    // -------------------------------------------------------------------------
    // DEBUG
    // -------------------------------------------------------------------------

    void DebugInput()
    {
        if (UnityEngine.InputSystem.Keyboard.current.pKey.wasPressedThisFrame)
        {
            TakeDamage(20f);
            Debug.Log($"[DEBUG] Dégâts infligés — Vie restante : {currentHealth}/{maxHealth}");
        }
    }

    // -------------------------------------------------------------------------
    // GIZMOS
    // -------------------------------------------------------------------------

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}