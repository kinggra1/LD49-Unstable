using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETools.Editor
{
	public class GraphNode : Attribute
	{
		public string nodeName;

		public GraphNode(string nodeName)
		{
			this.nodeName = nodeName;
		}
	}
}