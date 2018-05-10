//========================================================================================================================
// Edy Vehicle Physics - (c) Angel Garcia "Edy" - Oviedo, Spain
// Live demo: http://www.edy.es/unity/offroader.html
// 
// Terms & Conditions:
//  - Use for unlimited time, any number of projects, royalty-free.
//  - Keep the copyright notices on top of the source files.
//  - Resale or redistribute as anything except a final product to the end user (asset / library / engine / middleware / etc.) is not allowed.
//  - Put me (Angel Garcia "Edy") in your game's credits as author of the vehicle physics.
//
// Bug reports, improvements to the code, suggestions on further developments, etc are always welcome.
// Unity forum user: Edy
//========================================================================================================================
//
// GUICanvas
//
// Standalone class (not derived from MonoBehavior) for common GUI drawing operations.
//
//========================================================================================================================

#pragma strict


class GUICanvasJS
	{
	private var m_texture : Texture2D;		
	private var m_pixelsWd : float;
	private var m_pixelsHt : float;
	
	private var m_viewArea : Rect;			
	
	private var m_scaleX : float;
	private var m_scaleY : float;
	
	private var m_moveX : float;
	private var m_moveY : float;
	
	private var m_pixels : Color32[];
	private var m_buffer : Color32[];
	
	private var m_alpha = -1.0;
	private var m_alphaBlend = false;
	private var m_changed = false;
	
	private var m_clipArea : Rect;
	private var m_pixelsXMin : int;
	private var m_pixelsXMax : int;
	private var m_pixelsYMin : int;
	private var m_pixelsYMax : int;
	
	// Constructores e información
	
	private function InitCanvas (pixelsWd : int, pixelsHt : int, canvasX0 : float, canvasY0 : float, canvasWd : float, canvasHt : float)	
		{
		m_texture = new Texture2D(pixelsWd, pixelsHt, TextureFormat.ARGB32, false);
		m_texture.hideFlags = HideFlags.HideAndDontSave; 
		m_pixelsWd = pixelsWd;
		m_pixelsHt = pixelsHt;
		
		m_moveX = 0.0;
		m_moveY = 0.0;
		
		m_pixels = new Color32[pixelsWd * pixelsHt];
		
		ResizeCanvas(canvasX0, canvasY0, canvasWd, canvasHt);
		}
		
	function GUICanvasJS (pixelsWd : int, pixelsHt : int, canvasX0 : float, canvasY0 : float, canvasWd : float, canvasHt : float)
		{
		InitCanvas (pixelsWd, pixelsHt, canvasX0, canvasY0, canvasWd, canvasHt);
		}
		
	function GUICanvasJS (pixelsWd : int, pixelsHt : int, canvasWd : float, canvasHt : float)
		{
		InitCanvas (pixelsWd, pixelsHt, 0.0, 0.0, canvasWd, canvasHt);
		}	
	
	function ResizeCanvas (canvasX0 : float, canvasY0 : float, canvasWd : float, canvasHt : float)	
		{
		m_scaleX = m_pixelsWd / canvasWd;
		m_scaleY = m_pixelsHt / canvasHt;
		m_viewArea.x = canvasX0;
		m_viewArea.y = canvasY0;
		m_viewArea.width = canvasWd;
		m_viewArea.height = canvasHt;
		ResetClipArea();
		}
		
	function CanvasWidth () : float { return m_viewArea.width; }
	function CanvasHeight () : float { return m_viewArea.height; }
	function PixelsWidth () : float { return m_pixelsWd; }
	function PixelsHeight () : float { return m_pixelsHt; }
	function ScaleX () : float	{ return m_scaleX; }
	function ScaleY () : float	{ return m_scaleY; }
	
	function Pixels2CanvasX (pixels : int) : float { return pixels / m_scaleX; }
	function Pixels2CanvasY (pixels : int) : float { return pixels / m_scaleY; }
	function Canvas2PixelsX (canvas : float) : int { return Mathf.RoundToInt(canvas * m_scaleX); }
	function Canvas2PixelsY (canvas : float) : int { return Mathf.RoundToInt(canvas * m_scaleY); }
	
	function Position2Canvas (pixels : Vector2) : Vector2
		{
		return Vector2((pixels.x / m_scaleX) + m_viewArea.x, (pixels.y / m_scaleY) + m_viewArea.y);
		}
	
	function SetClipArea (clipArea : Rect)
		{
		// Se recorta con el área a la vista. Si está fuera quedará con ancho negativo, con lo que se clipeará todo.
		
		m_clipArea.xMin = Mathf.Max(clipArea.xMin, m_viewArea.xMin);
		m_clipArea.xMax = Mathf.Min(clipArea.xMax, m_viewArea.xMax);
		m_clipArea.yMin = Mathf.Max(clipArea.yMin, m_viewArea.yMin);
		m_clipArea.yMax = Mathf.Min(clipArea.yMax, m_viewArea.yMax);
		
		m_pixelsXMin = (m_clipArea.xMin - m_viewArea.xMin) * m_scaleX;
		m_pixelsXMax = (m_clipArea.xMax - m_viewArea.xMin) * m_scaleX;
		m_pixelsYMin = (m_clipArea.yMin - m_viewArea.yMin) * m_scaleY;
		m_pixelsYMax = (m_clipArea.yMax - m_viewArea.yMin) * m_scaleY;
		}
		
	function ResetClipArea ()
		{
		SetClipArea(m_viewArea);
		}
	
	// Destructor - necesario al usar GUICanvas desde el Editor, para evitar dejar Texture2D sin referenciar.
	
	function DestroyTexture ()
		{
		UnityEngine.Object.DestroyImmediate(m_texture);
		}
		
	// Establecer automáticamente la transparencia. Indicar -1 para que se use siempre la del color indicado.
	
	function SetAlpha (alpha : float)
		{
		m_alpha = alpha;
		}
		
	// Establecer modo alpha-blend
	
	function SetAlphaBlend (alphaBlend: boolean)
		{
		m_alphaBlend = alphaBlend;
		}
		
	// Dibujar Lineas
	
	function Line (x0 : float, y0 : float, x1 : float, y1 : float, col : Color32)
		{
		MoveTo(x0, y0);
		LineTo(x1, y1, col);
		}
		
	function DottedLine (x0 : float, y0 : float, x1 : float, y1 : float, n : int, col : Color32)
		{
		MoveTo(x0, y0);
		DottedLineTo(x1, y1, n, col);
		}
		
	function DashedLine (x0 : float, y0 : float, x1 : float, y1 : float, n : int, col : Color32)
		{
		MoveTo(x0, y0);
		DashedLineTo(x1, y1, n, col);
		}
		
	function MoveTo (x0 : float, y0 : float)
		{
		m_moveX = x0;
		m_moveY = y0;
		}
		
	function LineTo (xn : float, yn : float, col : Color32)
		{
		LineTo(xn, yn, col, TexLine, 0);
		}
		
	function DottedLineTo (xn : float, yn : float, n : int, col : Color32)
		{
		LineTo(xn, yn, col, TexLineDot, n);
		}
	
	function DashedLineTo (xn : float, yn : float, n : int, col : Color32)
		{
		LineTo(xn, yn, col, TexLineDash, n);
		}
	
	function LineTo (xn : float, yn : float, col : Color32, TexLineFn : function(int, int, int, int, int, Color32), n : int)
		{
		if (m_alpha >= 0) col.a = Mathf.Clamp01(m_alpha) * 255;
		
		var x0 = m_moveX;
		var y0 = m_moveY;
		var x1 = xn;
		var y1 = yn;
		
		m_moveX = xn;
		m_moveY = yn;		
		
		// Asegurar que x0 <= x1. Así organizamos mejor el crop.
		
		if (x0 > x1)
			{
			var tmp = x0; x0 = x1; x1 = tmp;
			tmp = y0; y0 = y1; y1 = tmp;
			}
			
		// Cropear por izquierda y derecha
		
		var sl = (y1-y0)/(x1-x0);
		
		if (x0 < m_clipArea.xMin)  { y0 = y0 + (m_clipArea.xMin-x0) * sl; x0 = m_clipArea.xMin; }
		if (x1 > m_clipArea.xMax)  { y1 = y1 + (m_clipArea.xMax-x1) * sl; x1 = m_clipArea.xMax; }
		
		// Ya podemos descartar las lineas que no cruzarán el cuadro
		
		if (x0 > m_clipArea.xMax || x1 < m_clipArea.xMin || 
			(y0 < m_clipArea.yMin && y1 < m_clipArea.yMin) || (y0 > m_clipArea.yMax && y1 > m_clipArea.yMax))
			return;
		
		// Si llega aquí la linea cruza el cuadro necesariamente. Ya tenemos las "X" cropeadas.
		// Ajustar las coordenadas "Y" que pudieran estar fuera.
		
		if (y0 < m_clipArea.yMin) { x0 = x0 + (m_clipArea.yMin-y0) / sl; y0 = m_clipArea.yMin; }		
		if (y0 > m_clipArea.yMax) { x0 = x0 + (m_clipArea.yMax-y0) / sl; y0 = m_clipArea.yMax; }

		if (y1 < m_clipArea.yMin) { x1 = x1 + (m_clipArea.yMin-y1) / sl; y1 = m_clipArea.yMin; }		
		if (y1 > m_clipArea.yMax) { x1 = x1 + (m_clipArea.yMax-y1) / sl; y1 = m_clipArea.yMax; }
		
		// Dibujar la linea

		TexLineFn((x0-m_viewArea.x)*m_scaleX, (y0-m_viewArea.y)*m_scaleY, (x1-m_viewArea.x)*m_scaleX, (y1-m_viewArea.y)*m_scaleY, n, col);
		m_changed = true;
		}
	
	// Lineas con Vector2
		
	function Line (P0 : Vector2, P1 : Vector2, col : Color32)
		{
		MoveTo(P0.x, P0.y);
		LineTo(P1.x, P1.y, col);
		}
		
	function MoveTo (P0 : Vector2)
		{
		MoveTo(P0.x, P0.y);
		}
		
	function LineTo (P1 : Vector2, col : Color32)
		{
		LineTo(P1.x, P1.y, col);
		}

	// Lineas horizontales / verticales
	
	function LineH (y : float, col : Color32)
		{
		if (m_alpha >= 0) col.a = Mathf.Clamp01(m_alpha) * 255;
		TexSegmentH(m_pixelsXMin, m_pixelsXMax, (y-m_viewArea.yMin)*m_scaleY, col);
		m_changed = true;
		}
		
	function LineV (x : float, col : Color32)
		{
		if (m_alpha >= 0) col.a = Mathf.Clamp01(m_alpha) * 255;
		TexSegmentV((x-m_viewArea.xMin)*m_scaleX, m_pixelsYMin, m_pixelsYMax, col);
		m_changed = true;
		}
		
	function DashedLineH (y : float, n : int, col : Color32)
		{
		MoveTo(m_viewArea.xMin, y);
		DashedLineTo(m_viewArea.xMax, y, n, col);
		}
		
	function DashedLineV (x : float, n : int, col : Color32)
		{
		MoveTo(x, m_viewArea.yMin);
		DashedLineTo(x, m_viewArea.yMax, n, col);
		}
		
	function DottedLineH (y : float, n : int, col : Color32)
		{
		MoveTo(m_viewArea.xMin, y);
		DottedLineTo(m_viewArea.xMax, y, n, col);
		}
		
	function DottedLineV (x : float, n : int, col : Color32)
		{
		MoveTo(x, m_viewArea.yMin);
		DottedLineTo(x, m_viewArea.yMax, n, col);
		}
		
	// Dibujar circunferencia. El radio se mide en la escala X.
	
	function Circumference (x : float, y : float, radius : float, col : Color32)
		{
		if (m_alpha >= 0) col.a = Mathf.Clamp01(m_alpha) * 255;
		var r = radius*m_scaleX;
		TexEllipse((x-m_viewArea.x)*m_scaleX, (y-m_viewArea.y)*m_scaleY, r, r, col);
		m_changed = true;
		}
		
	function Circle (x : float, y : float, radius : float, col : Color32)
		{
		if (m_alpha >= 0) col.a = Mathf.Clamp01(m_alpha) * 255;
		var r = radius*m_scaleX;
		TexFillEllipse((x-m_viewArea.x)*m_scaleX, (y-m_viewArea.y)*m_scaleY, r, r, col);
		m_changed = true;
		}
		
	// Dibujar elipse.
	
	function Ellipse (x : float, y : float, rx : float, ry : float, col : Color32)
		{
		if (m_alpha >= 0) col.a = Mathf.Clamp01(m_alpha) * 255;
		TexEllipse((x-m_viewArea.x)*m_scaleX, (y-m_viewArea.y)*m_scaleY, rx*m_scaleX, ry*m_scaleY, col);
		m_changed = true;
		}
		
	function FillEllipse (x : float, y : float, rx : float, ry : float, col : Color32)
		{
		if (m_alpha >= 0) col.a = Mathf.Clamp01(m_alpha) * 255;
		TexFillEllipse((x-m_viewArea.x)*m_scaleX, (y-m_viewArea.y)*m_scaleY, rx*m_scaleX, ry*m_scaleY, col);
		m_changed = true;
		}
		
	// Rellenar bloques
	
	function Clear (col : Color32)
		{
		var count = m_pixelsWd * m_pixelsHt;
		
		if (m_alpha >= 0) col.a = Mathf.Clamp01(m_alpha) * 255;
		for (var i=0; i<count; i++)
			m_pixels[i] = col;
			
		m_changed = true;
		}
		
	function FillRect (x : float, y : float, width : float, height : float, col : Color32)
		{
		var x0 : int = (x-m_viewArea.x)*m_scaleX;
		var y0 : int = (y-m_viewArea.y)*m_scaleY;
		var x1 : int = x0 + width*m_scaleX;
		var y1 : int = y0 + height*m_scaleY;
		
		if (x1 <= x0 || y1 <= y0) return;
		if (m_alpha >= 0) col.a = Mathf.Clamp01(m_alpha) * 255;
		
		for (var j=y0; j<y1; j++)
			TexSegmentH(x0, x1, j, col);
				
		m_changed = true;
		}
		
	// Dibujar spline
	
	function SpLine (P0 : Vector2, T0 : Vector2, P1 : Vector2, T1 : Vector2, col : Color32)
		{
		SpLine (P0, T0, P1, T1, col, 20);
		}		
		
	function SpLine (P0 : Vector2, T0 : Vector2, P1 : Vector2, T1 : Vector2, col : Color32, steps : int)
		{
		SpLine (P0, T0, P1, T1, col, steps, 1.0);
		}		
		
	function SpLine (P0 : Vector2, T0 : Vector2, P1 : Vector2, T1 : Vector2, col : Color32, steps : int, scaleY : float)
		{
		var s : float;
		var s2 : float;
		var s3 : float;
		var h1 : float;
		var h2 : float;
		var h3 : float;
		var h4 : float;
		var P : Vector2;
		
		MoveTo(P0.x, P0.y * scaleY);
		for (var t=0; t<=steps; t++)
			{
			s = t;
			s /= steps;
			s2 = s*s;
			s3 = s2*s;
			
			// Valores de las funciones de Hermite
			
			h1 =  2*s3 - 3*s2 + 1;
			h2 = -2*s3 + 3*s2;
			h3 =    s3 - 2*s2 + s;
			h4 =    s3 - s2;
			
			// Estas son las ecuaciones para curvas de Bezier - yo no he notado absolutamente ninguna diferencia.
			// h1 =   -s3 + 3*s2 - 3*s + 1;
			// h2 =  3*s3 - 6*s2 + 3*s;
			// h3 = -3*s3 + 3*s2;
			// h4 =    s3;
			
			// Punto interpolado
			
			P = h1*P0 + h2*P1 + h3*T0 + h4*T1;
			LineTo(P.x, P.y * scaleY, col);
			}
		}
			
	// Especializadas
	// -----------------------------------------------------------

	// Dibujar una rejilla a intervalos dados
	
	function Grid (stepX : float, stepY : float, col : Color32)
		{
		var f : float;
		
		if (stepX < Pixels2CanvasX(1)) stepX = Pixels2CanvasX(1);
		if (stepY < Pixels2CanvasY(1)) stepY = Pixels2CanvasY(1);
		
		var x0 = Mathf.FloorToInt(m_viewArea.x / stepX) * stepX;
		var y0 = Mathf.FloorToInt(m_viewArea.y / stepY) * stepY;
		
		for (f=x0; f<=m_viewArea.xMax; f+=stepX) LineV(f, col);
		for (f=y0; f<=m_viewArea.yMax; f+=stepY) LineH(f, col);
		}
		
		
	function DottedGrid (stepX : float, stepY : float, n : int, col : Color32)
		{
		var f : float;
		
		if (stepX < Pixels2CanvasX(1)) stepX = Pixels2CanvasX(1);
		if (stepY < Pixels2CanvasY(1)) stepY = Pixels2CanvasY(1);
		
		var x0 = Mathf.FloorToInt(m_viewArea.x / stepX) * stepX;
		var y0 = Mathf.FloorToInt(m_viewArea.y / stepY) * stepY;
		
		for (f=x0; f<=m_viewArea.xMax; f+=stepX) DottedLineV(f, n, col);
		for (f=y0; f<=m_viewArea.yMax; f+=stepY) DottedLineH(f, n, col);
		}
		

	function DotMatrix (stepX : float, stepY : float, col : Color32)
		{
		if (stepX < Pixels2CanvasX(1)) stepX = Pixels2CanvasX(1);
		if (stepY < Pixels2CanvasY(1)) stepY = Pixels2CanvasY(1);
		
		var x0 = Mathf.FloorToInt(m_viewArea.x / stepX) * stepX;
		var y0 = Mathf.FloorToInt(m_viewArea.y / stepY) * stepY;
		
		for (var y=y0; y<=m_viewArea.yMax; y+=stepY)
			for (var x=x0; x<=m_viewArea.xMax; x+=stepX)
				Dot(x, y, col);
		}

	// Dibujar una curva progresiva (Bias) entre dos puntos con el coeficiente dado
	
	function BiasCurve (P0 : Vector2, P1 : Vector2, bias : float, col : Color32)
		{
		var Steps = 20;
		
		var s : float;
		var c : float;
		
		var dX = P1.x - P0.x;
		var dY = P1.y - P0.y;
		
		MoveTo(P0);
		for (var t=0; t<=Steps; t++)
			{
			s = t;
			s /= Steps;			
			c = Bias(s, bias);
			
			LineTo(P0.x + dX*s, P0.y + dY*c, col);
			}
		}
		
	// Dibujar un punto visible que ilumine 3 pixels
	
	function Dot (x : float, y : float, col : Color32)
		{
		if (m_alpha >= 0) col.a = Mathf.Clamp01(m_alpha) * 255;
		
		var px = (x-m_viewArea.x)*m_scaleX;
		var py = (y-m_viewArea.y)*m_scaleY;

		TexSetPixel(px, py-1, col);
		TexSetPixel(px-1, py, col);
		TexSetPixel(px, py, col);
		TexSetPixel(px+1, py, col);
		TexSetPixel(px, py+1, col);		
		
		m_changed = true;		
		}
		
	function Dot (P : Vector2, col : Color32)
		{
		Dot(P.x, P.y, col);
		}	
	
	// Gráfica de lineas con los valores dados.
	// - Para una sola gráfica Values debe llevar pares consecutivos X,Y con ValueSize=2  (número total de valores en cada conjunto)
	// - Para n gráficas indicar valores consecutivos X,Y1,Y2,Y3..Yn con ValueSize=n+1
	//   Se dibujarán las gráficas (X,Y1), (X,Y2), (X,Y3) .. (X,Yn)
	
	private var COLORS = [Color.green, Color.yellow, Color.cyan, Color.magenta, Color.white, Color.blue, Color.red, Color.gray];
	
	function LineGraph (Values : float[], ValueSize : int)
		{
		var X : float;
		var Y : float;
		
		if (ValueSize < 2) return;
		if (Values.length < 2*ValueSize) return;

		for (var i=1; i<ValueSize; i++)
			{
			MoveTo(Values[0], Values[i]);
			
			for (var v=1; v<Values.length/ValueSize; v++)
				LineTo(Values[v*ValueSize], Values[v*ValueSize + i], COLORS[(i-1) % 8]);
			}
		}


	// Función y=f(x) entre los valores de x dados
	
	function Function(func : function(float) : float, x0 : float, x1 : float, col : Color32, stepSize : float, scaleY : float)
		{
		MoveTo(x0, func(x0) * scaleY);
		
		for (var x=x0; x<=x1; x+=stepSize)
			LineTo(x, func(x) * scaleY, col);
			
		if (x < x1)
			LineTo(x1, func(x1) * scaleY, col);
		}
		
	function Function(func : function(float) : float, x0 : float, x1 : float, col : Color32, stepSize : float)
		{
		this.Function(func, x0, x1, col, stepSize, 1.0);
		}
		
	function Function(func : function(float) : float, x0 : float, x1 : float, col : Color32)
		{
		this.Function(func, x0, x1, col, Pixels2CanvasX(3), 1.0);
		}

	function Function(func : function(float) : float, col : Color32)
		{
		this.Function(func, m_viewArea.xMin, m_viewArea.xMax, col, Pixels2CanvasX(3), 1.0);
		}
		
		
	// Función y=f(x, p) siendo p un parámetro fijo que se pasa a la función	
	
	function Function2(func : function(float, float) : float, p : float, x0 : float, x1 : float, col : Color32, stepSize : float, scaleY : float)
		{
		MoveTo(x0, func(x0, p) * scaleY);
		
		for (var x=x0; x<=x1; x+=stepSize)
			LineTo(x, func(x, p) * scaleY, col);
			
		if (x < x1)
			LineTo(x1, func(x1, p) * scaleY, col);
		}
		
	function Function2(func : function(float, float) : float, p : float, x0 : float, x1 : float, col : Color32, stepSize : float)
		{
		this.Function2(func, p, x0, x1, col, stepSize, 1.0);
		}
	
	function Function2(func : function(float, float) : float, p : float, x0 : float, x1 : float, col : Color32)
		{
		this.Function2(func, p, x0, x1, col, Pixels2CanvasX(3), 1.0);
		}
		
	function Function2(func : function(float, float) : float, p: float, col : Color32)
		{
		this.Function2(func, p, m_viewArea.xMin, m_viewArea.xMax, col, Pixels2CanvasX(3), 1.0);
		}		

		
	// Función y=f(x, p, q) siendo p y q dos parámetros fijos que se pasan a la función	
	
	function Function3(func : function(float, float, float) : float, p : float, q : float, x0 : float, x1 : float, col : Color32, stepSize : float, scaleY : float)
		{
		MoveTo(x0, func(x0, p, q) * scaleY);
		
		for (var x=x0; x<=x1; x+=stepSize)
			LineTo(x, func(x, p, q) * scaleY, col);
			
		if (x < x1)
			LineTo(x1, func(x1, p, q) * scaleY, col);
		}		

	function Function3(func : function(float, float, float) : float, p : float, q : float, x0 : float, x1 : float, col : Color32, stepSize : float)
		{
		this.Function3(func, p, q, x0, x1, col, stepSize, 1.0);
		}
	
	function Function3(func : function(float, float, float) : float, p : float, q : float, x0 : float, x1 : float, col : Color32)
		{
		this.Function3(func, p, q, x0, x1, col, Pixels2CanvasX(3), 1.0);
		}
		
	function Function3(func : function(float, float, float) : float, p : float, q : float, col : Color32)
		{
		this.Function3(func, p, q, m_viewArea.xMin, m_viewArea.xMax, col, Pixels2CanvasX(3), 1.0);
		}		
		
	function Function3(func : function(float, float, float) : float, p : float, q : float, col : Color32, stepSize : float)
		{
		this.Function3(func, p, q, m_viewArea.xMin, m_viewArea.xMax, col, stepSize, 1.0);
		}
	
	
	function FunctionColorFill (func : function(float) : Vector4, x0 : float, x1 : float, y0 : float)
		{
		var px0 : int = (x0-m_viewArea.x)*m_scaleX;
		var px1 : int = (x1-m_viewArea.x)*m_scaleX;
		var col : Color32;
		var raw : Vector4;
		
		var py0 = (y0-m_viewArea.y) * m_scaleY;
		var py1 : int;		
		
		if (x1 <= x0) return;
		
		col.a = m_alpha >= 0? Mathf.Clamp01(m_alpha) * 255 : 255;
		
		for (var i=px0; i<px1; i++)
			{
			raw = func((i/m_scaleX) + m_viewArea.x);
			col.r = raw.x * 255;
			col.g = raw.y * 255;
			col.b = raw.z * 255;
			
			py1 = Mathf.RoundToInt((raw.w-m_viewArea.y) * m_scaleY);
			TexSegmentV(i, py0, py1, col);
			}
		}
		
		
	function ContourFill (func : function(float, float) : Color32, x : float, y : float, width : float, height : float)
		{
		var x0 : int = (x-m_viewArea.x)*m_scaleX;
		var y0 : int = (y-m_viewArea.y)*m_scaleY;
		var x1 : int = x0 + width*m_scaleX;
		var y1 : int = y0 + height*m_scaleY;		
		var col : Color32;
		
		if (x1 <= x0 || y1 <= y0) return;
		
		for (var j=y0; j<y1; j++)
			for (var i=x0; i<x1; i++)
				{
				col = func((i/m_scaleX) + m_viewArea.x, (j/m_scaleY) + m_viewArea.y);
				if (m_alpha >= 0) col.a = Mathf.Clamp01(m_alpha) * 255;
				
				TexSetPixel(i, j, col);
				}
				
		m_changed = true;
		}		
		
	// Guardar / Restaurar
	// -----------------------------------------------------------

	function Save ()
		{
		if (m_buffer == null)
			m_buffer = m_pixels.Clone() as Color32[];
		else
			m_pixels.CopyTo(m_buffer, 0);
		}
		
	function Restore ()
		{
		if (m_buffer)
			{
			m_buffer.CopyTo(m_pixels, 0);
			m_changed = true;
			}
		}
	
	// Dibujar en el GUI. Invocar sólo desde función OnGUI
	// -----------------------------------------------------------
	
	function ApplyChanges()
		{
		if (m_changed)
			{
			m_texture.SetPixels32(m_pixels);			
			m_texture.Apply(false);				// Este es el comecpu
			m_changed = false;
			}
		}
	
	function GUIDraw (posX : int, posY : int)
		{
		ApplyChanges();
		GUI.DrawTexture(Rect(posX, posY, m_pixelsWd, m_pixelsHt), m_texture);
		}
		
	function GUIDrawDirty (posX : int, posY : int)
		{
		GUI.DrawTexture(Rect(posX, posY, m_pixelsWd, m_pixelsHt), m_texture);
		}		
		
	function GUIStretchDraw(posX : int, posY : int, width : int, height : int)
		{
		ApplyChanges();
		GUI.DrawTexture(Rect(posX, posY, width, height), m_texture);
		}
		
	function GUIStretchDrawWidth(posX : int, posY : int, width : int)
		{
		ApplyChanges();
		var ratio = m_pixelsHt / m_pixelsWd;
		GUI.DrawTexture(Rect(posX, posY, width, width * ratio), m_texture);
		}
		
	function GetTexture() : Texture
		{
		ApplyChanges();
		return m_texture;
		}
	}


