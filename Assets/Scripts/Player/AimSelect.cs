using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class AimSelect : MonoBehaviour
{
    [SerializeField] float aimWidth;
    [SerializeField] float maxDistance;
    [SerializeField] LayerMask layers;

    NPCSelectable currentHovered;
    IWorldUISelectable worldUIHovered;

    public event Action<NPCSelectable> onHover;
    public event Action<NPCSelectable> onSelect;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerState.Instance.AreInputFrozen)
        {
            DeselectNPC();
            return;
        }

        Aim3D();
        AimUI();
    }

    void AimUI()
    {
        PointerEventData pointer = new PointerEventData(EventSystem.current);
        pointer.position = new Vector2(Screen.width, Screen.height) * 0.5f;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, results);

        IWorldUISelectable newHovered = null;
        foreach (RaycastResult result in results)
        {
            if (result.gameObject.TryGetComponent(out IWorldUISelectable worldUI))
            {
                newHovered = worldUI;
                break;
            }
        }

        if (worldUIHovered != null && worldUIHovered != newHovered)
            worldUIHovered.OnHoverExit();

        worldUIHovered = newHovered;
        if (newHovered != null)
        {
            newHovered.OnHoverEnter();
        }

        //Click
        if (Mouse.current.leftButton.wasPressedThisFrame && worldUIHovered != null)
        {
            worldUIHovered.OnClick();
        }
    }

    void Aim3D()
    {
        Ray ray = new Ray(transform.position, Camera.main.transform.forward);
        RaycastHit[] hits = Physics.SphereCastAll(ray, aimWidth, maxDistance, layers);

        float minDist = Mathf.Infinity;
        NPCSelectable selectable = null;
        foreach (RaycastHit hit in hits)
        {
            if (!hit.collider.TryGetComponent(out NPCSelectable hitSelectable))
                continue;

            if (hit.distance < minDist)
            {
                minDist = hit.distance;
                selectable = hitSelectable;
            }
        }

        if (currentHovered != selectable && currentHovered != null)
        {
            currentHovered.OnSelectHoverExit();
        }

        currentHovered = selectable;

        if (selectable != null)
        {
            selectable.OnSelectHoverEnter();
            onHover?.Invoke(selectable);
        }

        if (Mouse.current.leftButton.wasPressedThisFrame && currentHovered != null)
        {
            currentHovered.NPCSelect();
            onSelect?.Invoke(currentHovered);
        }
    }

    void DeselectNPC()
    {
        currentHovered?.OnSelectHoverExit();
        currentHovered = null;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }
#endif
}
