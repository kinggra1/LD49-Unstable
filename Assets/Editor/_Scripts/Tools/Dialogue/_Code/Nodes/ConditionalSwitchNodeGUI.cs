using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using ETools.Dialogue;
using ETools;
using ETools.Conditionals;

namespace ETools.Editor.Dialogue
{
	[GraphNode("Conditional Switch Node")]
	public class ConditionalSwitchNodeGUI : Node
	{
		public static ConditionalSwitchNodeGUI New(ConversationNodeGraph parent)
		{
			ConditionalSwitchNodeGUI node = CreateInstance<ConditionalSwitchNodeGUI>();

			node.parentGraph = parent;

			//Node name and type
			node.nodeName = "Conditional Switch";
			node.isOutput = false;
			node.width = 350;
			node.tooltip = "Changes dialogue flow based on Conditionals.";

			//Properties
			node.NewProperty("Default", ValueType.None, true, true);
			node.NewProperty("Condition 1", typeof(Conditional), true, true);
			node.NewProperty("Condition 2", typeof(Conditional), true, true);
			node.NewProperty("Condition 3", typeof(Conditional), true, true);
			node.NewProperty("Condition 4", typeof(Conditional), true, true);

			return node;
		}

		public override void DrawNodePropertyView(GUISkin guiSkin)
		{
			base.DrawNodePropertyView(guiSkin);

			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Add New Conditional"))
			{
				NewProperty("Condition " + (properties.Count() - 1), typeof(Conditional), true, true);
			}
			if (GUILayout.Button("Remove Last Conditional"))
			{
				RemoveProperty("Condition " + (properties.Count()));
			}
			GUILayout.EndHorizontal();
		}

		public ConditionalSwitchNode ToGameData()
		{
			ConditionalSwitchNode node = CreateInstance<ConditionalSwitchNode>();

			foreach (Property property in properties.Values)
			{
				node.conditionals.Add(property.value.objectValue as Conditional);
			}

			node.name = ToString();
			node.guid = guid;

			return node;
		}

	}
}