

#pragma strict
import System.Collections.Generic;	// Queue, List, Dictionary

var maxDataCount = 2000;		// Usar 0 o negativo para ilimitado

var GuiPosX = 10;
var GuiPosY = 10;
var GuiWidth = 512;
var GuiHeight = 256;
var GuiAlpha = 0.7;

var displayRect = Rect(0.0, 0.0, 1.0, 1.0);
var displayColor = Color.black;
var caption = "";

var showCoordinates = true;
var showLegend = true;
var showRealtimeValues = false;
var showValueList = false;
var autoUpdateValueList = false;
var autoScrollValueList = false;

var showOrigin = true;
var originColor = Color.white;

var showGrid = true;
var gridResolution = Vector2(0.1, 0.1);
var gridColor = Color(0,0.2,0);

var showSecondaryGrid = true;
var secondaryGridResolution = Vector2(10.0, 1.0);
var secondaryGridColor = Color(0,0.5,0);

var showDotValues = false;
var showDotGrid = false;
var dotResolution = Vector2(0.05, 0.05);
var dotColor = Color.gray * 0.2;
var dotAutoHideBias = 10;				// Si es > 0 y la distancia entre dos puntos es menor que estos pixels, auto-ocultar los puntos.

var TextStyle = new GUIStyle();


private class GraphData
	{
	var name : String;
	var color : Color;
	var visible : boolean;
	var doubleSigned : boolean;
	var lineType : int;
	var lineTypeData : int;
	
	var Data : Queue.<Vector2>;
	var lastValue : float;
	}
	

private var m_Graphs = new Dictionary.<int, GraphData>();
private var m_Display : GUICanvasJS;
private var m_Legend : GUICanvasJS;
private var m_LegendValues : GUICanvasJS;

private var m_DataList = new List.<String>();
private var m_lastShowValueList = false;
private var m_lastValueListCount = 0;


private static var COLORS = [Color.green, Color.yellow, Color.cyan, Color.magenta, Color.white, Color.blue, Color.red, Color.gray];

static function GetStockColor(id : int) : Color
	{
	return COLORS[id % 8];
	}



private function InitDisplay()
	{
	m_Display = new GUICanvasJS(GuiWidth, GuiHeight, displayRect.x, displayRect.y, displayRect.width, displayRect.height);
	m_Display.SetAlpha(GuiAlpha);
	
	m_Legend = new GUICanvasJS(s_legendWidth, GuiHeight, 0, -GuiHeight, s_legendWidth, GuiHeight);
	m_Legend.SetAlpha(GuiAlpha);
	m_Legend.Clear(displayColor);
	
	m_LegendValues = GUICanvasJS(s_legendValuesWidth, GuiHeight, 0, -GuiHeight, s_legendValuesWidth, GuiHeight);
	m_LegendValues.SetAlpha(GuiAlpha);
	m_LegendValues.Clear(displayColor);
	}
	
	
private function InitGraph(graphId : int) : GraphData
	{
	// Crear un nuevo graph con los datos por defecto y meterlo en la lista.
	
	var Graph = new GraphData();
	//Graph.Data = new List.<Vector2>();
	Graph.Data = new Queue.<Vector2>();
	Graph.visible = true;
	Graph.color = COLORS[graphId % 8];
	Graph.name = graphId.ToString();
	m_Graphs.Add(graphId, Graph);
	
	RedrawLegend();	
	return Graph;
	}
	
	

function AddValue(graphId : int, value : Vector2)
	{
	var Graph : GraphData;
	
	if (!m_Graphs.TryGetValue(graphId, Graph))
		Graph = InitGraph(graphId);

	//Graph.Data.Add(value);
	Graph.Data.Enqueue(value);
	Graph.lastValue = value.y;
	
	if (maxDataCount > 0 && Graph.Data.Count > maxDataCount)
		Graph.Data.Dequeue();
	}
	
	
function AddValue(graphId : int, valuex : float, valuey : float)
	{
	AddValue(graphId, Vector2(valuex, valuey));
	}
	
	
function GetValueCount(graphId : int) : int
	{
	var Graph : GraphData;
	
	if (!m_Graphs.TryGetValue(graphId, Graph)) return 0;
	
	return Graph.Data.Count;
	}


	
	
function SetGraphInfo(graphId : int, name : String, color : Color, doubleSigned : boolean, lineType : int, lineTypeData : int)
	{
	var Graph : GraphData;
	
	if (!m_Graphs.TryGetValue(graphId, Graph))
		Graph = InitGraph(graphId);
		
	Graph.name = name;
	Graph.color = color;
	Graph.doubleSigned = doubleSigned;
	Graph.lineType = lineType;
	Graph.lineTypeData = lineTypeData;
	
	RedrawLegend();
	}
	
	
