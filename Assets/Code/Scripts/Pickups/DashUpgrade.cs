using HeroController;
using UnityEngine;

public class DashUpgrade : UpgradePickup
{
    public void Awake()
    {
        if (UpgradeHandler.GetInstance() != null && UpgradeHandler.GetInstance().HasDash()) 
        {
            this.gameObject.SetActive(false);
        }
    }
    public override void OnPickup()
    {
        UpgradeHandler.ActivateDash();
        this.sprite.SetActive(false);
    }
}
