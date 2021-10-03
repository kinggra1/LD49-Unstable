using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace ETools.Editor.Dialogue
{

	[Serializable]
	public class ViewBase
	{
		#region Variables

		public string viewTitle;
		public Rect viewRect;
		protected GUISkin viewSkin;
		protected ConversationNodeGraph curGraph;

		#endregion

		#region Constructors

		public ViewBase(string title)
		{
			viewTitle = title;
		}

		#endregion

		#region Main Methods

		/// <summary>
		/// Called to draw the view to the screen.
		/// </summary>
		/// <param name="editorRect">The rect of the entire window</param>
		/// <param name="perentageRect">The portion of the window this view takes up.</param>
		/// <param name="e">The current event</param>
		/// <param name="curGraph">The graph this view belongs to</param>
		public virtual void UpdateView(Rect editorRect, Rect perentageRect, Event e, ConversationNodeGraph curGraph)
		{
			//Get the skin if you need it
			if (viewSkin == null)
			{
				GetEditorSkin();
			}

			//Set the current view graph
			this.curGraph = curGraph;

			//Update view title
			if (curGraph != null)
			{
				viewTitle = curGraph.graphName;
			}
			else
			{
				viewTitle = "No Graph";
			}

			//Establish the viewRect
			viewRect = new Rect(editorRect.x * perentageRect.x,
						editorRect.y * perentageRect.y,
						editorRect.width * perentageRect.width,
						editorRect.height * perentageRect.height);

		}

		/// <summary>
		/// Processes input events.
		/// </summary>
		/// <param name="e">The current event</param>
		public virtual void ProcessEvents(Event e) { }


		#endregion

		#region Utility Methods

		/// <summary>
		/// Gets the skin from resources and saves it
		/// </summary>
		protected void GetEditorSkin()
		{
			viewSkin = Resources.Load("GUISkins/EditorSkins/NodeEditorSkin") as GUISkin;
		}


		#endregion
	}
}