function SetGraphInfo(graphId : int, name : String, color : Color, lineType : int, lineTypeData : int)
	{
	SetGraphInfo(graphId, name, color, false, lineType, lineTypeData);
	}
	
function SetGraphInfo(graphId : int, name : String, color : Color, doubleSigned : boolean)
	{
	SetGraphInfo(graphId, name, color, doubleSigned, 0, 0);
	}
	
function SetGraphInfo(graphId : int, name : String, color : Color)
	{
	SetGraphInfo(graphId, name, color, false);
	}
	
function SetGraphInfo(graphId : int, name : String)
	{
	SetGraphInfo(graphId, name, GetStockColor(graphId), false);
	}



function SetGraphVisible(graphId : int, visible : boolean)
	{
	var Graph : GraphData;
	
	if (!m_Graphs.TryGetValue(graphId, Graph))
		Graph = InitGraph(graphId);
		
	Graph.visible = visible;
	
	RedrawLegend();
	}
	
	
function ToggleGraphVisibleByOrdinal(num : int)
	{
	var items = m_Graphs.GetEnumerator();
	var Graph : GraphData;
	var i = 0;
	
	while (items.MoveNext() && i < m_Graphs.Count)
		{
		if (i++ == num) 
			{
			Graph = items.Current.Value;
			break;
			}
		}
		
	if (Graph)
		{
		Graph.visible = !Graph.visible;
		Redraw();
		RedrawLegend();
		}
	}


function DeleteGraph(graphId : int)
	{
	var Graph : GraphData;
	
	if (m_Graphs.TryGetValue(graphId, Graph))
		{
		Graph.Data.Clear();
		Graph.Data.TrimExcess();
		m_Graphs.Remove(graphId);
		}
		
	RedrawLegend();
	}


	
function ClearAllGraphData()
	{
	var items = m_Graphs.GetEnumerator();
	
	while (items.MoveNext())
		items.Current.Value.Data.Clear();
		
	Redraw();
	}
	
	
function DeleteAllGraphs()
	{	
	var items = m_Graphs.GetEnumerator();
	
	while (items.MoveNext())
		{
		items.Current.Value.Data.Clear();
		items.Current.Value.Data.TrimExcess();
		}
		
	m_Graphs.Clear();
	Redraw();
	RedrawLegend();
	}




function OnEnable ()
	{
	InitDisplay();
	Redraw();
	RedrawLegend();
	}
	
function OnDisable ()
	{
	m_Display.DestroyTexture();
	m_Legend.DestroyTexture();
	m_LegendValues.DestroyTexture();
	}



function Start ()
	{
	//AddValue(1, Vector2.one);
	//AddValue(1, Vector2.zero);
	//DeleteAllGraphs();
	}
	
	
	
static private function s_IsPointValid(P : Vector2)
	{
	return !float.IsNaN(P.x) && !float.IsNaN(P.y);
	}

	
static private function s_GraphLineTo(canvas : GUICanvasJS, graph : GraphData, P : Vector2)
	{
	switch (graph.lineType)
		{
		case 1:
			canvas.DottedLineTo(P.x, P.y, graph.lineTypeData, graph.color);
			break;
			
		case 2:
			canvas.DashedLineTo(P.x, P.y, graph.lineTypeData, graph.color);
			break;
		
		default:
			canvas.LineTo(P.x, P.y, graph.color);
		}
	}
	
	
