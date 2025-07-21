using UnityEngine;

public class UI_Reputation : MonoBehaviour
{
    private void Start()
    {
        ReputationManager.Instance.OnDataChanged += Refresh;
    }

    public void Refresh()
    {

    }
}
