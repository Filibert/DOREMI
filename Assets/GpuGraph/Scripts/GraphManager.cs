// © 2016 BRANISLV GRUJIC ALL RIGHTS RESERVED
// Provided AS IS
// For any official support, please use the contact on the unity asset store

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VR;

public class GraphManager : MonoBehaviour
{
    public static GraphManagerInstance Graph;
    public static GameObject GraphManagerUI;
	public static GameObject GraphsUI;

	// FIXME: This should not be fixed, but should we do?
	public static float ExpectedMaxValue = 0.03f;

	private Material m_GraphMaterial;
     
    static bool IsInVR()
    {
        return VRDevice.isPresent;
    }

    public class Matrix4x4Wrapper
    {
        public Matrix4x4Wrapper()
        {
            Rotation = Quaternion.Euler(0, 0, 0);
            TRS = Matrix4x4.TRS(Translation, Rotation, Scale);
        }

        public Matrix4x4Wrapper(Vector3 translation, Quaternion rotation, Vector3 scale)
        {
            Translation = translation;
            Rotation = rotation;
            Scale = scale;

            TRS = Matrix4x4.TRS(translation, rotation, scale);
        }

        public Vector3 Translation;
        public Quaternion Rotation;
        public Vector3 Scale;

        public Matrix4x4 TRS;
    }

    public static void DrawRectangle(Vector4 rect, bool isWorldSpace, Matrix4x4 trs, bool invertCoordinate)
    {
        GL.Color(Color.white);

        // input rectangle 
        // the coordinates are top left is 0,0
        Vector3 topL = new Vector3(rect.x, rect.y, 0.0f);
        Vector3 topR = new Vector3(rect.x + rect.z, rect.y, 0.0f);

        Vector3 bottomL = new Vector3(rect.x, rect.y + rect.w, 0.0f);
        Vector3 bottomR = new Vector3(rect.x + rect.z, rect.y + rect.w, 0.0f);

        if (isWorldSpace)
        {
            topL = trs.MultiplyPoint(topL);
            topR = trs.MultiplyPoint(topR);

            bottomL = trs.MultiplyPoint(bottomL);
            bottomR = trs.MultiplyPoint(bottomR);
        }

        if (isWorldSpace)
        {
            invertCoordinate = false;
        }

        DrawLine(topL, topR, invertCoordinate);
        DrawLine(bottomL, bottomR, invertCoordinate);

        DrawLine(topL, bottomL, invertCoordinate);
        DrawLine(topR, bottomR, invertCoordinate);
    }

    public static void DrawLine(Vector3 A, Vector3 B, bool invertCoordinate)
    {
        if (invertCoordinate)
        {
            // flip the y coordinate
            // opengl is bottom left is 0,0
            A.y = 1.0f - A.y;
            B.y = 1.0f - B.y;
        }

        GL.Vertex(A);
        GL.Vertex(B);
    }

    private static void StartRender(Material material, bool isWorldSpace)
    {
        material.SetPass(0);

        if (isWorldSpace == false)
            GL.LoadOrtho();

        GL.Begin(GL.LINES);
    }

    private static void EndRender()
    {
        GL.End();
    }

    public class GPUDataPair
    {
        public GPUDataPair()
        {
            Value = 0.0f;
            Color = Color.red;
        }

        public GPUDataPair(float value)
        {
            Value = value;
            Color = Color.red;
        }

        public GPUDataPair(float value, Color color)
        {
            Value = value;
            Color = color;
        }

        public float Value { get; set; }
        public Color Color { get; set; }
    }

    // Single graph container
    public class GPUGraphData
    {
        public GameObject ParentUI;

        public List<GPUDataPair> DataPairs;

        public string Name;
        // UI

        // WS
        public GameObject CanvasUI;
        public Canvas Canvas;
        public RectTransform CanvasRect;

        public bool IsWorldSpace { get; set; }
        public Matrix4x4Wrapper TRS { get; set; }

        public int MaxNumPoints { get; set; }
        private int PrevMaxNumPoints;

        public float MinValue { get; set; }
        public float MaxValue { get; set; }
        public float CurrentRange { get; set; }

        public Vector4 Rectangle { get; set; }

