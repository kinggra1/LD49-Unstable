using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Reflection;
using System.Linq;
using ETools.Dialogue;
using ETools.Utilities;

namespace ETools.Editor.Dialogue
{
	[Serializable]
	public class ConversationNodeGraph : ScriptableObject
	{

		#region Variables

		//Global vars
		public string graphName;
		public string graphPath;
		public List<Speaker> speakers = null;
		public Conversation.ConversationType conversationType;
		public List<Node> nodes;
		public Node endNode;
		public List<Node> selectedNodes = new List<Node>();

		#endregion

		#region Main Methods

		/// <summary>
		/// Initializes the graph once loaded.
		/// </summary>
		public void InitGraph()
		{
			if (nodes != null && nodes.Count > 0)
			{
				for (int i = 0; i < nodes.Count; i++)
				{
					nodes[i].InitNode();
				}
			}

			//Housekeeping
			if (nodes == null)
			{
				nodes = new List<Node>();
			}
			if (speakers == null)
			{
				speakers = new List<Speaker>();
			}
			if (graphName == null)
			{
				graphName = "New Graph";
			}

			speakers = new List<Speaker>(SystemsUtility.RemoveEmptyEntriesFromCollection<Speaker>(speakers));

		}

		/// <summary>
		/// GUI function for the graph.
		/// </summary>
		/// <param name="e">The current event</param>
		/// <param name="viewRect">The rect the graph takes up</param>
		/// <param name="viewSkin">The skin to be used.</param>
		public void UpdateGraphGUI(Event e, Rect viewRect, GUISkin viewSkin)
		{
			//Do nothing if no nodes
			if (nodes.Count == 0)
			{
				return;
			}

			//Update the gui for all the nodes
			for (int i = 0; i < nodes.Count; i++)
			{
				nodes[i].UpdateNodeGUI(e, viewRect, viewSkin);
			}

			//If a node is clicked, draw the connection
			if (GUIController.WantsConnection && GUIController.connectionNode != null)
			{
				DrawConnectionToMouse(e.mousePosition);
			}
			else if (GUIController.backwardsConnectionNode != null)
			{
				DrawConnectionToMouse(e.mousePosition);
			}

			//Process input events
			ProcessEvents(e, viewRect);

			if (GUIController.drawingMarquee)
			{
				var marq = GUIController.marqueeSelectPos;
				Rect marqRect = new Rect(
					Math.Min(marq.x, e.mousePosition.x),
					Math.Min(marq.y, e.mousePosition.y),
					Math.Abs(marq.x - e.mousePosition.x),
					Math.Abs(marq.y - e.mousePosition.y));
				GUI.Box(marqRect, new GUIContent(), viewSkin.GetStyle("MarqueeSelect"));
			}

			EditorUtility.SetDirty(this);
		}

