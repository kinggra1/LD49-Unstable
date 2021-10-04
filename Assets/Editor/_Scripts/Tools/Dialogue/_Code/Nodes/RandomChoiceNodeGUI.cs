using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using ETools.Dialogue;

namespace ETools.Editor.Dialogue
{
	[GraphNode("Random Choice Node")]
	public class RandomChoiceNodeGUI : Node
	{
		public static RandomChoiceNodeGUI New(ConversationNodeGraph parent)
		{
			RandomChoiceNodeGUI node = RandomChoiceNodeGUI.CreateInstance<RandomChoiceNodeGUI>();

			node.parentGraph = parent;
			node.nodeName = "Random Choice";
			node.tooltip = "Randomly select the next node.";

			//Properties
			node.NewProperty("Weight 1", ValueType.Int, true, true);
			node.NewProperty("Weight 2", ValueType.Int, true, true);
			node.NewProperty("Weight 3", ValueType.Int, true, true);
			node.NewProperty("Weight 4", ValueType.Int, true, true);

			node.width = 300;

			node.isOutput = false;

			return node;
		}

		public override void DrawNodePropertyView(GUISkin guiSkin)
		{
			base.DrawNodePropertyView(guiSkin);

			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Add New Choice"))
			{
				NewProperty("Weight " + properties.Count(), ValueType.Int, true, true);
			}
			if (GUILayout.Button("Remove Last Choice"))
			{
				RemoveProperty("Weight " + (properties.Count() - 1));
			}
			GUILayout.EndHorizontal();
		}

		public RandomChoiceNode ToGameNode()
		{
			RandomChoiceNode node = CreateInstance<RandomChoiceNode>();

			List<int> weights = new List<int>();

			foreach (Property property in properties.Values)
			{
				weights.Add(property.value.intValue);
			}

			node.randomWeighting = weights.ToArray();
			node.nextNodes = new DialogueNode[weights.Count];

			node.name = ToString();
			node.guid = guid;

			return node;
		}
	}
}