        public GPUGraphData()
        {
            DataPairs = new List<GPUDataPair>();
            MaxNumPoints = 60;
            PrevMaxNumPoints = 0;
            Rectangle = new Vector4(0, 0, 1.0f, 0.2f);
            Name = "";
            ParentUI = null;

            ResetDefault();
        }

        public void ResetDefault()
        {
            MinValue = -ExpectedMaxValue;
            MaxValue = ExpectedMaxValue;

            CurrentRange = MaxValue - MinValue;

            IsWorldSpace = false;
        }

        public void ResetUI()
        {
            if (CanvasUI)
            {
                GameObject.Destroy(CanvasUI);
                CanvasUI = null;

                Canvas = null;
                CanvasRect = null;
            }
        }

        public void UpdateStats(float dataPoint)
        {
            ResetDefault();

            for (int i = 0; i < DataPairs.Count; ++i)
            {
                float currentDataPoint = DataPairs[i].Value;

                MinValue = -ExpectedMaxValue;
                MaxValue = ExpectedMaxValue;
            }

            CurrentRange = MaxValue - MinValue;
        }

        public void InitializeParentUI()
        {
            if (ParentUI == null)
            {				
				if (GraphManagerUI == null || GraphsUI == null)
				{
					CreateGlobalObjects();
				}

				if(GraphManagerUI)
				{
					foreach (Transform transform in GraphManagerUI.transform)
					{
						if (string.Equals(transform.gameObject.name, "Graphs_UI"))
						{
							ParentUI = transform.gameObject;
							break;
						}
					}
				}
            }
        }

        public void UpdateUI()
        {
            InitializeParentUI();

            if (ParentUI)
            {
                if (CanvasUI == null)
                {
                    // we need to ensure this doesnt exist already because script compilation
                    string canvasObjectName = string.Format("{0}_Canvas_{1}", IsWorldSpace ? "WS" : "SS", Name);

                    CanvasUI = GameObject.Find(canvasObjectName);

                    if (CanvasUI == null)
                    {
                        CanvasUI = new GameObject();
                        CanvasUI.transform.parent = ParentUI.transform;
                        CanvasUI.gameObject.name = canvasObjectName;

                        Canvas = CanvasUI.AddComponent<Canvas>();

                        if(IsWorldSpace)
                        {
                            Canvas.renderMode = RenderMode.WorldSpace;
                        }
                        else
                        {
                            if(IsInVR())
                            {
                                Canvas.renderMode = RenderMode.ScreenSpaceCamera;
                                Canvas.worldCamera = Camera.current;
                                Canvas.planeDistance = 126.0f;
                            }
                            else
                            {
                                Canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                            }
                        }

                        CanvasRect = Canvas.GetComponent<RectTransform>();
                        CanvasRect.anchorMin = new Vector2(0, 0);
                        CanvasRect.anchorMax = new Vector2(0, 0);

                        CanvasScaler canvasScaler = CanvasUI.AddComponent<CanvasScaler>();
                        canvasScaler.dynamicPixelsPerUnit = 5000.0f;

                        CanvasUI.AddComponent<GraphicRaycaster>();
                    }
                    else
                    {
                        Canvas = CanvasUI.GetComponent<Canvas>();
                        CanvasRect = Canvas.GetComponent<RectTransform>();
                    }
                }

                // Update the position
                if (CanvasRect)
                {
                    CanvasRect.position = TRS.Translation;
                    CanvasRect.rotation = TRS.Rotation;
                    CanvasRect.sizeDelta = new Vector2(TRS.Scale.x, TRS.Scale.y);
                }

                Vector2 uiSize = Vector2.zero;

                if(IsWorldSpace)
                    uiSize = new Vector2(0.8f, 0.2f);
                else
                    uiSize = new Vector2(100.0f, 20.0f);
            }
        }

        // screens space methods
        public void AddPair(float dataPoint)
        {
            DataPairs.Add(new GPUDataPair(dataPoint));
            TRS = new Matrix4x4Wrapper();
            Shrink();
            UpdateStats(dataPoint);
            UpdateUI();
        }

