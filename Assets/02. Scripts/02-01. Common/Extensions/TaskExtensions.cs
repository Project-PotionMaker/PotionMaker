using System;
using System.Threading.Tasks;
using UnityEngine;

public static class TaskExtensions
{
    public static async void SafeFireAndForget(this Task task, Action<Exception> onError = null)
    {
        try
        {
            await task;
        }
        catch (Exception e)
        {
            if (onError != null)
            {
                onError.Invoke(e);
            }
            else
            {
                Debug.LogError(e);
            }
        }
    }
}