using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLimbs : MonoBehaviour
{

	public CharacterEntry Attributes;

	[Header("Sprites")]
	public SpriteRenderer Head;
	public SpriteRenderer Body;
	public SpriteRenderer Hand_L;
	public SpriteRenderer Hand_R;
	public SpriteRenderer Feet_L;
	public SpriteRenderer Feet_R;

	void Start() {
		LoadSprites();
	}

	void LoadSprites() {

		Head.sprite = EditorManager.LoadCharacterSpriteByPath(GameManager._.folders.list[Attributes.IndexRacaSelecionada].Cabecas[Attributes.CabecasIndex], 0.25f);
		Body.sprite = EditorManager.LoadCharacterSpriteByPath(GameManager._.folders.list[Attributes.IndexRacaSelecionada].Corpos[Attributes.CorposIndex], 0.5f);
		Hand_L.sprite = EditorManager.LoadCharacterSpriteByPath(GameManager._.folders.list[Attributes.IndexRacaSelecionada].Maos[Attributes.MaosIndex], 0.5f);
		Hand_R.sprite = EditorManager.LoadCharacterSpriteByPath(GameManager._.folders.list[Attributes.IndexRacaSelecionada].Maos[Attributes.MaosIndex], 0.5f);
		Feet_L.sprite = EditorManager.LoadCharacterSpriteByPath(GameManager._.folders.list[Attributes.IndexRacaSelecionada].Botas[Attributes.BotasIndex], 0.5f);
		Feet_R.sprite = EditorManager.LoadCharacterSpriteByPath(GameManager._.folders.list[Attributes.IndexRacaSelecionada].Botas[Attributes.BotasIndex], 0.5f);

	}
}
