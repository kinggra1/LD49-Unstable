using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETools.Editor
{
	public static class GUIUtils
	{
		/// <summary>
		/// Returns the GUI Style for a Red button
		/// </summary>
		/// <returns></returns>
		public static GUIStyle RedButton()
		{
			GUIStyle style = new GUIStyle("button");
			style.normal.textColor = Color.red;
			return style;
		}

		/// <summary>
		/// Returns the GUI Style for a standard-formatted Header
		/// </summary>
		public static GUIStyle HeaderGUIStyle()
		{
			GUIStyle style = new GUIStyle();
			style.font = Resources.Load("Fonts/MQSMagic") as Font;
			style.normal.textColor = Color.white;
			style.fontSize = 40;
			style.fontStyle = FontStyle.Bold;
			return style;
		}

		/// <summary>
		/// Returns the GUI Style for a standard-formatted Sub-header
		/// </summary>
		public static GUIStyle SubHeaderGUIStyle()
		{
			GUIStyle style = new GUIStyle();
			style.font = Resources.Load("Fonts/MQSMagic") as Font;
			style.normal.textColor = Color.white;
			style.fontSize = 26;
			style.fontStyle = FontStyle.Bold;
			return style;
		}

		/// <summary>
		/// Returns the GUI Style for important text
		/// </summary>
		/// <param name="color"></param>
		public static GUIStyle ImportantText(Color color)
		{
			GUIStyle style = new GUIStyle();
			style.fontSize = 14;
			style.fontStyle = FontStyle.Bold;
			style.contentOffset = new Vector2(5, 0);
			style.normal.textColor = color;
			return style;
		}

		/// <summary>
		/// Returns the GUI Style that enables word wrapping
		/// </summary>
		public static GUIStyle WordWrap()
		{
			GUIStyle style = new GUIStyle();
			style.fontSize = 11;
			style.normal.textColor = new Color(.15f, .15f, .15f);
			style.wordWrap = true;
			return style;
		}

		/// <summary>
		/// Returns the GUI Style for important text that supports word wrapping
		/// </summary>
		public static GUIStyle ImportantTextWordWrap(Color color)
		{
			GUIStyle style = ImportantText(color);
			style.wordWrap = true;
			return style;
		}

		/// <summary>
		/// Returns the GUI Style for an informative text box
		/// </summary>
		public static GUIStyle InfoTextBox()
		{
			GUIStyle style = new GUIStyle();
			style.fontSize = 11;
			style.normal.textColor = new Color(.6f, .6f, .6f);
			style.fontStyle = FontStyle.Italic;
			style.wordWrap = true;
			return style;
		}

		/// <summary>
		/// Draws a Horizontal Line in your GUI (GUI Layout)
		/// </summary>
		/// <param name="thickness">The thickness for the line</param>
		/// <param name="leadingSpace">How much space to place above the line</param>
		/// <param name="trailingSpace">How much space to place below the line</param>
		public static void HorizontalLine(int thickness = 1, int leadingSpace = 5, int trailingSpace = 10)
		{
			GUILayout.Space(leadingSpace);
			GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(thickness) });
			GUILayout.Space(trailingSpace);
		}


		#region Utility Classes

		public class DebugMessage
		{
			public string message = "";
			public enum Importance { Message, Warning, Error };
			public Importance importance;

			public DebugMessage(string debugMessage, Importance howImportant)
			{
				message = debugMessage;
				importance = howImportant;
			}

			public DebugMessage() { }
		}

		#endregion

	}
}

