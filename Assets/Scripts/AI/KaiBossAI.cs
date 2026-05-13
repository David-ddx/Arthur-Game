// Assets/Scripts/AI/KaiBossAI.cs
using UnityEngine;
using System.Collections;

public class KaiBossAI : MonoBehaviour
{
    public enum State { Disabled, WalkingToArena, WaitingAtArena, Fighting, Dead }
    public State currentState = State.Disabled;

    [Header("引用")]
    public Transform player;
    public Transform[] pathToArena;

    [Header("战斗参数")]
    public float detectionRange = 15f;
    public float attackRange = 2.5f;
    public float moveSpeed = 3f;
    public float attackCooldown = 2f;

    [Header("狂暴模式")]
    public float rageHealthPercent = 0.5f;
    public float rageSpeedMultiplier = 1.5f;
    public float rageAttackCooldown = 1f;

    [Header("死亡后回待机")]
    public float deathToIdleDelay = 3f;

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

        if (stats != null)
            stats.onDeath.AddListener(OnDeath);

        if (anim != null)
            anim.SetFloat("Speed", 0f);
    }

    void Update()
    {
        if (currentState == State.Dead) return;

        switch (currentState)
        {
            case State.Disabled:
                if (anim != null) anim.SetFloat("Speed", 0f);
                break;

            case State.WalkingToArena:
                WalkToArena();
                break;

            case State.WaitingAtArena:
                if (anim != null) anim.SetFloat("Speed", 0f);
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
            currentState = State.WaitingAtArena;

            if (anim != null)
                anim.SetFloat("Speed", 0f);

            Debug.Log("凯到达擂台，等待玩家");
            return;
        }

        Transform targetPoint = pathToArena[currentPathIndex];
        Vector3 direction = (targetPoint.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, targetPoint.position);

        if (distance > 0.5f)
        {
            transform.position += direction * moveSpeed * Time.deltaTime;

            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(direction),
                    10f * Time.deltaTime
                );
            }

            if (anim != null)
                anim.SetFloat("Speed", moveSpeed);
        }
        else
        {
            currentPathIndex++;
        }
    }

    void HandleFighting()
    {
        if (player == null || stats == null) return;

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
            ChasePlayer();
        }
        else
        {
            if (anim != null)
                anim.SetFloat("Speed", 0f);
        }
    }

    void ChasePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0f;

        float speed = isRage ? moveSpeed * rageSpeedMultiplier : moveSpeed;

        transform.position += direction * speed * Time.deltaTime;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }

        if (anim != null)
            anim.SetFloat("Speed", speed);
    }

    void AttackPlayer()
    {
        if (player == null) return;

        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0f;

        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);

        if (anim != null)
            anim.SetFloat("Speed", 0f);

        if (attackTimer > 0f) return;

        float cooldown = isRage ? rageAttackCooldown : attackCooldown;
        attackTimer = cooldown;

        if (anim != null)
            anim.SetTrigger("Attack");
    }

    void EnterRage()
    {
        isRage = true;
        Debug.Log("凯进入狂暴模式！");
    }

    void OnDeath()
    {
        if (currentState == State.Dead) return;

        currentState = State.Dead;

        if (anim != null)
        {
            anim.SetFloat("Speed", 0f);
            anim.ResetTrigger("Attack");
            anim.SetTrigger("Die");
        }

        Debug.Log("凯Boss死亡动画已触发。");

        StartCoroutine(DeathThenIdle());

        TrainingGroundManager manager = FindObjectOfType<TrainingGroundManager>();
        if (manager != null)
            manager.OnKaiDefeated();
    }

    private IEnumerator DeathThenIdle()
    {
        yield return new WaitForSeconds(deathToIdleDelay);

        if (anim != null)
        {
            anim.SetFloat("Speed", 0f);
        }

        currentState = State.WaitingAtArena;

        Debug.Log("凯死亡动画结束，回到Idle。");

        // 后面队员对话可以接这里
        // DialogManager.Instance.StartDialog("KaiDefeated");
    }

    public void StartWalkingToArena()
    {
        currentState = State.WalkingToArena;
        currentPathIndex = 0;
    }

    public void StartFighting()
    {
        if (currentState == State.Dead) return;
        currentState = State.Fighting;
    }

    [ContextMenu("Test Death")]
    void TestDeath()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("请先点 Play 再测试凯死亡。");
            return;
        }

        OnDeath();
    }
}
