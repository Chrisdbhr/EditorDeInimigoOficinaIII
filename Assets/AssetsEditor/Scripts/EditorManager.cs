using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.UI;
using System.Collections;
using SimpleFileBrowser;

/// <summary>
/// All game attributes changed by buttons and saved.
/// </summary>
public enum GameAttribute { vida, mana, ataque, defesa, velocidade, escudo, classe, pontos,
rateOfFire, hitDelay, minimumTimeOnAction, maxTimeFollowingPlayer, maxTimeOnStand, maxTimeOnFlee, fleePriority, atkPriority}

//- cadencia de tiro
//- velocidade do golpe/projetil
//- tempo maximo na acao de ATAQUE
//- tempo maximo na acao de PARADO
//- tempo maximo na acao de FUGINDO
//- pode fugir(um checkbox)

/// <summary>
/// EditorManager is a singleton.
/// To avoid having to manually link an instance to every class that needs it, it has a static property called
/// instance, so other objects that need to access it can just call:
///		EditorManager._.FunctionToCall();
/// </summary>
public class EditorManager : MonoBehaviour
{

	[Header("Debug: activate and desactivate")]
	public GameObject[] activateOnStart;
	public GameObject[] desactiveOnStart;

	[Space]

	public string pathToLoad;
	public int MaxAttributesValues = 100;

	[Space]

	[Header("Active Character")]
	public SpriteRenderer Head;
	public SpriteRenderer Body;
	public SpriteRenderer Hand_L;
	public SpriteRenderer Hand_R;
	public SpriteRenderer Feet_L;
	public SpriteRenderer Feet_R;

	[Space]
	public Text RacaText;
	public Text CabecaText;
	public Text CorpoText;
	public Text MaosText;
	public Text BotasText;
	public Text ClasseText;
	[Space]
	public InputField NomeDigitado;
	[SerializeField]
	private Text TotalDePontosNosAtributos;
	[SerializeField]
	private Button SaveButton;

	[Header("Menus GameObjects.")]
	public GameObject _menuGroup;
	public GameObject _newCharacterGroup;
	public GameObject _characterGroup;
	public GameObject _loadGroup;
	public GameObject _erasePopupConfirm;


	[Header("Load Character.")]

	[SerializeField]
	Transform _contentToCreateLoadedChars;

	List<CharacterEntry> _listCharactersToLoad = new List<CharacterEntry>();
	List<string> _listCharactersPath = new List<string>();

	public Button ConfirmLoadCharButton;

	public Button EraseCharButton;

	public CharacterEntry SelectedChar;

	public GameObject PrefabLoadedCharItem;

	public Image SelectedChar_Head;
	public Image SelectedChar_Body;
	public Image SelectedChar_Feet_L;
	public Image SelectedChar_Feet_R;
	public Image SelectedChar_Hand_L;
	public Image SelectedChar_Hand_R;

	[SerializeField]
	Text _previewNameText;

	[SerializeField]
	Text _noCharsLoadedText;

	[SerializeField]
	Sprite _emptyImage;


	[Space]

	public CharacterEntry editingCharacter = new CharacterEntry();
	public CharacterDatabase characterDB;
	public FolderDatabase allFolders;
	public Text debugText;
	bool copyAppearValues = true; // se deve copiar os valores da aparencia para o editChar, deve ser FALSE quando carregar arquivo

	#region Singleton Part

	// s_Instance is used to cache the instance found in the scene so we don't have to look it up every time.
	private static EditorManager s_Instance = null;

	// This defines a static instance property that attempts to find the manager object in the scene and
	// returns it to the caller.
	public static EditorManager _
	{
		get {
			if (s_Instance == null) {
				// This is where the magic happens.
				//  FindObjectOfType(...) returns the first AManager object in the scene.
				s_Instance = FindObjectOfType(typeof(EditorManager)) as EditorManager;
			}

			// If it is still null, create a new instance
			if (s_Instance == null) {
				GameObject obj = new GameObject("AManager");
				s_Instance = obj.AddComponent(typeof(EditorManager)) as EditorManager;
				Debug.Log("Could not locate an AManager object. // AManager was Generated Automaticly.");
			}

			return s_Instance;
		}
	}

	// Ensure that the instance is destroyed when the game is stopped in the editor.
	void OnApplicationQuit() {
		s_Instance = null;
	}

	#endregion


	void Start() {
		// Set filters (optional)
		// It is sufficient to set the filters just once (instead of each time before showing the file browser dialog), 
		// if all the dialogs will be using the same filters
		FileBrowserPlugin.SetFilters(true, new FileBrowserPlugin.Filter("Images", ".png"), new FileBrowserPlugin.Filter("Text Files", ".json"));

		// Set default filter that is selected when the dialog is shown (optional)
		// Returns true if the default filter is set successfully
		// In this case, set Images filter as the default filter
		FileBrowserPlugin.SetDefaultFilter(".png");

		// Set excluded file extensions (optional) (by default, .lnk and .tmp extensions are excluded)
		// Note that when you use this function, .lnk and .tmp extensions will no longer be
		// excluded unless you explicitly add them as parameters to the function
		FileBrowserPlugin.SetExcludedExtensions(".lnk", ".tmp", ".zip", ".rar", ".exe", ".ini", ".txt");

		// Add a new quick link to the browser (optional) (returns true if quick link is added successfully)
		// It is sufficient to add a quick link just once
		// Icon: default (folder icon)
		// Name: Users
		// Path: C:\Users
		//FileBrowserPlugin.AddQuickLink(null, "Users", "C:\\Users");

		// Show a save file dialog 
		// onSuccess event: not registered (which means this dialog is pretty useless)
		// onCancel event: not registered
		// Save file/folder: file, Initial path: "C:\", Title: "Save As", submit button text: "Save"
		// FileBrowser.ShowSaveDialog( null, null, false, "C:\\", "Save As", "Save" );

		// Show a select folder dialog 
		// onSuccess event: print the selected folder's path
		// onCancel event: print "Canceled"
		// Load file/folder: folder, Initial path: default (Documents), Title: "Select Folder", submit button text: "Select"
		// FileBrowser.ShowLoadDialog( (path) => { Debug.Log( "Selected: " + path ); }, 
		//                                () => { Debug.Log( "Canceled" ); }, 
		//                                true, null, "Select Folder", "Select" );

	}