		/// <summary>
		/// Customizable function to compile the graph into a usable gameplay asset.
		/// </summary>
		public void Compile()
		{
			//	Some error handling
			if (endNode.outgoing == null)
			{
				Debug.LogError("No node is plugged into the start node!");
				return;
			}


			Dictionary<string, Speaker> speakerLUT = new Dictionary<string, Speaker>();
			Dictionary<string, Node> nodeLUT = new Dictionary<string, Node>();
			Dictionary<string, DialogueNode> convoLUT = new Dictionary<string, DialogueNode>();

			//	Create convo
			Conversation convo;
			//string filePath = Path.Combine(Application.dataPath, "Data\\Conversations\\" + graphName + ".asset");

			string localPath = graphPath.Replace("DialogueGraphs", "Conversations");
			string filePath = EditorUtils.PathCombine(Application.dataPath, localPath.PathMinusAssets());
			Directory.CreateDirectory(filePath.GetFolder());
			if (File.Exists(filePath))
			{
				convo = AssetDatabase.LoadAssetAtPath(localPath, typeof(Conversation)) as Conversation;
				convo.firstNode = null;
				convo.speakers = new List<Speaker>();
				convo.DestroyAllChildrenInProject();
				convo.allNodes = new List<DialogueNode>();

			}
			else
			{
				convo = Conversation.CreateInstance<Conversation>();
				AssetDatabase.CreateAsset(convo, localPath);
			}

			convo.name = graphName;
			convo.conversationType = conversationType;

			//	Create list of graph node
			List<DialogueNode> compiledNodes = new List<DialogueNode>();

			//	Add speakers
			foreach (Speaker speaker in speakers)
			{
				convo.speakers.Add(speaker);
				speakerLUT[speaker.name] = speaker;
			}

			//	Build node list
			foreach (var guiNode in nodes)
			{
				DialogueNode gameNode = null;
				if (guiNode is SpeechNodeGUI)
				{
					SpeechNodeGUI node = guiNode as SpeechNodeGUI;
					gameNode = node.ToGameData();
				}
				if (guiNode is ChoiceNodeGUI)
				{
					ChoiceNodeGUI node = guiNode as ChoiceNodeGUI;
					gameNode = node.ToGameData();
					foreach (var choice in (gameNode as ChoiceNode).choices)
						AssetDatabase.AddObjectToAsset(choice, convo);
				}
				if (guiNode is RandomChoiceNodeGUI)
				{
					RandomChoiceNodeGUI node = guiNode as RandomChoiceNodeGUI;
					gameNode = node.ToGameNode();
				}
				if (guiNode is CustomScriptNodeGUI)
				{
					CustomScriptNodeGUI node = guiNode as CustomScriptNodeGUI;
					gameNode = node.ToGameData();
				}
				if (guiNode is AudioNodeGUI)
				{
					AudioNodeGUI node = guiNode as AudioNodeGUI;
					gameNode = node.ToGameData();
				}
				if (gameNode != null)
				{
					convo.allNodes.Add(gameNode);
					AssetDatabase.AddObjectToAsset(gameNode, convo);
					compiledNodes.Add(gameNode);
					nodeLUT[gameNode.ToString()] = guiNode;
					convoLUT[guiNode.ToString()] = gameNode;
				}
			}

			foreach (var node in compiledNodes)
			{
				if (node is SpeechNode)
				{
					SpeechNode sp = node as SpeechNode;
					SpeechNodeGUI match = (SpeechNodeGUI)nodeLUT[sp.ToString()];
					if (match.outgoing != null && convoLUT.ContainsKey(match.outgoing.ToString()))
					{
						sp.nextNode = convoLUT[match.outgoing.ToString()];
					}
				}
				else if (node is ChoiceNode)
				{
					ChoiceNode ch = node as ChoiceNode;
					ChoiceNodeGUI match = (ChoiceNodeGUI)nodeLUT[ch.ToString()];
					for (int i = 0; i < ch.choices.Count; i++)
					{
						Choice choice = ch.choices[i];
						Connectable submatch = match.properties["Option " + (i + 1)];
						if (submatch.outgoing != null && convoLUT.ContainsKey(submatch.outgoing.ToString()))
						{
							choice.nextNode = convoLUT[submatch.outgoing.ToString()];
						}
					}
				}
				else if (node is ConditionalSwitchNode)
				{
					ConditionalSwitchNode ccn = node as ConditionalSwitchNode;
					ConditionalSwitchNodeGUI match = (ConditionalSwitchNodeGUI)nodeLUT[ccn.ToString()];

					Node defaultNodeGUI = match.properties["Default"].outgoing;
					DialogueNode defaultNode = convoLUT[defaultNodeGUI.ToString()];
					ccn.defaultNextNode = defaultNode;

					for (int i = 1; i < ccn.conditionals.Count; i++)
					{
						Node nextNodeGUI = match.properties["Condition " + (i + 1)].outgoing;
						if (nextNodeGUI == null)
						{
							ccn.nextNodes.Add(null);
							continue;
						}
						//Node nextNodeGUI = outgoing.parent;
						DialogueNode nextNode = convoLUT[nextNodeGUI.ToString()];
						ccn.nextNodes.Add(nextNode);
					}
				}
				else if (node is MilestoneNode)
				{
					MilestoneNode miNode = node as MilestoneNode;
					MilestoneNodeGUI match = (MilestoneNodeGUI)nodeLUT[miNode.ToString()];

					if (match.outgoing != null && convoLUT.ContainsKey(match.outgoing.ToString()))
					{
						miNode.nextNode = convoLUT[match.outgoing.ToString()];
					}
				}
				else if (node is RandomChoiceNode)
				{
					RandomChoiceNode rcNode = node as RandomChoiceNode;
					RandomChoiceNodeGUI match = (RandomChoiceNodeGUI)nodeLUT[rcNode.ToString()];

					for (int i = 0; i < rcNode.nextNodes.Count(); i++)
					{
						Property prop = match.properties["Weight " + (i + 1)];
						if (prop.outgoing != null && convoLUT.ContainsKey(prop.outgoing.ToString()))
						{
							rcNode.nextNodes[i] = convoLUT[prop.outgoing.ToString()];
						}
					}
				}
				else if (node is CustomScriptNode)
				{
					CustomScriptNode cs = node as CustomScriptNode;
					CustomScriptNodeGUI match = (CustomScriptNodeGUI)nodeLUT[cs.ToString()];
					if (match.outgoing != null && convoLUT.ContainsKey(match.outgoing.ToString()))
					{
						cs.nextNode = convoLUT[match.outgoing.ToString()];
					}
				}
				if (node is AudioNode)
				{
					AudioNode an = node as AudioNode;
					AudioNodeGUI match = (AudioNodeGUI)nodeLUT[an.ToString()];
					if (match.outgoing != null && convoLUT.ContainsKey(match.outgoing.ToString()))
					{
						an.nextNode = convoLUT[match.outgoing.ToString()];
					}
				}
			}

			convo.firstNode = convoLUT[endNode.outgoing.ToString()];

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();

			EditorUtility.DisplayDialog("Compile Finished", "Compile finished successfully!", "Hell yeah");

		}

