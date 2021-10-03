using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using ETools.Dialogue;

namespace ETools.Editor.Dialogue
{

	[Serializable]
	public class ConversationEditorPropertyView : ViewBase
	{

		#region Variables

		Vector2 scrollVal = new Vector2();

		#endregion


		#region Main Methods

		/// <summary>
		/// Update the contents of the Property View
		/// </summary>
		/// <param name="editorRect">The rect for the entire window</param>
		/// <param name="percentageRect">The percentage that the property view takes up</param>
		/// <param name="e">The current input event</param>
		/// <param name="curGraph">The graph currently opened</param>
		public override void UpdateView(Rect editorRect, Rect percentageRect, Event e, ConversationNodeGraph curGraph)
		{
			//Always call the base
			base.UpdateView(editorRect, percentageRect, e, curGraph);

			if (e.type == EventType.MouseDown)
				EditorGUIUtility.keyboardControl = 0;

			//Rect definition
			Rect titleRect = new Rect(viewRect);
			titleRect.height = 20;
			titleRect.y += 5;
			titleRect.width *= .8f;
			titleRect.x += viewRect.width * .1f;

			//Set the graph name (editable)
			curGraph.graphName = GUI.TextField(titleRect, curGraph.graphName, viewSkin.GetStyle("Title"));

			//Set up the property area
			viewRect.x += 10;
			viewRect.width -= 10;

			GUILayout.BeginArea(viewRect);
			scrollVal = EditorGUILayout.BeginScrollView(scrollVal);
			GUILayout.Space(60);
			//Draw nothing if there's no graph (or no node)
			if (curGraph.selectedNodes == null || curGraph.selectedNodes.Count() == 0)
			{
				EditorGUILayout.LabelField("CONVERSATION TYPE");
				curGraph.conversationType = (Conversation.ConversationType)EditorGUILayout.EnumPopup(curGraph.conversationType);

				GUILayout.Space(10);

				//	Draw Speaker properties
				DrawSpeakerProperties();
			}

			//Draw the node properties
			else
			{
				foreach (var node in curGraph.selectedNodes)
				{
					node.DrawNodePropertyView(viewSkin);
					if (curGraph.selectedNodes.IndexOf(node) != curGraph.selectedNodes.Count - 1)
					{
						GUIUtils.HorizontalLine(3, 10, 15);
					}
				}
			}
			EditorGUILayout.EndScrollView();
			GUILayout.EndArea();


			ProcessEvents(e);
		}

		#endregion


		#region Utility Methods

		/// <summary>
		/// Process the current input event.
		/// </summary>
		/// <param name="e">The current input event</param>
		public override void ProcessEvents(Event e)
		{
			//Always call the base
			base.ProcessEvents(e);
		}

		/// <summary>
		/// Draws properties for the conversation's Speakers
		/// </summary>
		private void DrawSpeakerProperties()
		{
			GUILayout.Label("SPEAKERS");
			for (int i = 0; i < curGraph.speakers.Count; i++)
			{
				GUILayout.BeginHorizontal("box");
				curGraph.speakers[i] = (Speaker)EditorGUILayout.ObjectField(
					curGraph.speakers[i]?.characterName,
					curGraph.speakers[i], typeof(Speaker), false);
				if (GUILayout.Button("X", GUILayout.Width(20)))
				{
					curGraph.speakers.RemoveAt(i);
					i--;
				}
				GUILayout.EndHorizontal();
			}
			if (GUILayout.Button("Add Speaker"))
			{
				curGraph.speakers.Add(null);
				GUIUtility.ExitGUI();
			}
		}

		#endregion


		#region Constructor

		public ConversationEditorPropertyView() : base("Property View") { }

		#endregion

	}
}