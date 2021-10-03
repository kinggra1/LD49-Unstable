using ETools.Dialogue;
using ETools.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETools.Editor.Dialogue
{
	[GraphNode("Milestone Node")]
	public class MilestoneNodeGUI : Node
	{

		public static MilestoneNodeGUI New(ConversationNodeGraph parent)
		{
			MilestoneNodeGUI node = CreateInstance<MilestoneNodeGUI>();

			node.parentGraph = parent;

			node.width = 300;

			//Define node name and type
			node.nodeName = "Milestone";
			node.isOutput = true;
			node.tooltip = "Sets the value of a Milestone.";

			node.NewProperty("Milestone", typeof(Milestone), true);
			node.NewProperty("Set Value", ValueType.Int, true);

			return node;
		}

		public MilestoneNode ToGameData()
		{
			MilestoneNode node = CreateInstance<MilestoneNode>();
			node.milestone = properties["Milestone"].value.objectValue as Milestone;
			node.newValue = properties["Set Value"].value.intValue;

			node.name = ToString();
			node.guid = guid;

			return node;
		}

	}
}