        public void AddPair(float dataPoint, Color color)
        {
            DataPairs.Add(new GPUDataPair(dataPoint, color));
            TRS = new Matrix4x4Wrapper();
            Shrink();
            UpdateStats(dataPoint);
        }

        public void AddPair(float dataPoint, Color color, Rect rectangle)
        {
            // each graph is located in a specify position 
            Rectangle = new Vector4(rectangle.x / Screen.width, rectangle.y / Screen.height, rectangle.width / Screen.width, rectangle.height / Screen.height);

            DataPairs.Add(new GPUDataPair(dataPoint, color));
            TRS = new Matrix4x4Wrapper();

            Shrink();
            UpdateStats(dataPoint);
            UpdateUI();
        }

        public void AddPair(float dataPoint, Matrix4x4Wrapper trs)
        {
            AddPair(dataPoint);
            IsWorldSpace = true;
            TRS = trs;
            Rectangle = new Vector4(0, 0, 1, 1);

            UpdateUI();
        }

        public void AddPair(float dataPoint, Color color, Matrix4x4Wrapper trs)
        {
            AddPair(dataPoint, color);
            IsWorldSpace = true;
            TRS = trs;
            Rectangle = new Vector4(0, 0, 1, 1);

            UpdateUI();
        }

        public void Shrink()
        {
            // insert one, remove one, keep count at 100
            if (DataPairs.Count > MaxNumPoints)
            {
                DataPairs.RemoveAt(0);
            }
        }

        public void Reset()
        {
            DataPairs.RemoveRange(0, DataPairs.Count - 1);

            ResetDefault();
            ResetUI();
        }

        public void Update()
        {
            if (PrevMaxNumPoints != MaxNumPoints)
            {
                PrevMaxNumPoints = MaxNumPoints;

                if (DataPairs.Count > 0)
                {
                    Reset();
                }
            }
        }

        public float GetX(int index, float offset)
        {
            float fIndex = (float)index;
            float fMaxNumPoints = (float)MaxNumPoints - 1.0f;

            return fIndex / fMaxNumPoints + offset;
        }

        public float GetY(int index)
        {
            float currentValue = DataPairs[index].Value;
            return (currentValue - MinValue) / CurrentRange;
        }

        public void Render(Material graphMaterial)
        {
            if (CurrentRange == 0.0f || DataPairs.Count <= 1.0f)
                return;

            StartRender(graphMaterial, IsWorldSpace);

            // This is so that we can see the graph scrolling from right to left
            float screenOffset = DataPairs.Count != MaxNumPoints ? (1.0f - (float)DataPairs.Count / (float)MaxNumPoints) : 0.0f;

            for (int i = 1; i < DataPairs.Count; ++i)
            {
                // Set the color of the specific data pair
				Color color = DataPairs[i].Color;

                // Calculate positions
                Vector3 pPos = new Vector3(GetX(i - 1, screenOffset), GetY(i - 1), 0.0f);
                Vector3 cPos = new Vector3(GetX(i - 0, screenOffset), GetY(i - 0), 0.0f);

                if (IsWorldSpace)
                {
                    pPos.x -= 0.5f;
                    cPos.x -= 0.5f;

                    pPos.y -= 0.5f;
                    cPos.y -= 0.5f;

                    pPos = TRS.TRS.MultiplyPoint(pPos);
                    cPos = TRS.TRS.MultiplyPoint(cPos);

					// TODO: Compute color.a
                }
                else
                {
                    //Clamp to 1.0f, 1.0f
                    cPos = Vector3.Min(Vector3.one, cPos);
                    pPos = Vector3.Min(Vector3.one, pPos);

					color.a = Mathf.Abs(cPos.x);

                    // convert into rectangle space thats provided by user
                    pPos.x *= Rectangle.z;
                    pPos.y *= Rectangle.w;

                    pPos.x += Rectangle.x;
                    pPos.y = ((1.0f - Rectangle.y) - Rectangle.w) + pPos.y;

                    cPos.x *= Rectangle.z;
                    cPos.y *= Rectangle.w;

                    cPos.x += Rectangle.x;
                    cPos.y = ((1.0f - Rectangle.y) - Rectangle.w) + cPos.y;
                }
				
				GL.Color(color);
				
                // Draw the line pair
                DrawLine(pPos, cPos, false);
            }

            EndRender();
        }
    }

