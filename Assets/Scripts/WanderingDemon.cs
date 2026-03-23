using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class WanderingDemon : MonoBehaviour, IReactiveTarget
{
    Transform Player;
    Animator anim;
    NavMeshAgent agent;

    [SerializeField]
    AudioSource audioSrc;

    [SerializeField]
    AudioClip hit, die, attack, attack2, detect;

    [SerializeField]
    float walkingSpeed, runningSpeed;

    [SerializeField]    
    int Health = 100;

    NavMeshSurface surface;
    NavMeshTriangulation triangulation;

    Vector3 currentTarget;


    [SerializeField]
    GameObject fireball;

    [SerializeField]
    Transform attacklocation1,attacklocation2;
    
    [SerializeField]
    float basicattackCd = 1f;
    [SerializeField]
    float heavyattackCd = 10f;

    [SerializeField]
    float playerAttackRange = 6f;
    [SerializeField]
    float playerDetectRange = 20f;
    [SerializeField]
    float playerDetectAngle = 75f;

    float _basicAttackTimer = 0f;
    float _heavyAttackTimer = 0f;

    bool move = true;
    bool playerinsight = false;

    float detectcooldown = 20;
    float _detecttimer = 20;


    int maxhp = 0;


    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        triangulation = NavMesh.CalculateTriangulation();  
        SetNewTarget();

        maxhp = Health;
    }

    void Update()
    {
        if (Player == null || Health <= 0) return;

        float distanceToPlayer = Vector3.Distance(transform.position, Player.position);
        float angleToPlayer = Vector3.Angle(transform.forward, Player.position - transform.position);
        if (distanceToPlayer < playerDetectRange && angleToPlayer < playerDetectAngle)
        {
            Physics.SphereCast(transform.position + (transform.up * 2f), 0.25f, Player.position - (transform.position + (transform.up * 2f)), out RaycastHit hit);
            print("Distance to player: " + distanceToPlayer + ", Angle to player: " + angleToPlayer + ", Hit: " + (hit.collider != null ? hit.collider.name : "None"));
            playerinsight = (hit.collider != null && hit.collider.tag == "Player");
            if (playerinsight && Vector3.Distance(transform.position, Player.position) < playerAttackRange)
            {
                if (_heavyAttackTimer >= heavyattackCd && Health < maxhp/2)
                {
                    anim.ResetTrigger("smallattack");
                    anim.SetTrigger("largeattack");
                    _heavyAttackTimer = 0f;
                }                
                else if (_basicAttackTimer >= basicattackCd)
                {                    
                    anim.SetTrigger("smallattack");                    
                    _basicAttackTimer = 0f;
                }
            }
            if (playerinsight) currentTarget = Player.position; 
            
            if(_detecttimer >= detectcooldown)
            {
                audioSrc.PlayOneShot(detect);
                _detecttimer = 0f;
            }
        }
        else
            playerinsight = false;
        agent.SetDestination(currentTarget);
        if ((agent.remainingDistance <= agent.stoppingDistance || agent.pathStatus != NavMeshPathStatus.PathComplete) && !playerinsight)
        {
            SetNewTarget();
        }
        

        agent.speed = Health < maxhp / 2 ? runningSpeed : walkingSpeed;
        agent.speed *= move ? 1 : 0;
        anim.SetFloat("speed", move?(agent.velocity.magnitude - walkingSpeed) / (runningSpeed - walkingSpeed)+0.15f:0);
        print(agent.pathStatus);

        _basicAttackTimer += Time.deltaTime;
        _heavyAttackTimer += Time.deltaTime;
        _detecttimer += Time.deltaTime;
    }

    void SetNewTarget()
    {       
        currentTarget = triangulation.vertices[Random.Range(0, triangulation.vertices.Length)];
    }

    void takeDamage(int amt)
    {
        Health-=amt;        
        if (!playerinsight)
        {
            currentTarget = transform.position - transform.forward * 5f + (Random.Range(-2,2)*transform.right); // turn around when hit
        }
        if (Health < 0)
        {
            audioSrc.Stop();
            audioSrc.PlayOneShot(die);
            anim.ResetTrigger("smallhit");
            anim.ResetTrigger("largehit");
            anim.SetFloat("speed", 0);
            anim.SetBool("dead", true);
        }
    }

    void TakeSmallDamage()
    {
        takeDamage(1);
        if (Health > 0)     
        {
            audioSrc.PlayOneShot(hit);
            anim.SetTrigger("smallhit");
        }
    }

    void TakeLargeDamage()
    {
        takeDamage(2);
        if (Health > 0)
        {
            audioSrc.PlayOneShot(hit);
            anim.SetTrigger("largehit");
        }
    }


    void StopMoving()
    {
        move = false;
    }

    void StartMoving()
    {
        move = true;
    }

    void SmallAttackBegin()
    {
        audioSrc.PlayOneShot(attack);
    }

    void LargeAttackBegin()
    {
        audioSrc.PlayOneShot(attack2);
    }

    void SmallAttack()
    {
        GameObject fireballInstance = Instantiate(fireball, attacklocation1.position, transform.rotation);
        fireballInstance.GetComponent<Rigidbody>().linearVelocity = fireballInstance.transform.forward * 10f;
    }

    void LargeAttack()
    {
        for (int i = -2; i <= 2; i++)
        {
            GameObject fireballInstance = Instantiate(fireball, attacklocation2.position, transform.rotation);
            fireballInstance.GetComponent<Rigidbody>().linearVelocity = fireballInstance.transform.forward * 10f;
            fireballInstance.transform.Rotate(0, i * 15f, 0);
            fireballInstance.transform.Translate(i * 0.5f, 0, 0);
        }
    }

    public void ReactToHit(float amt)
    {
        if (Health <= 0) return;
        if(amt>=2)        
            TakeLargeDamage();        
        else
            TakeSmallDamage();
    }
}
