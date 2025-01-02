using UnityEngine;

public class LongRangeUpgrade : UpgradePickup
{
    public void Awake()
    {
        if (UpgradeHandler.GetInstance() != null && UpgradeHandler.GetInstance().HasLongrange())
        {
            this.gameObject.SetActive(false);
        }
    }
    public override void OnPickup()
    {
        UpgradeHandler.ActivateLongrange();
        this.gameObject.SetActive(false);
    }
}
