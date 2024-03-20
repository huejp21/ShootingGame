using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : LivingEntity
{
    public float moveSpeed = 5;
    public Crosshairs crosshairs;

    Camera viewCamera;
    PlayerController controller;
    GunController gunController;

    protected override void Start()
    {
        base.Start();
    }

    void Awake()
    {
        controller = GetComponent<PlayerController>();
        gunController = GetComponent<GunController>();
        viewCamera = Camera.main;
        FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
    }

    void OnNewWave(int waveNumber)
    {
        health = startingHealth;
        gunController.EquipGun(waveNumber - 1);
    }

    void Update()
    {
        // Movement input 
        if (Input.GetKey(KeyCode.W)
         || Input.GetKey(KeyCode.S)
         || Input.GetKey(KeyCode.A)
         || Input.GetKey(KeyCode.D))
        {
            int verticalPressed = 0;
            int horizontalPressed = 0;
            if (Input.GetKey(KeyCode.W))
            {
                verticalPressed = 1;
            }
            if (Input.GetKey(KeyCode.S))
            {
                verticalPressed = -1;
            }
            if (Input.GetKey(KeyCode.A))
            {
                horizontalPressed = -1;
            }
            if (Input.GetKey(KeyCode.D))
            {
                horizontalPressed = 1;
            }
            Vector3 moveInput = new Vector3(horizontalPressed, 0, verticalPressed);
            Vector3 moveVelocity = moveInput.normalized * moveSpeed;
            controller.Move(moveVelocity);
        }
        else
        {
            Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            Vector3 moveVelocity = moveInput.normalized * moveSpeed;
            controller.Move(moveVelocity);
        }

        // Look input 
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.up * gunController.GunHeight);
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            //Debug.DrawLine(ray.origin,point,Color.red); 
            controller.LookAt(point);
            crosshairs.transform.position = point;
            crosshairs.DetectTargets(ray);
            if ((new Vector2(point.x, point.z) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude > 1)
            {
                gunController.Aim(point);
            }
        }

        // Weapon input 
        if (Input.GetMouseButton(0))
        {
            gunController.OnTriggerHold();
        }
        if (Input.GetMouseButtonUp(0))
        {
            gunController.OnTriggerRelease();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            gunController.Reload();
        }

        if (transform.position.y < -10)
        {
            TakeDamage(health);
        }
    }

    public override void Die()
    {
        AudioManager.instance.PlaySound("Player Death", transform.position);
        base.Die();
    }
}



//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//[RequireComponent(typeof(PlayerController))]
//[RequireComponent(typeof(GunController))]
//public class Player : LivingEntity {

//    public float moveSpeed = 5.0f;

//    public Crosshairs crosshairs;

//    Camera viewCamera;
//    PlayerController controller;
//    GunController gunController;

//    // Use this for initialization
//    protected override void Start () {
//        base.Start();

//    }

//    void Awake()
//    {
//        controller = GetComponent<PlayerController>();
//        gunController = GetComponent<GunController>();
//        viewCamera = Camera.main;
//        FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
//    }

//    void OnNewWave(int waveNumber)
//    {
//        health = startingHealth;
//        gunController.EquipGun(waveNumber - 1);
//    }

//	// Update is called once per frame
//	void Update () {
//        // Movement input
//        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
//        Vector3 moveVelocity = moveInput.normalized * moveSpeed;
//        controller.Move(moveVelocity);

//        // Look input
//        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
//        Plane groundPlane = new Plane(Vector3.up, Vector3.up * gunController.GunHeight);
//        float rayDistance;

//        if (groundPlane.Raycast(ray, out rayDistance))
//        {
//            Vector3 point = ray.GetPoint(rayDistance);
//            //Debug.DrawLine(ray.origin, point, Color.red);
//            controller.LookAt(point);
//            crosshairs.transform.position = point;
//            crosshairs.DetectTargets(ray);
//            if ((new Vector2(point.x, point.z) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude > 1.1)
//            {
//                gunController.Aim(point);
//            }
//        }

//        // Weapon input
//        if (Input.GetMouseButton(0))
//        {
//            gunController.OntriggerHold();
//        }
//        if (Input.GetMouseButtonUp(0))
//        {
//            gunController.OntriggerRelease();
//        }
//        if (Input.GetKeyDown(KeyCode.R))
//        {
//            gunController.Reload();
//        }
//        if (transform.position.y < -10)
//        {
//            TakeDamage(health);
//        }
//    }

//    public override void Die()
//    {
//        AudioManager.instance.PlaySound("Player Death", transform.position);
//        base.Die();
//    }
//}
