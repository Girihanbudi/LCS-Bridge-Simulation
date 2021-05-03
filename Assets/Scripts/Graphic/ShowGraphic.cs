using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class ShowGraphic : MonoBehaviour
{
    public List<GameObject> graphs;
    public GameObject graphBG;
    [Space]
    public RectTransform canvasSimulation;
    public int paddingX = -5;
    public int paddingY = 5;
    public int offsetX = 10000;
    public int offsetY = 10000;
    private int currentOffsetX, currentOffsetY;

    //private int currentGraph;
    private bool minimized = false;

    [Space]
    public float minimizeScale = 0.4f;
    public int initGraphIndex = 0;
    public List<GameObject> hideObjects;

    [Space]
    public GameObject helpButton;
    public GameObject helpPanel;
    public bool helpActivated;

    private int currentActiveGraph;
    bool hideAllUI = false;

    
    private void Start()
    {
        GameObject[] graphArr = GameObject.FindGameObjectsWithTag("Graph");
        for(int i= 0; i < graphArr.Length ; i++)
        {
            graphs.Add(graphArr[i]);
        }
        graphs = graphs.OrderBy(go => go.name).ToList();

        currentActiveGraph = -1;
        minimized = true;
        ChangeGraph(0);
        MinimizeOrMaximisize(0, 0, 0, minimized);

        ShowOrHideHelpPanel(helpActivated);
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < graphs.Count; i++)
        {
            if(Input.GetKeyDown(string.Format("f{0}", i+1)))
            {
                if(i != currentActiveGraph)
                {
                    ChangeGraph(i);
                }
                else
                {
                    minimized = !minimized;
                    MinimizeOrMaximisize(i);
                }
            }
        }

        HideUI();

        if (Input.GetKeyDown(KeyCode.F10))
        {
            ShowOrHideHelpPanel(!helpActivated);
        }
    }

    private void HideUI()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            hideAllUI = !hideAllUI;

            if (hideAllUI)
            {
                currentOffsetX = offsetX;
                currentOffsetY = offsetX;

                canvasSimulation.SetAnchor(AnchorPresets.StretchAll, currentOffsetX, currentOffsetY);
                MinimizeOrMaximisize(currentActiveGraph, currentOffsetX, currentOffsetY);
            } else
            {
                currentOffsetX = 0;
                currentOffsetY = 0;

                canvasSimulation.SetAnchor(AnchorPresets.StretchAll, 0, 0);
                MinimizeOrMaximisize(currentActiveGraph, currentOffsetX, currentOffsetY);
            }
        }
    }
    
    private void ChangeGraph(int graphNumber)
    {
        if (currentActiveGraph == graphNumber) return;

        for(int i = 0; i < graphs.Count; i++)
        {
            if(i == graphNumber)
            {
                if (minimized)
                    MinimizeOrMaximisize(i, currentOffsetX, currentOffsetY, true);
                else
                    MinimizeOrMaximisize(i, currentOffsetX, currentOffsetY, false);

            } else
            {
                graphs[i].GetComponent<RectTransform>().SetAnchor(AnchorPresets.BottomLeft, 100000 + currentOffsetX, 100000 + currentOffsetY);
                graphBG.GetComponent<RectTransform>().SetAnchor(AnchorPresets.BottomLeft, 100000 + currentOffsetX, 100000 + currentOffsetY);
            }
        }

        currentActiveGraph = graphNumber;
    }

    private void MinimizeOrMaximisize(int graphNumber, int offsetX = 0, int offsetY = 0, bool? forceMinimized = null)
    {
        RectTransform graphTransform = graphs[graphNumber].GetComponent<RectTransform>();
        RectTransform graphBGTransform = graphBG.GetComponent<RectTransform>();

        bool setMinimized;
        if (forceMinimized != null)
        {
            setMinimized = forceMinimized.HasValue ? forceMinimized.Value : false;
        }
        else
        {
            setMinimized = minimized;
        }

        if (setMinimized)
        {
            ShowOrHideObjects(true);

            graphTransform.SetAnchor(AnchorPresets.BottomRight, paddingX + offsetX, paddingY + offsetY);
            graphTransform.sizeDelta = new Vector2(Screen.width, Screen.height) * minimizeScale;
            //graphTransform.localScale = Vector3.one * minimizeScale;
            graphTransform.localPosition = new Vector2(graphTransform.localPosition.x, graphTransform.localPosition.y) + 
                                           new Vector2( -graphTransform.sizeDelta.x, graphTransform.sizeDelta.y) / 2;


            graphBGTransform.SetAnchor(AnchorPresets.BottomRight, paddingX + offsetX, paddingY + offsetY);
            graphBGTransform.sizeDelta = new Vector2(Screen.width, Screen.height) * minimizeScale;
            //graphTransform.localScale = Vector3.one * minimizeScale;
            graphBGTransform.localPosition = new Vector2(graphBGTransform.localPosition.x, graphBGTransform.localPosition.y) +
                                           new Vector2(-graphBGTransform.sizeDelta.x, graphBGTransform.sizeDelta.y) / 2;


            graphTransform.GetComponent<AccelerometerGraph>().ResizeScale(minimizeScale);
        }
        else
        {
            graphTransform.SetAnchor(AnchorPresets.MiddleCenter, offsetX, offsetY);
            graphTransform.sizeDelta = new Vector2(Screen.width, Screen.height);

            graphBGTransform.SetAnchor(AnchorPresets.MiddleCenter, offsetX, offsetY);
            graphBGTransform.sizeDelta = new Vector2(Screen.width, Screen.height);

            //graphTransform.localScale = Vector3.one;
            graphTransform.GetComponent<AccelerometerGraph>().ResizeScale(1);

            ShowOrHideObjects(false);
            Debug.Log(offsetX);
        }
    }

    private void ShowOrHideObjects(bool activate)
    {
        foreach(GameObject obj in hideObjects) 
        {
            if(obj.activeSelf != activate)
                obj.SetActive(activate);
        }
    }

    private void ShowOrHideHelpPanel(bool? activate)
    {
        if (activate.HasValue) helpActivated = activate.Value;
        else helpActivated = !helpActivated;

        helpPanel.SetActive(helpActivated);
        helpButton.SetActive(!helpActivated);
    }

    public void SwitchHelpPanel(bool activate)
    {
        ShowOrHideHelpPanel(activate);
    }
    
    /*
    private void activateThisGraph(int graphNumber, bool? forceMinimized= null)
    {
        GameObject parent = graphs[graphNumber].transform.parent.gameObject;
        for (int i = 0; i < graphs.Count; i++)
        {
            if (i != graphNumber) graphs[i].transform.parent.localPosition = Vector3.one * 1000;
        }

        if(forceMinimized != null)
        {
            minimized = forceMinimized.HasValue? forceMinimized.Value : false;
        }

        if (!graphs[graphNumber].transform.parent.gameObject.activeSelf)
        {
            graphs[graphNumber].transform.parent.localPosition = Vector3.zero;
            foreach (GameObject graph in graphs)
            {
                if (graph != graphs[graphNumber])
                {
                    graph.transform.parent.localPosition = Vector3.one * 1000;
                }
            }
        }
        else
        {
            minimized = !minimized;
        }

        RectTransform graphTransform = parent.GetComponent<RectTransform>();
        if (minimized)
        {
            canvasSimulation.enabled = true;

            graphTransform.SetAnchor( AnchorPresets.BottomRight, paddingX, paddingY);
            graphTransform.localScale = Vector3.one * minimizeScale;

        }
        else
        {
            canvasSimulation.enabled = false;

            graphTransform.SetAnchor(AnchorPresets.MiddleCenter);
            graphTransform.localScale = Vector3.one;
        }
    }
    */
    
}
