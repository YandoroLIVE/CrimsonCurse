using UnityEngine;

[ExecuteInEditMode]
public class ParallaxCamera : MonoBehaviour
{
    public delegate void ParallaxCameraDelegate(Vector2 deltaMovement);
    public ParallaxCameraDelegate onCameraTranslate;

    Vector2 oldPosition;

    void Start()
    {
        oldPosition = transform.position;
    }

    void Update()
    {
        Vector2 delta = Vector2.zero;
        if (transform.position.x != oldPosition.x)
        {
            if (onCameraTranslate != null)
            {
                delta.x = oldPosition.x - transform.position.x;
            }

            oldPosition.x = transform.position.x;
        }
        if (transform.position.y != oldPosition.y) 
        {
            if (onCameraTranslate != null)
            {
                delta.y = oldPosition.y - transform.position.y;
            }

            oldPosition.y = transform.position.y;

        }
        onCameraTranslate(delta);
    }
}