// ----------- función de curva progresiva (Bias)

private var m_lastExponent = 0.0;
private var m_lastBias = -1.0;

private function BiasRaw(x : float, fBias : float) : float
	{
	if (x <= 0.0) return 0.0;
	if (x >= 1.0) return 1.0;

	if (fBias != m_lastBias)
		{
		if (fBias <= 0.0) return x >= 1.0? 1.0 : 0.0;
		else if (fBias >= 1.0) return x > 0.0? 1.0 : 0.0;
		else if (fBias == 0.5) return x;

		m_lastExponent = Mathf.Log(fBias) * -1.4427;
		m_lastBias = fBias;
		}

	return Mathf.Pow(x, m_lastExponent);
	}

	
// Bias simétrico usando sólo la curva inferior (fBias < 0.5)
// Admite rango -1, 1 aplicando efecto simétrico desde 0 hacia +1 y -1.

private function Bias(x : float, fBias : float) : float
	{
	var fResult : float;
		
	fResult = fBias <= 0.5? BiasRaw(Mathf.Abs(x), fBias) : 1.0 - BiasRaw(1.0 - Mathf.Abs(x), 1.0 - fBias);
	
	return x<0.0? -fResult : fResult;
	}

	

// ----------- funciones de UnifyWiki modificadas y funciones propias añadidas