	void OnEnable() {
		if (desactiveOnStart.Length > 0) {
			foreach (var obj in desactiveOnStart) {
				if (obj != null)
					obj.SetActive(false);
			}
		}
		if (activateOnStart.Length > 0) {
			foreach (var obj in activateOnStart) {
				if (obj != null)
					obj.SetActive(true);
			}
		}

		allFolders.list.Clear();
		allFolders = GetAllSpritesInFolder();

		UpdateEditingCharacterValuesShow();
		UpdateAppearenceValues();
		debugText.text = Application.dataPath;
	}

	/// <summary>
	/// Retorna uma lista coms todos as pastas de todas as sprites disponiveis.
	/// </summary>
	/// <returns></returns>
	public static FolderDatabase GetAllSpritesInFolder() {
		FolderDatabase folders = new FolderDatabase();

		//Pega todas as classes
		string dir = GetCharacterPartsDir();

		foreach (var folderName in Directory.GetDirectories(dir)) {
			folders.list.Add(new FolderClass(folderName));
		}

		// Pegar todos os caminhos dos arquivos png de cada diretorio
		foreach (var folderClass in folders.list) {
			foreach (var imagemPath in Directory.GetFiles(folderClass.NomeDaClasse + "\\Head", "*.png")) {
				folderClass.Cabecas.Add(imagemPath);
			}
			foreach (var imagemPath in Directory.GetFiles(folderClass.NomeDaClasse + "\\Body", "*.png")) {
				folderClass.Corpos.Add(imagemPath);
			}
			foreach (var imagemPath in Directory.GetFiles(folderClass.NomeDaClasse + "\\Hand", "*.png")) {
				folderClass.Maos.Add(imagemPath);
			}
			foreach (var imagemPath in Directory.GetFiles(folderClass.NomeDaClasse + "\\Feet", "*.png")) {
				folderClass.Botas.Add(imagemPath);
			}
		}
		return folders;
	}

	/// <summary>
	/// Atualiza apenas as aparencias.
	/// </summary>
	void UpdateAppearenceValues() {

		if (copyAppearValues) {
			editingCharacter.CabecasIndex = allFolders.list[editingCharacter.IndexRacaSelecionada].CabecasIndex;
			editingCharacter.CorposIndex = allFolders.list[editingCharacter.IndexRacaSelecionada].CorposIndex;
			editingCharacter.MaosIndex = allFolders.list[editingCharacter.IndexRacaSelecionada].MaosIndex;
			editingCharacter.BotasIndex = allFolders.list[editingCharacter.IndexRacaSelecionada].BotasIndex;
		}
		else {
			Debug.Log("vai");
			NomeDigitado.text = editingCharacter.CharacterName;
			allFolders.list[editingCharacter.IndexRacaSelecionada].CabecasIndex = editingCharacter.CabecasIndex;
			allFolders.list[editingCharacter.IndexRacaSelecionada].CorposIndex = editingCharacter.CorposIndex;
			allFolders.list[editingCharacter.IndexRacaSelecionada].MaosIndex = editingCharacter.MaosIndex;
			allFolders.list[editingCharacter.IndexRacaSelecionada].BotasIndex = editingCharacter.BotasIndex;
		}

		// Exibicao de texto
		RacaText.text = allFolders.list[editingCharacter.IndexRacaSelecionada].NomeDaClasse.Split('\\').Last();
		CabecaText.text = (allFolders.list[editingCharacter.IndexRacaSelecionada].CabecasIndex + 1) + " / " + allFolders.list[editingCharacter.IndexRacaSelecionada].Cabecas.Count;
		CorpoText.text = (allFolders.list[editingCharacter.IndexRacaSelecionada].CorposIndex + 1) + " / " + allFolders.list[editingCharacter.IndexRacaSelecionada].Corpos.Count;
		MaosText.text = (allFolders.list[editingCharacter.IndexRacaSelecionada].MaosIndex + 1) + " / " + allFolders.list[editingCharacter.IndexRacaSelecionada].Maos.Count;
		BotasText.text = (allFolders.list[editingCharacter.IndexRacaSelecionada].BotasIndex + 1) + " / " + allFolders.list[editingCharacter.IndexRacaSelecionada].Botas.Count;

		Head.sprite = LoadCharacterSpriteByPath(allFolders.list[editingCharacter.IndexRacaSelecionada].Cabecas[editingCharacter.CabecasIndex], 0.25f);
		Body.sprite = LoadCharacterSpriteByPath(allFolders.list[editingCharacter.IndexRacaSelecionada].Corpos[editingCharacter.CorposIndex], 0.5f);
		Hand_L.sprite = LoadCharacterSpriteByPath(allFolders.list[editingCharacter.IndexRacaSelecionada].Maos[editingCharacter.MaosIndex], 0.5f);
		Hand_R.sprite = LoadCharacterSpriteByPath(allFolders.list[editingCharacter.IndexRacaSelecionada].Maos[editingCharacter.MaosIndex], 0.5f);
		Feet_L.sprite = LoadCharacterSpriteByPath(allFolders.list[editingCharacter.IndexRacaSelecionada].Botas[editingCharacter.BotasIndex], 0.5f);
		Feet_R.sprite = LoadCharacterSpriteByPath(allFolders.list[editingCharacter.IndexRacaSelecionada].Botas[editingCharacter.BotasIndex], 0.5f);

		// se carregou arquivo, entao define de volta para TRUE
		copyAppearValues = true;
	}

