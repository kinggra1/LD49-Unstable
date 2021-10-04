using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using ETools.Dialogue;
using ETools.Utilities;
using ETools.Strings;

namespace ETools.Editor.Dialogue
{

	[System.Serializable]
	public class Value
	{
		public ValueType type;
		public string stringValue = "";
		public float floatValue = 0f;
		public int intValue = 0;
		public Speaker speakerValue = null;
		public UnityEngine.Object objectValue;
		public GameString gameStringValue;
		public bool boolValue;
		private Type _objectValueType;

		public Type ObjectValueType
		{
			get
			{
				if (_objectValueType == null)
					_objectValueType = SystemsUtility.TypeFromString(objectValueTypeName);
				return _objectValueType;
			}
		}
		public string objectValueTypeName;

		public Value() { }

		public Value(ValueType t)
		{
			type = t;
		}

		public string DisplayValue()
		{
			switch (type)
			{
				case ValueType.Float:
					return floatValue.ToString();

				case ValueType.String:
					return stringValue;

				case ValueType.Int:
					return intValue.ToString();
			}
			return "!ERROR";
		}

	}

	public enum ValueType { Int, Float, String, Bool, Object, Speaker, GameString, None };
}
		