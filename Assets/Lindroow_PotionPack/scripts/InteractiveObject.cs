using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractiveObject : MonoBehaviour
{

     public UnityEvent onInteraction;

    
    public void Interact()
    {
        onInteraction.Invoke();
    }

    

}