	public void UpdateEditingCharacterValuesShow() {

		// Definindo classe
		if (editingCharacter.Classe == 0) {
			ClasseText.text = "Distancia";
		}
		else {
			ClasseText.text = "Corpo a corpo";
		}

		editingCharacter.CharacterName = NomeDigitado.text;

		TotalDePontosNosAtributos.text = (editingCharacter.Vida + editingCharacter.Mana + editingCharacter.Ataque + editingCharacter.Defesa + editingCharacter.Velocidade + editingCharacter.Escudo).ToString();

		if (NomeDigitado.text != "" && NomeDigitado.text.Length > 2) {
			SaveButton.interactable = true;
		}
		else {
			SaveButton.interactable = false;
		}

	}

	public void MenuInicial() {
		_newCharacterGroup.SetActive(false);
		_characterGroup.SetActive(false);
		_loadGroup.SetActive(false);
		_erasePopupConfirm.SetActive(false);

		_menuGroup.SetActive(true);

	}

	public void MenuCriarPersonagem() {
		// Ativa os menus para edição
		_newCharacterGroup.SetActive(true);
		_characterGroup.SetActive(true);
		_menuGroup.SetActive(false);
	}

	public void RandomEverything() {
		RandomizarAparencia();
		RandomizarAtributos();
	}

	#region Manipulacao de arquivos

	/// <summary>
	/// Save the images paths, the values are already set.
	/// </summary>
	public void SaveCharacter() {

		editingCharacter.CharacterName = NomeDigitado.text;
		editingCharacter.RacaPath = allFolders.list[editingCharacter.IndexRacaSelecionada].NomeDaClasse.Split('\\').Last();
		editingCharacter.HeadImagePath = allFolders.list[editingCharacter.IndexRacaSelecionada].Cabecas[allFolders.list[editingCharacter.IndexRacaSelecionada].CabecasIndex].Replace(GetCharacterPartsDir(), "");
		editingCharacter.HandImagePath = allFolders.list[editingCharacter.IndexRacaSelecionada].Maos[allFolders.list[editingCharacter.IndexRacaSelecionada].MaosIndex].Replace(GetCharacterPartsDir(), "");
		editingCharacter.FeetImagePath = allFolders.list[editingCharacter.IndexRacaSelecionada].Botas[allFolders.list[editingCharacter.IndexRacaSelecionada].BotasIndex].Replace(GetCharacterPartsDir(), "");
		editingCharacter.BodyImagePath = allFolders.list[editingCharacter.IndexRacaSelecionada].Corpos[allFolders.list[editingCharacter.IndexRacaSelecionada].CorposIndex].Replace(GetCharacterPartsDir(), "");


		string json = JsonUtility.ToJson(editingCharacter);
		File.WriteAllText(GetCreatedCharactersDir() + "/Enemy_" + editingCharacter.CharacterName + ".json", json);

	}

	public IEnumerator OpenFileExplorerToLoadCharacter(CharPartToLoadImg partImg) {

		yield return StartCoroutine(ShowLoadDialogCoroutine());

		if (FileBrowserPlugin.Success) {

			switch (partImg) {
				case CharPartToLoadImg.cabeca:
					Head.sprite = LoadSpriteByPath(FileBrowserPlugin.Result, 0f);
					editingCharacter.HeadImagePath = allFolders.list[editingCharacter.IndexRacaSelecionada].Cabecas[allFolders.list[editingCharacter.IndexRacaSelecionada].CabecasIndex].Replace(GetCharacterPartsDir(), "");
					File.Copy(FileBrowserPlugin.Result, GetCharacterPartsDir() + "/" + editingCharacter.HeadImagePath, true);
					break;
				case CharPartToLoadImg.corpo:
					Body.sprite = LoadSpriteByPath(FileBrowserPlugin.Result, 0.5f);
					editingCharacter.BodyImagePath = allFolders.list[editingCharacter.IndexRacaSelecionada].Corpos[allFolders.list[editingCharacter.IndexRacaSelecionada].CorposIndex].Replace(GetCharacterPartsDir(), "");
					File.Copy(FileBrowserPlugin.Result, GetCharacterPartsDir() + "/" + editingCharacter.BodyImagePath, true);
					break;
				case CharPartToLoadImg.maos:
					Hand_L.sprite = LoadSpriteByPath(FileBrowserPlugin.Result, 0.5f);
					Hand_R.sprite = LoadSpriteByPath(FileBrowserPlugin.Result, 0.5f);
					editingCharacter.HandImagePath = allFolders.list[editingCharacter.IndexRacaSelecionada].Maos[allFolders.list[editingCharacter.IndexRacaSelecionada].MaosIndex].Replace(GetCharacterPartsDir(), "");
					File.Copy(FileBrowserPlugin.Result, GetCharacterPartsDir() + "/" + editingCharacter.HandImagePath, true);
					break;
				case CharPartToLoadImg.botas:
					Feet_L.sprite = LoadSpriteByPath(FileBrowserPlugin.Result, 0.5f);
					Feet_R.sprite = LoadSpriteByPath(FileBrowserPlugin.Result, 0.5f);
					editingCharacter.FeetImagePath = allFolders.list[editingCharacter.IndexRacaSelecionada].Botas[allFolders.list[editingCharacter.IndexRacaSelecionada].BotasIndex].Replace(GetCharacterPartsDir(), "");
					File.Copy(FileBrowserPlugin.Result, GetCharacterPartsDir() + "/" + editingCharacter.FeetImagePath, true);
					break;
			}

		}

	}

