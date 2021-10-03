using ETools.Strings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETools.Dialogue
{
	public class SpeechNode : DialogueNode
	{
		public Speaker speaker;
		public GameString text;

		public override string ToString()
		{
			return "Speech Node - " + speaker?.characterName + " - " + text;
		}
	}
}