using System.Collections;
using System.Linq;
using UnityEngine;

public class BowWeapon : MonoBehaviour
{
    private AudioSource audioSource;
    Animator anim;

    [SerializeField] Transform FollowTarget;

    [SerializeField]
    float MAXDRAWDURATION = 1;

    float drawamount = 0;
    bool isdrawing = false;
    bool candraw = true;

    [SerializeField]
    GameObject arrowPlaceholder;
    [SerializeField]
    Arrow arrowPrefab;

    BallShooter ballShooter;

    [SerializeField]
    AudioClip Draw, loose;

    

    private void Start()
    {
        anim = GetComponent<Animator>();
        ballShooter = FindFirstObjectByType<BallShooter>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (FollowTarget != null)
        {
            transform.position = Vector3.Lerp(transform.position, FollowTarget.position, 50 * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, FollowTarget.rotation, 50 * Time.deltaTime);
        }    
        if(candraw && Input.GetMouseButtonDown(0))
        {
            isdrawing = true;
            candraw = false;
            StartCoroutine(ShootingRoutine());
        }
    }

    IEnumerator ShootingRoutine()
    {
        anim.SetBool("Draw", true);
        audioSource.PlayOneShot(Draw);
        while (isdrawing || drawamount < (MAXDRAWDURATION / 3.5))
        {
            if (drawamount < MAXDRAWDURATION)
            {
                drawamount += Time.deltaTime;
            }
            else
                drawamount = MAXDRAWDURATION;
            isdrawing = Input.GetMouseButton(0);
            yield return null;
        }        
        anim.SetBool("Draw", false);
        audioSource.Stop();
        audioSource.PlayOneShot(loose);
        yield return new WaitForSeconds(0.3f);
        arrowPlaceholder.SetActive(true);
        candraw = true;
        drawamount = 0;
    }

    private void Shoot()
    {
        arrowPlaceholder.SetActive(false);
        var direction = Quaternion.FromToRotation(Vector3.forward, ballShooter.GetRayDirection(arrowPlaceholder.transform.position));
        var arrow = Instantiate(arrowPrefab, arrowPlaceholder.transform.position, direction);
        arrow.speed *= Mathf.Pow(drawamount / MAXDRAWDURATION,2);             
        
    }

    private void OnEnable()
    {
        candraw = true;
        drawamount = 0;
    }
}
