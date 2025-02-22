using Fusion;
using UnityEngine;

public class PickUpItem : NetworkBehaviour
{
    [SerializeField] private LayerMask layerPicker;
    [SerializeField] private int itemID;

    [Networked] private bool isPickedUp { get; set; } = false;


    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & layerPicker) != 0)
        {
            if (isPickedUp) return;

            if (HasStateAuthority)
            {
                var inventory = other.GetComponentInParent<PlayerInventory>();
                inventory.PickupNewWeapon(itemID);


                RPC_PickupItem();
            }
        }
    }


    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_PickupItem()
    {
        isPickedUp = true;
        gameObject.SetActive(false);
    }
}
