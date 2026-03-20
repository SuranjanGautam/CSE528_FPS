using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{    
    public float speed = 20f;
    [SerializeField]
    float lifetime = 2f;
    [SerializeField]
    bool usegravity = false;

    GameObject GM;  // GameManager
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GM = GameObject.Find("GameManager");
        StartCoroutine(SelfDestruct(lifetime));

        var rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.useGravity = usegravity;
        rb.linearVelocity = transform.forward * speed;
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
                enemy.SendMessage("ReactToHit");
            }            
        }

        if (collisionInfo.gameObject.GetComponent<IReactiveTarget>() != null)
        {
            IReactiveTarget target = collisionInfo.gameObject.GetComponent<IReactiveTarget>();
            target.ReactToHit();            
        }
        DestoryAfterHit();
    }

    void DestoryAfterHit()
    {
        GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        GetComponent<Collider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
        StartCoroutine(SelfDestruct(0.2f));
    }

    private IEnumerator SelfDestruct(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
