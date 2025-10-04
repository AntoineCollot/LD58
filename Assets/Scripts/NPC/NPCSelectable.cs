using UnityEngine;
using UnityEngine.Events;

public class NPCSelectable : MonoBehaviour
{
    bool isHovered;
    [SerializeField] MeshRenderer model;
    [SerializeField] bool lookPlayer;

    Quaternion refRotate;
    const float LOOK_SMOOTH = 0.4f;

    private void Update()
    {
        if (lookPlayer && isHovered)
            LookPlayerSmooth();
    }

    void LookPlayerSmooth()
    {
        Vector3 toPlayer = transform.position - PlayerState.Instance.transform.position;
        toPlayer.y = 0;
        Quaternion target = Quaternion.LookRotation(toPlayer, Vector3.up);
        transform.rotation =QuaternionUtils.SmoothDamp(transform.rotation, target, ref refRotate, LOOK_SMOOTH);
    }

    public void OnSelectHoverEnter()
    {
        if (isHovered)
            return;
        isHovered = true;
        //move to outline layer
        model.gameObject.layer = LayerMask.NameToLayer("NPCOutline");

        gameObject.SendMessage("HoverEnterMessage",SendMessageOptions.DontRequireReceiver);
    }

    public void OnSelectHoverExit()
    {
        if (!isHovered)
            return;
        isHovered = false;

        //move out of outline layer
        model.gameObject.layer = LayerMask.NameToLayer("NPC");

        gameObject.SendMessage("HoverExitMessage", SendMessageOptions.DontRequireReceiver);
    }

    public void NPCSelect()
    {
        gameObject.SendMessage("NPCSelectMessage", SendMessageOptions.DontRequireReceiver);
    }
}

public interface INPCSelectable
{
    void HoverEnterMessage();
    void HoverExitMessage();
    void NPCSelectMessage();
}