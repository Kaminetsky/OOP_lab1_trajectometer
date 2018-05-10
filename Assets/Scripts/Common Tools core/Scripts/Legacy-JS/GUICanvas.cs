//========================================================================================================================
// (c) Angel Garcia "Edy" - Oviedo, Spain
// http://www.edy.es
//
// Class for common GUI drawing operations.
//========================================================================================================================

using UnityEngine;


public class GUICanvas
	{
	private Texture2D m_texture;
	private float m_pixelsWd;
	private float m_pixelsHt;

	private Rect m_viewArea = new Rect();

	private float m_scaleX;
	private float m_scaleY;

	private float m_moveX;
	private float m_moveY;
	private int m_step;

	private Color32[] m_pixels;
	private Color32[] m_buffer;

	private float m_alpha = -1.0f;
	private bool m_alphaBlend = false;
	private bool m_changed = false;

	private Rect m_clipArea = new Rect();
	private int m_pixelsXMin;
	private int m_pixelsXMax;
	private int m_pixelsYMin;
	private int m_pixelsYMax;

	private int m_functionResolution = 3;	// Pixels

	private float m_lastExponent;
	private float m_lastBias = -1.0f;

	private Color[] COLORS = { Color.green, Color.yellow, Color.cyan, Color.magenta, Color.white, Color.blue, Color.red, Color.gray };

	// Constructors and information

	public GUICanvas (int pixelsWd, int pixelsHt, float canvasX0, float canvasY0, float canvasWd, float canvasHt)
		{
		InitCanvas(pixelsWd, pixelsHt, canvasX0, canvasY0, canvasWd, canvasHt);
		}

	public GUICanvas (int pixelsWd, int pixelsHt, float canvasWd, float canvasHt)
		{
		InitCanvas(pixelsWd, pixelsHt, 0.0f, 0.0f, canvasWd, canvasHt);
		}

	public GUICanvas (int pixelsWd, int pixelsHt)
		{
		InitCanvas(pixelsWd, pixelsHt, 0.0f, 0.0f, 1.0f, 1.0f);
		}

	private void InitCanvas (int pixelsWd, int pixelsHt, float canvasX0, float canvasY0, float canvasWd, float canvasHt)
		{
		Debug.LogWarning("GUICanvas is deprecated. Use TextureCanvas instead, unless you need specific functions of GUICanvas");

		m_texture = new Texture2D(pixelsWd, pixelsHt, TextureFormat.ARGB32, false);
		m_texture.hideFlags = HideFlags.HideAndDontSave;
		m_pixelsWd = (float)pixelsWd;
		m_pixelsHt = (float)pixelsHt;

		m_moveX = 0.0f;
		m_moveY = 0.0f;

		m_pixels = new Color32[pixelsWd * pixelsHt];

		ResizeCanvas(canvasX0, canvasY0, canvasWd, canvasHt);
		}

	public void ResizeCanvas (float canvasX0, float canvasY0, float canvasWd, float canvasHt)
		{
		m_scaleX = m_pixelsWd / canvasWd;
		m_scaleY = m_pixelsHt / canvasHt;
		m_viewArea.x = canvasX0;
		m_viewArea.y = canvasY0;
		m_viewArea.width = canvasWd;
		m_viewArea.height = canvasHt;

		ResetClipArea();
		}

	public void ResizeCanvas (float canvasWidth, float canvasHeight)
		{
		ResizeCanvas(0.0f, 0.0f, canvasWidth, canvasHeight);
		}

	public float CanvasWidth () { return m_viewArea.width; }
	public float CanvasHeight () { return m_viewArea.height; }
	public float CanvasXMin() { return m_viewArea.xMin; }
	public float CanvasXMax() { return m_viewArea.xMax; }
	public float CanvasYMin() { return m_viewArea.yMin; }
	public float CanvasYMax() { return m_viewArea.yMax; }

	public float PixelsWidth () { return m_pixelsWd; }
	public float PixelsHeight () { return m_pixelsHt; }
	public float ScaleX () { return m_scaleX; }
	public float ScaleY () { return m_scaleY; }

	public float Pixels2CanvasX (int pixels) { return (float)pixels / m_scaleX; }
	public float Pixels2CanvasY (int pixels) { return (float)pixels / m_scaleY; }
	public int Canvas2PixelsX (float canvas) { return Mathf.RoundToInt(canvas * m_scaleX); }
	public int Canvas2PixelsY (float canvas) { return Mathf.RoundToInt(canvas * m_scaleY); }


	public Vector2 Position2Canvas (Vector2 pixels)
		{
		return new Vector2(pixels.x / m_scaleX + m_viewArea.x, pixels.y / m_scaleY + m_viewArea.y);
		}

	public void SetClipArea (Rect clipArea)
		{
		// It's clipped against the visible area. If the clipped area falls outside the
		// visible area the width/height will become negative (= full clip).

		m_clipArea.xMin = Mathf.Max(clipArea.xMin, m_viewArea.xMin);
		m_clipArea.xMax = Mathf.Min(clipArea.xMax, m_viewArea.xMax);
		m_clipArea.yMin = Mathf.Max(clipArea.yMin, m_viewArea.yMin);
		m_clipArea.yMax = Mathf.Min(clipArea.yMax, m_viewArea.yMax);

		m_pixelsXMin = (int)((m_clipArea.xMin - m_viewArea.xMin) * m_scaleX);
		m_pixelsXMax = (int)((m_clipArea.xMax - m_viewArea.xMin) * m_scaleX);
		m_pixelsYMin = (int)((m_clipArea.yMin - m_viewArea.yMin) * m_scaleY);
		m_pixelsYMax = (int)((m_clipArea.yMax - m_viewArea.yMin) * m_scaleY);
		}

	public void ResetClipArea ()
		{
		SetClipArea(m_viewArea);
		}

	public void SetFunctionResolution (int pixels)
		{
		m_functionResolution = Mathf.Max(1, pixels);
		}

	// Destructor - manually release the texture. Use before discarding the GUICanvas object.

	public void DestroyTexture ()
		{
		Object.DestroyImmediate(m_texture);
		}

	// Set the transparency of all further drawings (instead of using the Color's "a" value).
	// Set to -1 to allow the color.a value to be used.

	public void SetAlpha (float alpha)
		{
		m_alpha = alpha;
		}

	// Enable the alpha blending mode. Further drawings are combined with actual ones.

	public void SetAlphaBlend (bool alphaBlend)
		{
		m_alphaBlend = alphaBlend;
		}

	// Draw lines

	public void Line (float x0, float y0, float x1, float y1, Color32 col)
		{
		MoveTo(x0, y0);
		LineTo(x1, y1, col);
		}

	public void DottedLine (float x0, float y0, float x1, float y1, int n, Color32 col)
		{
		MoveTo(x0, y0);
		DottedLineTo(x1, y1, n, col);
		}

	public void DashedLine (float x0, float y0, float x1, float y1, int n, Color32 col)
		{
		MoveTo(x0, y0);
		DashedLineTo(x1, y1, n, col);
		}

	public void MoveTo (float x0, float y0)
		{
		m_moveX = x0;
		m_moveY = y0;
		m_step = 0;
		}

	public void LineTo (float xn, float yn, Color32 col)
		{
		LineTo(xn, yn, col, TexLine, 0);
		}

	public void DottedLineTo (float xn, float yn, int n, Color32 col)
		{
		LineTo(xn, yn, col, TexLineDot, n);
		}

	public void DashedLineTo (float xn, float yn, int n, Color32 col)
		{
		LineTo(xn, yn, col, TexLineDash, n);
		}

	// System.Action<> : Delegate that takes the arguments and returns void.
	// System.Func<> : Latest argument is the return type.
	//
	// Both delegates take up to 4 arguments (not counting the return type).
	// More parameters require defining a custom deletegate.

	public delegate void DrawLineDelegate (int x0, int y0, int x1, int y1, int n, Color32 col);

	public void LineTo (float xn, float yn, Color32 col, DrawLineDelegate TexLineFn, int n)
		{
		if (m_alpha >= 0.0f) col.a = (byte)(Mathf.Clamp01(m_alpha) * 255.0f);

		float x0 = m_moveX;
		float y0 = m_moveY;
		float x1 = xn;
		float y1 = yn;

		m_moveX = xn;
		m_moveY = yn;

		// Ensure that x0 <= x1 for better crop calculation

		if (x0 > x1)
			{
			float tmp = x0; x0 = x1; x1 = tmp;
			tmp = y0; y0 = y1; y1 = tmp;
			}

		// Left / right crop

		float sl = (y1 - y0) / (x1 - x0);

		if (x0 < m_clipArea.xMin) { y0 += (m_clipArea.xMin - x0) * sl; x0 = m_clipArea.xMin; }
		if (x1 > m_clipArea.xMax) { y1 += (m_clipArea.xMax - x1) * sl; x1 = m_clipArea.xMax; }

		// We can now discard the lines that won't cross the visible view.

		if (x0 > m_clipArea.xMax || x1 < m_clipArea.xMin ||
			(y0 < m_clipArea.yMin && y1 < m_clipArea.yMin) || (y0 > m_clipArea.yMax && y1 > m_clipArea.yMax))
			return;

		// Here the line necessarily crosses the visible viewport. X coords are already cropped.
		// We now crop the Y coords that may be outside the view.

		if (y0 < m_clipArea.yMin) { x0 += (m_clipArea.yMin - y0) / sl; y0 = m_clipArea.yMin; }
		if (y0 > m_clipArea.yMax) { x0 += (m_clipArea.yMax - y0) / sl; y0 = m_clipArea.yMax; }

		if (y1 < m_clipArea.yMin) { x1 += (m_clipArea.yMin - y1) / sl; y1 = m_clipArea.yMin; }
		if (y1 > m_clipArea.yMax) { x1 += (m_clipArea.yMax - y1) / sl; y1 = m_clipArea.yMax; }

		// Draw the resulting line

		TexLineFn((int)((x0 - m_viewArea.x) * m_scaleX), (int)((y0 - m_viewArea.y) * m_scaleY), (int)((x1 - m_viewArea.x) * m_scaleX), (int)((y1 - m_viewArea.y) * m_scaleY), n, col);
		m_changed = true;
		}

	// Lines with Vector2

	public void Line (Vector2 P0, Vector2 P1, Color32 col)
		{
		MoveTo(P0.x, P0.y);
		LineTo(P1.x, P1.y, col);
		}

	public void MoveTo (Vector2 P0)
		{
		MoveTo(P0.x, P0.y);
		}

	// Horizontal / vertical lines

	public void LineTo (Vector2 P1, Color32 col)
		{
		LineTo(P1.x, P1.y, col);
		}

	public void LineH (float y, Color32 col)
		{
		if (m_alpha >= 0.0f) col.a = (byte)(Mathf.Clamp01(m_alpha) * 255.0f);

		TexSegmentH(m_pixelsXMin, m_pixelsXMax, (int)((y - m_viewArea.yMin) * m_scaleY), col);
		m_changed = true;
		}

	public void LineV (float x, Color32 col)
		{
		if (m_alpha >= 0.0f) col.a = (byte)(Mathf.Clamp01(m_alpha) * 255.0f);

		TexSegmentV((int)((x - m_viewArea.xMin) * m_scaleX), m_pixelsYMin, m_pixelsYMax, col);
		m_changed = true;
		}

	public void DashedLineH (float y, int n, Color32 col)
		{
		MoveTo(m_viewArea.xMin, y);
		DashedLineTo(m_viewArea.xMax, y, n, col);
		}

	public void DashedLineV (float x, int n, Color32 col)
		{
		MoveTo(x, m_viewArea.yMin);
		DashedLineTo(x, m_viewArea.yMax, n, col);
		}

	public void DottedLineH (float y, int n, Color32 col)
		{
		MoveTo(m_viewArea.xMin, y);
		DottedLineTo(m_viewArea.xMax, y, n, col);
		}

	public void DottedLineV (float x, int n, Color32 col)
		{
		MoveTo(x, m_viewArea.yMin);
		DottedLineTo(x, m_viewArea.yMax, n, col);
		}

	// Circumference and circle. Radius takes the scale in X.

	public void Circumference (float x, float y, float radius, Color32 col)
		{
		if (m_alpha >= 0.0f) col.a = (byte)(Mathf.Clamp01(m_alpha) * 255.0f);

		float num = radius * m_scaleX;
		TexEllipse((int)((x - m_viewArea.x) * m_scaleX), (int)((y - m_viewArea.y) * m_scaleY), (int)num, (int)num, col);
		m_changed = true;
		}

	public void Circle (float x, float y, float radius, Color32 col)
		{
		if (m_alpha >= 0.0f) col.a = (byte)(Mathf.Clamp01(m_alpha) * 255.0f);

		float num = radius * m_scaleX;
		TexFillEllipse((int)((x - m_viewArea.x) * m_scaleX), (int)((y - m_viewArea.y) * m_scaleY), (int)num, (int)num, col);
		m_changed = true;
		}

	// Ellipse

	public void Ellipse (float x, float y, float rx, float ry, Color32 col)
		{
		if (m_alpha >= 0.0f) col.a = (byte)(Mathf.Clamp01(m_alpha) * 255.0f);

		TexEllipse((int)((x - m_viewArea.x) * m_scaleX), (int)((y - m_viewArea.y) * m_scaleY), (int)(rx * m_scaleX), (int)(ry * m_scaleY), col);
		m_changed = true;
		}

	public void FillEllipse (float x, float y, float rx, float ry, Color32 col)
		{
		if (m_alpha >= 0.0f) col.a = (byte)(Mathf.Clamp01(m_alpha) * 255.0f);

		TexFillEllipse((int)((x - m_viewArea.x) * m_scaleX), (int)((y - m_viewArea.y) * m_scaleY), (int)(rx * m_scaleX), (int)(ry * m_scaleY), col);
		m_changed = true;
		}

	// Rects

	public void Clear (Color32 col)
		{
		if (m_alpha >= 0.0f) col.a = (byte)(Mathf.Clamp01(m_alpha) * 255.0f);

		for (int i = 0, count = (int)(m_pixelsWd * m_pixelsHt); i<count; i++)
			m_pixels[i] = col;

		m_changed = true;
		}


	public void Rect (float x, float y, float width, float height, Color32 col)
		{
		int x0 = (int)((x - m_viewArea.x) * m_scaleX);
		int y0 = (int)((y - m_viewArea.y) * m_scaleY);
		int x1 = (int)((x - m_viewArea.x + width) * m_scaleX);
		int y1 = (int)((y - m_viewArea.y + height) * m_scaleY);

		if (x1 <= x0 || y1 <= y0) return;
		if (m_alpha >= 0.0f) col.a = (byte)(Mathf.Clamp01(m_alpha) * 255.0f);

		TexSegmentH(x0, x1, y0, col);
		TexSegmentH(x0, x1, y1, col);
		TexSegmentV(x0, y0, y1, col);
		TexSegmentV(x1, y0, y1, col);

		m_changed = true;
		}


	public void FillRect (float x, float y, float width, float height, Color32 col)
		{
		int x0 = (int)((x - m_viewArea.x) * m_scaleX);
		int y0 = (int)((y - m_viewArea.y) * m_scaleY);
		int x1 = (int)((x - m_viewArea.x + width) * m_scaleX);
		int y1 = (int)((y - m_viewArea.y + height) * m_scaleY);

		if (x1 <= x0 || y1 <= y0) return;
		if (m_alpha >= 0.0f) col.a = (byte)(Mathf.Clamp01(m_alpha) * 255.0f);

		for (int i = y0; i < y1; i++)
			TexSegmentH(x0, x1, i, col);

		m_changed = true;
		}

	// SpLine draw

	public void SpLine(Vector2 P0, Vector2 T0, Vector2 P1, Vector2 T1, Color32 col, int steps = 20, float scaleY = 1.0f)
		{
		float s;
		float s2;
		float s3;
		float h1;
		float h2;
		float h3;
		float h4;
		Vector2 P;

		MoveTo(P0.x, P0.y * scaleY);
		for (int t = 0; t <= steps; t++)
			{
			s = t;
			s /= steps;
			s2 = s * s;
			s3 = s2 * s;

			// Valores de las funciones de Hermite

			h1 =  2.0f*s3 - 3.0f*s2 + 1.0f;
			h2 = -2.0f*s3 + 3.0f*s2;
			h3 =       s3 - 2.0f*s2 + s;
			h4 =       s3 -      s2;

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

	// Specialized functions
	// -----------------------------------------------------------

	// Draw a grid with the given intervals. It always crosses 0,0.

	public void Grid(float stepX, float stepY, Color32 col)
		{
		float f;

		if (stepX < Pixels2CanvasX(1)) stepX = Pixels2CanvasX(1);
		if (stepY < Pixels2CanvasY(1)) stepY = Pixels2CanvasY(1);

		float x0 = Mathf.FloorToInt(m_viewArea.x / stepX) * stepX;
		float y0 = Mathf.FloorToInt(m_viewArea.y / stepY) * stepY;

		for (f=x0; f<=m_viewArea.xMax; f+=stepX) LineV(f, col);
		for (f=y0; f<=m_viewArea.yMax; f+=stepY) LineH(f, col);
		}


	public void DottedGrid(float stepX, float stepY, int n, Color32 col)
		{
		float f;

		if (stepX < Pixels2CanvasX(1)) stepX = Pixels2CanvasX(1);
		if (stepY < Pixels2CanvasY(1)) stepY = Pixels2CanvasY(1);

		float x0 = Mathf.FloorToInt(m_viewArea.x / stepX) * stepX;
		float y0 = Mathf.FloorToInt(m_viewArea.y / stepY) * stepY;

		for (f=x0; f<=m_viewArea.xMax; f+=stepX) DottedLineV(f, n, col);
		for (f=y0; f<=m_viewArea.yMax; f+=stepY) DottedLineH(f, n, col);
		}


	public void DotMatrix(float stepX, float stepY, Color32 col)
		{
		if (stepX < Pixels2CanvasX(1)) stepX = Pixels2CanvasX(1);
		if (stepY < Pixels2CanvasY(1)) stepY = Pixels2CanvasY(1);

		float x0 = Mathf.FloorToInt(m_viewArea.x / stepX) * stepX;
		float y0 = Mathf.FloorToInt(m_viewArea.y / stepY) * stepY;

		for (float y = y0; y <= m_viewArea.yMax; y += stepY)
			for (float x = x0; x <= m_viewArea.xMax; x += stepX)
				Dot(x, y, col);
		}

	// Draw a biased curve between the given points with a given bias factor
	// 0.5 = straight line

	public void BiasCurve(Vector2 P0, Vector2 P1, float bias, Color32 col)
		{
		int num = 20;
		float num2 = default(float);
		float num3 = default(float);
		float num4 = P1.x - P0.x;
		float num5 = P1.y - P0.y;
		MoveTo(P0);
		for (int i = 0; i <= num; i++)
			{
			num2 = (float)i;
			num2 /= (float)num;
			num3 = Bias(num2, bias);
			LineTo(P0.x + num4 * num2, P0.y + num5 * num3, col);
			}
		}


	public void Dot (float x, float y, Color32 col)
		{
		if (m_alpha >= 0.0f)
			col.a = (byte)(Mathf.Clamp01(m_alpha) * 255.0f);

		int px = (int)((x - m_viewArea.x) * m_scaleX);
		int py = (int)((y - m_viewArea.y) * m_scaleY);

		TexSetPixel(px, py-1, col);
		TexSetPixel(px-1, py, col);
		TexSetPixel(px, py, col);
		TexSetPixel(px+1, py, col);
		TexSetPixel(px, py+1, col);
		m_changed = true;
		}


	public void Dot (Vector2 P, Color32 col)
		{
		Dot(P.x, P.y, col);
		}


	public void Cross (float x, float y, Color32 col, int pixelsX, int pixelsY)
		{
		if (m_alpha >= 0.0f)
			col.a = (byte)(Mathf.Clamp01(m_alpha) * 255.0f);

		int px = (int)((x - m_viewArea.x) * m_scaleX);
		int py = (int)((y - m_viewArea.y) * m_scaleY);

		for (int i = px-pixelsX; i <= px+pixelsX; i++)
			TexSetPixel(i, py, col);

		for (int j = py-pixelsY; j <= py+pixelsY; j++)
			TexSetPixel(px, j, col);

		m_changed = true;
		}


	public void Cross (Vector2 P, Color32 col, int pixelsX, int pixelsY)
		{
		Cross(P.x, P.y, col, pixelsX, pixelsY);
		}


	public void LineGraph(float[] Values, int ValueSize)
		{
		if (ValueSize >= 2)
			{
			if (Values.Length >= 2 * ValueSize)
				{
				for (int i = 1; i < ValueSize; i++)
					{
					MoveTo(Values[0], Values[i]);
					for (int j = 1, lim = Values.Length / ValueSize; j < lim; j++)
						{
						LineTo(Values[j * ValueSize], Values[j * ValueSize + i], COLORS[(i - 1) % 8]);
						}
					}
				}
			}
		}


	public void Function(System.Func<float, float> func, float x0, float x1, Color32 col, float scaleY = 1.0f)
		{
		float stepSize = Pixels2CanvasX(m_functionResolution);

		MoveTo(x0, func(x0) * scaleY);

		float x;
		for (x = x0; x <= x1; x += stepSize)
			LineTo(x, func(x) * scaleY, col);

		if (x < x1)
			LineTo(x1, func(x1) * scaleY, col);
		}


	public void DottedFunction(System.Func<float, float> func, float x0, float x1, Color32 col, int n = 5, float scaleY = 1.0f)
		{
		float stepSize = Pixels2CanvasX(m_functionResolution);

		MoveTo(x0, func(x0) * scaleY);

		float x;
		for (x = x0; x <= x1; x += stepSize)
			DottedLineTo(x, func(x) * scaleY, n, col);

		if (x < x1)
			DottedLineTo(x1, func(x1) * scaleY, n, col);
		}


	public void DashedFunction(System.Func<float, float> func, float x0, float x1, Color32 col, int n = 5, float scaleY = 1.0f)
		{
		float stepSize = Pixels2CanvasX(m_functionResolution);

		MoveTo(x0, func(x0) * scaleY);

		float x;
		for (x = x0; x <= x1; x += stepSize)
			DashedLineTo(x, func(x) * scaleY, n, col);

		if (x < x1)
			DashedLineTo(x1, func(x1) * scaleY, n, col);
		}



	public void Function(System.Func<float, float> func, Color32 col)
		{
		Function(func, m_viewArea.xMin, m_viewArea.xMax, col);
		}

	public void DottedFunction(System.Func<float, float> func, Color32 col, int n = 5)
		{
		DottedFunction(func, m_viewArea.xMin, m_viewArea.xMax, col, n);
		}

	public void DashedFunction(System.Func<float, float> func, Color32 col, int n = 5)
		{
		DashedFunction(func, m_viewArea.xMin, m_viewArea.xMax, col, n);
		}


	// BUG REPORT: Color no hace cast a Color32 cuando hay parámetros opcionales detrás.

	// public void Function(System.Func<float, float> func, Color32 col, float stepSize = -1.0f, float scaleY = 1.0f)
		// {
		// Function(func, m_viewArea.xMin, m_viewArea.xMax, col, stepSize, scaleY);
		// }



/// ------- Hay que quitar ésto


	public void Function2(System.Func<float, float, float> func, float p, float x0, float x1, Color32 col, float stepSize, float scaleY)
		{
		MoveTo(x0, func(x0, p) * scaleY);
		float num;
		for (num = x0; num <= x1; num += stepSize)
			{
			LineTo(num, func(num, p) * scaleY, col);
			}
		if (num < x1)
			{
			LineTo(x1, func(x1, p) * scaleY, col);
			}
		}

	public void Function2(System.Func<float, float, float> func, float p, float x0, float x1, Color32 col, float stepSize)
		{
		Function2(func, p, x0, x1, col, stepSize, 1f);
		}

	public void Function2(System.Func<float, float, float> func, float p, float x0, float x1, Color32 col)
		{
		Function2(func, p, x0, x1, col, Pixels2CanvasX(3), 1f);
		}

	public void Function2(System.Func<float, float, float> func, float p, Color32 col)
		{
		Function2(func, p, m_viewArea.xMin, m_viewArea.xMax, col, Pixels2CanvasX(3), 1f);
		}

	public void Function3(System.Func<float, float, float, float> func, float p, float q, float x0, float x1, Color32 col, float stepSize, float scaleY)
		{
		MoveTo(x0, func(x0, p, q) * scaleY);
		float num;
		for (num = x0; num <= x1; num += stepSize)
			{
			LineTo(num, func(num, p, q) * scaleY, col);
			}
		if (num < x1)
			{
			LineTo(x1, func(x1, p, q) * scaleY, col);
			}
		}

	public void Function3(System.Func<float, float, float, float> func, float p, float q, float x0, float x1, Color32 col, float stepSize)
		{
		Function3(func, p, q, x0, x1, col, stepSize, 1f);
		}

	public void Function3(System.Func<float, float, float, float> func, float p, float q, float x0, float x1, Color32 col)
		{
		Function3(func, p, q, x0, x1, col, Pixels2CanvasX(3), 1f);
		}

	public void Function3(System.Func<float, float, float, float> func, float p, float q, Color32 col)
		{
		Function3(func, p, q, m_viewArea.xMin, m_viewArea.xMax, col, Pixels2CanvasX(3), 1f);
		}

	public void Function3(System.Func<float, float, float, float> func, float p, float q, Color32 col, float stepSize)
		{
		Function3(func, p, q, m_viewArea.xMin, m_viewArea.xMax, col, stepSize, 1f);
		}

/// -------


	public void FunctionColorFill(System.Func<float, Vector4> func, float x0, float x1, float y0)
		{
		int num = (int)((x0 - m_viewArea.x) * m_scaleX);
		int num2 = (int)((x1 - m_viewArea.x) * m_scaleX);
		Color32 col = default(Color32);
		Vector4 vector = default(Vector4);
		float num4 = (y0 - m_viewArea.y) * m_scaleY;
		int y = default(int);
		if (x1 > x0)
			{
			col.a = (byte)((m_alpha < 0f) ? ((float)255) : (Mathf.Clamp01(m_alpha) * 255.0f));
			for (int i = num; i < num2; i++)
				{
				vector = func((float)i / m_scaleX + m_viewArea.x);
				col.r = (byte)(vector.x * 255.0f);
				col.g = (byte)(vector.y * 255.0f);
				col.b = (byte)(vector.z * 255.0f);
				y = Mathf.RoundToInt((vector.w - m_viewArea.y) * m_scaleY);
				TexSegmentV(i, (int)num4, y, col);
				}
			}
		}

	public void ContourFill(System.Func<float, float, Color32> func, float x, float y, float width, float height)
		{
		int num = (int)((x - m_viewArea.x) * m_scaleX);
		int num2 = (int)((y - m_viewArea.y) * m_scaleY);
		int num3 = (int)((float)num + width * m_scaleX);
		int num4 = (int)((float)num2 + height * m_scaleY);
		Color32 col = default(Color32);
		if (num3 > num && num4 > num2)
			{
			for (int i = num2; i < num4; i++)
				{
				for (int j = num; j < num3; j++)
					{
					col = func((float)j / m_scaleX + m_viewArea.x, (float)i / m_scaleY + m_viewArea.y);
					if (m_alpha >= 0.0f)
						{
						col.a = (byte)(Mathf.Clamp01(m_alpha) * 255.0f);
						}
					TexSetPixel(j, i, col);
					}
				}
			m_changed = true;
			}
		}
	public void Save()
		{
		if (m_buffer == null)
			{
			m_buffer = (m_pixels.Clone() as Color32[]);
			}
		else
			{
			m_pixels.CopyTo(m_buffer, 0);
			}
		}
	public void Restore()
		{
		if (m_buffer != null)
			{
			m_buffer.CopyTo(m_pixels, 0);
			m_changed = true;
			}
		}
	public void ApplyChanges()
		{
		if (m_changed)
			{
			m_texture.SetPixels32(m_pixels);
			m_texture.Apply(false);
			m_changed = false;
			}
		}
	public void GUIDraw(int posX, int posY)
		{
		ApplyChanges();
		GUI.DrawTexture(new Rect((float)posX, (float)posY, m_pixelsWd, m_pixelsHt), m_texture);
		}
	public void GUIDrawDirty(int posX, int posY)
		{
		GUI.DrawTexture(new Rect((float)posX, (float)posY, m_pixelsWd, m_pixelsHt), m_texture);
		}
	public void GUIStretchDraw(int posX, int posY, int width, int height)
		{
		ApplyChanges();
		GUI.DrawTexture(new Rect((float)posX, (float)posY, (float)width, (float)height), m_texture);
		}
	public void GUIStretchDrawWidth(int posX, int posY, int width)
		{
		ApplyChanges();
		float num = m_pixelsHt / m_pixelsWd;
		GUI.DrawTexture(new Rect((float)posX, (float)posY, (float)width, (float)width * num), m_texture);
		}
	public Texture2D GetTexture()
		{
		ApplyChanges();
		return m_texture;
		}
	private float BiasRaw(float x, float fBias)
		{
		float arg_AE_0;
		if (x <= 0f)
			{
			arg_AE_0 = 0f;
			}
		else
			{
			if (x >= 1f)
				{
				arg_AE_0 = 1f;
				}
			else
				{
				if (fBias != m_lastBias)
					{
					if (fBias <= 0f)
						{
						arg_AE_0 = ((x < 1f) ? ((float)0) : 1f);
						return arg_AE_0;
						}
					if (fBias >= 1f)
						{
						arg_AE_0 = ((x <= 0f) ? ((float)0) : 1f);
						return arg_AE_0;
						}
					if (fBias == 0.5f)
						{
						arg_AE_0 = x;
						return arg_AE_0;
						}
					m_lastExponent = Mathf.Log(fBias) * -1.4427f;
					m_lastBias = fBias;
					}
				arg_AE_0 = Mathf.Pow(x, m_lastExponent);
				}
			}
		return arg_AE_0;
		}
	private float Bias(float x, float fBias)
		{
		float num = default(float);
		num = ((fBias > 0.5f) ? (1f - BiasRaw(1f - Mathf.Abs(x), 1f - fBias)) : BiasRaw(Mathf.Abs(x), fBias));
		return (x >= 0f) ? num : (-num);
		}


	// ----------- funciones de UnifyWiki modificadas y funciones propias añadidas


	private void TexSetPixel(int x, int y, Color32 col)
		{
		if (x >= m_pixelsXMin && x < m_pixelsXMax && y >= m_pixelsYMin && y < m_pixelsYMax)
			{
			float num = (float)y * m_pixelsWd + (float)x;
			if (!m_alphaBlend)
				{
				m_pixels[(int)num] = col;
				}
			else
				{
				Color32 color = m_pixels[(int)num];
				float num2 = (float)col.a / 255f;
				float num3 = 1f - num2;
				color.r = (byte)((float)col.r * num2 + (float)color.r * num3);
				color.g = (byte)((float)col.g * num2 + (float)color.g * num3);
				color.b = (byte)((float)col.b * num2 + (float)color.b * num3);
				color.a = (byte)((float)col.a * num2 + (float)color.a * num3);
				m_pixels[(int)num] = color;
				}
			}
		}


	private void TexLine(int x0, int y0, int x1, int y1, int n, Color32 col)
		{
		int dy = y1 - y0;
		int dx = x1 - x0;

		int stepY;

		if (dy < 0)
			{
			dy = -dy;
			stepY = -1;
			}
		else
			{
			stepY = 1;
			}

		int stepX;

		if (dx < 0)
			{
			dx = -dx;
			stepX = -1;
			}
		else
			{
			stepX = 1;
			}

		dy <<= 1;
		dx <<= 1;

		TexSetPixel(x0, y0, col);

		if (dx > dy)
			{
			int fraction = dy - (dx >> 1);
			while (x0 != x1)
				{
				if (fraction >= 0)
					{
					y0 += stepY;
					fraction -= dx;
					}
				x0 += stepX;
				fraction += dy;
				TexSetPixel(x0, y0, col);
				}
			}
		else
			{
			int fraction = dx - (dy >> 1);
			while (y0 != y1)
				{
				if (fraction >= 0)
					{
					x0 += stepX;
					fraction -= dy;
					}
				y0 += stepY;
				fraction += dx;
				TexSetPixel(x0, y0, col);
				}
			}
		}


	private void TexLineDot(int x0, int y0, int x1, int y1, int n, Color32 col)
		{
		int dy = y1 - y0;
		int dx = x1 - x0;

		int stepY;
		if (dy < 0)
			{
			dy = -dy;
			stepY = -1;
			}
		else
			{
			stepY = 1;
			}

		int stepX;
		if (dx < 0)
			{
			dx = -dx;
			stepX = -1;
			}
		else
			{
			stepX = 1;
			}

		dy <<= 1;
		dx <<= 1;

		if (m_step++ % n == 0)
			TexSetPixel(x0, y0, col);

		if (dx > dy)
			{
			int fraction = dy - (dx >> 1);
			while (x0 != x1)
				{
				if (fraction >= 0)
					{
					y0 += stepY;
					fraction -= dx;
					}

				x0 += stepX;
				fraction += dy;

				if (m_step++ % n == 0)
					TexSetPixel(x0, y0, col);
				}
			}
		else
			{
			int fraction = dx - (dy >> 1);
			while (y0 != y1)
				{
				if (fraction >= 0)
					{
					x0 += stepX;
					fraction -= dy;
					}

				y0 += stepY;
				fraction += dx;

				if (m_step++ % n == 0)
					TexSetPixel(x0, y0, col);
				}
			}
		}


	private void TexLineDash(int x0, int y0, int x1, int y1, int n, Color32 col)
		{
		int dy = y1 - y0;
		int dx = x1 - x0;

		int stepY;
		if (dy < 0)
			{
			dy = -dy;
			stepY = -1;
			}
		else
			{
			stepY = 1;
			}

		int stepX;
		if (dx < 0)
			{
			dx = -dx;
			stepX = -1;
			}
		else
			{
			stepX = 1;
			}

		dy <<= 1;
		dx <<= 1;

		if (m_step++ % (n * 2) < n)
			TexSetPixel(x0, y0, col);

		if (dx > dy)
			{
			int fraction = dy - (dx >> 1);
			while (x0 != x1)
				{
				if (fraction >= 0)
					{
					y0 += stepY;
					fraction -= dx;
					}

				x0 += stepX;
				fraction += dy;

				if (m_step++ % (n * 2) < n)
					TexSetPixel(x0, y0, col);
				}
			}
		else
			{
			int fraction = dx - (dy >> 1);
			while (y0 != y1)
				{
				if (fraction >= 0)
					{
					x0 += stepX;
					fraction -= dy;
					}

				y0 += stepY;
				fraction += dx;

				if (m_step++ % (n * 2) < n)
					TexSetPixel(x0, y0, col);
				}
			}
		}


	private void TexSegmentV(int x, int y0, int y1, Color32 col)
		{
		if (y0 > y1)
			{
			int num = y0;
			y0 = y1;
			y1 = num;
			}
		if (x >= m_pixelsXMin && x < m_pixelsXMax && y1 >= m_pixelsYMin && y0 < m_pixelsYMax)
			{
			if (y0 < m_pixelsYMin)
				{
				y0 = m_pixelsYMin;
				}
			if (y1 >= m_pixelsYMax)
				{
				y1 = m_pixelsYMax;
				}
			int i = default(int);
			if (!m_alphaBlend)
				{
				for (i = y0; i < y1; i++)
					{
					m_pixels[(int)((float)i * m_pixelsWd + (float)x)] = col;
					}
				}
			else
				{
				int num2 = default(int);
				Color32 color = default(Color32);
				float num3 = (float)col.a / 255f;
				float num4 = 1f - num3;
				for (i = y0; i < y1; i++)
					{
					num2 = (int)((float)i * m_pixelsWd + (float)x);
					color = m_pixels[num2];
					color.r = (byte)((float)col.r * num3 + (float)color.r * num4);
					color.g = (byte)((float)col.g * num3 + (float)color.g * num4);
					color.b = (byte)((float)col.b * num3 + (float)color.b * num4);
					color.a = (byte)((float)col.a * num3 + (float)color.a * num4);
					m_pixels[num2] = color;
					}
				}
			}
		}

	private void TexSegmentH(int x0, int x1, int y, Color32 col)
		{
		if (x0 > x1)
			{
			int num = x0;
			x0 = x1;
			x1 = num;
			}
		if (y >= m_pixelsYMin && y < m_pixelsYMax && x1 >= m_pixelsXMin && x0 < m_pixelsXMax)
			{
			if (x0 < m_pixelsXMin)
				{
				x0 = m_pixelsXMin;
				}
			if (x1 > m_pixelsXMax)
				{
				x1 = m_pixelsXMax;
				}
			int i = default(int);
			float num2 = (float)y * m_pixelsWd;
			if (!m_alphaBlend)
				{
				for (i = x0; i < x1; i++)
					{
					m_pixels[(int)(num2 + (float)i)] = col;
					}
				}
			else
				{
				int num3 = default(int);
				Color32 color = default(Color32);
				float num4 = (float)col.a / 255f;
				float num5 = 1f - num4;
				for (i = x0; i < x1; i++)
					{
					num3 = (int)(num2 + (float)i);
					color = m_pixels[num3];
					color.r = (byte)((float)col.r * num4 + (float)color.r * num5);
					color.g = (byte)((float)col.g * num4 + (float)color.g * num5);
					color.b = (byte)((float)col.b * num4 + (float)color.b * num5);
					color.a = (byte)((float)col.a * num4 + (float)color.a * num5);
					m_pixels[num3] = color;
					}
				}
			}
		}
	private void TexEllipse(int cx, int cy, int rx, int ry, Color32 col)
		{
		float num = Mathf.Sqrt((float)2);
		int i = default(int);
		int j = default(int);
		int num2 = default(int);
		int num3 = default(int);
		if (rx >= ry)
			{
			j = rx;
			num2 = -rx;
			num3 = (int)Mathf.Ceil((float)rx / num);
			float num4 = 1f * (float)ry / (float)rx;
			for (i = 0; i <= num3; i++)
				{
				TexSetPixel(cx + i, (int)((float)cy + (float)j * num4), col);
				TexSetPixel(cx + i, (int)((float)cy - (float)j * num4), col);
				TexSetPixel(cx - i, (int)((float)cy + (float)j * num4), col);
				TexSetPixel(cx - i, (int)((float)cy - (float)j * num4), col);
				TexSetPixel(cx + j, (int)((float)cy + (float)i * num4), col);
				TexSetPixel(cx - j, (int)((float)cy + (float)i * num4), col);
				TexSetPixel(cx + j, (int)((float)cy - (float)i * num4), col);
				TexSetPixel(cx - j, (int)((float)cy - (float)i * num4), col);
				num2 += 2 * i + 1;
				if (num2 > 0)
					{
					int arg_133_0 = num2;
					int arg_132_0 = 2;
					int arg_131_0 = 2;
					int num5;
					j = (num5 = j) - 1;
					num2 = arg_133_0 + (arg_132_0 - arg_131_0 * num5);
					}
				}
			}
		else
			{
			i = ry;
			num2 = -ry;
			num3 = (int)Mathf.Ceil((float)ry / num);
			float num6 = 1f * (float)rx / (float)ry;
			for (j = 0; j <= num3; j++)
				{
				TexSetPixel((int)((float)cx + (float)j * num6), cy + i, col);
				TexSetPixel((int)((float)cx + (float)j * num6), cy - i, col);
				TexSetPixel((int)((float)cx - (float)j * num6), cy + i, col);
				TexSetPixel((int)((float)cx - (float)j * num6), cy - i, col);
				TexSetPixel((int)((float)cx + (float)i * num6), cy + j, col);
				TexSetPixel((int)((float)cx - (float)i * num6), cy + j, col);
				TexSetPixel((int)((float)cx + (float)i * num6), cy - j, col);
				TexSetPixel((int)((float)cx - (float)i * num6), cy - j, col);
				num2 += 2 * j + 1;
				if (num2 > 0)
					{
					int arg_252_0 = num2;
					int arg_251_0 = 2;
					int arg_250_0 = 2;
					int num7;
					i = (num7 = i) - 1;
					num2 = arg_252_0 + (arg_251_0 - arg_250_0 * num7);
					}
				}
			}
		}
	private void TexFillEllipse(int cx, int cy, int rx, int ry, Color32 col)
		{
		float num = Mathf.Sqrt((float)2);
		int i = default(int);
		int j = default(int);
		int num2 = default(int);
		int num3 = default(int);
		if (rx >= ry)
			{
			j = rx;
			num2 = -rx;
			num3 = (int)Mathf.Ceil((float)rx / num);
			float num4 = 1f * (float)ry / (float)rx;
			for (i = 0; i <= num3; i++)
				{
				TexSegmentV(cx + i, cy, (int)((float)cy + (float)j * num4), col);
				TexSegmentV(cx + i, cy, (int)((float)cy - (float)j * num4), col);
				TexSegmentV(cx - i, cy, (int)((float)cy + (float)j * num4), col);
				TexSegmentV(cx - i, cy, (int)((float)cy - (float)j * num4), col);
				TexSegmentV(cx + j, cy, (int)((float)cy + (float)i * num4), col);
				TexSegmentV(cx - j, cy, (int)((float)cy + (float)i * num4), col);
				TexSegmentV(cx + j, cy, (int)((float)cy - (float)i * num4), col);
				TexSegmentV(cx - j, cy, (int)((float)cy - (float)i * num4), col);
				num2 += 2 * i + 1;
				if (num2 > 0)
					{
					int arg_13B_0 = num2;
					int arg_13A_0 = 2;
					int arg_139_0 = 2;
					int num5;
					j = (num5 = j) - 1;
					num2 = arg_13B_0 + (arg_13A_0 - arg_139_0 * num5);
					}
				}
			}
		else
			{
			i = ry;
			num2 = -ry;
			num3 = (int)Mathf.Ceil((float)ry / num);
			float num6 = 1f * (float)rx / (float)ry;
			for (j = 0; j <= num3; j++)
				{
				TexSegmentH((int)((float)cx + (float)j * num6), cx, cy + i, col);
				TexSegmentH((int)((float)cx + (float)j * num6), cx, cy - i, col);
				TexSegmentH((int)((float)cx - (float)j * num6), cx, cy + i, col);
				TexSegmentH((int)((float)cx - (float)j * num6), cx, cy - i, col);
				TexSegmentH((int)((float)cx + (float)i * num6), cx, cy + j, col);
				TexSegmentH((int)((float)cx - (float)i * num6), cx, cy + j, col);
				TexSegmentH((int)((float)cx + (float)i * num6), cx, cy - j, col);
				TexSegmentH((int)((float)cx - (float)i * num6), cx, cy - j, col);
				num2 += 2 * j + 1;
				if (num2 > 0)
					{
					int arg_262_0 = num2;
					int arg_261_0 = 2;
					int arg_260_0 = 2;
					int num7;
					i = (num7 = i) - 1;
					num2 = arg_262_0 + (arg_261_0 - arg_260_0 * num7);
					}
				}
			}
		}
	}