function Redraw()
	{
	if (!this.enabled) return;
	
	m_Display.Clear(displayColor);
	m_Display.ResizeCanvas(displayRect.x, displayRect.y, displayRect.width, displayRect.height);
	
	if (showDotGrid && m_Display.Canvas2PixelsX(dotResolution.x) >= dotAutoHideBias && m_Display.Canvas2PixelsY(dotResolution.y) >= dotAutoHideBias)
		m_Display.DotMatrix(dotResolution.x, dotResolution.y, dotColor);
	
	if (showGrid) m_Display.Grid(gridResolution.x, gridResolution.y, gridColor);
	if (showSecondaryGrid) m_Display.Grid(secondaryGridResolution.x, secondaryGridResolution.y, secondaryGridColor);
	if (showOrigin) 
		{
		m_Display.LineH(0.0, originColor);
		m_Display.LineV(0.0, originColor);
		}
	
	// Representar los datos de cada serie
	
	var items : IEnumerator.< KeyValuePair.<int, GraphData> > = m_Graphs.GetEnumerator();
	while (items.MoveNext())
		{
		// Acceso a los datos de la serie
		
		var graph = items.Current.Value;
		if (!graph.visible) continue;
		
		var data : IEnumerator.<Vector2> = graph.Data.GetEnumerator();
		
		// Representar todos los valores válidos de la serie
		
		var lastPointValid = false;
		
		while (data.MoveNext())
			{
			if (s_IsPointValid(data.Current))
				{
				if (data.Current.x >= displayRect.xMin)
					{
					if (lastPointValid)
						s_GraphLineTo(m_Display, graph, data.Current);
					else
						m_Display.MoveTo(data.Current);
					
					if (data.Current.x > displayRect.xMax) break;
					if (showDotValues && m_Display.Canvas2PixelsX(dotResolution.x) >= dotAutoHideBias) m_Display.Dot(data.Current, graph.color);
					}
				else
					m_Display.MoveTo(data.Current);
				
				lastPointValid = true;
				}
			else
				lastPointValid = false;
			}
			
		// Si lleva signo doble, replicar con la Y invertida.
		
		if (graph.doubleSigned)
			{
			data = graph.Data.GetEnumerator();			
			lastPointValid = false;
			
			while (data.MoveNext())
				{
				if (s_IsPointValid(data.Current))				
					{
					if (data.Current.x >= displayRect.xMin)
						{
						if (lastPointValid)
							s_GraphLineTo(m_Display, graph, Vector2(data.Current.x, -data.Current.y));
						else
							m_Display.MoveTo(Vector2(data.Current.x, -data.Current.y));
						
						if (data.Current.x > displayRect.xMax) break;
						if (showDotValues && m_Display.Canvas2PixelsX(dotResolution.x) >= dotAutoHideBias) m_Display.Dot(Vector2(data.Current.x, -data.Current.y), graph.color);
						}
					else
						m_Display.MoveTo(data.Current);
						
					lastPointValid = true;
					}
				else
					lastPointValid = false;
				}
			}
		}	
	}
	
	
private function GenerateDataList()
	{
	if (!this.enabled) return;
	
	var sHeader = "----t----   ";
	m_DataList.Clear();
	
	// Obtener un enumerador para cada serie.
	
	var enumList = new List.< IEnumerator.<Vector2> >();
	var items : IEnumerator.< KeyValuePair.<int, GraphData> > = m_Graphs.GetEnumerator();	
	while (items.MoveNext())
		{
		// Acceso a los datos de la serie
		
		var graph = items.Current.Value;
		if (!graph.visible) continue;
		
		enumList.Add(graph.Data.GetEnumerator());
		
		// Componer el header
		
		sHeader += graph.name.PadLeft(8).Substring(0, 8) + "  ";
		}
	
	if (enumList.Count == 0) return;
	
	// La primera string es el header
	
	m_DataList.Add(sHeader);
	
	// Poner todas las series en el primer elemento

	var eos = false;
	
	for (var i=0; i<enumList.Count; i++)
		if (!enumList[i].MoveNext()) eos = true;
		
	var sDataRow : String;
		
	while (!eos)
		{
		// Localizar la X menor de todos los valores
		
		var xMin = Mathf.Infinity;
		
		for (i=0; i<enumList.Count; i++)
			{
			var V = enumList[i].Current;
			if (V.x < xMin) xMin = V.x;
			}
			
		sDataRow = String.Format("{0,7:0.00}   ", xMin);
			
		// Representar todos aquellos cuya X sea la mínima.
		
		for (i=0; i<enumList.Count; i++)
			{
			V = enumList[i].Current;
			if (Mathf.Approximately(V.x, xMin))
				{
				sDataRow += String.Format("{0,9:0.000} ", V.y);
				if (!enumList[i].MoveNext()) eos = true;
				}
			else
				sDataRow += "          ";
			}
		
		m_DataList.Add(sDataRow);
		}
	}


private function GetGraphDataCount() : int
	{
	var count = 0;
	
	if (m_Graphs.Count > 0)
		{
		var items : IEnumerator.< KeyValuePair.<int, GraphData> > = m_Graphs.GetEnumerator();
		
		while (items.MoveNext())
			count += items.Current.Value.Data.Count;
		}
	
	return count;
	}
	
	
private static var s_legendSpacing = 16;
private static var s_legendDistanceX = 32;
private static var s_legendMargin = 8;
private static var s_legendWidth = 200;
private static var s_legendValuesWidth = 80;
	
