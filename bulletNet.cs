using UnityEngine;
using Unity.Netcode;

public class bulletNet : NetworkBehaviour
{
    [SerializeField]
    private float speed = 20f;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        GetComponent<Rigidbody2D>().velocity = this.transform.forward * speed;
    }
}
