using Fusion;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            Debug.Log("Đây là player của mình");
        }
    }
}