private function RedrawLegend()
	{
	if (!this.enabled) return;
	
	var yPos = s_legendSpacing;
	
	m_Legend.Clear(displayColor);
	
	var items = m_Graphs.GetEnumerator();
	while (items.MoveNext())
		{
		var graph = items.Current.Value;
		
		if (graph.visible)
			{
			for (var i=0; i<3; i++)
				{
				m_Legend.MoveTo(s_legendMargin, -yPos+i);
				s_GraphLineTo(m_Legend, graph, Vector2(s_legendMargin + s_legendSpacing, -yPos+i));
				}
			}
				
		yPos += s_legendSpacing;
		}	
	}
	
	
private function GUIDrawLegendCaptions(guiX : int, guiY : int)
	{
	var yPos = s_legendSpacing/2;
	
	var items = m_Graphs.GetEnumerator();
	while (items.MoveNext())
		{
		var graph = items.Current.Value;
		
		GUI.Label(Rect(guiX+s_legendSpacing+s_legendMargin*2, guiY+yPos, s_legendWidth, 20), graph.name, TextStyle);
		
		yPos += s_legendSpacing;
		}
	}
	
private function GUIDrawLatestValues(guiX : int, guiY : int)
	{
	var yPos = s_legendSpacing/2;
	
	var items = m_Graphs.GetEnumerator();
	while (items.MoveNext())
		{
		var graph = items.Current.Value;
		
		if (graph.visible)		
			GUI.Label(Rect(guiX, guiY+yPos, s_legendValuesWidth, 20), String.Format("{0,9:0.000}", graph.lastValue), TextStyle);
		
		yPos += s_legendSpacing;
		}
	}
	
	
	
private var m_scrollPosition : Vector2;
	
private function GUIDrawDataList()
	{
	GUILayout.BeginArea(Rect(GuiPosX, GuiPosY+20, GuiWidth-40, GuiHeight-40));	
	
	// Header fijo
	
	var dataRows = m_DataList.GetEnumerator();	
	if (dataRows.MoveNext())
		GUILayout.Label(dataRows.Current, TextStyle);
		
	// Datos en area scrollable
	
	m_scrollPosition = GUILayout.BeginScrollView(m_scrollPosition);
	
	while (dataRows.MoveNext())
		GUILayout.Label(dataRows.Current, TextStyle);
		
	GUILayout.EndScrollView();	
	GUILayout.EndArea();
	}
	
	

function OnGUI ()
	{
	m_Display.GUIDraw(GuiPosX, GuiPosY);
	
	if (showValueList)
		{
		var dataCount = autoUpdateValueList? GetGraphDataCount() : 0;
		
		if (showValueList != m_lastShowValueList || autoUpdateValueList && dataCount != m_lastValueListCount)
			{
			GenerateDataList();
			m_lastValueListCount = dataCount;
			if (autoScrollValueList)
				m_scrollPosition.y = Mathf.Infinity;
			}
		GUIDrawDataList();
		}		
	m_lastShowValueList = showValueList;

	GUI.Label(Rect(GuiPosX+4, GuiPosY+4, GuiWidth-50, 20), caption, TextStyle);	
	
	if (showCoordinates)
		{
		GUI.Label(Rect(GuiPosX+4, GuiPosY+GuiHeight-16, 48, 20), String.Format("{0:0.00}", displayRect.xMin), TextStyle);
		GUI.Label(Rect(GuiPosX+GuiWidth-48, GuiPosY+GuiHeight-16, 48, 20), String.Format("{0,7:0.00}", displayRect.xMax), TextStyle);
		
		GUI.Label(Rect(GuiPosX+GuiWidth-48, GuiPosY+4, 48, 20), String.Format("{0,7:0.00}", displayRect.yMax), TextStyle);
		GUI.Label(Rect(GuiPosX+GuiWidth-48, GuiPosY+GuiHeight-32, 48, 20), String.Format("{0,7:0.00}", displayRect.yMin), TextStyle);
		}
		
	if (showLegend)
		{
		m_Legend.GUIDraw(GuiPosX+GuiWidth+s_legendDistanceX, GuiPosY);
		GUIDrawLegendCaptions(GuiPosX+GuiWidth+s_legendDistanceX, GuiPosY);
		
		if (showRealtimeValues)
			{
			m_LegendValues.GUIDraw(GuiPosX+GuiWidth+s_legendWidth+s_legendDistanceX*1.5, GuiPosY);
			GUIDrawLatestValues(GuiPosX+GuiWidth+s_legendWidth+s_legendDistanceX*1.2+s_legendMargin, GuiPosY);
			}
		}
	}
	


function Update ()
	{
	}
	
