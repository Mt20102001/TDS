using Fusion;

public class PlayerInventory : NetworkBehaviour
{
    [Networked]
    public int _currentWeaponID { get; private set; }

    // [Networked]
    // public int _currentAmmo { get; private set; }


    public override void Spawned()
    {
        ResetInventory();
    }

    public void ResetInventory()
    {
        _currentWeaponID = -1;
        // _currentAmmo = 0;
    }

    public void PickupNewWeapon(int weaponID)
    {
        RPC_PickupWeapon(weaponID);
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.All)]
    private void RPC_PickupWeapon(int weaponID)
    {
        this._currentWeaponID = weaponID;
    }

    // public void UpdateAmmo(int value)
    // {
    //     _currentAmmo += value;
    // }
}
