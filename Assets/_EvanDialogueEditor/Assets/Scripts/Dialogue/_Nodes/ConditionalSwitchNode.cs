using ETools;
using ETools.Conditionals;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETools.Dialogue
{
	public class ConditionalSwitchNode : DialogueNode
	{
		public List<Conditional> conditionals = new List<Conditional>();
		public List<DialogueNode> nextNodes = new List<DialogueNode>();
		public DialogueNode defaultNextNode;
		public override string ToString()
		{
			string sum = "";
			foreach (Conditional cond in conditionals)
			{
				sum += cond?.name;
			}

			return sum.TrimEnd(',');
		}
	}
}