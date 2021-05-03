using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetActiveUI : MonoBehaviour
{
    public List<GameObject> others;
    public Image currentImage;
    public Image currentBackground;

    [Space]
    public Color deactivateColor = new Color(255, 255, 255, 0.3f);
    public Color activeColor = new Color(255, 255, 255, 255);

    [Space]
    public bool activateThis;

    private void Start()
    {
        if (activateThis)
        {
            ActivateCurrent();
        }
    }

    public void ActivateCurrent()
    {
        if(currentImage.enabled)
            currentImage.color = activeColor;

        if(currentBackground != null)
            currentBackground.enabled = false;

        foreach(GameObject obj in others)
        {
            SetActiveUI activeUI = obj.GetComponent<SetActiveUI>();

            if(activeUI.currentImage.enabled)
                activeUI.currentImage.color = deactivateColor;
    
            if(activeUI.currentBackground != null)
                activeUI.currentBackground.enabled =true;
        }
    }
}
