using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Globalization;

public class AccelerometerGraph : MonoBehaviour
{
	public AccelerometerFormula acclFormula;

	[Space]
	public GameObject graphTitlePrefab;
	public string titleName;
	private GameObject graphTitle;

	[Space]
	public Object emptyGraphPrefab;
	public bool plotOnStart;

	[Space]
	public int maxXLabel = 12;

	[Space]
	public MonitoringCapture monitoringCaptureEvent;

	float sendPhotoCounter = 1;
	bool readyToTakePhoto = false;
	
	public bool PlottingData
	{
		get { return _plottingData; }
		set
		{
			if (_plottingData != value)
			{
				_plottingData = value;
				plottingDataC.Changed();
			}
		}
	}
	[SerializeField] private bool _plottingData;

	public float plotIntervalSeconds;
	public float plotAnimationSeconds;
	const Ease plotEaseType = Ease.OutQuad;
	public float xInterval;
	public bool useAreaShading;
	public bool useComputeShader;
	public bool blinkCurrentPoint;
	public float blinkAnimDuration;

	[Space]
	public float minYAxis = -4.0f, maxYAxis = 4.0f;
	public float tresholdCautionVal = 1.0f, tresholdDangerVal = 1.5f;

	[Space]
	public Object tresholdInputPrefabs;
	const float blinkScale = 2;
	public bool moveXaxisMinimum;

	[Space]
	public Object indicatorPrefabH;
	public Object indicatorPrefabV;
	public Object tresholdCautionPrefab;
	public Object tresholdDangerPrefab;
	public int indicatorNumDecimals;

	[Space]
	public MapIndicator mapIndicator;

	private List<WMG_Change_Obj> changeObjs = new List<WMG_Change_Obj>();
	private WMG_Change_Obj plottingDataC = new WMG_Change_Obj();
	private Treshold graphTreshold;

	WMG_Axis_Graph graph;
	WMG_Grid grid;
	WMG_Series seriesH;
	GameObject indicatorH;
	WMG_Series seriesV;
	GameObject indicatorV;
	GameObject graphOverlay;

	System.Globalization.NumberFormatInfo tooltipNumberFormatInfo = new System.Globalization.CultureInfo("en-US", false).NumberFormat;
	System.Globalization.NumberFormatInfo yAxisNumberFormatInfo = new System.Globalization.CultureInfo("en-US", false).NumberFormat;
	System.Globalization.NumberFormatInfo seriesDataLabelsNumberFormatInfo = new System.Globalization.CultureInfo("en-US", false).NumberFormat;
	System.Globalization.NumberFormatInfo indicatorLabelNumberFormatInfo = new System.Globalization.CultureInfo("en-US", false).NumberFormat;

	float addPointAnimTimeline;
	Tween blinkingTween;
	private GraphScrollEvent scrollEvent;

	[HideInInspector]
	public GameObject tresholdCaution1, tresholdDanger1, tresholdCaution2, tresholdDanger2;
	[HideInInspector]
	public Text tresholdCautionText1, tresholdDangerText1, tresholdCautionText2, tresholdDangerText2;


