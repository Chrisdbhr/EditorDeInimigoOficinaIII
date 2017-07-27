using UnityEngine;
using UnityEngine.UI;

public class LoadedCharacterButton : MonoBehaviour
{

	public CharacterEntry CharacterHere;

	Text _nameText;


	void Start() {
		_nameText = GetComponentInChildren<Text>();

	}

	public void SelectThis() {
		EditorManager._.SetSelectedCharToLoad(CharacterHere);

	}

	public void SetValues(CharacterEntry characterHere) {
		if (_nameText == null) {
			_nameText = GetComponentInChildren<Text>();
		}
		// assign variable
		CharacterHere = characterHere;

		// set the shown text to his name
		_nameText.text = CharacterHere.CharacterName;

	}



}
