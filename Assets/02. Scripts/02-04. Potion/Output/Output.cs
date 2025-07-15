using UnityEngine;

public class Output : MonoBehaviour
{
    private OutputData _data;
    public OutputData Data => _data;

    private EInputType _state;
    public EInputType State => _state;

    private void Awake()
    {
        
    }

    public void Init(OutputData data)
    {
        _data = data;
    }


}