	// Use this for initialization
	void Start()
	{
		scrollEvent = GetComponent<GraphScrollEvent>();
		scrollEvent.scrollVal = maxXLabel;

		changeObjs.Add(plottingDataC);

		GameObject graphGO = GameObject.Instantiate(emptyGraphPrefab) as GameObject;
		graphGO.transform.SetParent(this.transform, false);
		InitGraph(graphGO);

		grid = graph.yAxis.GridLines.GetComponent<WMG_Grid>();

		graphOverlay = new GameObject();
		graphOverlay.AddComponent<RectTransform>();
		graphOverlay.name = "Graph Overlay";
		graphOverlay.transform.SetParent(graphGO.transform, false);

		//HORIZONTAL & VERTICAL DATA
		(seriesH, indicatorH) = InitSeries("Horizontal", Color.green, indicatorPrefabH);
		(seriesV, indicatorV) = InitSeries("Vertical", Color.red, indicatorPrefabV);

		GameObject treshold = Instantiate(tresholdInputPrefabs, graph.transform) as GameObject;
		treshold.GetComponent<RectTransform>().SetAnchor(AnchorPresets.BottonCenter, 25, 75);
		graphTreshold = treshold.GetComponent<Treshold>();
		graphTreshold.cautionTreshold = tresholdCautionVal;
		graphTreshold.dangerTreshold = tresholdDangerVal;

		(tresholdCaution1, tresholdCautionText1)	= InitTreshold(tresholdCaution1, tresholdCautionPrefab, tresholdCautionVal);
		(tresholdCaution2, tresholdCautionText2)	= InitTreshold(tresholdCaution2, tresholdCautionPrefab, tresholdCautionVal);
		(tresholdDanger1, tresholdDangerText1)		= InitTreshold(tresholdDanger1, tresholdDangerPrefab, tresholdDangerVal);
		(tresholdDanger2, tresholdDangerText2)		= InitTreshold(tresholdDanger2, tresholdDangerPrefab, tresholdDangerVal);

		if (useAreaShading)
		{
			seriesH.areaShadingType = WMG_Series.areaShadingTypes.Gradient;
			seriesH.areaShadingAxisValue = graph.yAxis.AxisMinValue;
			seriesH.areaShadingColor = new Color(80f / 255f, 100f / 255f, 60f / 255f, 1f);
			seriesH.areaShadingUsesComputeShader = useComputeShader;
		}
		graph.tooltipDisplaySeriesName = false;

		// define our own custom functions for labeling
		//graph.theTooltip.tooltipLabeler = customTooltipLabeler; // override the default labeler for the tooltip
		//graph.yAxis.axisLabelLabeler = customYAxisLabelLabeler; // override the default labeler for the yAxis
		//seriesH.seriesDataLabeler = customSeriesDataLabeler; // override the default labeler for data labels (appear over points when data labels on the series are enabled)

		readyToTakePhoto = true;

		plottingDataC.OnChange += PlottingDataChanged;
		if (plotOnStart)
		{
			PlottingData = true;
		}
	}

	private void Update()
	{
		if (scrollEvent.scrollVal != maxXLabel) 
		{
			maxXLabel = scrollEvent.scrollVal;
			graph.xAxis.AxisMaxValue = maxXLabel;
		}

		if(sendPhotoCounter > 0)
		{
			sendPhotoCounter -= Time.deltaTime;
		} else
		{
			if (!readyToTakePhoto)
			{
				sendPhotoCounter = 1;
				readyToTakePhoto = true;
			}
		}
	}

	private void InitGraph(GameObject graphGO)
	{
		graph = graphGO.GetComponent<WMG_Axis_Graph>();
		graph.useGroups = false;
		graph.groups.Clear();

		graphTitle = Instantiate(graphTitlePrefab, graph.transform) as GameObject;
		graph.graphTitle = graphTitle;
		graph.graphTitleString = titleName;
		graph.graphTitleSize = 25;
		graph.graphTitleOffset = new Vector2(0, -50);

		graph.legend.hideLegend = false;
		graph.legend.oppositeSideLegend = true;
		graph.legend.offset = 50;
		graph.legend.background.SetActive(false);

		graph.stretchToParent(graphGO);
		graph.GraphBackgroundChanged += UpdateIndicatorSize;
		graph.autoPaddingEnabled = false;
		graph.paddingLeftRight = new Vector2(100, 75);
		graph.paddingTopBottom = new Vector2(100, 100);
		graph.axisWidth = 1;
		graph.autoAnimationsEnabled = false;

		graph.groups.Clear();

		//graph.xAxis.LabelType = WMG_Axis.labelTypes.ticks;
		graph.xAxis.SetLabelsUsingMaxMin = false;
		graph.xAxis.hideLabels = false;
		graph.xAxis.hideTicks = false;
		graph.xAxis.hideGrid = true;
		graph.xAxis.hideAxisLine = false;
		graph.xAxis.LabelType = WMG_Axis.labelTypes.groups;
		graph.xAxis.AxisLabelSkipInterval = (int)(plotIntervalSeconds * 100);
		//graph.xAxis.MaxAutoGrow = true;
		//graph.xAxis.MaxAutoShrink = true;
		//graph.xAxis.MinAutoGrow = true;
		//graph.xAxis.MinAutoShrink = true;
		graph.xAxis.AxisLabelSpaceOffset = 25;

		graph.yAxis.AxisNumTicks = 9;
		graph.yAxis.hideTicks = true;
		graph.yAxis.MaxAutoGrow = false; // auto increase yAxis max if a point value exceeds max
		graph.yAxis.MinAutoGrow = false; // auto decrease yAxis min if a point value exceeds min
		graph.yAxis.AxisMinValue = minYAxis;
		graph.yAxis.AxisMaxValue = maxYAxis;
	}