	IEnumerator ShowLoadDialogCoroutine() {
		// Show a load file dialog and wait for a response from user
		// Load file/folder: file, Initial path: default (Documents), Title: "Load File", submit button text: "Load"
		yield return FileBrowserPlugin.WaitForLoadDialog(false, null, "Escolha o arquivo de imagem", "Carregar");

		// Dialog is closed
		// Print whether a file is chosen (FileBrowser.Success)
		// and the path to the selected file (FileBrowser.Result) (null, if FileBrowser.Success is false)
		Debug.Log(FileBrowserPlugin.Success + " " + FileBrowserPlugin.Result);


		//TODO: Verificar arquivo carregado.
	}

	// Get directoties
	public static string GetCharacterPartsDir() {
		string dir = Application.dataPath;
		int lastIndex = dir.LastIndexOf("/");
		if (lastIndex != -1) {
			dir = dir.Remove(lastIndex).Trim();
		}
		dir = dir + "/CharacterParts";

		if (!Directory.Exists(dir)) {
			Directory.CreateDirectory(dir);
		}

		return dir;
	}
	public static string GetCreatedCharactersDir() {
		string dir = Application.dataPath;
		int lastIndex = dir.LastIndexOf("/");
		if (lastIndex != -1) {
			dir = dir.Remove(lastIndex).Trim();
		}
		dir = dir + "/CreatedCharacters";

		if (!Directory.Exists(dir)) {
			Directory.CreateDirectory(dir);
		}
		return dir;
	}
	public static string GetMapsDir() {
		string dir = Application.dataPath;
		int lastIndex = dir.LastIndexOf("/");
		if (lastIndex != -1) {
			dir = dir.Remove(lastIndex).Trim();
		}
		dir = dir + "/GameMaps";

		if (!Directory.Exists(dir)) {
			Directory.CreateDirectory(dir);
		}
		return dir;
	}

	/// <summary>
	/// Load with parent path.
	/// </summary>
	/// <returns>Returns the loaded path.</returns>
	public static Sprite LoadCharacterSpriteByPath(string pathToLoad, float pivotY) {
		string dir = GetCharacterPartsDir();
		pathToLoad = pathToLoad.Replace(GetCharacterPartsDir(), "");
		pathToLoad = dir + pathToLoad;

		byte[] data = File.ReadAllBytes(pathToLoad);
		Texture2D imageToLoad = new Texture2D(192, 192, TextureFormat.ARGB32, false);
		imageToLoad.LoadImage(data);
		imageToLoad.name = Path.GetFileName(pathToLoad);
		return Sprite.Create(imageToLoad, new Rect(0, 0, imageToLoad.width, imageToLoad.height), new Vector2(0.5f, pivotY), 100);
	}

	/// <summary>
	/// Loads map png sprite by name.
	/// </summary>
	/// <param name="imgName"></param>
	/// <returns>Returns the loaded Sprite.</returns>
	public static Sprite LoadMapSpriteByName(string imgName) {
		string dir = GetMapsDir();
		imgName = dir + "/" + imgName + ".png";

		byte[] data = File.ReadAllBytes(imgName);
		Texture2D imageToLoad = new Texture2D(768, 768, TextureFormat.ARGB32, false);
		imageToLoad.LoadImage(data);
		imageToLoad.name = Path.GetFileName(imgName);
		return Sprite.Create(imageToLoad, new Rect(0, 0, imageToLoad.width, imageToLoad.height), new Vector2(0, 1), 100);
	}

	/// <summary>
	/// Need to pass the full path.
	/// </summary>
	/// <returns>Returns the loaded path.</returns>
	public static Sprite LoadSpriteByPath(string pathToLoad, float pivotY) {
		byte[] data = File.ReadAllBytes(pathToLoad);
		Texture2D imageToLoad = new Texture2D(192, 192, TextureFormat.ARGB32, false);
		imageToLoad.LoadImage(data);
		imageToLoad.name = Path.GetFileName(pathToLoad);
		return Sprite.Create(imageToLoad, new Rect(0, 0, imageToLoad.width, imageToLoad.height), new Vector2(0.5f, pivotY), 100);
	}

	#endregion

	#region Editor Attributes Buttons 

