using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ETools.Editor.Dialogue
{

	public class ConversationPopupWindow : EditorWindow
	{

		#region Variables

		static ConversationPopupWindow curPopup;
		string wantedName = "Enter a Name...";

		#endregion


		#region Main Methods

		/// <summary>
		/// Create the popup to create the graph.
		/// </summary>
		public static void InitNodePopup()
		{
			curPopup = EditorWindow.GetWindow<ConversationPopupWindow>() as ConversationPopupWindow;
			GUIContent t = new GUIContent("New Convo");
			curPopup.titleContent = t;
		}

		/// <summary>
		/// GUI Function for the popup window
		/// </summary>
		private void OnGUI()
		{
			GUILayout.Space(20);
			GUILayout.BeginHorizontal();
			GUILayout.Space(20);

			GUILayout.BeginVertical();

			//Graph window title
			EditorGUILayout.LabelField("Create new Convo:", EditorStyles.boldLabel);

			//Graph name field
			wantedName = EditorGUILayout.TextField("Enter Name:", wantedName);

			GUILayout.Space(10);

			GUILayout.BeginHorizontal();

			//Create graph button
			if (GUILayout.Button("Create Graph", GUILayout.Height(40)))
			{
				if (!string.IsNullOrEmpty(wantedName) && wantedName != "Enter a Name...")
				{
					if (ConversationEditorUtils.CreateNewGraph(wantedName))
					{
						this.Close();
					}
				}
				else
				{
					EditorUtility.DisplayDialog("Node Message:", "Please enter a valid graph name.", "OK");
				}
			}

			//Cancel button
			if (GUILayout.Button("Cancel", GUILayout.Height(40)))
			{
				this.Close();
			}
			GUILayout.EndHorizontal();

			GUILayout.EndVertical();

			GUILayout.Space(20);
			GUILayout.EndHorizontal();
			GUILayout.Space(20);
		}

		#endregion

	}
}