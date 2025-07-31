using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UI_Machine : MonoBehaviour
{
    [SerializeField]
    private Machine _machine;
    [SerializeField]
    private TextMeshProUGUI _progressTextUI;

    private void Update()
    {
        transform.forward = Camera.main.transform.forward;

        if (_machine != null)
        {
            _progressTextUI.text = $"{_machine.CurrentProgress:F0}";
        }
    }
}