using UnityEngine;

public class LookPlayerDirection : MonoBehaviour
{
    PlayerMovement movement;
    float angle, targetAngle, refAngle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movement = GetComponentInParent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 camVector = movement.playerCam.transform.forward;
        camVector.y = 0;
        targetAngle = Vector3.SignedAngle(Vector3.forward,camVector.normalized, Vector3.up);
        angle = Mathf.SmoothDampAngle(angle, targetAngle, ref refAngle, 0.1f);

        transform.rotation = Quaternion.Euler(0, angle, 0);
    }
}
