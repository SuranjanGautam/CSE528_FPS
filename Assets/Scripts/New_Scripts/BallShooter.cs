using UnityEngine;
using System.Collections;

public class BallShooter : MonoBehaviour 
{

    [SerializeField] bool use_projectile_origin_location = false;
    [SerializeField] Transform projectile_origin;
    [SerializeField][Tooltip("Enable to set projectile target to a raycasted point, else shoot in forward direction")] bool RayCastTargetting = false;

    public GameObject Ball;
    public float Force = 50.0f;
    public Vector3 Torque = new Vector3(100, 0, 0);

    public  Gun currentGun;

    private bool cursorLocked = false;

	void Start() 
    {
        LockCursor();
    }

    void Update() 
    {
        if (Input.GetKeyDown(KeyCode.C))
            LockCursor();

        if (currentGun == null)
        {
            Debug.LogWarning("No gun found in the scene");
            return;
        }

        if (currentGun.ReadyToFire() && currentGun.WeaponType == GunType.SemiAuto && Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
        
        if(currentGun.ReadyToFire() && currentGun.WeaponType == GunType.Auto && Input.GetMouseButton(0))
        {
            Shoot();
        }
	}

    void Shoot()
    {
        if (!currentGun.gameObject.activeInHierarchy)
        {
            return;
        }
        //call Bang method to perform gun animation and sounds
        currentGun.Bang();

        // Note: transform.position returns object's position in the World space
        if (RayCastTargetting && use_projectile_origin_location)
        {
            projectile_origin.transform.forward = GetRayDirection(projectile_origin.position);
        }

        GameObject ball = (GameObject)Instantiate(Ball, transform.position,
            Quaternion.identity);
        Rigidbody ball_rb = ball.GetComponent<Rigidbody>();
        ball.name = "ball";
        // Fire the ball 2 unit forward from the camera

        ball.transform.position = transform.TransformPoint(2 * Vector3.forward);
        ball.transform.forward = transform.forward;

        if (use_projectile_origin_location)
        {
            ball.transform.position = projectile_origin.position;
            ball.transform.forward = projectile_origin.forward;
        }

        //ball_rb.velocity = transform.TransformDirection(new Vector3(0, 0, Force));
        if (ball.GetComponent<BallScript>() != null)
        {
            ball_rb.AddForce(ball_rb.transform.TransformDirection(new Vector3(0, 0, Force)));
            ball_rb.AddTorque(Torque);
        }
    }

    public Vector3 GetRayDirection(Vector3 origin) //helper function
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            return (hit.point - origin).normalized;
        }
        else
        {
            return (ray.GetPoint(20) - origin).normalized;
        }
    }

    void LockCursor()
    {
        if (!cursorLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            cursorLocked = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            cursorLocked = false;
        }
    }
}