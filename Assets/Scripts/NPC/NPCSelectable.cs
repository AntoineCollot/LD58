using UnityEngine;
using UnityEngine.Events;

public class NPCSelectable : MonoBehaviour
{
    bool isHovered;
    [SerializeField] GameObject model;
    [SerializeField] bool lookPlayer;

    Quaternion refRotate;
    const float LOOK_SMOOTH = 0.3f;
    Animator anim;

    const string SELECTED_PARAM = "Selected";

    private void Start()
    {
        anim = GetComponentInChildren<Animator>(true);

        if (anim != null)
            anim.SetFloat(SELECTED_PARAM, 1);
    }

    private void Update()
    {
        if (lookPlayer && isHovered)
            LookPlayerSmooth();
    }

    void LookPlayerSmooth()
    {
        Vector3 toPlayer = transform.position - PlayerState.Instance.transform.position;
        toPlayer.y = 0;
        Quaternion target = Quaternion.LookRotation(-toPlayer, Vector3.up);
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

        if (anim != null)
            anim.SetFloat(SELECTED_PARAM, 2);

        SFXManager.PlaySound(GlobalSFX.HoverNPC,gameObject.GetInstanceID());
    }

    public void OnSelectHoverExit()
    {
        if (!isHovered)
            return;
        isHovered = false;

        //move out of outline layer
        model.gameObject.layer = LayerMask.NameToLayer("NPC");

        gameObject.SendMessage("HoverExitMessage", SendMessageOptions.DontRequireReceiver);

        if (anim != null)
            anim.SetFloat(SELECTED_PARAM, 1);
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