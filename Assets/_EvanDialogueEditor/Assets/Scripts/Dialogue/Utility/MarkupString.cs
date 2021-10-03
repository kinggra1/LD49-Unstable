using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace ETools.Dialogue.Utility
{
	//	A string container that can be parsed and return substrings while still respecting the original markup

	public class MarkupString
	{
		#region Variables

		//	Markup is surrounded by pointy braces
		private const string markupRegex = "</?[^>]*>";
		//	Text events are surrounded by pounds
		private const string textEventRegex = "[#][^#]*[#]";

		public class Markup
		{
			public int index;
			public string markupString;

			public Markup(int index, string markup)
			{
				this.index = index;
				this.markupString = markup;
			}

			public int Length { get { return markupString.Length; } }
		}

		public class TextEvent
		{
			public int index;
			public string eventString;
			public string[] args;

			public TextEvent(int index, string eventString)
			{
				this.index = index;
				eventString = eventString.Trim('#');
				string[] split = eventString.Split(',');

				this.eventString = split[0];
				this.args = split.Skip(1).ToArray();
			}
		}

		public string baseString { get; private set; }
		public string plainString { get; private set; }
		public string displayString { get; private set; }
		public List<Markup> markups = new List<Markup>();
		public List<TextEvent> textEvents = new List<TextEvent>();

		#endregion

		#region Constructors

		public MarkupString(string baseString)
		{
			this.baseString = baseString;
			string baseMinusMarkup = Regex.Replace(baseString, markupRegex, "");
			string baseMinusTextEvents = Regex.Replace(baseString, textEventRegex, "");
			this.displayString = baseMinusTextEvents;
			this.plainString = Regex.Replace(this.displayString, markupRegex, "");

			//	Find all matches in the original string for each regex
			MatchCollection markupMatches = Regex.Matches(baseMinusTextEvents, markupRegex);
			MatchCollection textEventMatches = Regex.Matches(baseMinusMarkup, textEventRegex);

			//	Bad chars keeps track of how many 'non-real' characters have been included in the string (non-real characters are characters that are part of markup or an event and wouldn't render)
			int badChars = 0;
			foreach(Match match in markupMatches)
			{
				Markup markup = new Markup(match.Index - badChars, match.Value);
				markups.Add(markup);
				badChars += match.Value.Length;
			}
			badChars = 0;
			foreach(Match match in textEventMatches)
			{
				TextEvent tev = new TextEvent(match.Index - badChars, match.Value);
				textEvents.Add(tev);
				badChars += match.Value.Length;
			}

		}


		#endregion

		#region Public Methods

		/// <summary>
		/// Take a substring starting at index "startIndex" and continuing forward "length"
		/// </summary>
		/// <param name="startIndex">The index of the first character to be included in the substring</param>
		/// <param name="length">How many characters are to be included in the substring</param>
		public string Substring(int startIndex, int length)
		{
			if (startIndex + length > plainString.Length)
				return Substring(startIndex, plainString.Length - startIndex);
			string sub = plainString.Substring(startIndex, length);

			int markupCharacters = 0;

			foreach(Markup markup in markups)
			{
				if (markup.index < startIndex)
					sub = sub.Insert(markupCharacters, markup.markupString);
				else if (markup.index < startIndex + length)
					sub = sub.Insert(markup.index - startIndex + markupCharacters, markup.markupString);
				else
					sub += markup.markupString;
				markupCharacters += markup.Length;
			}
			return sub;
		}

		/// <summary>
		/// Query a single event at index
		/// </summary>
		/// <param name="index">The index to check for an event</param>
		/// <returns>The event at that index, or null if none exist</returns>
		public TextEvent QueryEvent(int index)
		{
			foreach(TextEvent tev in textEvents)
			{
				if (tev.index == index)
					return tev;
			}
			return null;
		}

		/// <summary>
		/// Query any number of events at index
		/// </summary>
		/// <param name="index">The index to check for events</param>
		/// <returns>A collection of all events at that index</returns>
		public IEnumerable<TextEvent> QueryEvents(int index)
		{
			List<TextEvent> events = new List<TextEvent>();
			foreach(TextEvent tev in textEvents)
			{
				if (tev.index == index)
					events.Add(tev);
			}
			return events;
		}

		/// <summary>
		/// Inserts lines into the text based on the size of a uiText
		/// </summary>
		/// <param name="uiText">The Text component rendering text to the screen</param>
		/// <param name="width">The width at which new lines should be formed and line-breaks inserted</param>
		public void InsertNewLines(Text uiText, float width)
		{
			TextGenerator gen = uiText.cachedTextGenerator;

			MatchCollection matches = Regex.Matches(plainString, @"[\s-]");

			StringBuilder sb = new StringBuilder(plainString);

			int lastNewLineIndex = 0;
			int previousMatchIndex = 0;
			foreach(Match match in matches)
			{
				string textSnippet = plainString.Substring(lastNewLineIndex, match.Index - lastNewLineIndex);
				float textWidth = gen.GetPreferredWidth(textSnippet, uiText.GetGenerationSettings(uiText.GetComponent<RectTransform>().rect.size));
				if(textWidth > width)
				{
					sb[previousMatchIndex] = '\n';
					lastNewLineIndex = previousMatchIndex;
				}
				previousMatchIndex = match.Index;
			}
			plainString = sb.ToString();
		}

		#endregion
	
	}
}