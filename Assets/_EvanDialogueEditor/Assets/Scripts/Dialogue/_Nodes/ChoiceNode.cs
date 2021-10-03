using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETools.Dialogue
{
	public class ChoiceNode : DialogueNode
	{
		public List<Choice> choices = new List<Choice>();

		public override string ToString()
		{
			string sum = name + " - ";
			foreach (Choice choice in choices)
			{
				sum += choice.text + ",";
			}
			return sum.TrimEnd(',');
		}
	}
}