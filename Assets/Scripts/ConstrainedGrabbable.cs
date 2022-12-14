using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class ConstrainedGrabbable : OVRGrabbable
{
    [SerializeField]
    private Transform _handle;
    Rigidbody _handleRB;
    private bool _grabbed;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        _handleRB = _handle.GetComponent<Rigidbody>();
    }

    public override void GrabBegin(OVRGrabber hand, Collider grabPoint)
    {
        base.GrabBegin(hand, grabPoint);
        StartCoroutine(UpdateHandle());
        grabbedBy.GetComponentInChildren<Renderer>().enabled = false;
    }

    // Update is called once per frame
    IEnumerator UpdateHandle()
    {
        _grabbed = true;
        while (_grabbed) 
        {
            _handleRB.MovePosition(transform.position);
            yield return null;
        }
    }

    public override void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity) 
    {
        _grabbed = false;
        transform.position = _handle.position;
        transform.rotation = _handle.rotation;
        grabbedBy.GetComponentInChildren<Renderer>().enabled = true;
        base.GrabEnd(linearVelocity, angularVelocity);

    }
}
