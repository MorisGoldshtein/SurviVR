using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Nokobot/Modern Guns/Simple Shoot")]
public class SimpleShoot : MonoBehaviour
{
    [Header("Prefab Refrences")]
    public GameObject bulletPrefab;
    public GameObject casingPrefab;
    public GameObject muzzleFlashPrefab;

    [Header("Location Refrences")]
    [SerializeField] private Animator gunAnimator;
    [SerializeField] private Transform barrelLocation;
    [SerializeField] private Transform casingExitLocation;

    [Header("Settings")]
    [Tooltip("Specify time to destory the casing object")] [SerializeField] private float destroyTimer = 2f;
    [Tooltip("Bullet Speed")] [SerializeField] private float shotPower = 500f;
    [Tooltip("Casing Ejection Speed")] [SerializeField] private float ejectPower = 150f;

    public AudioSource source;
    public AudioClip fireSound;
    public SpawnInCube spawner;

    private bool shooter = true;
    public OVRGrabbable ss;
    public ScoreSystem scoreSys;

    LineRenderer laserLine;

    void Start()
    {
        if (barrelLocation == null)
            barrelLocation = transform;

        if (gunAnimator == null)
            gunAnimator = GetComponentInChildren<Animator>();

        laserLine = GetComponent<LineRenderer>();
    }

    void shooterFunc()
    {
        shooter = true;
    }

    void FixedUpdate()
    {
        bool rightTrigger = OVRInput.Get(OVRInput.RawButton.RIndexTrigger);
        //If you want a different input, change it here

        if (ss.isGrabbed && rightTrigger && shooter)
        {
            shooter = false;
            //Calls animation on the gun that has the relevant animation events that will fire
            gunAnimator.SetTrigger("Fire");
        }
    }

    public void ShootAnim()
    {
        gunAnimator.SetTrigger("Fire");
    }

    void RayTest()
    {
        RaycastHit hit;
        laserLine.SetPosition(0, barrelLocation.position);

        if (Physics.Raycast(barrelLocation.position, barrelLocation.forward, out hit, 1000))
        {
            laserLine.SetPosition(1, hit.point);
            if (hit.transform.gameObject.tag == "Target") 
            {
                hit.transform.gameObject.GetComponent<enemy>().scoreUpdater();
                Destroy(hit.transform.gameObject);
            }
        }
        else
        {
            laserLine.SetPosition(1, barrelLocation.position + (barrelLocation.forward * 1000));
        }
        StartCoroutine(ShootLaser());
    }

    IEnumerator ShootLaser()
    {
        laserLine.enabled = true;
        yield return new WaitForSeconds(0.2f);
        laserLine.enabled = false;
    }

    //This function creates the bullet behavior
    void Shoot()
    {
        //custom noise clip
        source.PlayOneShot(fireSound);

        if (muzzleFlashPrefab)
        {
            //Create the muzzle flash
            GameObject tempFlash;
            tempFlash = Instantiate(muzzleFlashPrefab, barrelLocation.position, barrelLocation.rotation);

            //Destroy the muzzle flash effect
            Destroy(tempFlash, destroyTimer);
        }

        //cancels if there's no bullet prefeb
        if (!bulletPrefab)
        { return; }
    }

    //This function creates a casing at the ejection slot
    void CasingRelease()
    {
        //Cancels function if ejection slot hasn't been set or there's no casing
        if (!casingExitLocation || !casingPrefab)
        { return; }

        //Create the casing
        GameObject tempCasing;
        tempCasing = Instantiate(casingPrefab, casingExitLocation.position, casingExitLocation.rotation) as GameObject;
        //Add force on casing to push it out
        tempCasing.GetComponent<Rigidbody>().AddExplosionForce(Random.Range(ejectPower * 0.7f, ejectPower), (casingExitLocation.position - casingExitLocation.right * 0.3f - casingExitLocation.up * 0.6f), 1f);
        //Add torque to make casing spin in random direction
        tempCasing.GetComponent<Rigidbody>().AddTorque(new Vector3(0, Random.Range(100f, 500f), Random.Range(100f, 1000f)), ForceMode.Impulse);

        //Destroy casing after X seconds
        Destroy(tempCasing, destroyTimer);
    }

}
