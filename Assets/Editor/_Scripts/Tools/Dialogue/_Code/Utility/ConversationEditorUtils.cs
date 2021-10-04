using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using System.Linq;
using ETools.Dialogue;
using ETools.Utilities;
using System.IO;

namespace ETools.Editor.Dialogue
{
	public static class ConversationEditorUtils
	{
		#region Public Functions

		/// <summary>
		/// Create and return a new node-graph
		/// </summary>
		/// <param name="wantedName">The desired name for the graph</param>
		/// <param name="extension">The extension for the graph to have (prefix of ".asset")</param>
		/// <returns>The created node-graph</returns>
		public static ConversationNodeGraph CreateNewGraph(string wantedName)
		{
			//Create the graph
			ConversationNodeGraph curGraph = ScriptableObject.CreateInstance<ConversationNodeGraph>();

			//Exception catching
			if (curGraph == null)
			{
				EditorUtility.DisplayDialog("Error", "Unable to create new graph, no idea what happened.", "Well shit");
				return null;
			}

			//Check to see if a graph exists at that name
			var exists = AssetDatabase.LoadAssetAtPath<ConversationNodeGraph>("Assets/NodeEditor/Database/" + wantedName + ".asset");
			if (exists != null)
			{
				EditorUtility.DisplayDialog("File Exists", "A graph already exists at that location.  Delete that one to make one with the same name.", "Damn, you got me.");
				return null;
			}

			//Initialize the graph
			curGraph.graphName = wantedName;

			//Serialize it
			string path = "Assets/Data/DialogueGraphs/" + wantedName + ".asset";
			Debug.Log(path);
			EditorUtils.CreateAssetAndFoldersInDatabase(curGraph, path);

			//Load the graph
			curGraph.graphPath = path;

			curGraph.InitGraph();

			//Create the end-node for the graph
			curGraph.endNode = CreateNode(curGraph, typeof(StartNodeGUI), new Vector2(500, 250));

			//Initialize the editor
			ConversationEditorWindow.InitEditorWindow();
			ConversationEditorWindow curWindow = EditorWindow.GetWindow<ConversationEditorWindow>();
			curWindow.titleContent = new GUIContent("Conversation Starter");
			if (curWindow != null)
			{
				curWindow.curGraph = ScriptableObject.CreateInstance<ConversationNodeGraph>();
				curWindow.curGraph = curGraph;
			}

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();

			return curGraph;
		}

		/// <summary>
		/// Handle the context-menu for reverse node creation based on the current node type
		/// </summary>
		/// <param name="curGraph">The current graph</param>
		/// <param name="mousePos">The mouse position</param>
		public static void TypeSensitiveContextMenu(ConversationNodeGraph curGraph, Vector2 mousePos)
		{
			//Type of node to be created
			//ValueType type = curGraph.backwardsConnectionNode.Type;

			//Init the context menu
			GenericMenu menu = new GenericMenu();

			//Populate the struct
			GraphNodeCollection[] coll = new GraphNodeCollection[GUIController.NodesAvailable.Count];
			for (int i = 0; i < coll.Length; i++)
			{
				GraphNodeCollection c = new GraphNodeCollection();
				c.graph = curGraph;
				c.nodeIndex = i;
				c.mousePos = mousePos;
				coll[i] = c;
			}

			//Populate the menu					
			for (int i = 0; i < GUIController.NodesAvailable.Count; i++)
			{

				menu.AddItem(new GUIContent(GUIController.NodesAvailable[i].GetCustomAttribute<GraphNode>().nodeName), false, TypeSensitiveContextMenuCallback, coll[i]);
			}

			menu.ShowAsContext();
		}

		/// <summary>
		/// Struct containing necessary information for the context menu
		/// </summary>
		private struct GraphNodeCollection
		{
			public ConversationNodeGraph graph;
			public int nodeIndex;
			public Vector2 mousePos;
		}

		/// <summary>
		/// Handles the input events for the context menu
		/// </summary>
		/// <param name="obj">Graph Node Collection to be passed in</param>
		public static void TypeSensitiveContextMenuCallback(object obj)
		{
			//type cast
			GraphNodeCollection c = (GraphNodeCollection)obj;

			//Create the node
			var node = CreateNode(c.graph, GUIController.NodesAvailable[c.nodeIndex], c.mousePos);

			//Make the connection to the selected node
			if (node.isOutput)
			{
				MakeConnection(GUIController.backwardsConnectionNode, node, c.graph);
			}
			GUIController.backwardsConnectionNode = null;
		}

		/// <summary>
		/// Make a connection between a node and a property
		/// </summary>
		/// <param name="input">The node to be connected</param>
		/// <param name="property">The property to be connected</param>
		/// <param name="curGraph">The current graph</param>
		public static void MakeConnection(Node input, Connectable output, ConversationNodeGraph curGraph)
		{
			if (output.outgoing != null)
			{
				RemoveConnection(output.outgoing, output, curGraph);
			}

			output.outgoing = input;

			if (!input.incoming.Contains(output))
			{
				input.incoming.Add(output);
			}
		}

