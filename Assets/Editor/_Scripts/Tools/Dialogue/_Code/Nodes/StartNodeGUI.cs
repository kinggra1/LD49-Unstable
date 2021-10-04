using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETools.Editor.Dialogue
{
	public class StartNodeGUI : Node
	{
		public static StartNodeGUI New(ConversationNodeGraph parent)
		{
			StartNodeGUI node = CreateInstance<StartNodeGUI>();

			node.parentGraph = parent;

			//Node name and type
			node.nodeName = "Start";
			node.isOutput = true;
			node.tooltip = "This is where the dialogue starts.";

			return node;
		}

		public override void UpdateNode(Event e, Rect viewRect)
		{
			base.UpdateNode(e, viewRect);
			name = "Start Node";
		}
	}
}