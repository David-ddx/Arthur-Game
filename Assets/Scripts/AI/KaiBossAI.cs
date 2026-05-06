// Assets/Scripts/AI/KaiBossAI.cs
using UnityEngine;

public class KaiBossAI : MonoBehaviour
{
    public enum State { Disabled, WalkingToArena, WaitingAtArena, Fighting, Dead }
    public State currentState = State.Disabled;

    [Header("引用")]
    public Transform player;
    public Transform[] pathToArena;  // 路径点数组（从训练场到擂台）

    [Header("战斗参数")]
    public float detectionRange = 15f;
    public float attackRange = 2.5f;
    public float moveSpeed = 3f;
    public float attackCooldown = 2f;

    [Header("狂暴模式")]
    public float rageHealthPercent = 0.5f;
    public float rageSpeedMultiplier = 1.5f;
    public float rageAttackCooldown = 1f;

    private CharacterStats stats;
    private Animator anim;
    private float attackTimer = 0f;
    private bool isRage = false;
    private int currentPathIndex = 0;

    void Start()
    {
        stats = GetComponent<CharacterStats>();
        anim = GetComponent<Animator>();

        if (player == null)
            player = GameObject.Find("Hero_Knight")?.transform;

        stats.onDeath.AddListener(OnDeath);

        anim.SetFloat("Speed", 0f);
    }

    void Update()
    {
        if (currentState == State.Dead) return;

        switch (currentState)
        {
            case State.Disabled:
                anim.SetFloat("Speed", 0f);
                break;

            case State.WalkingToArena:
                WalkToArena();
                break;

            case State.WaitingAtArena:
                anim.SetFloat("Speed", 0f);
                break;

            case State.Fighting:
                HandleFighting();
                break;
        }
    }

    void WalkToArena()
    {
        if (pathToArena == null || pathToArena.Length == 0)
        {
            Debug.LogError("KaiBossAI：没有设置路径点！");
            currentState = State.WaitingAtArena;
            return;
        }

        if (currentPathIndex >= pathToArena.Length)
        {
            // 到达擂台
            currentState = State.WaitingAtArena;
            anim.SetFloat("Speed", 0f);
            Debug.Log("凯到达擂台，等待玩家");
            return;
        }

        Transform targetPoint = pathToArena[currentPathIndex];
        Vector3 direction = (targetPoint.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, targetPoint.position);

        if (distance > 0.5f)
        {
            // 移动向路径点
            transform.position += direction * moveSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(direction),
                10f * Time.deltaTime
            );
            anim.SetFloat("Speed", moveSpeed);
        }
        else
        {
            // 到达当前路径点，前往下一个
            currentPathIndex++;
        }
    }

    void HandleFighting()
    {
        if (!isRage && stats.GetHealthPercent() <= rageHealthPercent)
            EnterRage();

        attackTimer -= Time.deltaTime;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            AttackPlayer();
        }
        else if (distanceToPlayer <= detectionRange)
        {
            ChasePlayer(distanceToPlayer);
        }
        else
        {
            anim.SetFloat("Speed", 0f);
        }
    }

    void ChasePlayer(float distance)
    {
        Vector3 direction = (player.position - transform.position).normalized;
        float speed = isRage ? moveSpeed * rageSpeedMultiplier : moveSpeed;
        transform.position += direction * speed * Time.deltaTime;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);

        anim.SetFloat("Speed", speed);
    }

    void AttackPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0f;
        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);

        anim.SetFloat("Speed", 0f);

        if (attackTimer > 0f) return;

        float cooldown = isRage ? rageAttackCooldown : attackCooldown;
        attackTimer = cooldown;
        anim.SetTrigger("Attack");
    }

    void EnterRage()
    {
        isRage = true;
        Debug.Log("凯进入狂暴模式！");
    }

    void OnDeath()
    {
        if (currentState == State.Dead) return;  // 防止重复调用

        currentState = State.Dead;
        anim.SetBool("Dead", true);

        // 禁用所有组件
        this.enabled = false;
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Debug.Log("凯Boss被击败！");

        // 通知TrainingGroundManager
        TrainingGroundManager manager = FindObjectOfType<TrainingGroundManager>();
        if (manager != null)
            manager.OnKaiDefeated();
    }

    // 由TrainingGroundManager调用
    public void StartWalkingToArena()
    {
        currentState = State.WalkingToArena;
        currentPathIndex = 0;
    }

    public void StartFighting()
    {
        currentState = State.Fighting;
    }
}