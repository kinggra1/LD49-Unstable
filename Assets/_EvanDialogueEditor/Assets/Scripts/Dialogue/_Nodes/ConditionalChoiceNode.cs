using ETools.Conditionals;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETools.Dialogue
{
	public class ConditionalChoiceNode : DialogueNode
	{
		public List<Choice> choices = new List<Choice>();
		public List<Conditional> conditionals = new List<Conditional>();
		public DialogueNode defaultNextNode;

		public override string ToString()
		{
			string sum = "";
			foreach (Choice choice in choices)
			{
				sum += choice.text + ",";
			}
			foreach (Conditional cond in conditionals)
			{
				sum += cond?.name;
			}

			return sum.TrimEnd(',');
		}
	}
}