private function TexSetPixel (x : int, y : int, col : Color32)
	{
	if (x >= m_pixelsXMin && x < m_pixelsXMax && y >= m_pixelsYMin && y < m_pixelsYMax)
		{
		var pixel = y * m_pixelsWd + x;
		
		if (!m_alphaBlend)
			{
			m_pixels[pixel] = col;
			}
		else
			{
			var dst = m_pixels[pixel];
			var srcAlpha = col.a / 255.0;
			var dstAlpha = 1.0 - srcAlpha;
			
			dst.r = col.r*srcAlpha + dst.r*dstAlpha;
			dst.g = col.g*srcAlpha + dst.g*dstAlpha;
			dst.b = col.b*srcAlpha + dst.b*dstAlpha;
			dst.a = col.a*srcAlpha + dst.a*dstAlpha;
			
			m_pixels[pixel] = dst;
			}
		}
	}
	
	
private function TexLine (x0 : int, y0 : int, x1 : int, y1 : int, n : int, col : Color32) 
	{
    var dy = y1-y0;
    var dx = x1-x0;
    
    if (dy < 0) {dy = -dy; var stepy = -1;}
    else {stepy = 1;}
    if (dx < 0) {dx = -dx; var stepx = -1;}
    else {stepx = 1;}
    dy <<= 1;
    dx <<= 1;
	
    TexSetPixel(x0, y0, col);
    if (dx > dy) {
        var fraction = dy - (dx >> 1);
        while (x0 != x1) {
            if (fraction >= 0) {
                y0 += stepy;
                fraction -= dx;
            }
            x0 += stepx;
            fraction += dy;
            TexSetPixel(x0, y0, col);
        }
    }
    else {
        fraction = dx - (dy >> 1);
        while (y0 != y1) {
            if (fraction >= 0) {
                x0 += stepx;
                fraction -= dy;
            }
            y0 += stepy;
            fraction += dx;
            TexSetPixel(x0, y0, col);
        }
    }
}

