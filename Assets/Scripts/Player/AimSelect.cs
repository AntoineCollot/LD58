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

    InputMap inputMap;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inputMap = new InputMap();
        inputMap.Enable();
        inputMap.Main.Click.performed += OnClickPerformed;
    }

    private void OnDestroy()
    {
        if (inputMap != null)
        {
            inputMap.Main.Click.performed -= OnClickPerformed;
            inputMap.Disable();
            inputMap.Dispose();
        }
    }

    private void OnClickPerformed(InputAction.CallbackContext obj)
    {
        //Click
        if (worldUIHovered != null)
        {
            worldUIHovered.OnClick();
        }
        else if (currentHovered != null)
        {
            currentHovered.NPCSelect();
            onSelect?.Invoke(currentHovered);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!PlayerState.Instance.CanClick)
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
        float minDist = maxDistance;
        foreach (RaycastResult result in results)
        {
            if (result.gameObject.TryGetComponent(out IWorldUISelectable worldUI))
            {
                Vector3 toUI = result.gameObject.transform.position - transform.position;
                toUI.y = 0;
                float dist = toUI.magnitude;
                if (dist < minDist)
                {
                    minDist = dist;
                    newHovered = worldUI;
                }
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
