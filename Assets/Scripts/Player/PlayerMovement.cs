using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    Rigidbody body;
    public Camera playerCam;

    InputMap inputMap;
    Vector3 moveForce;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inputMap = new InputMap();
        inputMap.Enable();
        body = GetComponent<Rigidbody>();
    }

    private void OnDestroy()
    {
        if (inputMap != null)
        {
            inputMap.Disable();
            inputMap.Dispose();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!PlayerState.Instance.CanMove)
            return;

        Vector2 inputs = inputMap.Main.Move.ReadValue<Vector2>();
        inputs.Normalize();

        //Lateral
        Vector3 lateralMove = playerCam.transform.right;
        lateralMove.y = 0;
        lateralMove.Normalize();
        lateralMove*= inputs.x * moveSpeed;

        //Forward
        Vector3 forwardMove = playerCam.transform.forward;
        forwardMove.y = 0;
        forwardMove.Normalize();
        forwardMove *= inputs.y * moveSpeed;

        moveForce = lateralMove + forwardMove;
    }

    private void FixedUpdate()
    {
        if (!PlayerState.Instance.CanMove)
            return;

        body.AddForce(moveForce, ForceMode.Acceleration);
    }
}
