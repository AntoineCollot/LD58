using UnityEngine;

public class TutoSelectTeam : MonoBehaviour
{
    [SerializeField] GameObject panel;
    [SerializeField] TeamSelection teamSelection;

    private void Start()
    {
        panel.SetActive(false);
    }

    void Update()
    {
        //activate
        if (!panel.activeSelf && CardCollectionDisplay.Instance.IsVisible && PlayerCardCollection.GetDifferentCardCount()>0)
        {
            panel.SetActive(true);
        }

        //end
        if (panel.activeSelf && teamSelection.isInTeamSelection)
        {
            gameObject.SetActive(false);
        }
    }
}