	public void RandomizarAtributos() {

		editingCharacter.Vida = Random.Range(1, MaxAttributesValues + 1);
		editingCharacter.Mana = Random.Range(1, MaxAttributesValues + 1);
		editingCharacter.Ataque = Random.Range(1, MaxAttributesValues + 1);
		editingCharacter.Defesa = Random.Range(1, MaxAttributesValues + 1);
		editingCharacter.Velocidade = Random.Range(1, MaxAttributesValues + 1);
		editingCharacter.Escudo = Random.Range(1, MaxAttributesValues + 1);
		editingCharacter.Classe = Random.Range(0, 2);
		editingCharacter.Pontos = Random.Range(1, MaxAttributesValues + 1);

		editingCharacter.RateOfFire = Random.Range(1, MaxAttributesValues + 1);
		editingCharacter.HitDelay = Random.Range(1, MaxAttributesValues + 1);
		editingCharacter.MinimumTimeOnAction = Random.Range(1, MaxAttributesValues + 1);
		editingCharacter.MaxTimeFollowingPlayer = Random.Range(1, MaxAttributesValues + 1);
		editingCharacter.MaxTimeOnStand = Random.Range(1, MaxAttributesValues + 1);
		editingCharacter.MaxTimeOnFlee = Random.Range(1, MaxAttributesValues + 1);
		editingCharacter.FleePriority = Random.Range(1, MaxAttributesValues + 1);
		editingCharacter.AtkPriority = Random.Range(1, MaxAttributesValues + 1);
		
		_newCharacterGroup.BroadcastMessage("DelayedUpdateItself", SendMessageOptions.DontRequireReceiver);

		UpdateEditingCharacterValuesShow();
	}
	public void RandomizarAparencia() {
		editingCharacter.IndexRacaSelecionada = Random.Range(0, allFolders.list.Count);
		allFolders.list[editingCharacter.IndexRacaSelecionada].CabecasIndex = Random.Range(0, allFolders.list[editingCharacter.IndexRacaSelecionada].Cabecas.Count);
		allFolders.list[editingCharacter.IndexRacaSelecionada].CorposIndex = Random.Range(0, allFolders.list[editingCharacter.IndexRacaSelecionada].Corpos.Count);
		allFolders.list[editingCharacter.IndexRacaSelecionada].MaosIndex = Random.Range(0, allFolders.list[editingCharacter.IndexRacaSelecionada].Maos.Count);
		allFolders.list[editingCharacter.IndexRacaSelecionada].BotasIndex = Random.Range(0, allFolders.list[editingCharacter.IndexRacaSelecionada].Botas.Count);

		UpdateAppearenceValues();
	}

	public void TrocaClasse() {
		if (editingCharacter.Classe == 0) {
			editingCharacter.Classe = 1;
			ClasseText.text = "Distancia";
		}
		else {
			editingCharacter.Classe = 0;
			ClasseText.text = "Corpo a corpo";
		}
		UpdateEditingCharacterValuesShow();

	}

	public void SetAttributePoint(GameAttribute atributo, int value) {
		switch (atributo) {
			case GameAttribute.vida:
				if (value > 0 && value <= MaxAttributesValues)
					editingCharacter.Vida = value;
				break;
			case GameAttribute.mana:
				if (value >= 0 && value <= MaxAttributesValues)
					editingCharacter.Mana = value;
				break;
			case GameAttribute.ataque:
				if (value > 0 && value <= MaxAttributesValues)
					editingCharacter.Ataque = value;
				break;
			case GameAttribute.defesa:
				if (value >= 0 && value <= MaxAttributesValues)
					editingCharacter.Defesa = value;
				break;
			case GameAttribute.velocidade:
				if (value > 0 && value <= MaxAttributesValues)
					editingCharacter.Velocidade = value;
				break;
			case GameAttribute.escudo:
				if (value >= 0 && value <= MaxAttributesValues)
					editingCharacter.Escudo = value;
				break;
			case GameAttribute.classe:
				TrocaClasse();
				break;
			case GameAttribute.pontos:
				if (value > 0 && value <= MaxAttributesValues)
					editingCharacter.Pontos = value;
				break;
			case GameAttribute.rateOfFire:
				if (value > 0 && value <= MaxAttributesValues)
					editingCharacter.RateOfFire = value;
				break;
			case GameAttribute.hitDelay:
				if (value > 0 && value <= MaxAttributesValues)
					editingCharacter.HitDelay = value;
				break;
			case GameAttribute.minimumTimeOnAction:
				if (value > 0 && value <= MaxAttributesValues)
					editingCharacter.MinimumTimeOnAction = value;
				break;
			case GameAttribute.maxTimeFollowingPlayer:
				if (value > 0 && value <= MaxAttributesValues)
					editingCharacter.MaxTimeFollowingPlayer = value;
				break;
			case GameAttribute.maxTimeOnStand:
				if (value > 0 && value <= MaxAttributesValues)
					editingCharacter.MaxTimeOnStand = value;
				break;
			case GameAttribute.maxTimeOnFlee:
				if (value > 0 && value <= MaxAttributesValues)
					editingCharacter.MaxTimeOnFlee = value;
				break;
			case GameAttribute.fleePriority:
				if (value > 0 && value <= MaxAttributesValues)
					editingCharacter.FleePriority = value;
				break;
			case GameAttribute.atkPriority:
				if (value > 0 && value <= MaxAttributesValues)
					editingCharacter.AtkPriority = value;
				break;
		}
		UpdateEditingCharacterValuesShow();
	}

