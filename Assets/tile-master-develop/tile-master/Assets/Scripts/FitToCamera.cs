using UnityEngine;

public class FitToCamera : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private float lastCameraHorizontalExtent;
    private float lastCameraVerticalExtent;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        lastCameraHorizontalExtent = Camera.main.orthographicSize * Screen.width / Screen.height;
        lastCameraVerticalExtent = Camera.main.orthographicSize;
        UpdateScale();
    }

    private void LateUpdate()
    {
        float cameraHorizontalExtent = Camera.main.orthographicSize * Screen.width / Screen.height;
        float cameraVerticalExtent = Camera.main.orthographicSize;

        if (cameraHorizontalExtent != lastCameraHorizontalExtent || cameraVerticalExtent != lastCameraVerticalExtent)
        {
            lastCameraHorizontalExtent = cameraHorizontalExtent;
            lastCameraVerticalExtent = cameraVerticalExtent;
            UpdateScale();
        }
    }

    private void UpdateScale()
    {
        float spriteWidth = spriteRenderer.sprite.bounds.size.x;
        float spriteHeight = spriteRenderer.sprite.bounds.size.y;

        Vector2 newScale = transform.localScale;
        newScale.x = lastCameraHorizontalExtent * 2 / spriteWidth;
        newScale.y = lastCameraVerticalExtent * 2 / spriteHeight;
        transform.localScale = newScale;
    }
}
