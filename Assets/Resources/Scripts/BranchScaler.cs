using UnityEngine;
using UnityEngine.UI;

public class BranchScaler : MonoBehaviour
{
    [SerializeField] RectTransform sideGroupCanvas;
    [SerializeField] RectTransform groupCanvas;
    private RectTransform rectTransform;

    private Vector2 defaultSize = new Vector2(200, 60);

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        checkSize();
    }

    private void checkSize()
    {
        if (groupCanvas.sizeDelta.y > 0)
        {
            float groupCanvasHeight = groupCanvas.sizeDelta.y;
            float sideGroupCanvasHeight = sideGroupCanvas.sizeDelta.y + Mathf.Abs(sideGroupCanvas.localPosition.y);

            groupCanvas.GetComponent<VerticalLayoutGroup>().enabled = false;
            if (groupCanvasHeight / 2 > sideGroupCanvasHeight)
            {
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, groupCanvasHeight);
            }
            else
            {
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, sideGroupCanvasHeight * 2);
            }
            groupCanvas.GetComponent<VerticalLayoutGroup>().enabled = true;
        }
        else
        {
            rectTransform.sizeDelta = defaultSize;
        }
    }
}