private function TexLineDot (x0 : int, y0 : int, x1 : int, y1 : int, n : int, col : Color32) 
	{
    var dy = y1-y0;
    var dx = x1-x0;
	var i = 0;
    
    if (dy < 0) {dy = -dy; var stepy = -1;}
    else {stepy = 1;}
    if (dx < 0) {dx = -dx; var stepx = -1;}
    else {stepx = 1;}
    dy <<= 1;
    dx <<= 1;
	
    TexSetPixel(x0, y0, col);
    if (dx > dy) {
        var fraction = dy - (dx >> 1);
        while (x0 != x1) {
            if (fraction >= 0) {
                y0 += stepy;
                fraction -= dx;
            }
            x0 += stepx;
            fraction += dy;
			if (++i >= n)
				{
				TexSetPixel(x0, y0, col);
				i=0;
				}
        }
    }
    else {
        fraction = dx - (dy >> 1);
        while (y0 != y1) {
            if (fraction >= 0) {
                x0 += stepx;
                fraction -= dy;
            }
            y0 += stepy;
            fraction += dx;
			if (++i >= n)
				{
				TexSetPixel(x0, y0, col);
				i=0;
				}
        }
    }
}


private function TexLineDash (x0 : int, y0 : int, x1 : int, y1 : int, n : int, col : Color32) 
	{
    var dy = y1-y0;
    var dx = x1-x0;
	var i = 0;
    
    if (dy < 0) {dy = -dy; var stepy = -1;}
    else {stepy = 1;}
    if (dx < 0) {dx = -dx; var stepx = -1;}
    else {stepx = 1;}
    dy <<= 1;
    dx <<= 1;
	
    TexSetPixel(x0, y0, col);
    if (dx > dy) {
        var fraction = dy - (dx >> 1);
        while (x0 != x1) {
            if (fraction >= 0) {
                y0 += stepy;
                fraction -= dx;
            }
            x0 += stepx;
            fraction += dy;
			if (++i % (n*2) < n)
				TexSetPixel(x0, y0, col);
        }
    }
    else {
        fraction = dx - (dy >> 1);
        while (y0 != y1) {
            if (fraction >= 0) {
                x0 += stepx;
                fraction -= dy;
            }
            y0 += stepy;
            fraction += dx;
			if (++i % (n*2) < n)
				TexSetPixel(x0, y0, col);
        }
    }
}



