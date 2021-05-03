using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EventMessage : MonoBehaviour
{
    public GameObject messageHolder;
    public Image background;
    public Text messageText;
    [SerializeField]
    private float messageLiveDefault = 4.0f;
    [SerializeField]
    private Color backgroundColorDefault = new Color(0,0,0,250);

    private float currentTimeToDeative;

    private void Start()
    {
        messageHolder.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(currentTimeToDeative > 0)
        {
            currentTimeToDeative -= Time.deltaTime;
        }
        else
        {
            if(messageHolder.activeSelf) messageHolder.SetActive(false);
        }
    }

    public void SetMessage(string message, Color? backgroundColor = null, float? messageLive = null)
    {
        messageHolder.SetActive(true);
        
        Color bgColor = backgroundColor.HasValue? backgroundColor.Value : backgroundColorDefault;
        float msgLive = messageLive.HasValue ? messageLive.Value : messageLiveDefault;

        messageText.text = message;
        background.color = bgColor;
        currentTimeToDeative = msgLive;
    }

}
