using UnityEngine;

public class CanvasAutoscaler : MonoBehaviour
{
    [SerializeField] Camera camera;

    void Start()
    {
        float initialScreenWidth = 16.0f;
        float initialScreenHeight = 9.0f;

        float newScreenWidth = Screen.width;
        float newScreenHeight = Screen.height;

        if ((newScreenHeight - initialScreenHeight) * (initialScreenWidth/initialScreenHeight) < (newScreenWidth - initialScreenWidth))
        {
            transform.localScale = new Vector3(1, 1, 1) * (newScreenHeight / 1080);
            camera.orthographicSize = newScreenHeight / 2;
            camera.rect = new Rect((1 - 1920 * (newScreenHeight / 1080) / newScreenWidth)/2, 0, 1920 * (newScreenHeight / 1080) / newScreenWidth, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1) * (newScreenWidth / 1920);
            camera.orthographicSize = 1080 * (newScreenWidth / 1920) / 2;
            camera.rect = new Rect(0, (1- camera.orthographicSize / (newScreenHeight / 2))/2, 1, camera.orthographicSize/(newScreenHeight / 2));
        }
    }
}
