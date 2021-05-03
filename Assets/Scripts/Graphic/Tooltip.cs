using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;

    private Text tooltipText;
    private RectTransform backgroundRectTransform;
    public float horizontalTextPadding = 5f;
    public float verticalTextPadding = 5f;

    private void Awake()
    {
        backgroundRectTransform = transform.Find("Tooltip Background").GetComponent<RectTransform>();
        tooltipText = transform.Find("Tooltip Text").GetComponent<Text>();

        ShowToolTip("Test Random");
    }

    private void Update()
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
                                                                transform.parent.GetComponent<RectTransform>(),
                                                                Input.mousePosition, 
                                                                mainCamera, 
                                                                out localPoint
                                                                );
        transform.localPosition = localPoint;
    }

    private void ShowToolTip(string tooltipString)
    {
        gameObject.SetActive(true);

        tooltipText.text = tooltipString;
        
        Vector2 backgroundSize = new Vector2(tooltipText.preferredWidth + horizontalTextPadding, tooltipText.preferredHeight + verticalTextPadding);
        backgroundRectTransform.sizeDelta = backgroundSize;
    }

    private void HideToolTip()
    {
        gameObject.SetActive(false);
    }
}
