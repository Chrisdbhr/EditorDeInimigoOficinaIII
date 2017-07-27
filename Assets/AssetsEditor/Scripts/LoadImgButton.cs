using UnityEngine;
using System.Collections;

public enum CharPartToLoadImg { cabeca, corpo, maos, botas }

public class LoadImgButton : MonoBehaviour
{
	public CharPartToLoadImg partImg;

	public void LoadButton() {
		StartCoroutine(EditorManager._.OpenFileExplorerToLoadCharacter(partImg));

	}
}
