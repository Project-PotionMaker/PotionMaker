using UnityEngine;

public struct ResultMessage
{
    public bool Result;
    public string Message;

    public ResultMessage(bool result, string message)
    {
        Result = result;
        Message = message;
    }
}
