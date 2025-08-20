using Mirror.Discovery;
using TMPro;
using UnityEngine;

public class CodeInputUI : MonoBehaviour
{
    [SerializeField] 
    private TMP_InputField _inputField;
    [SerializeField]
    private TextMeshProUGUI _warningField;

    public void OnJoinByCode()
    {
        string text = _inputField.text.Trim();
        _warningField.gameObject.SetActive(false);

        if (string.IsNullOrEmpty(text) || text.Length != 5)
        {
            _warningField.text = "방 코드는 5글자의 숫자 또는 영어가 포함된 글자입니다.";
            _warningField.gameObject.SetActive(true);
            return;
        }

        string code = text.ToUpper();
        string address = RoomDirectory.Instance.GetAddress(code);

        if (string.IsNullOrEmpty(address))
        {
            _warningField.text = "존재하지 않는 방입니다.";
            _warningField.gameObject.SetActive(true);
            return;
        }

        MirrorNetworkManager.Instance.networkAddress = address;
        MirrorNetworkManager.Instance.StartClient();
    }
}
