using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAddon : MonoBehaviour
{
    public Rigidbody rb;
    private bool targetHit;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision) 
    {
        if (targetHit)
            return;
        else
            targetHit = true;

        //  make sure projectile sticks to surface
        rb.isKinematic = true;

        //  make sure projectile moves with target
        transform.SetParent(collision.transform);
    }

}
