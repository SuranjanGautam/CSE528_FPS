using UnityEngine;
using System.Collections;


public class WanderingAI : MonoBehaviour 
{
	public float speed = 3.0f;  // Wandering forward speed
	public float obstacleRange = 2.0f;

    public float Force = 50.0f;
    public Vector3 Torque = new Vector3(100, 0, 0);

    [SerializeField] private GameObject fireballPrefab;
    private GameObject _fireball;

    CharacterController charcontrol;

    private bool _alive;

    float yvel = 0;

    bool useray = true;

    void Start()
    {
        _alive = true;
        charcontrol = GetComponent<CharacterController>();        
    }

    void Update()
    {
        if (!_alive) return; // this enemy may die before this enemy game object is destroyed

        Ray ray = new Ray(transform.position, transform.forward);
		RaycastHit hit;
        
        if ((useray?Physics.Raycast(ray,  out hit) : Physics.SphereCast(ray,0.25f,out hit)))
        {
            GameObject hitObject = hit.transform.gameObject;

            //print($"{Vector3.Angle(hit.normal, -ray.direction)} dot {Vector3.Dot(hit.normal, Vector3.up)}");
            
            if (hitObject.tag == "Player")
            {
                // Attack the player
                if (_fireball == null)
                {
                    _fireball = Instantiate(fireballPrefab);
                    _fireball.transform.position = transform.TransformPoint(Vector3.forward * 1.5f);
                    _fireball.transform.rotation = transform.rotation;
                    _fireball.GetComponent<Rigidbody>().linearVelocity =
                                transform.TransformDirection(new Vector3(0, 0, Force));
                    _fireball.GetComponent<Rigidbody>().AddTorque(Torque);
                }
            }
            else if (hit.distance < obstacleRange && (Vector3.Angle(hit.normal, -ray.direction) > charcontrol.slopeLimit || Vector3.Dot(hit.normal,Vector3.up) <= 0))// && hitObject.tag != "Fire")
            {               
                float angle = Random.Range(-110.0f, 110.0f);
                transform.Rotate(0, angle, 0);
            }
        }
        Vector3 movement = transform.forward * speed;
        if (!charcontrol.isGrounded)
            yvel += -9.8f * Time.deltaTime;
        else
            yvel = -9.8f * Time.deltaTime;

        movement += Vector3.up * yvel;

        charcontrol.Move(movement * Time.deltaTime);
        useray = charcontrol.velocity.magnitude > 0.9f * speed;
        //transform.Translate(0, 0, speed * Time.deltaTime);
    }

    public void SetAlive(bool alive)
    {
        _alive = alive;
    }

    public bool IsAlive()
    {
        return _alive;
    }
}