	/// <summary>
	/// Add um ponto no atributo.
	/// </summary>
	/// <param name="atributo">Atributo a somar.</param>
	/// <returns>Retorna o valor atualizado do atributo.</returns>
	public int AddAttributePoint(GameAttribute atributo) {
		switch (atributo) {
			case GameAttribute.vida:
				if (editingCharacter.Vida < MaxAttributesValues)
					editingCharacter.Vida++;
				UpdateEditingCharacterValuesShow();
				return editingCharacter.Vida;
			case GameAttribute.mana:
				if (editingCharacter.Mana < MaxAttributesValues)
					editingCharacter.Mana++;
				UpdateEditingCharacterValuesShow();
				return editingCharacter.Mana;
			case GameAttribute.ataque:
				if (editingCharacter.Ataque < MaxAttributesValues)
					editingCharacter.Ataque++;
				UpdateEditingCharacterValuesShow();
				return editingCharacter.Ataque;
			case GameAttribute.defesa:
				if (editingCharacter.Defesa < MaxAttributesValues)
					editingCharacter.Defesa++;
				UpdateEditingCharacterValuesShow();
				return editingCharacter.Defesa;
			case GameAttribute.velocidade:
				if (editingCharacter.Velocidade < MaxAttributesValues)
					editingCharacter.Velocidade++;
				UpdateEditingCharacterValuesShow();
				return editingCharacter.Velocidade;
			case GameAttribute.escudo:
				if (editingCharacter.Escudo < MaxAttributesValues)
					editingCharacter.Escudo++;
				UpdateEditingCharacterValuesShow();
				return editingCharacter.Escudo;
			case GameAttribute.classe:
				TrocaClasse();
				UpdateEditingCharacterValuesShow();
				return 0;
			case GameAttribute.pontos:
				if (editingCharacter.Pontos < MaxAttributesValues)
					editingCharacter.Pontos++;
				UpdateEditingCharacterValuesShow();
				return editingCharacter.Pontos;
			case GameAttribute.rateOfFire:
				if (editingCharacter.RateOfFire < MaxAttributesValues)
					editingCharacter.RateOfFire++;
				UpdateEditingCharacterValuesShow();
				return editingCharacter.RateOfFire;
			case GameAttribute.hitDelay:
				if (editingCharacter.HitDelay < MaxAttributesValues)
					editingCharacter.HitDelay++;
				UpdateEditingCharacterValuesShow();
				return editingCharacter.HitDelay;
			case GameAttribute.minimumTimeOnAction:
				if (editingCharacter.MinimumTimeOnAction < MaxAttributesValues)
					editingCharacter.MinimumTimeOnAction++;
				UpdateEditingCharacterValuesShow();
				return editingCharacter.MinimumTimeOnAction;
			case GameAttribute.maxTimeFollowingPlayer:
				if (editingCharacter.MaxTimeFollowingPlayer < MaxAttributesValues)
					editingCharacter.MaxTimeFollowingPlayer++;
				UpdateEditingCharacterValuesShow();
				return editingCharacter.MaxTimeFollowingPlayer;
			case GameAttribute.maxTimeOnStand:
				if (editingCharacter.MaxTimeOnStand < MaxAttributesValues)
					editingCharacter.MaxTimeOnStand++;
				UpdateEditingCharacterValuesShow();
				return editingCharacter.MaxTimeOnStand;
			case GameAttribute.maxTimeOnFlee:
				if (editingCharacter.MaxTimeOnFlee < MaxAttributesValues)
					editingCharacter.MaxTimeOnFlee++;
				UpdateEditingCharacterValuesShow();
				return editingCharacter.MaxTimeOnFlee;
			case GameAttribute.fleePriority:
				if (editingCharacter.FleePriority < MaxAttributesValues)
					editingCharacter.FleePriority++;
				UpdateEditingCharacterValuesShow();
				return editingCharacter.FleePriority;
			case GameAttribute.atkPriority:
				if (editingCharacter.AtkPriority < MaxAttributesValues)
					editingCharacter.AtkPriority++;
				UpdateEditingCharacterValuesShow();
				return editingCharacter.AtkPriority;

		}

		return -1;
	}

	/// <summary>
	/// Remove um ponto no atributo.
	/// </summary>
	/// <param name="atributo">Atributo a remover.</param>
	/// <returns>Retorna o valor atualizado do atributo.</returns>
	public int RemoveAttributePoint(GameAttribute atributo) {
		switch (atributo) {
			case GameAttribute.vida:
				if (editingCharacter.Vida > 1)
					editingCharacter.Vida--;
				UpdateEditingCharacterValuesShow();
				return editingCharacter.Vida;
			case GameAttribute.mana:
				if (editingCharacter.Mana > 0)
					editingCharacter.Mana--;
				UpdateEditingCharacterValuesShow();
				return editingCharacter.Mana;
			case GameAttribute.ataque:
				if (editingCharacter.Ataque > 1)
					editingCharacter.Ataque--;
				UpdateEditingCharacterValuesShow();
				return editingCharacter.Ataque;
			case GameAttribute.defesa:
				if (editingCharacter.Defesa > 0)
					editingCharacter.Defesa--;
				UpdateEditingCharacterValuesShow();
				return editingCharacter.Defesa;
			case GameAttribute.velocidade:
				if (editingCharacter.Velocidade > 1)
					editingCharacter.Velocidade--;
				UpdateEditingCharacterValuesShow();
				return editingCharacter.Velocidade;
			case GameAttribute.escudo:
				if (editingCharacter.Escudo > 0)
					editingCharacter.Escudo--;
				UpdateEditingCharacterValuesShow();
				return editingCharacter.Escudo;
			case GameAttribute.classe:
				TrocaClasse();
				UpdateEditingCharacterValuesShow();
				return 0;
			case GameAttribute.pontos:
				if (editingCharacter.Pontos > 1)
					editingCharacter.Pontos--;
				UpdateEditingCharacterValuesShow();
				return editingCharacter.Pontos;

			case GameAttribute.rateOfFire:
				if (editingCharacter.RateOfFire > 1)
					editingCharacter.RateOfFire--;
				UpdateEditingCharacterValuesShow();
				return editingCharacter.RateOfFire;
			case GameAttribute.hitDelay:
				if (editingCharacter.HitDelay > 1)
					editingCharacter.HitDelay--;
				UpdateEditingCharacterValuesShow();
				return editingCharacter.HitDelay;
			case GameAttribute.minimumTimeOnAction:
				if (editingCharacter.MinimumTimeOnAction > 1)
					editingCharacter.MinimumTimeOnAction--;
				UpdateEditingCharacterValuesShow();
				return editingCharacter.MinimumTimeOnAction;
			case GameAttribute.maxTimeFollowingPlayer:
				if (editingCharacter.MaxTimeFollowingPlayer > 1)
					editingCharacter.MaxTimeFollowingPlayer--;
				UpdateEditingCharacterValuesShow();
				return editingCharacter.MaxTimeFollowingPlayer;
			case GameAttribute.maxTimeOnStand:
				if (editingCharacter.MaxTimeOnStand > 1)
					editingCharacter.MaxTimeOnStand--;
				UpdateEditingCharacterValuesShow();
				return editingCharacter.MaxTimeOnStand;
			case GameAttribute.maxTimeOnFlee:
				if (editingCharacter.MaxTimeOnFlee > 1)
					editingCharacter.MaxTimeOnFlee--;
				UpdateEditingCharacterValuesShow();
				return editingCharacter.MaxTimeOnFlee;
			case GameAttribute.fleePriority:
				if (editingCharacter.FleePriority > 1)
					editingCharacter.FleePriority--;
				UpdateEditingCharacterValuesShow();
				return editingCharacter.FleePriority;
			case GameAttribute.atkPriority:
				if (editingCharacter.AtkPriority > 1)
					editingCharacter.AtkPriority--;
				UpdateEditingCharacterValuesShow();
				return editingCharacter.AtkPriority;

		}
		return -1;
	}

