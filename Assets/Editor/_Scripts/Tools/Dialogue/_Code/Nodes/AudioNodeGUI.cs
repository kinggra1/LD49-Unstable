using ETools.Dialogue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETools.Editor.Dialogue
{
	[GraphNode("Audio Node")]
	public class AudioNodeGUI : Node
	{

		public static AudioNodeGUI New(ConversationNodeGraph parent)
		{
			AudioNodeGUI node = CreateInstance<AudioNodeGUI>();

			node.parentGraph = parent;

			//Define node name and type
			node.nodeName = "Audio";
			node.isOutput = true;
			node.tooltip = "An audio clip to be played.";
			node.width = 300;

			//Define the properties
			node.NewProperty("Audio Clip", typeof(AudioClip), true);
			node.NewProperty("Is Music?", ValueType.Bool, true);
			node.NewProperty("Pause Conversation?", ValueType.Bool, true);

			return node;
		}

		public AudioNode ToGameData()
		{
			AudioNode node = CreateInstance<AudioNode>();

			node.audioClip = properties["Audio Clip"].value.objectValue as AudioClip;
			node.isMusic = properties["Is Music?"].value.boolValue;
			node.pauseConversation = properties["Pause Conversation?"].value.boolValue;

			node.name = ToString();
			node.guid = guid;
			return node;
		}

	}
}