		#endregion

		#region Utility Methods

		/// <summary>
		/// Handle input events.
		/// </summary>
		/// <param name="e">The current input event.</param>
		/// <param name="viewRect">The rect being evaluated.</param>
		void ProcessEvents(Event e, Rect viewRect)
		{
			if (e.type == EventType.Layout)
				return;
			if (viewRect.Contains(e.mousePosition))
			{
				//Left click
				if (e.button == 0)
				{
					if (e.type == EventType.MouseDown && e.alt == false)
					{
						List<Node> nodesToSelect = new List<Node>();
						if (e.shift)
							nodesToSelect.AddRange(selectedNodes);
						for (int i = 0; i < nodes.Count; i++)
						{
							if (nodes[i].GraphSpaceRect.Contains(e.mousePosition))
							{
								if (selectedNodes.Contains(nodes[i]))
								{
									if (e.shift)
										nodesToSelect.Remove(nodes[i]);
									else
										nodesToSelect.AddRange(selectedNodes);
								}
								else
									nodesToSelect.Add(nodes[i]);
								break;
							}
						}

						DeselectAllNodes();
						selectedNodes.AddRange(nodesToSelect.Distinct());

					}
				}
				//Marquee select
				if (e.type == EventType.MouseDrag &&
					e.button == 0 &&
					e.alt == false &&
					!GUIController.drawingDeleteLine)
				{
					if (selectedNodes.Count == 0)
					{
						if (!GUIController.drawingMarquee)
						{
							GUIController.drawingMarquee = true;
							GUIController.marqueeSelectPos = e.mousePosition;
						}
					}
				}
				if (e.type == EventType.MouseUp)
				{
					if (GUIController.drawingMarquee)
					{
						Vector2 marq = GUIController.marqueeSelectPos;
						Rect marqRect = new Rect(
							Math.Min(marq.x, e.mousePosition.x),
							Math.Min(marq.y, e.mousePosition.y),
							Math.Abs(marq.x - e.mousePosition.x),
							Math.Abs(marq.y - e.mousePosition.y));
						foreach (var node in nodes)
						{
							Vector2 center = node.GraphSpaceRect.center;
							if (marqRect.Contains(center))
							{
								selectedNodes.Add(node);
							}
						}
					}
					GUIController.drawingMarquee = false;
				}

				//Drag background
				if (
					(e.type == EventType.MouseDrag &&
					(e.alt == true && e.button == 0) || (e.button == 2)))
				{
					Vector2 delta = e.delta / (e.button == 0 ? 1f : 2f);    //	Dividing by 2 here to fix a bug,
					GUIController.lookPosTarget += delta / GUIController.zoom;
				}
			}
		}

		/// <summary>
		/// Draw a connection to the mouse if you are connecting two nodes.
		/// </summary>
		/// <param name="mousePosition">Position of the mouse.</param>
		void DrawConnectionToMouse(Vector2 mousePosition)
		{
			//Values for the curve.
			Texture2D tex = (Texture2D)Resources.Load("Textures/Editor/line");
			float width = HandleUtility.GetHandleSize(Vector3.zero) * .1f;

			//Values for the tangents.
			Vector2 startPoint = new Vector2();
			float dist;
			float distVal;
			if (GUIController.WantsConnection)
			{
				startPoint = GUIController.connectionNode.OutputPosition;// new Vector2(connectionNode.GraphSpaceRect.x + connectionNode.GraphSpaceRect.width + 14, connectionNode.GraphSpaceRect.y + connectionNode.GraphSpaceRect.height * .5f);
				dist = Vector2.Distance(startPoint, mousePosition);
				distVal = dist / 2f;
			}
			else if (GUIController.WantsBackwardsConnection)
			{
				startPoint = GUIController.backwardsConnectionNode.InputPosition;
				dist = Vector2.Distance(startPoint, mousePosition);
				distVal = dist / -2f;
			}
			else
			{
				return;
			}


			//Drawing

			Handles.DrawBezier(new Vector3(startPoint.x, startPoint.y, 0),
					 new Vector3(mousePosition.x, mousePosition.y, 0f),
					 new Vector3(startPoint.x + distVal, startPoint.y, 0), new Vector3(mousePosition.x - distVal, mousePosition.y, 0), Color.white, tex, width);

		}

		/// <summary>
		/// Deselect all nodes.
		/// </summary>
		void DeselectAllNodes()
		{
			selectedNodes.Clear();
		}

		#endregion

	}
}