	private (WMG_Series, GameObject) InitSeries(string seriesName, Color lineColor, Object indicatorPrefabs = null, float lineScale = 0.5f, float width = 5.0f)
	{
		//HORIZONTAL DATA
		WMG_Series series = graph.addSeries();
		series.name = seriesName;
		series.seriesName = seriesName;
		series.hidePoints = true;
		series.lineColor = lineColor;
		series.lineScale = lineScale;
		series.pointWidthHeight = width;

		if (indicatorPrefabs == null)
			return (series, null);

		GameObject indicator = GameObject.Instantiate(indicatorPrefabs) as GameObject;
		indicator.transform.SetParent(graphOverlay.transform, false);
		indicator.SetActive(false);

		return (series, indicator);
	}

	(GameObject, Text) InitTreshold(GameObject treshold, Object prefab, float g, float paddingX = 10)
	{
		treshold = Instantiate(prefab, graph.transform) as GameObject;
		RectTransform tresholdRect = treshold.GetComponent<RectTransform>();
		tresholdRect.localPosition = new Vector2(10, g * grid.gridLinkLengthY);
		tresholdRect.sizeDelta = new Vector2(grid.gridLinkLengthX, tresholdRect.sizeDelta.y);

		Text tresholdText = treshold.GetComponentInChildren<Text>();
		tresholdText.text = string.Format("Danger Limit : {0}g", g);

		return (treshold, tresholdText);
	}

	public void ResizeScale(float targetScale)
	{
		graph.Refresh();

		UpdateTreshold(tresholdCaution1, tresholdCautionText1, "Caution", graphTreshold.cautionTreshold, targetScale);
		UpdateTreshold(tresholdCaution2, tresholdCautionText2, "Caution", -graphTreshold.cautionTreshold, targetScale);
		UpdateTreshold(tresholdDanger1, tresholdDangerText1, "Danger", graphTreshold.dangerTreshold, targetScale);
		UpdateTreshold(tresholdDanger2, tresholdDangerText2, "Danger", -graphTreshold.dangerTreshold, targetScale);

		graphTreshold.GetComponent<RectTransform>().sizeDelta = Vector2.one * targetScale;
	}

	public void UpdateTreshold(GameObject treshold, Text tresholdText, string tresholdName, float g, float? targetScale=null, float paddingX = 10)
	{
		RectTransform tresholdRect = treshold.GetComponent<RectTransform>();

		float yPos = (g * ((grid.gridNumNodesY - 1) / 2) / maxYAxis) * grid.gridLinkLengthY;
		tresholdRect.localPosition = new Vector2(paddingX, yPos);
		tresholdRect.sizeDelta = new Vector2(grid.gridLinkLengthX, tresholdRect.sizeDelta.y);

		tresholdText.text = string.Format("{0} Limit : {1}g", tresholdName, g);

		float target = tresholdRect.localScale.y;
		if (targetScale != null)
		{
			target = targetScale.HasValue ? targetScale.Value : target;
		}

		tresholdRect.localScale = Vector2.one * target;
	}

	void PlottingDataChanged()
	{
		//Debug.Log("plottingData: " + plottingData);
		if (PlottingData)
		{
			StartCoroutine(PlotData());
		}
	}

	
	private (float r, List<float>) CalculateVibrationVector()
	{
		float r;
		List<float> vibrationData;
		(r, vibrationData) = acclFormula.CalculateVibration();
		return (r, vibrationData);
	}
	

