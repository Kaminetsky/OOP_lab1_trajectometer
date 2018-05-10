
using UnityEngine;
using System.Collections.Generic;

using System;
using System.IO;
using System.Xml.Serialization;
using System.Reflection;
using System.Reflection.Emit;


public static class CommonTools
	{
	// Universal color parser
	//
	// ColorUtility.TryParseHtmlString raises compilation errors if invoked from
	// instance field initializers.


	static Dictionary<string, Color> m_colors = new Dictionary<string, Color>();


	static CommonTools ()
		{
		m_colors["red"]     = Color.red;
		m_colors["green"]   = Color.green;
		m_colors["blue"]    = Color.blue;
		m_colors["white"]   = Color.white;
		m_colors["black"]   = Color.black;
		m_colors["yellow"]  = Color.yellow;
		m_colors["cyan"]    = Color.cyan;
		m_colors["magenta"] = Color.magenta;
		m_colors["gray"]    = Color.gray;
		m_colors["grey"]    = Color.grey;
		m_colors["clear"]   = Color.clear;
		}


	public static int HexToDecimal (char ch)
		{
		switch (ch)
			{
			case '0': return 0x0;
			case '1': return 0x1;
			case '2': return 0x2;
			case '3': return 0x3;
			case '4': return 0x4;
			case '5': return 0x5;
			case '6': return 0x6;
			case '7': return 0x7;
			case '8': return 0x8;
			case '9': return 0x9;
			case 'a':
			case 'A': return 0xA;
			case 'b':
			case 'B': return 0xB;
			case 'c':
			case 'C': return 0xC;
			case 'd':
			case 'D': return 0xD;
			case 'e':
			case 'E': return 0xE;
			case 'f':
			case 'F': return 0xF;
			}

		return 0x0;
		}


	public static Color ParseColor (string col)
		{
		// Colores básicos por nombre

		if (m_colors.ContainsKey(col))
			return m_colors[col];

		// Colores en formato #FFF/#FFFA ó #FFFFFF/#FFFFFFAA

		Color result = Color.black;
		int l = col.Length;
		float f;

		if (l > 0 && col[0] == "#"[0])
			{
			if (l == 4 || l == 5)
				{
				f = 1.0f / 15.0f;

				result.r = HexToDecimal(col[1]) * f;
				result.g = HexToDecimal(col[2]) * f;
				result.b = HexToDecimal(col[3]) * f;

				if (l == 5)
					result.a = HexToDecimal(col[4]) * f;
				}
			else if (l == 7 || l == 9)
				{
				f = 1.0f / 255.0f;

				result.r = ((HexToDecimal(col[1]) << 4) | HexToDecimal(col[2])) * f;
				result.g = ((HexToDecimal(col[3]) << 4) | HexToDecimal(col[4])) * f;
				result.b = ((HexToDecimal(col[5]) << 4) | HexToDecimal(col[6])) * f;

				if (l == 9)
					result.a = ((HexToDecimal(col[7]) << 4) | HexToDecimal(col[8])) * f;
				}
			}

		return result;
		}


	// Returns an angle valued clamped as [-180 .. +180]

	public static float ClampAngle (float angle)
		{
		angle = angle % 360.0f;
		if (angle > 180.0f) angle -= 360.0f;
		return angle;
		}


	// Returns an angle valued clamped as [0 .. +360] suitable for Mathf.LerpAngle

	public static float ClampAngle360 (float angle)
		{
		angle = angle % 360.0f;
		if (angle < 0.0f) angle += 360.0f;
		return angle;
		}


	// This will always be faster than Mathf.Abs, noticeable on intensive inner loops.

	public static float FastAbs (float x)
		{
		return x < 0.0f? -x : x;
		}


	public static bool Vector3Equals (Vector3 a, Vector3 b)
		{
		return a.x == b.x && a.y == b.y && a.z == b.z;
		}


	public static float MaxAbs (float a, float b)
		{
		return FastAbs(a) >= FastAbs(b)? a : b;
		}


	public static float MinAbs (float a, float b)
		{
		return FastAbs(a) < FastAbs(b)? a : b;
		}


	// Draws a debug crossmark at the given position using the given transform for orientation

	public static void DrawCrossMark (Vector3 pos, Transform trans, Color col, float length = 0.1f)
		{
		DrawCrossMark(pos, trans.forward, trans.right, trans.up, col, length);
		}

	public static void DrawCrossMark (Vector3 pos, Vector3 forward, Vector3 right, Vector3 up, Color col, float length = 0.1f)
		{
		length *= 0.5f;

		Vector3 F = forward * length;
		Vector3 U = up * length;
		Vector3 R = right * length;

		Debug.DrawLine(pos - F, pos + F, col);
		Debug.DrawLine(pos - U, pos + U, col);
		Debug.DrawLine(pos - R, pos + R, col);
		}


	// Converting lineal to logaritmic values, useful for debug lines

	public static float Lin2Log (float val)
		{
		return Mathf.Log(CommonTools.FastAbs(val)+1) * Mathf.Sign(val);
		}

	public static Vector3 Lin2Log (Vector3 val)
		{
		return Vector3.ClampMagnitude(val, Lin2Log(val.magnitude));
		}


	// Method for cloning serializable classes
	// Usage: newObject = CommonTools.CloneObject(objectToBeCloned);
	//
	// Source: http://stackoverflow.com/questions/78536/deep-cloning-objects
	//
	// Edy: Modified for using XmlSerializer instead of BinaryFormatter, which
	// seems to support basic types only.

	public static T CloneObject<T> (T source)
		{
		#if NETFX_CORE
		if (!typeof(T).GetTypeInfo().IsSerializable)
		#else
		if (!typeof(T).IsSerializable)
		#endif
			throw new ArgumentException("The type must be serializable.", "source");

		// Don't serialize a null object, simply return the default for that object
		if (ReferenceEquals(source, null))
			return default(T);

		XmlSerializer serializer = new XmlSerializer(typeof(T));
		Stream stream = new MemoryStream();
		using (stream)
			{
			serializer.Serialize(stream, source);
			stream.Seek(0, SeekOrigin.Begin);
			return (T)serializer.Deserialize(stream);
			}
		}


	// A faster method for cloning objects (any object, not only serializables).
	// Usage: newObject = CommonTools.CloneObjectFast(objectToBeCloned);
	//
	// It dynamically creates a memberwise-clone method for the T type using IL opcodes.
	// Then the method is invoked on each Clone operation for the T type.
	// The first call for a given type is slower (creates the method). All others are fast.
	//
	// Source: http://whizzodev.blogspot.com/2008/03/object-cloning-using-il-in-c.html

    public delegate TResult Func<T1, TResult>(T1 arg1);
    static Dictionary<Type, Delegate> _cachedIL = new Dictionary<Type, Delegate>();

	public static T CloneObjectFast<T> (T myObject)
		{
		Delegate myExec = null;

		if (!_cachedIL.TryGetValue(typeof(T), out myExec))
			{
			// Create ILGenerator
			DynamicMethod dymMethod = new DynamicMethod("DoClone", typeof(T), new Type[] { typeof(T) }, true);
			ConstructorInfo cInfo = myObject.GetType().GetConstructor(new Type[] { });

			ILGenerator generator = dymMethod.GetILGenerator();

			//LocalBuilder lbf = generator.DeclareLocal(typeof(T));
			//lbf.SetLocalSymInfo("_temp");

			generator.Emit(OpCodes.Newobj, cInfo);
			generator.Emit(OpCodes.Stloc_0);
			foreach (FieldInfo field in myObject.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
				{
				// Load the new object on the eval stack... (currently 1 item on eval stack)
				generator.Emit(OpCodes.Ldloc_0);
				// Load initial object (parameter)          (currently 2 items on eval stack)
				generator.Emit(OpCodes.Ldarg_0);
				// Replace value by field value             (still currently 2 items on eval stack)
				generator.Emit(OpCodes.Ldfld, field);
				// Store the value of the top on the eval stack into the object underneath that value on the value stack.
				//  (0 items on eval stack)
				generator.Emit(OpCodes.Stfld, field);
				}

			// Load new constructed obj on eval stack -> 1 item on stack
			generator.Emit(OpCodes.Ldloc_0);
			// Return constructed object.   --> 0 items on stack
			generator.Emit(OpCodes.Ret);

			myExec = dymMethod.CreateDelegate(typeof(Func<T, T>));
			_cachedIL.Add(typeof(T), myExec);
			}

		return ((Func<T, T>)myExec)(myObject);
		}


	// Method for copying an object's values to another object overwriting the existing values using Reflection.
	// Usage: CommonTools.CopyObjectOverwrite(objectToBeCloned, ref targetObject);
	//
	// Adapted from: http://whizzodev.blogspot.com/2008/03/object-cloning-using-il-in-c.html


	public static void CopyObjectOverwrite<T> (T source, ref T target)
		{
        // Get all the fields of the type, also the privates.

        FieldInfo[] fis = source.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        // Loop through all the fields and copy the information from the source object
        // to the destination object

        foreach (FieldInfo fi in fis)
			{
            fi.SetValue(target, fi.GetValue(source));
			}
		}


	// Unclamped Lerp methods


	public static float FastLerp (float from, float to, float t)
		{
		return from + (to-from) * t;
		}


	public static float LinearLerp (float x0, float y0, float x1, float y1, float x)
		{
		return y0 + (x-x0) * (y1-y0) / (x1-x0);
		}


	public static float LinearLerp (Vector2 from, Vector2 to, float t)
		{
		return LinearLerp(from.x, from.y, to.x, to.y, t);
		}


	public static float CubicLerp (float x0, float y0, float x1, float y1, float x)
		{
		// Hermite-based cubic polinomial function (spline) with horizontal tangents (0)
		//
		// h1(t) =  2*t3 - 3*t2 + 1;	-> start point
		// h2(t) = -2*t3 + 3*t2;		-> end point

		float t = (x - x0) / (x1 - x0);
		float t2 = t*t;
		float t3 = t*t2;

		return y0 * (2*t3 - 3*t2 + 1) + y1 * (-2*t3 + 3*t2);
		}


	public static float CubicLerp (Vector2 from, Vector2 to, float t)
		{
		return CubicLerp(from.x, from.y, to.x, to.y, t);
		}


	// Smooth interpolation with simplified tangent adjustment


	public static float TangentLerp (float x0, float y0, float x1, float y1, float a, float b, float x)
		{
		float h = y1 - y0;
		float tg0 = 3.0f * h * a;
		float tg1 = 3.0f * h * b;

		// Hermite-based cubic polinomial function (spline)
		//
		// h1(t) =  2*t3 - 3*t2 + 1;	-> start point
		// h2(t) = -2*t3 + 3*t2;		-> end point
		// h3(t) =    t3 - 2*t2 + t;	-> start tangent
		// h4(t) =    t3 - t2;			-> end tangent

		float t = (x - x0) / (x1 - x0);
		float t2 = t*t;
		float t3 = t*t2;

		return y0 * (2*t3 - 3*t2 + 1) + y1 * (-2*t3 + 3*t2) + tg0 * (t3 - 2*t2 + t) + tg1 * (t3 - t2);
		}


	public static float TangentLerp (Vector2 from, Vector2 to, float a, float b, float t)
		{
		return TangentLerp(from.x, from.y, to.x, to.y, a, b, t);
		}


	// Unclamped smooth step methods
	// https://chicounity3d.wordpress.com/2014/05/23/how-to-lerp-like-a-pro


	public static float SmoothStep (float from, float to, float t)
		{
		t = Mathf.Clamp01(t);
		t = t*t * (3.0f - 2.0f*t);

		return from + (to-from) * t;
		}


	public static float SmootherStep (float from, float to, float t)
		{
		t = Mathf.Clamp01(t);
		t = t*t*t * (t * (6.0f*t - 15.0f) + 10.0f);

		return from + (to-from) * t;
		}


	// Hermite interpolation with full control on tangents


	public static float HermiteLerp (float x0, float y0, float x1, float y1, float outTangent, float inTangent, float x)
		{
		// Hermite-based cubic polinomial function (spline)
		//
		// h1(t) =  2*t3 - 3*t2 + 1;	-> start point
		// h2(t) = -2*t3 + 3*t2;		-> end point
		// h3(t) =    t3 - 2*t2 + t;	-> start tangent
		// h4(t) =    t3 - t2;			-> end tangent

		float t = (x - x0) / (x1 - x0);
		float t2 = t*t;
		float t3 = t*t2;

		return y0 * (2*t3 - 3*t2 + 1) + y1 * (-2*t3 + 3*t2) + outTangent * (t3 - 2*t2 + t) + inTangent * (t3 - t2);
		}


	// Bezier Splines
	//
	// Result is based on successive Lerps simplified, so the calculations are more straightforward
	// and the path tangent can be easily calculated.
	//
	// Bezier splines are wonderfully explained here:
	// https://www.youtube.com/watch?v=o9RK6O2kOKo
	// (amazing video on spline-based procedural mesh generation)
	//
	// 		point[0] = start point
	// 		point[1] = start control
	// 		point[2] = end control
	// 		point[3] = end point
	//
	// Difference with Hermite:
	//
	// 	Hermite tangents are relative to their points (local). Bezier uses control points in the
	// 	same reference as their points. The Hermite equivalent tangents can be calculated as:
	//		outTangent = start control - start point
	//		inTangent = end point - start control


	public static Vector3 Bezier (Vector3[] points, float t)
		{
		float omt = 1.0f - t;
		float omt2 = omt * omt;
		float t2 = t * t;

		return points[0] * (omt2 * omt)
		       + points[1] * (3.0f * omt2 * t)
		       + points[2] * (3.0f * omt * t2)
		       + points[3] * (t2 * t);
		}


	public static Vector3 BezierTangent (Vector3[] points, float t)
		{
		float omt = 1.0f - t;
		float omt2 = omt * omt;
		float t2 = t * t;

		Vector3 tangent = points[0] * (-omt2)
		                  + points[1] * (3.0f * omt2 - 2.0f * omt)
		                  + points[2] * (-3.0f * t2 + 2.0f * t)
		                  + points[3] * (t2);

		return tangent.normalized;
		}


	public static Vector3 BezierNormal (Vector3[] points, float t, Vector3 up)
		{
		Vector3 tangent = BezierTangent(points, t);
		Vector3 binormal = Vector3.Cross(up, tangent).normalized;
		return Vector3.Cross(tangent, binormal);
		}


	// Generic biased lerp with optional context optimization:
	//
	// 	BiasedLerp(x, bias)				generic unoptimized
	//	BiasedLerp(x, bias, context)	optimized for bias which changes unfrequently


	public class BiasLerpContext
		{
		public float lastBias = -1.0f;
		public float lastExponent = 0.0f;
		}


	static float BiasWithContext (float x, float bias, BiasLerpContext context)
		{
		if (x <= 0.0f) return 0.0f;
		if (x >= 1.0f) return 1.0f;

		if (bias != context.lastBias)
			{
			if (bias <= 0.0f) return x >= 1.0f? 1.0f : 0.0f;
			else if (bias >= 1.0f) return x > 0.0f? 1.0f : 0.0f;
			else if (bias == 0.5f) return x;

			context.lastExponent = Mathf.Log(bias) * -1.442695f;
			context.lastBias = bias;
			}

		return Mathf.Pow(x, context.lastExponent);
		}


	static float BiasRaw (float x, float bias)
		{
		if (x <= 0.0f) return 0.0f;
		if (x >= 1.0f) return 1.0f;

		if (bias <= 0.0f) return x >= 1.0f? 1.0f : 0.0f;
		else if (bias >= 1.0f) return x > 0.0f? 1.0f : 0.0f;
		else if (bias == 0.5f) return x;

		float exponent = Mathf.Log(bias) * -1.442695f;
		return Mathf.Pow(x, exponent);
		}


	public static float BiasedLerp (float x, float bias)
		{
		float result = bias <= 0.5f? BiasRaw(CommonTools.FastAbs(x), bias) :
		               1.0f - BiasRaw(1.0f - CommonTools.FastAbs(x), 1.0f - bias);

		return x < 0.0f? -result : result;
		}


	public static float BiasedLerp (float x, float bias, BiasLerpContext context)
		{
		float result = bias <= 0.5f? BiasWithContext(CommonTools.FastAbs(x), bias, context) :
		               1.0f - BiasWithContext(1.0f - CommonTools.FastAbs(x), 1.0f - bias, context);

		return x < 0.0f? -result : result;
		}


	// TO-DO: GainedLerp
	// http://www.gamedev.net/topic/678537-distributing-a-variable-0-1-logarithmically/#entry5290979


	// Trunc float values to a fixed number of decimals


	static float GetMultiplier (int decimals)
		{
		float mult = 1.0f;

		switch (decimals)
			{
			case 0:	break;
			case 1: mult = 10.0f; break;
			case 2: mult = 100.0f; break;
			case 3: mult = 1000.0f; break;
			case 4: mult = 10000.0f; break;

			default:
				mult = decimals > 0? Mathf.Pow(10.0f, decimals) : 0.0f;
				break;
			}

		return mult;
		}


	public static float FloorDecimals (float value, int decimals)
		{
		float mult = GetMultiplier(decimals);
		return Mathf.Floor(value * mult) / mult;
		}


	public static Vector3 FloorDecimals (Vector3 value, int decimals)
		{
		float mult = GetMultiplier(decimals);
		return new Vector3(
		           Mathf.Floor(value.x * mult) / mult,
		           Mathf.Floor(value.y * mult) / mult,
		           Mathf.Floor(value.z * mult) / mult);
		}


	public static float RoundDecimals (float value, int decimals)
		{
		float mult = GetMultiplier(decimals);
		return Mathf.Round(value * mult) / mult;
		}


	public static Vector3 RoundDecimals (Vector3 value, int decimals)
		{
		float mult = GetMultiplier(decimals);
		return new Vector3(
		           Mathf.Round(value.x * mult) / mult,
		           Mathf.Round(value.y * mult) / mult,
		           Mathf.Round(value.z * mult) / mult);
		}
	}
