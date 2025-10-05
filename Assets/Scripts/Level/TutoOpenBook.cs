using UnityEngine;

public class TutoOpenBook : MonoBehaviour
{
    [SerializeField] GameObject panel;

    private void Start()
    {
        panel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //activate
        if (!panel.activeSelf && PlayerCardCollection.GetDifferentCardCount() > 0)
            panel.SetActive(true);

        //end
        if(panel.activeSelf && CardCollectionDisplay.Instance.IsVisible)
        {
            gameObject.SetActive(false);
        }
    }
}
