using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    //bool for keeping track of gun's ability to fire
    private bool readyToFire;
    public GunType WeaponType;
    public float firerate = 200;
    float cooldown = 20000;

    [SerializeField] private AudioClip gunShot;
    private AudioSource audioSource;
    Animator anim;

    [SerializeField] Transform FollowTarget;

    void Start ()
    {
        readyToFire = true;
        audioSource = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (FollowTarget != null)
        {     
            if(Vector3.Distance(transform.position, FollowTarget.position) < 1f)
                transform.position = Vector3.Lerp(transform.position, FollowTarget.position, 50 * Time.deltaTime);
            else
                transform.position = FollowTarget.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, FollowTarget.rotation, 50 * Time.deltaTime);
        }
        cooldown += Time.deltaTime;
    }

    public void Bang()
    {
        anim.SetTrigger("Fire");
        if (audioSource != null)
        {            
            audioSource.PlayOneShot(gunShot);
        }
        cooldown = 0;
    }

    //accessor for checking gun's state
    public bool ReadyToFire()
    {
        return cooldown>60f/firerate;
    }
}

public enum GunType
{
    SemiAuto,
    Auto
}
