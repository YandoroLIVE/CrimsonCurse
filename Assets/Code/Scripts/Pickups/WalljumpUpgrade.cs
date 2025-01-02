using UnityEngine;

public class WalljumpUpgrade : UpgradePickup
{
    public void Awake()
    {
        if (UpgradeHandler.GetInstance() != null && UpgradeHandler.GetInstance().HasWalljump())
        {
            this.gameObject.SetActive(false);
        }
    }
    public override void OnPickup()
    {
        UpgradeHandler.ActivateWallJump();
        this.gameObject.SetActive(false);
    }
}