		public static void RemoveConnection(Node input, Connectable output, ConversationNodeGraph curGraph)
		{
			output.outgoing = null;
			input.incoming.Remove(output);
		}

		/// <summary>
		/// Load a graph
		/// </summary>
		/// <returns>Whether the task function succeeded</returns>
		public static bool LoadGraph()
		{
			//Find the path to the graph
			ConversationNodeGraph curGraph = null;
			string extension = "asset";
			string graphPath = EditorUtility.OpenFilePanel("Load Graph", Path.Combine(Application.dataPath, "Data", "DialogueGraphs"), extension);

			//Return false if load fails
			if (graphPath == null || graphPath == "")
			{
				return false;
			}

			//Eschew the path
			int appPathLen = Application.dataPath.Length;
			string finalPath = graphPath.Substring(appPathLen - 6);

			//Load the graph
			curGraph = (ConversationNodeGraph)AssetDatabase.LoadAssetAtPath(finalPath, typeof(ConversationNodeGraph));
			curGraph.graphPath = finalPath;
			if (curGraph != null)
			{
				//Set up the window
				ConversationEditorWindow curWindow = EditorWindow.GetWindow<ConversationEditorWindow>();
				if (curWindow != null)
				{
					curWindow.curGraph = curGraph;
					curWindow.curGraph.InitGraph();
					curWindow.titleContent = new GUIContent("Conversation Editor", EditorGUIUtility.Load("Assets/Editor/EditorResources/conversationEditorIcon.psd") as Texture);
				}
			}
			//Error handling
			else
			{
				EditorUtility.DisplayDialog("Node Message", "Unable to load selected graph", "Well shit");
				return false;
			}
			return true;
		}

		/// <summary>
		/// Create a node
		/// </summary>
		/// <param name="curGraph">The current graph</param>
		/// <param name="nodeBase">The node to be instantiated</param>
		/// <param name="mousePos">Mouse position</param>
		/// <returns>The created node</returns>
		public static Node CreateNode(ConversationNodeGraph curGraph, Type type, Vector2 mousePos)
		{
			//exception checking
			if (curGraph == null)
			{
				return null;
			}

			//Create the node
			var curNodeObj = ScriptableObject.CreateInstance(type);

			if (!(curNodeObj is Node))
			{
				Debug.LogError("Type " + type.ToString() + " supplied is not a node!");
				return null;
			}
			Node curNode = (Node)type.GetMethod("New").Invoke(null, new object[1] { curGraph });
			//curNode = Object.Instantiate(nodeBase);

			if (curNode != null)
			{
				//Init the node
				curNode.InitNode();
				curNode.nodeRect = new Rect(
					curNode.nodeRect.x + mousePos.x,
					curNode.nodeRect.y + mousePos.y,
					curNode.nodeRect.width,
					curNode.nodeRect.height
					);
				curNode.parentGraph = curGraph;
				curGraph.nodes.Add(curNode);

				//Serialize it
				AssetDatabase.AddObjectToAsset(curNode, curGraph);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}
			return curNode;
		}

		/// <summary>
		/// Delete a node
		/// </summary>
		/// <param name="nodeID">The ID of the node to delete</param>
		/// <param name="curGraph">The current graph</param>
		public static void DeleteNode(int nodeID, ConversationNodeGraph curGraph)
		{
			//exception checking
			if (curGraph == null)
			{
				return;
			}

			if (curGraph.nodes.Count >= nodeID)
			{
				//Find the node to be deleted
				Node deleteNode = curGraph.nodes[nodeID];

				//Remove incoming nodes
				foreach (var incoming in deleteNode.incoming)
				{
					incoming.outgoing = null;
				}

				//Remove outgoing nodes
				if (deleteNode.outgoing)
					deleteNode.outgoing.incoming.Remove(deleteNode);

				//Remove outgoing from properties
				foreach (var prop in deleteNode.properties.Values)
				{
					if (prop.outgoing)
						prop.outgoing.incoming.Remove(prop);
				}

				//Make sure it's not an end-node
				if (deleteNode.Undeletable)
				{
					Debug.Log("Cannot delete that node - it's the start node!");
					return;
				}

				if (deleteNode != null)
				{

					curGraph.nodes.RemoveAt(nodeID);
					if (curGraph.selectedNodes.Contains(deleteNode))
						curGraph.selectedNodes.Remove(deleteNode);

					//Kill it in data
					foreach (var prop in deleteNode.properties.Values)
					{
						GameObject.DestroyImmediate(prop, true);
					}
					GameObject.DestroyImmediate(deleteNode, true);
				}
			}
		}