private function TexSegmentV(x : int, y0 : int, y1 : int, col : Color32)
	{
	if (y0 > y1)
		{
		var swap = y0;
		y0 = y1;
		y1 = swap;
		}
		
	if (x < m_pixelsXMin || x >= m_pixelsXMax || y1 < m_pixelsYMin || y0 >= m_pixelsYMax) return;
	
	if (y0 < m_pixelsYMin) y0 = m_pixelsYMin;
	if (y1 >= m_pixelsYMax) y1 = m_pixelsYMax;
	
	var y : int;
	
	if (!m_alphaBlend)
		{
		for (y = y0; y < y1; y++)
			m_pixels[y * m_pixelsWd + x] = col;
		}
	else
		{
		var pixel : int;
		var dst : Color32;
		
		var srcAlpha = col.a / 255.0;
		var dstAlpha = 1.0 - srcAlpha;
		
		for (y = y0; y < y1; y++)
			{
			pixel = y * m_pixelsWd + x;
			
			dst = m_pixels[pixel];
			dst.r = col.r*srcAlpha + dst.r*dstAlpha;
			dst.g = col.g*srcAlpha + dst.g*dstAlpha;
			dst.b = col.b*srcAlpha + dst.b*dstAlpha;
			dst.a = col.a*srcAlpha + dst.a*dstAlpha;
			m_pixels[pixel] = dst;
			}
		}
	}


