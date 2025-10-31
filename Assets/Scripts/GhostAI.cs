using UnityEngine;

public class GhostAI : MonoBehaviour
{
    public float speed = 3f;
    public float turnSpeed = 6f;
    public float sightRange = 8f;
    public LayerMask obstacleMask;
    public Transform target; // assign player at runtime

    private Vector3 wanderDir;
    private float changeDirTimer;

    void Start()
    {
        PickNewWander();
        var gm = FindObjectOfType<GameManager>();
        if (gm) target = gm.player;
    }

    void Update()
    {
        bool sees = CanSeeTarget();
        Vector3 desired;
        if (sees && target)
            desired = (target.position - transform.position).normalized;
        else
        {
            changeDirTimer -= Time.deltaTime;
            if (changeDirTimer <= 0) PickNewWander();
            desired = wanderDir;
        }
        Vector3 forward = Vector3.Slerp(transform.forward, desired, turnSpeed * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(new Vector3(forward.x, 0, forward.z));
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    bool CanSeeTarget()
    {
        if (!target) return false;
        if (Vector3.Distance(transform.position, target.position) > sightRange) return false;
        Vector3 dir = (target.position - transform.position).normalized;
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, dir, out RaycastHit hit, sightRange, ~obstacleMask))
            return hit.transform.CompareTag("Player");
        return false;
    }

    void PickNewWander()
    {
        changeDirTimer = Random.Range(1.5f, 3.5f);
        Vector2 v = Random.insideUnitCircle.normalized;
        wanderDir = new Vector3(v.x, 0, v.y);
    }
}
