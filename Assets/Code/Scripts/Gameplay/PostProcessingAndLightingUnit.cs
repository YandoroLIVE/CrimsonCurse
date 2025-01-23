using UnityEngine;

public class PostProcessingAndLightingUnit : MonoBehaviour
{
    static PostProcessingAndLightingUnit ins;
    private void Awake()
    {
        if (ins == null) 
        {
            ins = this;
        }
        else 
        {
            Destroy(this.gameObject);
        }
    }
}
