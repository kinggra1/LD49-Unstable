using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;
using UnityEditor;
using System;
using ETools.Editor;
using ETools.Utilities;

namespace ETools.Editor.Dialogue
{

	public class ConversationEditorWorkView : ViewBase
	{

		#region Variables

		Vector2 mousePos;
		int deleteNodeID;

		#endregion

		#region Primary Functions

		/// <summary>
		/// Constructor
		/// </summary>
		public ConversationEditorWorkView() : base("Work View") { }

		/// <summary>
		/// GUI function for the work view
		/// </summary>
		/// <param name="editorRect">The rect for the entire editor window</param>
		/// <param name="percentageRect">The percentage of that window this window takes up</param>
		/// <param name="e">The current event</param>
		/// <param name="curGraph">The graph being edited</param>
		public override void UpdateView(Rect editorRect, Rect percentageRect, Event e, ConversationNodeGraph curGraph)
		{
			base.UpdateView(editorRect, percentageRect, e, curGraph);
			GUI.Box(viewRect, viewTitle, viewSkin.GetStyle("ViewBG"));

			GUIController.zoom = Mathf.Lerp(GUIController.zoom, GUIController.zoomTarget, GUIController.zoomSpeed);
			GUIController.lookPos = Vector2.Lerp(GUIController.lookPos, GUIController.lookPosTarget, GUIController.lookPosSpeed);

			//Draw Grid
			ConversationEditorUtils.DrawGrid(viewRect, 50f, .25f, Color.white);
			ConversationEditorUtils.DrawGrid(viewRect, 100f, .25f, Color.white);
			ConversationEditorUtils.DrawGrid(viewRect, 10f, .1f, Color.white);

			//Update the Graph GUI
			GUILayout.BeginArea(viewRect);
			curGraph.UpdateGraphGUI(e, viewRect, viewSkin);
			GUILayout.EndArea();

			//Draw delete line
			if (GUIController.WantsConnection)
			{
				GUIController.drawingDeleteLine = false;
			}
			if (GUIController.drawingDeleteLine)
			{
				DrawDeleteLine(e);
			}

			//Compile Button Rect setup
			if (DrawCompileButton(percentageRect))
			{
				curGraph.Compile();
			}

			ProcessEvents(e);

		}


		/// <summary>
		/// Handle input events
		/// </summary>
		/// <param name="e">The current input event</param>
		public override void ProcessEvents(Event e)
		{

			mousePos = e.mousePosition;

			//	Frame nodes
			if (e.type == EventType.KeyDown && EditorGUIUtility.keyboardControl == 0)
			{
				if (e.control)
				{
					if (e.keyCode == KeyCode.F)
					{
						HandleFrameNodes();
					}

					if (e.keyCode == KeyCode.C)
					{
						HandleCopyNodes();
						Debug.Log(GUIController.nodeClipboard.Count + " nodes added to clipboard.");
					}

					if (e.keyCode == KeyCode.V)
					{
						HandlePasteNodes();
					}
				}
			}

			//	Zoom via the scroll wheel
			if (e.isScrollWheel && (viewRect.Contains(e.mousePosition)))
			{
				HandleZoom(e);
			}

			//Delete key
			if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Delete && EditorGUIUtility.keyboardControl == 0)
			{
				HandleDeleteKey();
			}

			//If they've clicked somewhere
			if (e.type == EventType.MouseDown)
			{
				if (e.button == 0)
				{
					if (viewRect.Contains(e.mousePosition))
					{
						if (GUIController.WantsConnection == false && (curGraph.selectedNodes == null || curGraph.selectedNodes.Count() == 0))
						{
							HandleDeleteLine(e);
						}
					}
				}
				//Right-click
				if (e.button == 1)
				{
					HandleContextMenu(e);
				}
			}

			//Deselect and reset state on click (outside of a node)
			if (e.type == EventType.MouseDown)
			{
				GUIController.connectionNode = null;
				//curGraph.WantsConnection = false;
			}

			//Run type-sensitive menu if backwards connection
			if (e.type == EventType.MouseDown && GUIController.WantsBackwardsConnection)
			{
				//curGraph.WantsBackwardsConnection = false;
				ConversationEditorUtils.TypeSensitiveContextMenu(curGraph, e.mousePosition);
			}

			//Drag a selected node
			foreach (var node in curGraph.nodes)
			{
				HandleDragNode(e, node);
			}
		}

		#endregion

		#region Utility Methods

