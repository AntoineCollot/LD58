using UnityEngine;

public class TutoDontForget : MonoBehaviour
{
    [SerializeField] GameObject panel;
   CardDuelist playerDuelist;

    private void Start()
    {
        panel.SetActive(false);
        playerDuelist = PlayerState.Instance.GetComponent<CardDuelist>();
    }

    void Update()
    {
        //activate
        int cardCount = PlayerCardCollection.GetDifferentCardCount();
        int teamCount = playerDuelist.CardsAsPlayer.Count;
        if (cardCount > 1 && cardCount> teamCount)
        {
            panel.SetActive(true);
        }

        //end
        if (panel.activeSelf && teamCount>1)
        {
            gameObject.SetActive(false);
        }
    }
}
