using UnityEngine;

public struct ResultMessage
{
    public readonly bool Result;
    public readonly string Message;

    public ResultMessage(bool result, string message)
    {
        Result = result;
        Message = message;
    }
}
