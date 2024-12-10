using HeroController;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    [SerializeField] Transform targetTransform;
    public Vector3 offset;

    private void Start()
    {
        if (targetTransform == null) 
        { 
            targetTransform = FindAnyObjectByType<CameraFollow>().transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (targetTransform != null) 
        {

            Vector3 newPos = new Vector3(targetTransform.position.x, targetTransform.position.y, this.transform.position.z) + offset;
            transform.position = newPos;
        
        }
        
    }
}