private function TexSegmentH(x0 : int, x1 : int, y : int, col : Color32)
	{
	if (x0 > x1)
		{
		var swap = x0;
		x0 = x1;
		x1 = swap;
		}
		
	if (y < m_pixelsYMin || y >= m_pixelsYMax || x1 < m_pixelsXMin || x0 >= m_pixelsXMax) return;
	
	if (x0 < m_pixelsXMin) x0 = m_pixelsXMin;
	if (x1 > m_pixelsXMax) x1 = m_pixelsXMax;
	
	var x : int;
	var offset = y * m_pixelsWd;
	
	if (!m_alphaBlend)
		{
		for (x = x0; x < x1; x++)
			m_pixels[offset + x] = col;
		}
	else
		{
		var pixel : int;
		var dst : Color32;
		
		var srcAlpha = col.a / 255.0;
		var dstAlpha = 1.0 - srcAlpha;
		
		for (x = x0; x < x1; x++)
			{
			pixel = offset + x;
			
			dst = m_pixels[pixel];
			dst.r = col.r*srcAlpha + dst.r*dstAlpha;
			dst.g = col.g*srcAlpha + dst.g*dstAlpha;
			dst.b = col.b*srcAlpha + dst.b*dstAlpha;
			dst.a = col.a*srcAlpha + dst.a*dstAlpha;
			m_pixels[pixel] = dst;
			}
		}
	}


