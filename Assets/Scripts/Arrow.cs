using System.Collections;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float speed = 20f;
    [SerializeField]
    float lifetime = 2f;
    [SerializeField]
    bool usegravity = false;

    GameObject GM;  // GameManager

    Rigidbody rb;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GM = GameObject.Find("GameManager");
        StartCoroutine(SelfDestruct(lifetime));
        FireArrow();
    }

    public void FireArrow()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.useGravity = usegravity;
        rb.linearVelocity = transform.forward * speed;

        print("Arrow launched with velocity: " + speed);
    }


    private void OnCollisionEnter(Collision collisionInfo)
    {
        if (collisionInfo.gameObject.tag == "enemy") //&& 
                                                     //collisionInfo.impactForceSum.magnitude > targetThresh)
        {
            GameObject enemy = collisionInfo.gameObject;
            WanderingAI ai = enemy.GetComponent<WanderingAI>();
            if (ai != null && ai.IsAlive())
            {
                ai.SetAlive(false);
                if (GM != null) GM.SendMessage("EnemyHit");
                //enemy.SendMessage("ReactToHit");
            }
            transform.SetParent(collisionInfo.transform, true);
        }

        if (collisionInfo.gameObject.GetComponent<IReactiveTarget>() != null)
        {
            transform.SetParent(collisionInfo.transform, true);
            IReactiveTarget target = collisionInfo.gameObject.GetComponent<IReactiveTarget>();
            target.ReactToHit(2);
        }
        
        DestoryAfterHit();
    }

    void DestoryAfterHit()
    {
        GetComponent<TrailRenderer>().emitting = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;
        GetComponentInChildren<Collider>().enabled = false;
        transform.position += transform.forward * 0.2f;
        StartCoroutine(SelfDestruct(lifetime));
    }

    private IEnumerator SelfDestruct(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

    private void Update()
    {
        if(rb.linearVelocity.sqrMagnitude > 25)
            transform.forward = GetComponent<Rigidbody>().linearVelocity.normalized;
    }
}
