using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection);
    void TakeDamage(float damage);
}

//public class IDamageable : MonoBehaviour {

//	// Use this for initialization
//	void Start () {
		
//	}
	
//	// Update is called once per frame
//	void Update () {
		
//	}
//}
