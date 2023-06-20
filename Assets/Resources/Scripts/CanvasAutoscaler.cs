using UnityEngine;

public class CanvasAutoscaler : MonoBehaviour
{
    [SerializeField] Camera camera;

    void Start()
    {
        float initialWidth = 16.0f;
        float initialHeight = 9.0f;

        float newWidth = Screen.width;
        float newHeight = Screen.height;

        if ((newHeight - initialHeight) * (initialWidth/initialHeight) < (newWidth - initialWidth))
        {
            transform.localScale = new Vector3(1, 1, 1) * (newHeight / 1080);
            camera.orthographicSize = newHeight / 2;
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1) * (newWidth / 1920);
            camera.orthographicSize = newHeight / 2;
        }
    }
}