		/// <summary>
		/// Handles the Context menu for the work view
		/// </summary>
		/// <param name="e">Current event type</param>
		/// <param name="contextID">ID for the context menu</param>
		void ProcessContextMenu(Event e, int contextID)
		{
			GUIController.activeContextMenu = new GenericMenu();

			//Context menu setup
			if (contextID == 0)
			{
				GUIController.activeContextMenu.AddItem(new GUIContent("Create Graph"), false, ContextCallback, "0");
				GUIController.activeContextMenu.AddItem(new GUIContent("Load Graph"), false, ContextCallback, "1");

				if (curGraph != null)
				{
					GUIController.activeContextMenu.AddSeparator("");
					int iter = 4;
					foreach (var item in GUIController.NodesAvailable)
					{
						if (item != null)
						{
							GUIController.activeContextMenu.AddItem(new GUIContent(item.GetCustomAttribute<GraphNode>().nodeName), false, ContextCallback, iter);
							iter++;
						}
					}
				}
			}

			//Additional option if a node is selected
			if (contextID == 1)
			{
				GUIController.activeContextMenu.AddItem(new GUIContent("Delete Node"), false, ContextCallback, "2");
				GUIController.activeContextMenu.AddItem(new GUIContent("Duplicate Node"), false, ContextCallback, "3");
			}

			GUIController.activeContextMenu.ShowAsContext();
			e.Use();
		}


		/// <summary>
		/// Handle the results of the context menu
		/// </summary>
		/// <param name="obj">Results of the context menu</param>
		void ContextCallback(object obj)
		{
			switch (obj.ToString())
			{
				case "0":
					ConversationPopupWindow.InitNodePopup();
					return;

				case "1":
					ConversationEditorUtils.LoadGraph();
					return;

				case "2":
					ConversationEditorUtils.DeleteNode(deleteNodeID, curGraph);
					AssetDatabase.SaveAssets();
					AssetDatabase.Refresh();
					return;
				case "3":
					ConversationEditorUtils.DuplicateNode(curGraph, curGraph.nodes[deleteNodeID]);
					return;
			}
			int index = (int)obj;
			List<System.Type> nodesAvailable = new List<System.Type>();

			ConversationEditorUtils.CreateNode(curGraph, GUIController.NodesAvailable[index - 4], GUIController.ToDataSpace(mousePos));
			return;
		}

		#endregion

		#region GUI Methods

		/// <summary>
		/// Draws the compile button to the screen
		/// </summary>
		/// <param name="viewRect"></param>
		/// <returns>Whether or not the button was clicked</returns>
		private bool DrawCompileButton(Rect viewRect)
		{
			Rect compileButtonRect = new Rect(viewRect);
			compileButtonRect.x = compileButtonRect.x + 7;
			compileButtonRect.y += 7;
			compileButtonRect.width = 100;
			compileButtonRect.height = 30;

			//Compile Button
			return GUI.Button(compileButtonRect, "Compile");
		}

		/// <summary>
		/// Draws the delete-connection line to the screen
		/// </summary>
		/// <param name="e"></param>
		private void DrawDeleteLine(Event e)
		{
			Handles.color = Color.red;
			Handles.DrawLine(new Vector3(GUIController.deleteLineStart.x, GUIController.deleteLineStart.y, 0),
							 new Vector3(e.mousePosition.x, e.mousePosition.y, 0));
			Handles.color = Color.white;

			//Visuals to accompany drawing the line
			foreach (var node in curGraph.nodes)
			{
				foreach (var inc in node.incoming)
				{
					Vector2 start1 = inc.OutputPosition;
					Vector2 end1 = node.InputRect.position;
					Vector2 start2 = GUIController.deleteLineStart;
					Vector2 end2 = e.mousePosition;

					if (ConversationEditorUtils.CheckForIntersection(start1, end1, start2, end2))
					{
						inc.connectionMarkedForDelete = true;
					}
					else
					{
						inc.connectionMarkedForDelete = false;
					}
				}
			}
		}

		/// <summary>
		/// Handles user input for framing nodes (with the F key)
		/// </summary>
		private void HandleFrameNodes()
		{
			List<Node> nodesToFrame = new List<Node>();
			if (curGraph.selectedNodes.Count > 0)
				nodesToFrame = curGraph.selectedNodes;
			else
				nodesToFrame = curGraph.nodes;

			IEnumerable<Rect> nodeRects = from n in nodesToFrame select n.nodeRect;
			IEnumerable<Vector2> nodePositions = from n in nodesToFrame select n.nodeRect.center;
			Vector2 positionSum = new Vector2(0, 0);
			foreach (var vec in nodePositions)
				positionSum += vec;
			positionSum /= nodePositions.Count();

			Rect bounds = ConversationEditorUtils.CreateBoundingRect(nodeRects.ToArray());
			GUIController.zoomTarget = Mathf.Min(viewRect.width / bounds.width, viewRect.height / bounds.height, GUIController.zoomTarget);
			GUIController.lookPosTarget = -bounds.center + (viewRect.size / GUIController.zoomTarget) * 0.5f;
		}

		/// <summary>
		/// Adds currently selected nodes to the clipboard
		/// </summary>
		private void HandleCopyNodes()
		{
			List<Node> copyNodes = new List<Node>();
			foreach (var node in curGraph.selectedNodes)
			{
				copyNodes.Add(ConversationEditorUtils.CreateCopyOfNode(node));
			}
			GUIController.nodeClipboard = copyNodes;
		}

