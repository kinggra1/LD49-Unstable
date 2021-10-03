using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ETools.Dialogue
{
	public class CustomScriptNode : DialogueNode
	{
		public string customScriptClassName;

		public Type CustomScriptCall { get { return Assembly.GetExecutingAssembly().GetType(customScriptClassName); } }

		public override string ToString()
		{
			return "Custom Script Node - " + customScriptClassName;
		}
	}
}