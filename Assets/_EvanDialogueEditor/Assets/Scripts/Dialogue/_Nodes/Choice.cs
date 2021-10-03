using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETools.Dialogue
{
	public class Choice : BObject
	{
		public string text;
		public DialogueNode nextNode;
	}
}