using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using ETools.Utilities;
using ETools.Strings;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ETools.Editor.Dialogue
{
	[System.Serializable]
	public class PropertyTable : SerializableDictionaryBase<string, Property> { }

	[Serializable]
	public class Node : Connectable
	{

		#region Variables

		//Admin vars
		public string nodeName;
		public ConversationNodeGraph parentGraph;
		public string tooltip = "";


		[SerializeField]
		public PropertyTable properties = new PropertyTable();
		public int guid = -1;
		public List<Connectable> incoming = new List<Connectable>();

		//GUI vars
		protected int width = 150;
		protected int height = 30;
		protected GUISkin nodeSkin;

		#region Properties

		public bool IsSelected
		{
			get
			{
				return parentGraph.selectedNodes.Contains(this);
			}
		}

		public bool Undeletable
		{
			get
			{
				return parentGraph.endNode == this;
			}
		}

		#endregion

		#endregion

		#region Constructors

		public void OnEnable()
		{
			guid = SystemsUtility.GetUniqueGUID(8);
		}

		#endregion

		#region Main Methods

		/// <summary>
		/// Initialize the node once it's loaded.
		/// </summary>
		public virtual void InitNode()
		{
			name = nodeName;
			if (nodeRect == Rect.zero)
			{
				nodeRect = new Rect(10f, 10f, width, height);
			}
		}

		/// <summary>
		/// Adds a property to this node
		/// </summary>
		/// <param name="displayName">The user-facing display name for the property</param>
		/// <param name="valueType">The type of the serialized value for the property</param>
		/// <param name="exposable">Whether or not the property should be exposed as an input for a node</param>
		/// <param name="usesOutput">Whether or not the property should output directly to another node</param>
		/// <returns>The property being added</returns>
		protected Property NewProperty(string displayName, ValueType valueType, bool exposable = false, bool usesOutput = false)
		{
			Property prop = Property.New(this, displayName, valueType, exposable, usesOutput);
			AssetDatabase.AddObjectToAsset(prop, parentGraph);
			properties[displayName] = prop;
			return prop;
		}

		/// <summary>
		/// Adds an Object property to this node
		/// </summary>
		/// <param name="displayName">The user-facing display name for the property</param>
		/// <param name="valueType">The type of the serialized value for the property - must be a Unity Object</param>
		/// <param name="exposable">Whether or not the property should be exposed as an input for a node</param>
		/// <param name="usesOutput">Whether or not the property should output directly to another node</param>
		/// <returns>The property being added</returns>
		protected Property NewProperty(string displayName, Type valueType, bool exposable = false, bool usesOutput = false)
		{
			Property prop = Property.New(this, displayName, valueType, exposable, usesOutput);
			AssetDatabase.AddObjectToAsset(prop, parentGraph);
			properties[displayName] = prop;
			return prop;
		}

		/// <summary>
		/// Removes a property from this node by name
		/// </summary>
		/// <param name="displayName">The name of the property to remove</param>
		protected void RemoveProperty(string displayName)
		{
			properties.Remove(displayName);
		}

		/// <summary>
		/// Update the GUI for the node.
		/// </summary>
		/// <param name="e">Current input event</param>
		/// <param name="viewRect">The current rect for the view</param>
		/// <param name="viewSkin">GUI skin for the node editor</param>
		public virtual void UpdateNodeGUI(Event e, Rect viewRect, GUISkin viewSkin)
		{
			//Set up the guistyles
			GUIStyle nodeStyle;
			GUIStyle propertyStyle;
			if (!IsSelected)
			{
				nodeStyle = new GUIStyle(viewSkin.GetStyle("NodeDefault"));
				propertyStyle = new GUIStyle(viewSkin.GetStyle("PropertyDefault"));
			}
			else
			{
				nodeStyle = new GUIStyle(viewSkin.GetStyle("NodeSelected"));
				propertyStyle = new GUIStyle(viewSkin.GetStyle("PropertySelected"));
			}

			if (outgoing != null)
				DrawLineToOutput(this);

			GUIStyle standardStyle = new GUIStyle(viewSkin.GetStyle("Button"));
			GUIStyle connectedStyle = new GUIStyle(viewSkin.GetStyle("ButtonGreen"));

			//Iterate through all the properties
			for (int i = 0; i < properties.Count; i++)
			{
				//Update the property
				var prop = properties.ElementAt(i).Value;

				//Draw the property to the screen
				DisplayPropertyGUI(i, viewSkin);

				//Set up the appropriate visuals for the input button
				if (prop.isOutput)
				{
					GUIStyle buttonStyle = standardStyle;

					//	Draw the connection line if it's plugged in
					if (prop.HasOutput)
					{
						buttonStyle = connectedStyle;
						DrawLineToOutput(prop);
					}
					//	Handle the response for clicking the output button
					Rect propertyOutputRect = prop.OutputRect;
					if (GUI.Button(propertyOutputRect, "", buttonStyle))
					{
						PropertyOutputClick(prop);
					}
				}

			}

			//	Scale font by screen zoom
			nodeStyle.fontSize = (int)(nodeStyle.fontSize * GUIController.zoom);

			//Draw the node
			GUI.Box(GraphSpaceRect, nodeName, nodeStyle);

			//Tooltip Rect Setup
			Rect tooltipRect = new Rect(GraphSpaceRect);
			tooltipRect.height *= .6f;
			tooltipRect.width = tooltipRect.height;
			tooltipRect.x = tooltipRect.x + GraphSpaceRect.width - tooltipRect.width - 10;
			tooltipRect.y = tooltipRect.y + .5f * (GraphSpaceRect.height - tooltipRect.height);

			//Tooltip button
			GUI.Box(tooltipRect, " ", viewSkin.GetStyle("Tooltip"));
			if (tooltipRect.Contains(Event.current.mousePosition))
			{
				Rect toolTextRect = new Rect(GraphSpaceRect);
				toolTextRect.height *= .75f;
				toolTextRect.y -= toolTextRect.height;
				toolTextRect.x = Event.current.mousePosition.x;
				toolTextRect.width = 15 + tooltip.Length * viewSkin.GetStyle("TooltipBox").fontSize * 0.5f;
				GUI.Box(toolTextRect, tooltip, viewSkin.GetStyle("TooltipBox"));
			}

			GUIStyle outputStyle = standardStyle;
			if (outgoing != null)
			{
				outputStyle = viewSkin.GetStyle("ButtonGreen");
			}

			//Output button
			if (isOutput)
			{
				if (GUI.Button(OutputRect, "", outputStyle))
				{
					NodeOutputClick();
				}
			}

			if (incoming.Count > 0)
				outputStyle = connectedStyle;
			else
				outputStyle = standardStyle;
			if (!Undeletable)
			{
				if (GUI.Button(InputRect, "", outputStyle))
				{
					NodeInputClick();
				}
			}
			EditorUtility.SetDirty(this);
		}

		#endregion

		#region GUI Methods

		/// <summary>
		/// Draw the properties for the node to the property view.
		/// </summary>
		public virtual void DrawNodePropertyView(GUISkin guiSkin)
		{
			nodeName = GUILayout.TextField(nodeName, guiSkin.GetStyle("NodeNamePropertyView"));
			GUILayout.Space(10);
			GUILayout.Label(tooltip, GUIUtils.ImportantText(Color.black));
			GUIUtils.HorizontalLine();

			//Properties
			GUILayout.Label("Properties", GUIUtils.ImportantText(Color.black));
			GUILayout.Space(7);
			foreach (var prop in properties.Values)
				prop.DisplayEditableValue();

		}

		/// <summary>
		/// Draws a line from a node to its output
		/// </summary>
		/// <param name="node"></param>
		void DrawLineToOutput(Connectable node)
		{
			if (node.outgoing == null) return;
			//Define the handle settigns
			Texture2D tex = (Texture2D)Resources.Load("Textures/Editor/line");
			float width = HandleUtility.GetHandleSize(Vector3.zero) * .1f;
			Color handleColor = Color.white;

			//Define the handle points
			Vector2 startPoint = node.OutputPosition;
			Vector2 endPoint = node.outgoing.InputPosition;
			float dist = Vector2.Distance(startPoint, endPoint);
			float distVal = dist / 2f;

			//Turn the line yellow if it's about to be deleted
			if (node.connectionMarkedForDelete)
			{
				width *= 2f;
				handleColor = Color.yellow;
			}

			//Draw the curve
			Handles.DrawBezier(new Vector3(startPoint.x, startPoint.y, 0),
					 new Vector3(endPoint.x, endPoint.y, 0f),
					 new Vector3(startPoint.x + distVal, startPoint.y, 0), new Vector3(endPoint.x - distVal, endPoint.y, 0), handleColor, tex, width);

		}

		/// <summary>
		/// Draws the Node GUI for an individual property by its index
		/// </summary>
		/// <param name="i">Index of the property to draw</param>
		/// <param name="viewSkin"></param>
		private void DisplayPropertyGUI(int i, GUISkin viewSkin)
		{
			Property prop = properties.ElementAt(i).Value;

			GUIStyle propertyStyle;
			if (IsSelected)
				propertyStyle = new GUIStyle(viewSkin.GetStyle("PropertySelected"));
			else
				propertyStyle = new GUIStyle(viewSkin.GetStyle("PropertyDefault"));

			float soFar = 0;
			for (int c = 0; c < i; c++)
			{
				soFar += (int)properties.ElementAt(c).Value.nodeRect.height;
			}

			//	Draw the backdrop for the property
			prop.nodeRect = new Rect(nodeRect.x, nodeRect.y + nodeRect.height + soFar, nodeRect.width, prop.nodeRect.height);
			Rect propertyRect = prop.GraphSpaceRect;
			propertyStyle.fontSize = (int)(propertyStyle.fontSize * GUIController.zoom);
			GUI.Box(propertyRect, prop.propertyName, propertyStyle);

			//Define the rect for the field
			Rect fieldRect = new Rect(propertyRect);
			fieldRect.width *= .5f;
			fieldRect.x += fieldRect.width;
			fieldRect.width *= .9f;
			fieldRect.height -= 4;
			fieldRect.y += 2;

			//	Draw the property conetnts
			switch (prop.Type)
			{
				case ValueType.Float:
					prop.value.floatValue = EditorGUI.FloatField(fieldRect, prop.value.floatValue, propertyStyle);
					break;

				case ValueType.String:
					prop.value.stringValue = EditorGUI.TextField(fieldRect, prop.value.stringValue);
					break;

				case ValueType.Int:
					prop.value.intValue = EditorGUI.IntField(fieldRect, prop.value.intValue);
					break;

				case ValueType.Bool:
					prop.value.boolValue = EditorGUI.Toggle(fieldRect, prop.value.boolValue);
					break;

				case ValueType.Object:
					prop.value.objectValue = EditorGUI.ObjectField(fieldRect, prop.value.objectValue, prop.value.ObjectValueType, false);
					if (prop.value.objectValue != null)
						EditorUtility.SetDirty(prop.value.objectValue);
					break;

				case ValueType.Speaker:
					if(parentGraph.speakers.Count == 0)
					{
						EditorGUI.Popup(fieldRect, 0, new string[1] { "<NO SPEAKERS>"});
						break;
					}
					int priorIndex = parentGraph.speakers.IndexOf(prop.value.speakerValue);
					if (priorIndex == -1) priorIndex = 0;
					string[] options = (from s in parentGraph.speakers where s != null select s.characterName).ToArray();
					int index = EditorGUI.Popup(fieldRect, priorIndex, options);
					prop.value.speakerValue = parentGraph.speakers[index];
					break;

				case ValueType.GameString:
					if (prop.value.gameStringValue == null)
					{
						Rect topHalfRect = new Rect(fieldRect.position, new Vector2(fieldRect.width, fieldRect.height * 0.5f));
						Rect bottomHalfRect = new Rect(fieldRect.position + new Vector2(0, fieldRect.height * 0.5f), new Vector2(fieldRect.width, fieldRect.height * 0.5f));
						prop.value.gameStringValue = (GameString)EditorGUI.ObjectField(topHalfRect, prop.value.gameStringValue, typeof(GameString), false);
						if(GUI.Button(bottomHalfRect, new GUIContent("Create GameString")))
						{
							prop.value.gameStringValue = GameStringManager.CreateGameStringInProject(parentGraph.graphName + "_SpeechNode");
						}
					}
					else
						prop.value.gameStringValue.EnglishString = EditorGUI.TextField(fieldRect, prop.value.gameStringValue.EnglishString);
					break;
			}
		}

		/// <summary>
		/// Handles response for clicking a Property output
		/// </summary>
		/// <param name="prop"></param>
		private void PropertyOutputClick(Property prop)
		{
			if (GUIController.WantsBackwardsConnection)
			{
				ConversationEditorUtils.MakeConnection(GUIController.backwardsConnectionNode, prop, parentGraph);
				GUIController.backwardsConnectionNode = null;
				//prop.outgoing = GUIController.backwardsConnectionNode;
				//GUIController.backwardsConnectionNode = null;
			}
			else
			{
				GUIController.connectionNode = prop;
			}
		}

		/// <summary>
		/// Handles the reponse for clicking a Node output
		/// </summary>
		private void NodeOutputClick()
		{
			if (!GUIController.WantsBackwardsConnection)
			{
				GUIController.connectionNode = this;
			}
			else
			{
				ConversationEditorUtils.MakeConnection(GUIController.backwardsConnectionNode, this, parentGraph);
				GUIController.backwardsConnectionNode = null;
			}
		}

		/// <summary>
		/// Handles the response for clicking a Node input
		/// </summary>
		private void NodeInputClick()
		{
			if (GUIController.WantsConnection && !GUIController.WantsBackwardsConnection)
			{
				ConversationEditorUtils.MakeConnection(this, GUIController.connectionNode, parentGraph);
				GUIController.connectionNode = null;
			}
			else
			{
				GUIController.backwardsConnectionNode = this;
			}
		}

		#endregion

		#region Utility Methods

		/// <summary>
		///	Generates a clean, readable name for the Node.  
		///	Use this only when you aren't concerned about string collision in dictionaries,
		///	since we can't guarantee that nodes won't be named the same thing.
		/// </summary>
		/// <returns></returns>
		public string Name()
		{
			return nodeName + "(" + GetType().Name + ")";
		}

		/// <summary>
		/// Update the node each frame.
		/// </summary>
		/// <param name="e">The current input event.</param>
		/// <param name="viewRect">The current view rect</param>
		public virtual void UpdateNode(Event e, Rect viewRect)
		{
			//this.name = nodeName;
			this.name = Name();
		}

		public override string ToString()
		{
			return nodeName + " (" + guid + ")";
		}

		#endregion
	}
}