    // Contains the wrapper with all graph
    public class GraphManagerInstance
    {
        public GraphManagerInstance()
        {
            Graphs = new Dictionary<string, GPUGraphData>();
        }

        public void Update()
        {
            foreach (KeyValuePair<string, GPUGraphData> graph in Graphs)
            {
                graph.Value.Update();
            }
        }

        public GPUGraphData Retrieve(string key, float value)
        {
            GPUGraphData graphData;

            // If we don't have data with that key
            if (!Graphs.TryGetValue(key, out graphData))
            {
                // Allocate this graph
                graphData = new GPUGraphData();
                graphData.Name = key;

                // insert this new graph into dictionary
                Graphs.Add(key, graphData);
            }

            return graphData;
        }

        public void ResetAll()
        {
            foreach (KeyValuePair<string, GPUGraphData> graph in Graphs)
            {
                graph.Value.Reset();
            }

            Graphs.Clear();
        }

        public bool Reset(string key)
        {
            GPUGraphData graphData;
            bool wasReset = Graphs.TryGetValue(key, out graphData);

            // If we don't have data with that key
            if (wasReset)
            {
                graphData.Reset();
                Graphs.Remove(key);
            }

            return wasReset;
        }

        // screen space plot
        public void Plot(string key, float value)
        {
            Retrieve(key, value).AddPair(value);
        }

        // world space
        public void Plot(string key, float value, Matrix4x4Wrapper trs)
        {
            Retrieve(key, value).AddPair(value, trs);
        }

        public void Plot(string key, float value, Color color)
        {
            Retrieve(key, value).AddPair(value, color);
        }

        // screen space plot
        public void Plot(string key, float value, Color color, Rect rectangle)
        {
            Retrieve(key, value).AddPair(value, color, rectangle);
        }

        // world space plot
        public void Plot(string key, float value, Color color, Matrix4x4Wrapper trs)
        {
            Retrieve(key, value).AddPair(value, color, trs);
        }

        public Dictionary<string, GPUGraphData> Graphs;
    }

    static GameObject CreateGlobalObjects()
    {
        if (GraphManagerUI == null)
        {
            GraphManagerUI = GameObject.Find("GraphManager_UI");

            if (GraphManagerUI == null)
            {
                GraphManagerUI = new GameObject();
                GraphManagerUI.transform.position = new Vector3(0, 0, 0);
                GraphManagerUI.name = "GraphManager_UI";
            }
		}

		if (GraphManagerUI && GraphsUI == null)
		{
			GraphsUI = GameObject.Find("Graphs_UI");

			if (GraphsUI == null)
			{
				// create two children objects as well
				GraphsUI = new GameObject();
				GraphsUI.transform.parent = GraphManagerUI.transform;
				GraphsUI.name = "Graphs_UI";
			}
		}

		return GraphManagerUI;
	}

    // Use this for initialization
    void Start()
    {
        // Create static instance
        Graph = new GraphManagerInstance();

        // Initialize the default material/shader used for the rendering
        if (!m_GraphMaterial)
        {
            m_GraphMaterial = new Material(Shader.Find("GPUGraph/Graph"));
            m_GraphMaterial.hideFlags = HideFlags.HideAndDontSave;
			m_GraphMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
			m_GraphMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        }

        CreateGlobalObjects();
    }

    // Update is called once per frame
    void Update()
    {
        // Update the graph, this updates min/max/current/avg
        if (Graph != null)
        {
            Graph.Update();
        }
        else
        {
            Graph = new GraphManagerInstance();
            CreateGlobalObjects();
        }
    }

    private void RenderGraph()
    {
        if (Graph != null)
        {
            //GL.PushMatrix();

            foreach (KeyValuePair<string, GPUGraphData> graph in Graph.Graphs)
            {
                graph.Value.Render(m_GraphMaterial);
            }

            //GL.PopMatrix();
        }
    }

#if UNITY_EDITOR
    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnScriptsReloaded()
    {
        if (Graph != null)
        {
            Graph.ResetAll();
        }
    }
#endif

    // Render the graph in viewport
    public void OnPostRender()
    {
        RenderGraph();
    }
}
