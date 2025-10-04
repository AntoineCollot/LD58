using System.Security.Cryptography;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public bool keepOffset = false;
    [SerializeField] Transform target;
    Vector3 offset;

    private void Start()
    {
        offset = transform.position - target.position;
    }

    void LateUpdate()
    {
        Vector3 targetPos = target.position;
        if (keepOffset)
            targetPos += offset;
        transform.position = targetPos;
    }
}