private function TexEllipse (cx : int, cy : int, rx : int, ry : int, col : Color32)
	{
	var sqrt2 = Mathf.Sqrt(2);
	var x : int;
	var y : int;
	var d : int;
	var end : int;

	if (rx >= ry)
		{
		y = rx;
		d = -rx;
		end = Mathf.Ceil(rx/sqrt2);

		var sy = 1.0 * ry/rx;		// 1.0 para que haga la división float
	 
		for (x = 0; x <= end; x++)
			{
			TexSetPixel(cx+x, cy+y*sy, col);
			TexSetPixel(cx+x, cy-y*sy, col);
			TexSetPixel(cx-x, cy+y*sy, col);
			TexSetPixel(cx-x, cy-y*sy, col);

			TexSetPixel(cx+y, cy+x*sy, col);
			TexSetPixel(cx-y, cy+x*sy, col);
			TexSetPixel(cx+y, cy-x*sy, col);
			TexSetPixel(cx-y, cy-x*sy, col);
			
			d += 2*x+1;
			if (d > 0) d += 2 - 2*y--;
			}
		}
	else
		{		
		x = ry;
		d = -ry;
		end = Mathf.Ceil(ry/sqrt2);
		
		var sx = 1.0 * rx/ry;

		for (y = 0; y <= end; y++)
			{
			TexSetPixel(cx+y*sx, cy+x, col);
			TexSetPixel(cx+y*sx, cy-x, col);
			TexSetPixel(cx-y*sx, cy+x, col);
			TexSetPixel(cx-y*sx, cy-x, col);
			
			TexSetPixel(cx+x*sx, cy+y, col);
			TexSetPixel(cx-x*sx, cy+y, col);
			TexSetPixel(cx+x*sx, cy-y, col);
			TexSetPixel(cx-x*sx, cy-y, col);
			
			d += 2*y+1;
			if (d > 0) d += 2 - 2*x--;
			}
		}
		
	}
	

