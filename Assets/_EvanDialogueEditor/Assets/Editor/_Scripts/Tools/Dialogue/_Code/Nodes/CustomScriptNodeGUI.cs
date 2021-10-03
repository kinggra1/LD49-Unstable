using ETools.Dialogue;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ETools.Editor.Dialogue 
{
	[GraphNode("Custom Script Node")]
	public class CustomScriptNodeGUI : Node
	{
		public static CustomScriptNodeGUI New(ConversationNodeGraph parent)
		{
			CustomScriptNodeGUI node = CustomScriptNodeGUI.CreateInstance<CustomScriptNodeGUI>();

			node.parentGraph = parent;

			//Define node name and type
			node.nodeName = "Custom Script";
			node.isOutput = true;
			node.tooltip = "A custom script to be run at this point in the conversation.";
			node.width = 300;

			//Define the properties
			node.NewProperty("Script", typeof(MonoScript), true);

			return node;
		}

		public CustomScriptNode ToGameData()
		{
			CustomScriptNode node = CreateInstance<CustomScriptNode>();

			MonoScript script = properties["Script"].value.objectValue as MonoScript;
			if (script == null)
			{
				Debug.LogError("Can't compile custom script with null MonoBehavior in node \"" + node.name + "\"");
				return node;
			}
			if (!script.GetClass().GetInterfaces().Contains(typeof(IDialogueCustomScript)))
			{
				Debug.LogError("Can't compile custom script that doesn't implement IDialogueCustomScript in node \"" + node.name + "\"");
				return node;
			}

			node.customScriptClassName = script.GetClass().FullName;

			node.name = ToString();
			node.guid = guid;

			return node;
		}
	}
}