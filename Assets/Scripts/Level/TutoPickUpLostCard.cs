using UnityEngine;

public class TutoPickUpLostCard : MonoBehaviour
{
    [SerializeField] LostCard lostCard;

    void Update()
    {
        transform.LookAt(PlayerState.Instance.transform.position);
        if (lostCard.hasBeenPickedUp)
            gameObject.SetActive(false);
    }
}