		/// <summary>
		/// Returns a deep copy of a node
		/// </summary>
		/// <param name="node">The node to be copied</param>
		/// <returns>A deep copy of the node</returns>
		public static Node CreateCopyOfNode(Node node)
		{
			Node newNode = ScriptableObject.Instantiate(node);

			for (int i = 0; i < newNode.properties.Count; i++)
			{
				Property newProp = newNode.properties.ElementAt(i).Value;
				newNode.properties[newProp.propertyName] = ScriptableObject.Instantiate<Property>(newProp);
			}
			newNode.incoming.Clear();
			newNode.outgoing = null;
			foreach (var property in newNode.properties.Values)
				property.outgoing = null;

			return newNode;
		}

		/// <summary>
		/// Duplicates a node
		/// </summary>
		/// <param name="graph">The graph the node is contained in</param>
		/// <param name="node">The node to be duplicated</param>
		public static void DuplicateNode(ConversationNodeGraph graph, Node node)
		{
			Node newNode = CreateCopyOfNode(node);

			newNode.nodeRect.position += newNode.nodeRect.size;
			newNode.guid = SystemsUtility.GetUniqueGUID(8);
			graph.nodes.Add(newNode);


			AssetDatabase.AddObjectToAsset(newNode, graph);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		/// <summary>
		/// Deletes multiple nodes by indices
		/// </summary>
		/// <param name="graph">The graph the nodes belong to</param>
		/// <param name="nodeIDs">The indices for the nodes in the array</param>
		public static void DeleteNodes(ConversationNodeGraph graph, params int[] nodeIDs)
		{
			Array.Sort<int>(nodeIDs);
			for (int i = 0; i < nodeIDs.Length; i++)
			{

				DeleteNode(nodeIDs[i] - i, graph);
			}
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		/// <summary>
		/// Draw a grid
		/// </summary>
		/// <param name="viewRect">The rect to take up</param>
		/// <param name="gridSpacing">Spacing between the lines</param>
		/// <param name="gridOpacity">Opacity for the grid</param>
		/// <param name="gridColor">Color for the grid</param>
		/// <param name="curGraph">The current graph</param>
		public static void DrawGrid(Rect viewRect, float gridSpacing, float gridOpacity, Color gridColor)
		{
			gridSpacing *= GUIController.zoom;
			Vector2 gridOffset = GUIController.lookPos * GUIController.zoom;

			Handles.BeginGUI();
			Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);
			for (float x = gridOffset.x % gridSpacing; x < viewRect.width; x += gridSpacing)
			{
				Handles.DrawLine(new Vector3(x, 0f, 0f), new Vector3(x, viewRect.height, 0f));
			}
			for (float y = gridOffset.y % gridSpacing; y < viewRect.height; y += gridSpacing)
			{
				Handles.DrawLine(new Vector3(0f, y, 0f), new Vector3(viewRect.width, y, 0f));
			}

			Handles.color = Color.white;
			Handles.EndGUI();

		}

		/// <summary>
		/// Check for intersection between two line segments
		/// </summary>
		/// <param name="p1">Start point of line1</param>
		/// <param name="q1">End point of line1</param>
		/// <param name="p2">Start point of line2</param>
		/// <param name="q2">End point of line2</param>
		/// <returns>Whether or not the two line segments intersect</returns>      
		public static bool CheckForIntersection(Vector2 p1, Vector2 q1, Vector2 p2, Vector2 q2)
		{
			int orient1 = Orientation(p1, q1, p2);
			int orient2 = Orientation(p1, q1, q2);
			int orient3 = Orientation(p2, q2, p1);
			int orient4 = Orientation(p2, q2, q1);
			if (orient1 != orient2 && orient3 != orient4)
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Creates a Rect that bounds multiple rects
		/// </summary>
		/// <param name="rects">The rects to be contained</param>
		/// <returns>The bounding rect</returns>
		public static Rect CreateBoundingRect(params Rect[] rects)
		{
			if (rects == null || rects.Length == 0)
				return new Rect();
			float xMin = 99999;
			float xMax = -99999;
			float yMin = 99999;
			float yMax = -99999;
			foreach (Rect rect in rects)
			{
				if (rect.x < xMin)
					xMin = rect.x;
				if (rect.x + rect.width > xMax)
					xMax = rect.x + rect.width;
				if (rect.y < yMin)
					yMin = rect.yMin;
				if (rect.y + rect.height > yMax)
					yMax = rect.yMax + rect.height;
			}
			return new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
		}

		#endregion

		#region Private Functions

		/// <summary>
		/// Get the orientation of a set of points
		/// </summary>
		/// <param name="point1">Point 1</param>
		/// <param name="point2">Point 2</param>
		/// <param name="point3">Point 3</param>
		/// <returns>The orientation of the points: 1 is clockwise, 2 is counter-clockwise</returns>
		private static int Orientation(Vector2 point1, Vector2 point2, Vector2 point3)
		{
			float val = (point2.y - point1.y) * (point3.x - point2.x) -
				  (point2.x - point1.x) * (point3.y - point2.y);
			if (val > 0)
			{
				return 1;
			}
			if (val < 0)
			{
				return 2;
			}
			return 0;
		}

		#endregion

	}
}