using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETools.Conditionals
{
	[CreateAssetMenu(fileName = "New Conditional Group", menuName = "Gameplay/Conditional Group", order = 15)]
	public class ConditionalGroup : Conditional, IConditional
	{
		[SerializeField]
		public Conditional firstCondition;
		public List<Conditional> addConditions = new List<Conditional>();
		public List<ConditionalOperator> operators = new List<ConditionalOperator>();

		/// <summary>
		/// Evaluates the conditional group and returns whether it is true or false
		/// </summary>
		/// <returns>The result of the evaluation</returns>
		public override bool Evaluate()
		{
			if (firstCondition == null)
				return true;
			bool result = firstCondition.Evaluate();
			for (int i = 0; i < addConditions.Count; i++)
			{
				Conditional currentCondition = addConditions[i];
				ConditionalOperator op = operators[i];
				bool currentResult = currentCondition.Evaluate();

				switch (op)
				{
					case ConditionalOperator.And:
						result = result && currentResult;
						break;

					case ConditionalOperator.Or:
						result = result || currentResult;
						break;

					case ConditionalOperator.Equals:
						result = result == currentResult;
						break;

					case ConditionalOperator.Not:
						result = result != currentResult;
						break;
				}
			}
			return result;
		}

		/// <summary>
		/// Outputs to a nice, readable string
		/// </summary>
		/// <param name="richText">Should the output string use Rich Text</param>
		public override string ToReadabilityString(bool richText = true)
		{
			string readabilityString = "";
			if (richText)
				readabilityString += "----------------------------------\n";

			if (firstCondition)
			{
				readabilityString += (firstCondition.ToReadabilityString());

				for (int i = 0; i < addConditions.Count; i++)
				{
					Conditional cond = addConditions[i];
					ConditionalOperator op = operators[i];
					if (richText)
					{
						readabilityString += "\n" + ("<i>" + op.ToString() + "</i>");
						readabilityString += "\n" + (cond.ToReadabilityString());
					}
					else
					{
						readabilityString += "\n" + (op.ToString());
						readabilityString += "\n" + (cond.ToReadabilityString());
					}
				}
			}
			else
			{
				readabilityString = "No conditions specified in Condition Group!";
			}
			if (richText)
				readabilityString += "\n----------------------------------";
			return readabilityString;
		}

		/// <summary>
		/// Adds a conditional to the group
		/// </summary>
		/// <param name="op">The operator to apply to the conditional</param>
		/// <param name="newCondition">The new conditional to be added</param>
		public void AddCondition(ConditionalOperator op, Conditional newCondition)
		{
			if (newCondition == null)
				Debug.LogError("Cannot supply null Conditions!");
			addConditions.Add(newCondition);
			operators.Add(op);
		}
	}
}