		/// <summary>
		/// Pastes and creates nodes from the clipboard
		/// </summary>
		private void HandlePasteNodes()
		{
			Rect oldBoundingRect = ConversationEditorUtils.CreateBoundingRect((from n in GUIController.nodeClipboard select n.nodeRect).ToArray());
			Vector2 oldCenter = oldBoundingRect.center;
			Vector2 difference = -GUIController.lookPos - oldCenter + new Vector2((oldBoundingRect.width + viewRect.width) * 0.5f, 0);

			foreach (var node in GUIController.nodeClipboard)
			{
				Node newNode = ConversationEditorUtils.CreateCopyOfNode(node);
				node.nodeRect.position -= difference;
				node.guid = SystemsUtility.GetUniqueGUID(8);
				node.parentGraph.nodes.Add(node);
				AssetDatabase.AddObjectToAsset(node, node.parentGraph);
				foreach(var property in node.properties)
				{
					AssetDatabase.AddObjectToAsset(property.Value, node.parentGraph);
				}
			}

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		/// <summary>
		/// Handles user input for the scrollwheel-zoom
		/// </summary>
		/// <param name="e"></param>
		private void HandleZoom(Event e)
		{
			if (e.delta.y < 0)
				GUIController.zoomTarget /= GUIController.scrollSensitivity;
			else
				GUIController.zoomTarget *= GUIController.scrollSensitivity;
			if (GUIController.zoomTarget < GUIController.zoomMin)
				GUIController.zoomTarget = GUIController.zoomMin;
			else if (GUIController.zoomTarget > GUIController.zoomMax)
				GUIController.zoomTarget = GUIController.zoomMax;
		}

		/// <summary>
		/// Handles user input for the Delete key
		/// </summary>
		private void HandleDeleteKey()
		{
			if (curGraph.selectedNodes != null && curGraph.selectedNodes.Count() > 0)
			{
				List<int> nodesToDelete = new List<int>();
				for (int i = 0; i < curGraph.nodes.Count; i++)
				{
					if (curGraph.selectedNodes.Contains(curGraph.nodes[i]) && !curGraph.nodes[i].Undeletable)
					{
						nodesToDelete.Add(i);
					}
				}
				ConversationEditorUtils.DeleteNodes(curGraph, nodesToDelete.ToArray());
				deleteNodeID = 0;
			}
		}

		/// <summary>
		/// Handles user input for the delete-connection line
		/// </summary>
		/// <param name="e"></param>
		private void HandleDeleteLine(Event e)
		{
			if (GUIController.drawingDeleteLine)
			{
				foreach (var node in curGraph.nodes)
				{
					for (int i = 0; i < node.incoming.Count; i++)   //	Can't foreach here because we modify the collection
					{
						Connectable inc = node.incoming[i];
						Vector2 start1 = inc.OutputPosition;
						Vector2 end1 = node.InputRect.position;
						Vector2 start2 = GUIController.deleteLineStart;
						Vector2 end2 = e.mousePosition;

						if (ConversationEditorUtils.CheckForIntersection(start1, end1, start2, end2))
						{
							ConversationEditorUtils.RemoveConnection(node, inc, curGraph);
							inc.connectionMarkedForDelete = false;
							i--;
						}
					}
				}
				GUIController.drawingDeleteLine = false;
			}
			else
			{
				if (e.clickCount > 1)
				{
					GUIController.drawingDeleteLine = true;
					GUIController.deleteLineStart = e.mousePosition;
				}
			}
		}

		/// <summary>
		/// Handles user input for the context menu
		/// </summary>
		/// <param name="e"></param>
		private void HandleContextMenu(Event e)
		{
			if (e.type == EventType.MouseDown)
			{
				mousePos = e.mousePosition;
				bool overNode = false;
				deleteNodeID = 0;
				if (curGraph != null)
				{
					if (curGraph.nodes.Count > 0)
					{
						for (int i = 0; i < curGraph.nodes.Count; i++)
						{
							if (curGraph.nodes[i].GraphSpaceRect.Contains(mousePos))
							{
								overNode = true;
								deleteNodeID = i;
							}
						}
					}
				}
				if (!overNode)
				{
					ProcessContextMenu(e, 0);
				}
				else
				{
					ProcessContextMenu(e, 1);
				}
			}
		}

		/// <summary>
		/// Handles user input for dragging a node
		/// </summary>
		/// <param name="e"></param>
		/// <param name="node"></param>
		private void HandleDragNode(Event e, Node node)
		{
			if (node.IsSelected && e.alt == false && viewRect.Contains(e.mousePosition) && !GUIController.drawingMarquee)
			{
				if (viewRect.Contains(e.mousePosition))
				{
					if (e.type == EventType.MouseDrag && e.button == 0)
					{
						node.nodeRect = new Rect(
							node.nodeRect.x + e.delta.x / GUIController.zoom,
							node.nodeRect.y + e.delta.y / GUIController.zoom,
							node.nodeRect.width,
							node.nodeRect.height);
					}
				}
			}
		}

		#endregion

	}
}