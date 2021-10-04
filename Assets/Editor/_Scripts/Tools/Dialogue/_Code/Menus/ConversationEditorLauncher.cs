using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ETools.Editor.Dialogue
{
	public class ConversationEditorLauncher : EditorWindow
	{
		[MenuItem("Imagine/Tools/Dialogue Editor")]
		[MenuItem("Window/Imagine/Dialogue Editor")]
		public static void InitNodeEditor()
		{
			ConversationEditorLauncher.Init();
		}

		public static void Init()
		{
			ConversationEditorLauncher win = EditorWindow.CreateInstance<ConversationEditorLauncher>();
			GUIContent t = new GUIContent("Launcher");
			win.titleContent = t;
			win.minSize = new Vector2(600, 150);
			win.maxSize = new Vector2(600, 150);
			win.Show();
		}

		private void OnGUI()
		{
			GUILayout.Space(15);
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label("Convo Starter", GUIUtils.HeaderGUIStyle());
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.Space(20);

			GUILayout.BeginHorizontal();

			GUILayout.Space(7);

			if (GUILayout.Button("Create New Convo", GUILayout.Height(44)))
			{
				ConversationPopupWindow.InitNodePopup();
				Close();
			}

			GUILayout.Space(7);

			if (GUILayout.Button("Load a Convo", GUILayout.Height(44)))
			{
				bool b = ConversationEditorUtils.LoadGraph();
				if (b)
				{
					Close();
				}
			}

			GUILayout.Space(7);

			GUILayout.EndHorizontal();

			GUILayout.Space(30);

		}
	}
}