	public IEnumerator PlotData()
	{
		while (true)
		{
			yield return new WaitForSeconds(plotIntervalSeconds);
			if (!PlottingData) break;

			string currentTime = System.DateTime.Now.ToString("hh:mm:ss tt", CultureInfo.InvariantCulture);
			float r;
			List<float> vibrationData;
			(r, vibrationData) = CalculateVibrationVector();

			graph.groups.Add(currentTime);
			foreach(float yVal in vibrationData)
			{
				mapIndicator.ChangeStatus(yVal, graphTreshold);
				
				// SEND PHOTO TO SERVER
				if(sendPhotoCounter <= 0 && readyToTakePhoto)
				{
					
					if (yVal >= graphTreshold.dangerTreshold)
					{
						monitoringCaptureEvent.TakeScreenShot(graph.graphTitleString, 1, true);
						readyToTakePhoto = false;
					}
					else if (yVal <= -graphTreshold.dangerTreshold)
					{
						monitoringCaptureEvent.TakeScreenShot(graph.graphTitleString, 1, true);
						readyToTakePhoto = false;
					}
					else if (yVal >= graphTreshold.cautionTreshold)
					{
						monitoringCaptureEvent.TakeScreenShot(graph.graphTitleString, 2, true);
						readyToTakePhoto = false;
					}
					
					else if (yVal <= -graphTreshold.cautionTreshold)
					{
						monitoringCaptureEvent.TakeScreenShot(graph.graphTitleString, 2, true);
						readyToTakePhoto = false;
					}

				}

				// ADD VALUE TO GRAPH
				char rLastDigit = r.ToString().Last();
				if ((int)rLastDigit % 2 == 0)
				{
					AnimateAddPointFromEnd(seriesH, indicatorH, new Vector2((seriesH.pointValues.Count == 0 ? 0 : (seriesH.pointValues[seriesH.pointValues.Count - 1].x + xInterval)),
													yVal),
									  plotAnimationSeconds);

					AnimateAddPointFromEnd(seriesV, indicatorV, new Vector2((seriesH.pointValues.Count == 0 ? 0 : (seriesH.pointValues[seriesH.pointValues.Count - 1].x + xInterval)),
													0),
									  plotAnimationSeconds);
				}
				else
				{
					AnimateAddPointFromEnd(seriesV, indicatorV, new Vector2((seriesH.pointValues.Count == 0 ? 0 : (seriesH.pointValues[seriesH.pointValues.Count - 1].x + xInterval)),
													yVal),
									  plotAnimationSeconds);

					AnimateAddPointFromEnd(seriesH, indicatorH, new Vector2((seriesH.pointValues.Count == 0 ? 0 : (seriesH.pointValues[seriesH.pointValues.Count - 1].x + xInterval)),
													0),
									  plotAnimationSeconds);
				}
			}

			if(graph.groups.Count > maxXLabel)
			{
				int dif = graph.groups.Count - maxXLabel;
				for (int i = 0; i < dif; i++)
				{
					graph.groups.RemoveAt(i);
				}
			}

			if (blinkCurrentPoint)
			{
				BlinkCurrentPointAnimation(seriesH);
				BlinkCurrentPointAnimation(seriesV);
			}
		}
	}

	void AnimateAddPointFromEnd(WMG_Series series, GameObject indicator, Vector2 pointVec, float animDuration)
	{
		if (series.pointValues.Count == 0)
		{ // no end to animate from, just add the point
			series.pointValues.Add(pointVec);
			indicator.SetActive(true);
			graph.Refresh(); // Ensures gamobject list of series points is up to date based on pointValues
			UpdateIndicator(series, indicator);
		}
		else
		{
			series.pointValues.Add(series.pointValues[series.pointValues.Count - 1]);
			if (pointVec.x > graph.xAxis.AxisMaxValue)
			{ // the new point will exceed the x-axis max
				addPointAnimTimeline = 0; // animates from 0 to 1
				Vector2 oldEnd = new Vector2(series.pointValues[series.pointValues.Count - 1].x, series.pointValues[series.pointValues.Count - 1].y);
				Vector2 newStart = new Vector2(series.pointValues[1].x, series.pointValues[1].y);
				Vector2 oldStart = new Vector2(series.pointValues[0].x, series.pointValues[0].y);
				WMG_Anim.animFloatCallbacks(() => addPointAnimTimeline, x => addPointAnimTimeline = x, animDuration, 1,
											() => OnUpdateAnimateAddPoint(series, indicator, pointVec, oldEnd, newStart, oldStart),
											() => OnCompleteAnimateAddPoint(series), plotEaseType);
			}
			else
			{
				WMG_Anim.animVec2CallbackU(() => series.pointValues[series.pointValues.Count - 1], x => series.pointValues[series.pointValues.Count - 1] = x, animDuration, pointVec,
										   () => UpdateIndicator(series, indicator), plotEaseType);
			}
		}
	}

	void BlinkCurrentPointAnimation(WMG_Series series, bool fromOnCompleteAnimateAdd = false)
	{
		graph.Refresh(); // Ensures gamobject list of series points is up to date based on pointValues
		WMG_Node lastPoint = series.getLastPoint().GetComponent<WMG_Node>();
		string blinkingPointAnimId = series.GetHashCode() + "blinkingPointAnim";
		DOTween.Kill(blinkingPointAnimId);
		blinkingTween = lastPoint.objectToScale.transform.DOScale(new Vector3(blinkScale, blinkScale, blinkScale), blinkAnimDuration).SetEase(plotEaseType)
			.SetUpdate(false).SetId(blinkingPointAnimId).SetLoops(-1, LoopType.Yoyo);
		if (series.pointValues.Count > 1)
		{ // ensure previous point scale reset
			WMG_Node blinkingNode = series.getPoints()[series.getPoints().Count - 2].GetComponent<WMG_Node>();
			if (fromOnCompleteAnimateAdd)
			{ // removing a pointValues index deletes the gameobject at the end, so need to set the timeline from the previous tween
				blinkingTween.Goto(blinkAnimDuration * blinkingNode.objectToScale.transform.localScale.x / blinkScale, true);
			}
			blinkingNode.objectToScale.transform.localScale = Vector3.one;
		}
	}