	#endregion

	#region Appearence PREVIOUS and NEXT buttons
	public void NextRaca() {
		if (editingCharacter.IndexRacaSelecionada + 1 >= allFolders.list.Count) return; // if invertido
		editingCharacter.IndexRacaSelecionada++;
		UpdateAppearenceValues();
	}
	public void PreviousRaca() {
		if (editingCharacter.IndexRacaSelecionada <= 0) return; // if invertido
		editingCharacter.IndexRacaSelecionada--;
		UpdateAppearenceValues();
	}

	public void NextHead() {
		if (allFolders.list[editingCharacter.IndexRacaSelecionada].CabecasIndex + 1 >= allFolders.list[editingCharacter.IndexRacaSelecionada].Cabecas.Count) return; // if invertido
		allFolders.list[editingCharacter.IndexRacaSelecionada].CabecasIndex++;
		UpdateAppearenceValues();
	}
	public void PreviousHead() {
		if (allFolders.list[editingCharacter.IndexRacaSelecionada].CabecasIndex <= 0) return; // if invertido
		allFolders.list[editingCharacter.IndexRacaSelecionada].CabecasIndex--;
		UpdateAppearenceValues();
	}

	public void NextBody() {
		if (allFolders.list[editingCharacter.IndexRacaSelecionada].CorposIndex + 1 >= allFolders.list[editingCharacter.IndexRacaSelecionada].Corpos.Count) return; // if invertido
		allFolders.list[editingCharacter.IndexRacaSelecionada].CorposIndex++;
		UpdateAppearenceValues();
	}
	public void PreviousBody() {
		if (allFolders.list[editingCharacter.IndexRacaSelecionada].CorposIndex <= 0) return; // if invertido
		allFolders.list[editingCharacter.IndexRacaSelecionada].CorposIndex--;
		UpdateAppearenceValues();
	}

	public void NextHand() {
		if (allFolders.list[editingCharacter.IndexRacaSelecionada].MaosIndex + 1 >= allFolders.list[editingCharacter.IndexRacaSelecionada].Maos.Count) return; // if invertido
		allFolders.list[editingCharacter.IndexRacaSelecionada].MaosIndex++;
		UpdateAppearenceValues();
	}
	public void PreviousHand() {
		if (allFolders.list[editingCharacter.IndexRacaSelecionada].MaosIndex <= 0) return; // if invertido
		allFolders.list[editingCharacter.IndexRacaSelecionada].MaosIndex--;
		UpdateAppearenceValues();
	}

	public void NextFeet() {
		if (allFolders.list[editingCharacter.IndexRacaSelecionada].BotasIndex + 1 >= allFolders.list[editingCharacter.IndexRacaSelecionada].Botas.Count) return; // if invertido
		allFolders.list[editingCharacter.IndexRacaSelecionada].BotasIndex++;
		UpdateAppearenceValues();
	}
	public void PreviousFeet() {
		if (allFolders.list[editingCharacter.IndexRacaSelecionada].BotasIndex <= 0) return; // if invertido
		allFolders.list[editingCharacter.IndexRacaSelecionada].BotasIndex--;
		UpdateAppearenceValues();
	}
	#endregion

	#region Load Character 

	public void LoadCharaters() {
		// disable others menus
		_newCharacterGroup.SetActive(false);
		_characterGroup.SetActive(false);
		_menuGroup.SetActive(false);
		_erasePopupConfirm.SetActive(false);
		// and enable the Load menu
		_loadGroup.SetActive(true);

		// erase previews
		_previewNameText.text = "Nenhum personagem";
		SelectedChar_Head.sprite = _emptyImage;
		SelectedChar_Body.sprite = _emptyImage;
		SelectedChar_Hand_L.sprite = _emptyImage;
		SelectedChar_Hand_R.sprite = _emptyImage;
		SelectedChar_Feet_L.sprite = _emptyImage;
		SelectedChar_Feet_R.sprite = _emptyImage;

		// erase all children objects if exists
		foreach (RectTransform child in _contentToCreateLoadedChars) {
			if (child.gameObject != _contentToCreateLoadedChars.gameObject) {
				Destroy(child.gameObject);

			}
		}

		// load all characters in folder
		string dir = GetCreatedCharactersDir();
		foreach (var monsterJsonPath in Directory.GetFiles(dir, "*.json")) {
			var arquivoCarregado = File.OpenRead(monsterJsonPath);
			string fileContents;
			using (StreamReader reader = new StreamReader(arquivoCarregado)) {
				fileContents = reader.ReadToEnd();
			}
			CharacterEntry createdCharInfo = JsonUtility.FromJson<CharacterEntry>(fileContents);
			_listCharactersToLoad.Add(createdCharInfo);
			_listCharactersPath.Add(monsterJsonPath);

			// create charracter on screen
			GameObject createdCharItem = Instantiate(PrefabLoadedCharItem, _contentToCreateLoadedChars);

			// get the button component
			var createdCharButton = createdCharItem.GetComponent<LoadedCharacterButton>();
			// assign his values to this
			createdCharButton.SetValues(createdCharInfo);

		}

		// see if characters are loaded
		if (_listCharactersToLoad.Count > 0) {
			// set the selected character to the first of the list
			SetSelectedCharToLoad(_listCharactersToLoad[0]);
			_noCharsLoadedText.gameObject.SetActive(false);
		}
		else {
			_noCharsLoadedText.gameObject.SetActive(true);

		}

		// activate or not the buttons
		if (SelectedChar != null) {
			EraseCharButton.interactable = true;
			ConfirmLoadCharButton.interactable = true;
		}
		else {
			EraseCharButton.interactable = false;
			ConfirmLoadCharButton.interactable = false;

		}

	}

