using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ThrowingScript : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("References")]
    public Transform cam;
    public Transform attackPoint;
    public GameObject objectToThrow;

    [Header("Settings")]
    public int totalThrows;
    public float throwCooldown;

    [Header("Throwing")]
    public KeyCode throwKey = KeyCode.Mouse0;
    public float throwForce;
    public float throwUpwardForce;
    bool readyToThrow;

    private void Start() 
    {
        readyToThrow = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(throwKey) && readyToThrow && totalThrows > 0) 
        {
            Throw();
        }
    }

    private void Throw()
    {
        readyToThrow = true;

        //  instantiate object to throw
        GameObject projectile = Instantiate(objectToThrow, attackPoint.position, cam.rotation);

        //  get rigidbody component
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        //  calculate direction -- angles thrown projectile (launched from arm) towards crosshair (center) -- (likely not needed)

        Vector3 forceDirection = cam.transform.forward;
        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, 500f))
        {
            forceDirection = (hit.point - attackPoint.position).normalized;
        }
        Vector3 forceToAdd = forceDirection * throwForce + transform.up * throwUpwardForce;

        //  add force
        //  Vector3 forceToAdd = cam.transform.forward * throwForce + transform.up * throwUpwardForce;

        projectileRb.AddForce(forceToAdd, ForceMode.Impulse);

        totalThrows--;

        //  implement throwCooldown
        Invoke(nameof(ResetThrow), throwCooldown);
    }

    private void ResetThrow() 
    {
        readyToThrow = true;
    }
}
