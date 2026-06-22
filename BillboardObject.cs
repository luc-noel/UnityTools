using UnityEngine;

public class BillboardObject : MonoBehaviour
{
    [Tooltip("Use a custom target for the billboards to face? Defaults to the Main Camera otherwise.")]
    public Transform target;

    [Header("Rotation Locks")]
    public bool lockXAxis;
    public bool lockYAxis;
    public bool lockZAxis;

    private Vector3 _lock;

    private void Start()
    {
        if (!target && Camera.main)
            target = Camera.main.transform;
        else
            Debug.LogWarning("BillboardObject on " + this.name + " is missing a valid target.");
    }

    void OnEnable()
    {
        _lock = new Vector3(lockXAxis ? 0.0f : 1.0f, lockYAxis ? 0.0f : 1.0f, lockZAxis ? 0.0f : 1.0f);
    }

    void LateUpdate()
    {
        if (!target)
            return;

        Vector3 newRotation = Quaternion.LookRotation(target.position - transform.position).eulerAngles;

        // Multiply by the lock Vector3 to zero out values
        transform.eulerAngles = Vector3.Scale(newRotation, _lock);
    }
}