	public void SetSelectedCharToLoad(CharacterEntry characterItem) {
		SelectedChar = characterItem;

		_previewNameText.text = SelectedChar.CharacterName;

		SelectedChar_Head.sprite = LoadCharacterSpriteByPath(SelectedChar.HeadImagePath, 0f);
		SelectedChar_Body.sprite = LoadCharacterSpriteByPath(SelectedChar.BodyImagePath, 0.5f);
		SelectedChar_Hand_L.sprite = LoadCharacterSpriteByPath(SelectedChar.HandImagePath, 0.5f);
		SelectedChar_Hand_R.sprite = LoadCharacterSpriteByPath(SelectedChar.HandImagePath, 0.5f);
		SelectedChar_Feet_L.sprite = LoadCharacterSpriteByPath(SelectedChar.FeetImagePath, 0.5f);
		SelectedChar_Feet_R.sprite = LoadCharacterSpriteByPath(SelectedChar.FeetImagePath, 0.5f);

		if (SelectedChar != null) {
			EraseCharButton.interactable = true;
			ConfirmLoadCharButton.interactable = true;
		}
		else {
			EraseCharButton.interactable = false;
			ConfirmLoadCharButton.interactable = false;

		}
	}

	public void ConfirmCharacterSelection() {

		editingCharacter = SelectedChar;

		copyAppearValues = false; // deve ser FALSE para carregar um arquivo

		MenuCriarPersonagem();
		UpdateAppearenceValues();

		_loadGroup.SetActive(false);
		_erasePopupConfirm.SetActive(false);
		_menuGroup.SetActive(false);

		_newCharacterGroup.SetActive(true);
		_characterGroup.SetActive(true);

	}

	public void EraseSelectedCharacter() {
		_erasePopupConfirm.SetActive(true);

	}

	public void ConfirmEraseCharacter() {

		for (int i = 0; i < _listCharactersToLoad.Count; i++) {
			// get the right character file and remove from lists
			if (_listCharactersToLoad[i] == SelectedChar) {
				_listCharactersToLoad.Remove(SelectedChar);
				File.Delete(_listCharactersPath[i]);
				_listCharactersPath.RemoveAt(i);
				break;
			}
		}

		//So reload the folder again
		LoadCharaters();
	}

	public void CancelEraseCharacter() {
		_erasePopupConfirm.SetActive(false);
	}


	#endregion


}

#region Classes de personagem

/// <summary>
/// Classe do personagem/inimigo. Contem todos os atributos necessarios.
/// </summary>
[System.Serializable]
public class CharacterEntry
{
	[Header("Aparência")]
	public string CharacterName = "Meu nome";
	public string RacaPath;
	public string HeadImagePath;
	public string BodyImagePath;
	public string HandImagePath;
	public string FeetImagePath;
	[Header("Atributos")]
	public int Vida;
	public int Mana;
	public int Ataque;
	public int Defesa;
	public int Velocidade;
	public int Escudo;
	[Space]
	public int RateOfFire;
	public int HitDelay = 15;
	public int MinimumTimeOnAction = 1;
	public int MaxTimeFollowingPlayer = 6;
	public int MaxTimeOnStand = 3;
	public int MaxTimeOnFlee = 2;
	public int FleePriority = 30;
	public int AtkPriority = 60;
	[Space]
	[Header("I.A.")]
	public int Classe = 0; // 0 corpo a corpo, 1 distancia
	[Tooltip("Quantos pontos da ao morrer. Também serve como custo para ser spawnado no jogo")]
	public int Pontos = 1;

	[Space]

	public int IndexRacaSelecionada;

	[Space]

	// Para quando salvar e carregar
	public int CabecasIndex;
	public int CorposIndex;
	public int MaosIndex;
	public int BotasIndex;
}

[System.Serializable]
public class CharacterDatabase
{
	public List<CharacterEntry> list = new List<CharacterEntry>();
}

[System.Serializable]
public class FolderClass
{
	public string NomeDaClasse;

	public List<string> Cabecas = new List<string>();
	public int CabecasIndex = 0;
	public List<string> Corpos = new List<string>();
	public int CorposIndex = 0;
	public List<string> Maos = new List<string>();
	public int MaosIndex = 0;
	public List<string> Botas = new List<string>();
	public int BotasIndex = 0;

	public FolderClass(string name) {
		NomeDaClasse = name;
	}

}

[System.Serializable]
public class FolderDatabase
{
	public List<FolderClass> list = new List<FolderClass>();
}
#endregion
