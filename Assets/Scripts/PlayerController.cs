using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {

    Vector3 velocity;
    Rigidbody myRigidbody;

	// Use this for initialization
	void Start () {
        myRigidbody = GetComponent<Rigidbody>();

    }

    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;
    }

    public void LookAt(Vector3 lookPoint)
    {
        Vector3 heightCorrectedPpint = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);
        transform.LookAt(heightCorrectedPpint);
    }

    void FixedUpdate()
    {
        myRigidbody.MovePosition(myRigidbody.position + velocity * Time.fixedDeltaTime);
    }
    //// Update is called once per frame
    //void Update () {

    //}
}
