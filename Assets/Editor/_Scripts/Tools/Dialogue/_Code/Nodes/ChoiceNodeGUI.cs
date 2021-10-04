using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
using ETools.Dialogue;

namespace ETools.Editor.Dialogue
{
	[GraphNode("Choice Node")]
	public class ChoiceNodeGUI : Node
	{
		public static ChoiceNodeGUI New(ConversationNodeGraph parent)
		{
			ChoiceNodeGUI node = ChoiceNodeGUI.CreateInstance<ChoiceNodeGUI>();

			node.parentGraph = parent;
			node.nodeName = "Choice";
			node.width = 300;
			node.isOutput = false;
			node.tooltip = "Give the player a choice as to what to say next.";

			//Properties
			node.NewProperty("Option 1", ValueType.String, true, true);
			node.NewProperty("Option 2", ValueType.String, true, true);
			node.NewProperty("Option 3", ValueType.String, true, true);
			node.NewProperty("Option 4", ValueType.String, true, true);

			return node;
		}

		public override void DrawNodePropertyView(GUISkin skin)
		{
			base.DrawNodePropertyView(skin);

			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Add New Choice"))
			{
				NewProperty("Option " + properties.Count(), ValueType.String, true, true);
			}
			if (GUILayout.Button("Remove Last Choice"))
			{
				RemoveProperty("Option " + (properties.Count() - 1));
			}
			GUILayout.EndHorizontal();
		}

		public ChoiceNode ToGameData()
		{
			ChoiceNode node = CreateInstance<ChoiceNode>();

			foreach (Property property in properties.Values)
			{
				if (property.value.stringValue == "") continue;
				Choice choice = Choice.CreateInstance<Choice>();
				choice.text = property.value.stringValue;
				choice.name = "Choice - " + choice.text;
				node.choices.Add(choice);

			}

			node.name = node.name = this.nodeName + "(" + this.guid + ")";
			return node;
		}



	}
}