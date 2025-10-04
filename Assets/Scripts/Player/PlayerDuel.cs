using UnityEngine;

public class PlayerDuel : MonoBehaviour
{
    AimSelect selectManager;
    CardDuelist playerDuelist;

    void Awake()
    {
        selectManager = GetComponent<AimSelect>();
        playerDuelist = GetComponent<CardDuelist>();
    }

    void Start()
    {
        selectManager.onSelect += OnSelect;
    }

    private void OnDestroy()
    {
        if (selectManager != null)
            selectManager.onSelect -= OnSelect;
    }

    private void OnSelect(NPCSelectable obj)
    {
        if (!obj.TryGetComponent(out CardDuelist otherDuelist))
            return;

        CardDuelManager.Instance.StartDuel(playerDuelist, otherDuelist);
    }
}