private function TexFillEllipse (cx : int, cy : int, rx : int, ry : int, col : Color32)
	{
	var sqrt2 = Mathf.Sqrt(2);
	var x : int;
	var y : int;
	var d : int;
	var end : int;

	if (rx >= ry)
		{
		y = rx;
		d = -rx;
		end = Mathf.Ceil(rx/sqrt2);

		var sy = 1.0 * ry/rx;		// 1.0 para que haga la división float
	 
		for (x = 0; x <= end; x++)
			{
			TexSegmentV(cx+x, cy, cy+y*sy, col);						
			TexSegmentV(cx+x, cy, cy-y*sy, col);			
			TexSegmentV(cx-x, cy, cy+y*sy, col);
			TexSegmentV(cx-x, cy, cy-y*sy, col);

			TexSegmentV(cx+y, cy, cy+x*sy, col);
			TexSegmentV(cx-y, cy, cy+x*sy, col);
			TexSegmentV(cx+y, cy, cy-x*sy, col);
			TexSegmentV(cx-y, cy, cy-x*sy, col);
			
			d += 2*x+1;
			if (d > 0) d += 2 - 2*y--;
			}
		}
	else
		{		
		x = ry;
		d = -ry;
		end = Mathf.Ceil(ry/sqrt2);
		
		var sx = 1.0 * rx/ry;

		for (y = 0; y <= end; y++)
			{
			TexSegmentH(cx+y*sx, cx, cy+x, col);
			TexSegmentH(cx+y*sx, cx, cy-x, col);
			TexSegmentH(cx-y*sx, cx, cy+x, col);
			TexSegmentH(cx-y*sx, cx, cy-x, col);
			
			TexSegmentH(cx+x*sx, cx, cy+y, col);
			TexSegmentH(cx-x*sx, cx, cy+y, col);
			TexSegmentH(cx+x*sx, cx, cy-y, col);
			TexSegmentH(cx-x*sx, cx, cy-y, col);
			
			d += 2*y+1;
			if (d > 0) d += 2 - 2*x--;
			}
		}
	}


