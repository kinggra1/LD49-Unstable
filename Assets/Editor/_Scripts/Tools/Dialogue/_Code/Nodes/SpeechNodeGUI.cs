using ETools.Dialogue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETools.Editor.Dialogue
{
	[GraphNode("Speech Node")]
	public class SpeechNodeGUI : Node
	{

		public static SpeechNodeGUI New(ConversationNodeGraph parent)
		{
			SpeechNodeGUI node = SpeechNodeGUI.CreateInstance<SpeechNodeGUI>();

			node.parentGraph = parent;

			//Define node name and type
			node.nodeName = "Speech";
			node.isOutput = true;
			node.tooltip = "A line spoken by one of the Speakers.";
			node.width = 300;

			//Define the properties
			node.NewProperty("Speaker", ValueType.Speaker, true);
			Property prop = node.NewProperty("Text", ValueType.GameString, true);
			prop.nodeRect.height = 50;

			return node;
		}

		public SpeechNode ToGameData()
		{
			SpeechNode node = CreateInstance<SpeechNode>();

			node.text = properties["Text"].value.gameStringValue;
			node.speaker = properties["Speaker"].value.speakerValue;

			node.name = ToString();
			node.guid = guid;
			return node;
		}

	}
}