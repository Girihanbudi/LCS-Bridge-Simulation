using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HideObject : MonoBehaviour
{
    public GameObject objectView;

    [Space]
    public Image camImg;
    public Image camBackground;

    Color deactivateColor;
    Color activeColor;

    Color deactivateColorBG;
    Color activeColorBG;

    [Space]
    public bool activateOnAwake;

    // Start is called before the first frame update
    void Start()
    {
        deactivateColor = new Color(255, 255, 255, 0.3f);
        activeColor = new Color(255, 255, 255, 255);

        deactivateColorBG = new Color(0, 0, 0, 0.8f);
        activeColorBG = new Color(0, 0, 0, 0.3f);

        if (activateOnAwake)
        {
            TurnFirst(true);
        } else
        {
            TurnFirst(false);
        }
    }

    void TurnFirst(bool? activated=null)
    {
        bool command;
        if (activated != null)
        {
            command = activated.HasValue?  activated.Value: false;
        } else
        {
            command = !objectView.activeSelf;
        }

        objectView.SetActive(command);
    
        if(command == true)
        {
            camImg.color = activeColor;
            camBackground.color = activeColorBG;
        } else
        {
            camImg.color = deactivateColor;
            camBackground.color =deactivateColorBG;
        }
    }

    public void TurnView()
    {
        objectView.SetActive(!objectView.activeSelf);

        if (objectView.activeSelf)
        {
            camImg.color = activeColor;
            camBackground.color = activeColorBG;
        }
        else
        {
            camImg.color = deactivateColor;
            camBackground.color = deactivateColorBG;
        }
    }
}
