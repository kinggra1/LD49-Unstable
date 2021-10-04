using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Profiling;

namespace ETools.Editor.Dialogue
{

	public class ConversationEditorWindow : EditorWindow
	{

		#region Variables

		public static ConversationEditorWindow curWindow;

		public ConversationEditorPropertyView propertyView;
		public ConversationEditorWorkView workView;

		public ConversationNodeGraph curGraph = null;

		public float viewPercentage = 0.75f;

		#endregion

		#region Main Methods

		/// <summary>
		/// Create the Window
		/// </summary>
		public static void InitEditorWindow()
		{
			curWindow = EditorWindow.GetWindow<ConversationEditorWindow>() as ConversationEditorWindow;
			GUIContent t = new GUIContent("Conversation Editor");
			curWindow.titleContent = t;

			CreateViews();
		}

		/// <summary>
		/// GUI for the window
		/// </summary>
		private void OnGUI()
		{
			//Check for null views
			if (propertyView == null || workView == null)
			{
				CreateViews();
				return;
			}

			//Get and process the current event
			Event e = Event.current;
			ProcessEvents(e);

			//Update views
			workView.UpdateView(position, new Rect(0f, 0f, .75f, 1f), e, curGraph);
			propertyView.UpdateView(new Rect(position.width, position.y, position.width, position.height),
									new Rect(.75f, 0f, .25f, 1f), e, curGraph);
			Repaint();
		}

		#endregion

		#region Utility Methods

		/// <summary>
		/// Create the initial views if none exist
		/// </summary>
		static void CreateViews()
		{
			curWindow = EditorWindow.GetWindow<ConversationEditorWindow>() as ConversationEditorWindow;
			if (curWindow != null)
			{
				curWindow.propertyView = new ConversationEditorPropertyView();
				curWindow.workView = new ConversationEditorWorkView();
				if (curWindow.curGraph == null)
				{
					curWindow.curGraph = new ConversationNodeGraph();
				}
			}
		}

		public void ProcessEvents(Event e) { }

		#endregion

	}
}