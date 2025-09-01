using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using Unity.Burst.CompilerServices;

public class PlayerMovementController : NetworkBehaviour
{
    public float Speed = 1f;
    void Update()
    {
        if (isLocalPlayer)
        {
            Move();
        }
    }
    public void SetPosition()
    {
        transform.position = new Vector3(Random.Range(-2, 2), Random.Range(-2, 2));
    }
    public void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        Vector3 dir = new Vector3(x, 0, y);
        transform.position += dir * Speed * Time.deltaTime;
    }
}