	void UpdateIndicator(WMG_Series series, GameObject indicator)
	{
		if (series.getPoints().Count == 0) return;
		WMG_Node lastPoint = series.getLastPoint().GetComponent<WMG_Node>();
		graph.changeSpritePositionToY(indicator, lastPoint.transform.localPosition.y);
		Vector2 nodeData = series.getNodeValue(lastPoint);
		indicatorLabelNumberFormatInfo.CurrencyDecimalDigits = indicatorNumDecimals;
		string textToSet = nodeData.y.ToString("0.00");
		graph.changeLabelText(indicator.transform.GetChild(0).GetChild(0).gameObject, textToSet);
	}

	void OnUpdateAnimateAddPoint(WMG_Series series, GameObject indicator, Vector2 newEnd, Vector2 oldEnd, Vector2 newStart, Vector2 oldStart)
	{
		series.pointValues[series.pointValues.Count - 1] = WMG_Util.RemapVec2(addPointAnimTimeline, 0, 1, oldEnd, newEnd);
		graph.xAxis.AxisMaxValue = WMG_Util.RemapFloat(addPointAnimTimeline, 0, 1, oldEnd.x, newEnd.x);

		UpdateIndicator(series, indicator);

		if (moveXaxisMinimum)
		{
			series.pointValues[0] = WMG_Util.RemapVec2(addPointAnimTimeline, 0, 1, oldStart, newStart);
			graph.xAxis.AxisMinValue = WMG_Util.RemapFloat(addPointAnimTimeline, 0, 1, oldStart.x, newStart.x);
		}
	}

	void OnCompleteAnimateAddPoint(WMG_Series series)
	{
		if (moveXaxisMinimum)
		{
			series.pointValues.RemoveAt(0);
			BlinkCurrentPointAnimation(series, true);
		}
	}

	string CustomTooltipLabeler(WMG_Series aSeries, WMG_Node aNode)
	{
		Vector2 nodeData = aSeries.getNodeValue(aNode);
		tooltipNumberFormatInfo.CurrencyDecimalDigits = aSeries.theGraph.tooltipNumberDecimals;
		string textToSet = nodeData.y.ToString("C", tooltipNumberFormatInfo);
		if (aSeries.theGraph.tooltipDisplaySeriesName)
		{
			textToSet = aSeries.seriesName + ": " + textToSet;
		}
		return textToSet;
	}

	/*
	string customYAxisLabelLabeler(WMG_Axis axis, int labelIndex)
	{
		float num = axis.AxisMinValue + labelIndex * (axis.AxisMaxValue - axis.AxisMinValue) / (axis.axisLabels.Count - 1);
		yAxisNumberFormatInfo.CurrencyDecimalDigits = axis.numDecimalsAxisLabels;
		return num.ToString("C", yAxisNumberFormatInfo);
	}

	string customSeriesDataLabeler(WMG_Series series, float val, int labelIndex)
	{
		seriesDataLabelsNumberFormatInfo.CurrencyDecimalDigits = series.dataLabelsNumDecimals;
		return val.ToString("C", seriesDataLabelsNumberFormatInfo);
	}
	*/

	void UpdateIndicatorSize(WMG_Axis_Graph aGraph)
	{
		aGraph.changeSpritePositionTo(graphOverlay, aGraph.graphBackground.transform.parent.transform.localPosition);
		float indicatorWidth = (aGraph.getSpriteWidth(aGraph.graphBackground) - aGraph.paddingLeftRight[0] - aGraph.paddingLeftRight[1]);
		aGraph.changeSpriteSize(indicatorH, Mathf.RoundToInt(indicatorWidth), 2);
		aGraph.changeSpritePositionToX(indicatorH, indicatorWidth / 2f);

		aGraph.changeSpriteSize(indicatorV, Mathf.RoundToInt(indicatorWidth), 2);
		aGraph.changeSpritePositionToX(indicatorV, indicatorWidth / 2f);
		//updateIndicator();
	}
}
