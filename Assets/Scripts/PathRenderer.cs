using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class MarkPoint{
	public Vector3 pos = Vector3.zero;
	public Vector3 posl = Vector3.zero;
	public Vector3 posr = Vector3.zero;
	public float intensity = 0.0f;
	public int lastIndex = 0;
}

public class PathRenderer : MonoBehaviour {

	public int maxMarks = 1024;
	public float minDistance = 0.1f;
	public float width = 0.28f;
	public float groundOffset = 0.02f;
	public float textureOffsetY = 0.05f;
	[Range(0,1)]
	public float fadeOutRange = 0.5f;

	public Material material;

	int m_markCount;
	int m_markArraySize = 0;
	MarkPoint[] m_markPoints;

	CommonTools.BiasLerpContext m_biasCtx = new CommonTools.BiasLerpContext();

	bool m_segmentsUpdated;
	int m_segmentCount;
	int m_segmentArraySize;



	Mesh m_mesh;
	Vector3[] m_vertices;
	Color[] m_colors;
	Vector2[] m_uvs;
	int[] m_triangles;
	Vector2[] m_values;

	void OnEnable (){

		transform.position = Vector3.zero;
		transform.rotation = Quaternion.identity;

		MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
		if (meshFilter == null)
			meshFilter = gameObject.AddComponent<MeshFilter>();

		MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
		if (meshRenderer == null)
			{
			meshRenderer = gameObject.AddComponent<MeshRenderer>();
			meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
			meshRenderer.material = material;
			}

		if (maxMarks < 10) maxMarks = 10;


		m_markPoints = new MarkPoint[maxMarks*2];
		for (int i = 0, c = m_markPoints.Length; i < c; i++)
			m_markPoints[i] = new MarkPoint();

		m_markCount = 0;
		m_markArraySize = m_markPoints.Length;

		m_vertices = new Vector3[maxMarks * 4];
		m_colors = new Color[maxMarks * 4];
		m_uvs = new Vector2[maxMarks * 4];
		m_triangles = new int[maxMarks * 6];
		m_values = new Vector2[maxMarks];

		m_segmentCount = 0;
		m_segmentArraySize = maxMarks;
		m_segmentsUpdated = false;

		// Elements that will be invariant

		for (int i = 0; i < m_segmentArraySize; i++)
			{
			m_uvs[i * 4 + 0] = new Vector2(0, textureOffsetY);
			m_uvs[i * 4 + 1] = new Vector2(1, textureOffsetY);
			m_uvs[i * 4 + 2] = new Vector2(0, 1-textureOffsetY);
			m_uvs[i * 4 + 3] = new Vector2(1, 1-textureOffsetY);

			m_triangles[i * 6 + 0] = i * 4 + 0;
			m_triangles[i * 6 + 2] = i * 4 + 1;
			m_triangles[i * 6 + 1] = i * 4 + 2;

			m_triangles[i * 6 + 3] = i * 4 + 2;
			m_triangles[i * 6 + 5] = i * 4 + 1;
			m_triangles[i * 6 + 4] = i * 4 + 3;
			}

		m_mesh = new Mesh();
		m_mesh.name = "Path_Mesh";
		m_mesh.MarkDynamic();
		m_mesh.vertices = m_vertices;
		m_mesh.colors = m_colors;
		m_mesh.triangles = m_triangles;
		m_mesh.uv = m_uvs;
		m_mesh.RecalculateBounds();

		meshFilter.mesh = m_mesh;
		}

		void OnValidate (){
			if (m_uvs != null){
				for (int i = 0; i < m_uvs.Length/4; i++){
					m_uvs[i * 4 + 0] = new Vector2(0, textureOffsetY);
					m_uvs[i * 4 + 1] = new Vector2(1, textureOffsetY);
					m_uvs[i * 4 + 2] = new Vector2(0, 1-textureOffsetY);
					m_uvs[i * 4 + 3] = new Vector2(1, 1-textureOffsetY);
					}
				}
			m_segmentsUpdated = true;
		}

