using ETools.Conditionals;
using ETools.Dialogue.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ETools.Dialogue {
	[RequireComponent(typeof(AudioSource))]
	public class DialogueSystem : Singleton<DialogueSystem> {
		#region Variables

		//	Public
		[Header("References")]
		public GameObject rootObj;
		public RawImage speakerPortrait;
		public Text dialogueText;
		public Button[] buttons;
		public Button nextButton;
		public TextMeshProUGUI speakerName;
		public AudioClip defaultBlip;
		public AudioSource audioSource;

		[Header("Dialogue Options")]
		public float defaultTextSpeed = 0.05f;
		public float commaMultiplier = 2f;
		public float periodMultiplier = 4f;

		//	Private
		private Conversation _conversation;
		private DialogueNode _currentNode;
		private float _currentTextSpeed = 0.05f;
		private bool _printing = false;
		private IEnumerator _textRevealCoroutine;


		#endregion

		#region Public Methods

		public void StartConversation(Conversation conversation, bool playAutomatically = true) {
			_conversation = conversation;
			_currentNode = conversation.firstNode;
			EnableUI();
			FreezePlayer();
			if (playAutomatically)
				PlayNode();
		}

		public void EndConversation() {
			_conversation = null;
			_currentNode = null;
			StopCoroutine(_textRevealCoroutine);
			_textRevealCoroutine = null;
			DisableUI();
			UnFreezePlayer();
		}

		public void OnAdvance(int choice = -1)//(InputAction.CallbackContext context)
		{
			if (_conversation == null)
				return;
			//if (!context.performed)
			//	return;
			if (_printing) {
				StopCoroutine(_textRevealCoroutine);
				ShowFinishedSpeechText(_currentNode as SpeechNode);
			}
			else {
				PlayNextNode(choice);
			}
		}

		#endregion

		#region Mono Methods

		void Start() {
			rootObj.SetActive(false);
		}

		void Update() {

		}

		private void OnEnable() {
			_currentTextSpeed = defaultTextSpeed;
			if (audioSource == null)
				audioSource = GetComponent<AudioSource>();
		}

		#endregion

		#region Private Methods

		private void EnableUI() {
			rootObj.SetActive(true);
			//Time.SetGameTimeScale(0f);
		}

		private void DisableUI() {
			rootObj.SetActive(false);
			_conversation = null;
			//Time.SetGameTimeScale(1f);
		}

		private void PlayNextNode(int choice = -1) {
			AdvanceNode(choice);
			PlayNode();

		}

		private void PlayNode() {
			if (!_currentNode)
				return;
			switch (_currentNode) {
				case SpeechNode speechNode:
					dialogueText.gameObject.SetActive(true);
					nextButton.gameObject.SetActive(true);
					speakerName.text = speechNode.speaker.characterName;
					speakerPortrait.gameObject.SetActive(true);
					speakerName.gameObject.SetActive(true);
					speakerPortrait.texture = speechNode.speaker.characterPortrait;
					foreach (var button in buttons)
						button.gameObject.SetActive(false);
					_textRevealCoroutine = RevealSpeechText(speechNode);
					StartCoroutine(_textRevealCoroutine);
					break;

				case ChoiceNode choiceNode:
					nextButton.gameObject.SetActive(false);
					dialogueText.gameObject.SetActive(false);
					speakerPortrait.gameObject.SetActive(false);
					speakerName.gameObject.SetActive(false);
					for (int i = 0; i < choiceNode.choices.Count; i++) {
						Button button = buttons[i];
						button.gameObject.SetActive(true);
						button.GetComponentInChildren<Text>().text = choiceNode.choices[i].text;
					}
					break;

				case MilestoneNode milestoneNode:
					milestoneNode.milestone.SetValue(milestoneNode.newValue);
					PlayNextNode(-1);
					break;

				case RandomChoiceNode randomChoiceNode:
					PlayNextNode(randomChoiceNode.GetChoiceIndex());
					break;

				case ConditionalSwitchNode condSwitchNode:
					PlayNextNode();
					break;

				case CustomScriptNode customScriptNode:
					Type scriptType = customScriptNode.CustomScriptCall;
					MethodInfo doAction = scriptType.GetMethod("DoAction");
					doAction.Invoke(Activator.CreateInstance(scriptType), new object[] { });
					PlayNextNode();
					break;

				default:
					Debug.LogError("Node type '" + _currentNode.GetType().ToString() + "' not yet supported!");
					break;
			}
		}

		private void AdvanceNode(int choice = -1) {
			if (_currentNode == null)
				_currentNode = _conversation.firstNode;
			else
				_currentNode = GetNextNode(_currentNode, choice);

			if (_currentNode == null) {

				EndConversation();
				return;
			}
		}

		private DialogueNode GetNextNode(DialogueNode node, int choice = -1) {
			switch (node) {
				case SpeechNode speechNode:
					return _currentNode.nextNode;

				case ChoiceNode choiceNode:
					return choiceNode.choices[choice].nextNode;

				case MilestoneNode milestoneNode:
					return milestoneNode.nextNode;

				case RandomChoiceNode randomChoiceNode:
					return randomChoiceNode.nextNodes[choice];

				case CustomScriptNode customScriptNode:
					return customScriptNode.nextNode;

				case ConditionalSwitchNode condSwitchNode:
					for (int i = 0; i < condSwitchNode.conditionals.Count; i++) {
						Conditional condition = condSwitchNode.conditionals[i];
						if (condition.Evaluate())
							return condSwitchNode.nextNodes[i];
					}
					return condSwitchNode.defaultNextNode;


				default:
					Debug.LogError("Node type '" + node.GetType().ToString() + "' not yet supported!");
					return null;
			}
		}

		private IEnumerator RevealSpeechText(SpeechNode speechNode) {
			int i = 1;
			string textSoFar;

			string speechText = speechNode.text.ToString();
			MarkupString ms = new MarkupString(speechText);
			ms.InsertNewLines(dialogueText, dialogueText.GetComponent<RectTransform>().rect.width);

			while (i <= ms.plainString.Length) {
				if (!_conversation)
					break;
				_printing = true;
				textSoFar = ms.Substring(0, i);
				char newChar = ms.plainString[i - 1];

				ProcessTextEvent(ms.QueryEvent(i));

				if (!char.IsSeparator(newChar) && newChar != '\n') {
					PlayBlip();
				}
				dialogueText.text = textSoFar;
				i++;
				yield return new WaitForSeconds(_currentTextSpeed * PuncVal(newChar));
			}
			ShowFinishedSpeechText(speechNode);
		}

		private void ShowFinishedSpeechText(SpeechNode speechNode) {
			MarkupString ms = new MarkupString(speechNode.text.ToString());
			dialogueText.text = ms.Substring(0, ms.plainString.Length);
			_printing = false;
		}

		private void ProcessTextEvent(MarkupString.TextEvent tev) {
			if (tev == null)
				return;
			Debug.Log("Text event '" + tev.eventString + "' at index " + tev.index);
		}

		private void PlayBlip() {
			audioSource.PlayOneShot(defaultBlip);
		}

		private float PuncVal(char c) {
			if (!char.IsPunctuation(c))
				return 1;
			else if (c == ',')
				return commaMultiplier;
			else if (c == '.' || c == '!' || c == '?')
				return periodMultiplier;
			else return 1;
		}

		private void FreezePlayer() {
			CharacterController.Instance.FreezePlayer();
		}

		private void UnFreezePlayer() {
			CharacterController.Instance.UnfreezePlayer();
		}

		#endregion
	}
}