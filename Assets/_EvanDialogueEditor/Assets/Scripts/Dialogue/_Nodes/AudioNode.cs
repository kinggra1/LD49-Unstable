using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETools.Dialogue
{
	public class AudioNode : DialogueNode
	{
		public AudioClip audioClip;
		public bool isMusic = false;
		public bool pauseConversation = false;

		public override string ToString()
		{
			return "Audio Node - " + audioClip?.name;
		}
	}
}
