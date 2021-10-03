using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;
using ETools.Strings;

namespace ETools.Editor.Dialogue
{
	[System.Serializable]
	public class Property : Connectable
	{
		public string propertyName = "";
		public ValueType Type { get { return value.type; } }
		public Value value;
		public bool HasOutput
		{
			get
			{
				return outgoing != null;
			}
		}
		public Vector2 propertyPosition = new Vector2();
		public Node parent;

		private Property() { }

		private Property(string displayName, ValueType valueType, bool exposable = false)
		{
			value.type = valueType;
			propertyName = displayName;
		}

		public static Property New(Node node, string displayName, ValueType valueType, bool exposable = false, bool usesOutput = false)
		{
			Property p = ScriptableObject.CreateInstance<Property>();
			p.name = node.nodeName + ".property - " + displayName;
			p.parent = node;
			p.value = new Value(valueType);
			p.propertyName = displayName;
			p.isOutput = usesOutput;

			p.nodeRect = new Rect(0, 0, 0, 20);

			return p;
		}

		public static Property New(Node node, string displayName, Type type, bool exposable = false, bool usesOutput = false)
		{
			Property p = ScriptableObject.CreateInstance<Property>();
			p.name = node.nodeName + ".property - " + displayName;
			p.parent = node;
			p.value = new Value();
			p.value.type = ValueType.Object;
			p.value.objectValueTypeName = type.FullName;
			p.propertyName = displayName;
			p.isOutput = usesOutput;

			p.nodeRect = new Rect(0, 0, 0, 20);

			return p;
		}

		public void DisplayEditableValue()
		{
			switch (Type)
			{
				case ValueType.Float:
					value.floatValue = EditorGUILayout.FloatField(propertyName + " (" + Type.ToString() + ") ", value.floatValue);
					break;

				case ValueType.String:
					GUILayout.Label(propertyName + " (" + Type.ToString() + ") ");
					EditorStyles.textField.wordWrap = true;
					value.stringValue = EditorGUILayout.TextArea(value.stringValue);
					break;

				case ValueType.Int:
					value.intValue = EditorGUILayout.IntField(propertyName + " (" + Type.ToString() + ") ", value.intValue);
					break;

				case ValueType.Speaker:
					GUILayout.Label(propertyName + " (" + Type.ToString() + ") ");
					if(parent.parentGraph.speakers.Count == 0)
					{
						EditorGUILayout.Popup(0, new string[] { "<NO SPEAKERS>" });
						break;
					}
					int priorIndex = parent.parentGraph.speakers.IndexOf(value.speakerValue);
					if (priorIndex == -1) priorIndex = 0;
					string[] options = (from s in parent.parentGraph.speakers where s != null select s.characterName).ToArray();
					int index = EditorGUILayout.Popup(priorIndex, options);
					value.speakerValue = parent.parentGraph.speakers[index];
					break;

				case ValueType.Bool:
					value.boolValue = EditorGUILayout.Toggle(propertyName + " (" + Type.ToString() + ") ", value.boolValue);
					break;

				case ValueType.Object:
					GUILayout.Label(propertyName + " (" + Type.ToString() + ") ");
					value.objectValue = EditorGUILayout.ObjectField(value.objectValue, value.ObjectValueType, false);
					break;

				case ValueType.GameString:
					GUILayout.Label(propertyName);
					value.gameStringValue = (GameString)EditorGUILayout.ObjectField(value.gameStringValue, typeof(GameString), false);
					if (value.gameStringValue != null)
						value.gameStringValue.EnglishString = EditorGUILayout.TextArea(value.gameStringValue.EnglishString);
					break;

				case ValueType.None:
					break;

				default:
					Debug.Log("ERROR " + Type);
					break;
			}
		}

		public override string ToString()
		{
			switch (Type)
			{
				case ValueType.Float:
					return value.floatValue.ToString();

				case ValueType.Int:
					return value.intValue.ToString();

				case ValueType.None:
					return "None";

				case ValueType.Speaker:
					return value.speakerValue.characterName;

				case ValueType.String:
					return value.stringValue;

				case ValueType.Object:
					if (value.objectValue == null)
						return "null";
					else
						return value.objectValue.name;
				case ValueType.Bool:
					return value.boolValue.ToString();
				case ValueType.GameString:
					return value.gameStringValue.ToString();
				default:
					return "ERROR";
			}
		}
	}
}