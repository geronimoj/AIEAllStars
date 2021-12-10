using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleHook : Bullet
{
    private bool hitTarget = false;

    private LineRenderer _renderer = null;

    private Transform _grapplePoint = null;

    protected override void Start()
    {
        base.Start();

        _renderer = GetComponent<LineRenderer>();
        _grapplePoint = attacker.GetComponentInChildren<GrapplePoint>().transform;
    }

    private void OnDestroy()
    {
        if (!hitTarget)
            attacker.GetComponent<Animator>().SetTrigger("Fail");
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.transform == attacker)
            return;
        //If we hit a wall or player, zip to them
        if (other.CompareTag("Wall") || other.TryGetComponent<Player>(out Player p))
        {
            attacker.GetComponent<Animator>().SetTrigger("Succeed");
            hitTarget = true;
            attacker.GetComponent<VibeCityPlayer>().ZipToTarget(transform.position);
            Destroy(gameObject);
        }
    }

    protected override void Update()
    {
        base.Update();

        if (_renderer && _grapplePoint)
        {
            _renderer.SetPosition(0, transform.position);
            _renderer.SetPosition(1, _grapplePoint.position);
        }
    }
}
