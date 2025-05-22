using UnityEngine;

public class AIInput : MonoBehaviour
{
    public GameObject self;
    public GameObject target;

    [SerializeField]
    private float detectionRange = 200f;

    private void Update()
    {
        UpdateTarget();
    }

    private void UpdateTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRange);

        float closestDistance = float.MaxValue;
        GameObject closest = null;

        foreach (var hit in hits)
        {
            if (hit.gameObject == self)
            {
                continue;
            }
            if (!hit.gameObject.TryGetComponent<Entity>(out Entity t))
            { 
                continue;
            }

            float distance = Vector3.Distance(self.transform.position, hit.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = hit.gameObject;
            }
        }

        target = closest; // ���� ����� Ÿ���� ����
    }

    public float DistanceToTarget()
    {
        if (self == null || target == null)
        {
            return float.MaxValue;
        }

        return Vector3.Distance(target.transform.position, self.transform.position);
    }
}