		public void Clear (){
			if (isActiveAndEnabled){
				m_markCount = 0;
				m_segmentCount = 0;

				for (int i = 0, c = m_vertices.Length; i < c; i++)
					m_vertices[i] = Vector3.zero;

				m_mesh.vertices = m_vertices;
				m_segmentsUpdated = true;
				}
			}

		public int AddPathPoint (Vector3 pos, float intensity, int lastIndex){

			Vector3 normal = Vector3.up;

			if (!isActiveAndEnabled || m_markArraySize == 0) return -1;

			intensity = Mathf.Clamp01(intensity);

			Vector3 newPos = pos + normal * groundOffset;
			if (lastIndex >= 0 && Vector3.Distance(newPos, m_markPoints[lastIndex % m_markArraySize].pos) < minDistance)
				return lastIndex;


			MarkPoint current = m_markPoints[m_markCount % m_markArraySize];
			current.pos = newPos;
			current.intensity = intensity;
			current.lastIndex = lastIndex;

			if (lastIndex >= 0 && lastIndex > m_markCount - m_markArraySize){
				MarkPoint last = m_markPoints[lastIndex % m_markArraySize];
				Vector3 dir = (current.pos - last.pos);
				Vector3 crossDir = Vector3.Cross(dir, normal).normalized;
				Vector3 widthDir = 0.5f * width * crossDir;

			current.posl = current.pos + widthDir;
			current.posr = current.pos - widthDir;

			if (last.lastIndex < 0){
				last.posl = current.pos + widthDir;
				last.posr = current.pos - widthDir;
			}

			AddSegment(last, current);
		}

		m_markCount++;
		return m_markCount-1;
		}

		void AddSegment (MarkPoint first, MarkPoint second){
			int segmentIndex = (m_segmentCount % m_segmentArraySize) * 4;

			m_vertices[segmentIndex + 0] = first.posl;
			m_vertices[segmentIndex + 1] = first.posr;
			m_vertices[segmentIndex + 2] = second.posl;
			m_vertices[segmentIndex + 3] = second.posr;

			m_colors[segmentIndex + 0].a = first.intensity;
			m_colors[segmentIndex + 1].a = first.intensity;
			m_colors[segmentIndex + 2].a = second.intensity;
			m_colors[segmentIndex + 3].a = second.intensity;

			m_values[segmentIndex/4] = new Vector2(first.intensity, second.intensity);

			if (m_segmentCount == 0)
				{
				Vector3 v = m_vertices[0];
				for (int i = 4, c = m_vertices.Length; i < c; i++)
					m_vertices[i] = v;
				}

			m_segmentCount++;
			m_segmentsUpdated = true;
			}

		void LateUpdate ()
			{
			if (!m_segmentsUpdated) return;
			m_segmentsUpdated = false;

			int toFade = (int)(m_segmentArraySize * fadeOutRange);
			if (toFade > 0)
				{
				int segment = m_segmentCount - m_segmentArraySize;
				int fadeStart = 0;

				if (segment < 0)
					{
					fadeStart = -segment;
					segment = 0;
					}

				float fadeStep = 1.0f / toFade;

				for (int i = fadeStart; i < toFade; i++)
					{
					int valueIndex = segment % m_segmentArraySize;
					int colorIndex = valueIndex * 4;

					float decay = i * fadeStep;
					float intensity1 = m_values[valueIndex].x * decay;
					float intensity2 = m_values[valueIndex].y * decay + fadeStep;

					m_colors[colorIndex + 0].a = intensity1;
					m_colors[colorIndex + 1].a = intensity1;
					m_colors[colorIndex + 2].a = intensity2;
					m_colors[colorIndex + 3].a = intensity2;

					segment++;
					}
				}


			m_mesh.MarkDynamic();
			m_mesh.vertices = m_vertices;
			m_mesh.colors = m_colors;
			m_mesh.RecalculateBounds();
			}
		}
