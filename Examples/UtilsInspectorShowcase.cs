using System.Collections.Generic;
using UnityEngine;
using AnkleBreaker.Utils.Inspector;

/// <summary>
/// Showcase script demonstrating ALL UtilsInspector custom attributes.
/// Attach to a GameObject in TestingScene.
/// </summary>
public class UtilsInspectorShowcase : MonoBehaviour
{
    [SerializeField] private AB_SerializedDictionary<string, int> myDictionary;
    // ============================================================
    // SECTION HEADER STYLES
    // ============================================================
    [SectionHeader("Style: CenterLine (Default)")]
    public string centerLineStyleDemo = "SectionHeaderStyle.CenterLine";

    [SectionHeader("Style: Line", SectionHeaderStyle.Line)]
    public string lineStyleDemo = "SectionHeaderStyle.Line";

    [SectionHeader("Style: Box", SectionHeaderStyle.Box)]
    public string boxStyleDemo = "SectionHeaderStyle.Box";

    [SectionHeader("Style: Clean", SectionHeaderStyle.Clean)]
    public string cleanStyleDemo = "SectionHeaderStyle.Clean";

    // ============================================================
    // SECTION HEADER WITH CUSTOM COLORS
    // ============================================================
    [SectionHeader("Red Section", 1f, 0.3f, 0.3f)]
    public string redSectionDemo = "CenterLine with red color";

    [SectionHeader("Green Section", SectionHeaderStyle.Line, 0.3f, 0.9f, 0.3f)]
    public string greenSectionDemo = "Line with green color";

    [SectionHeader("Blue Section", SectionHeaderStyle.Box, 0.3f, 0.5f, 1f)]
    public string blueSectionDemo = "Box with blue color";

    // ============================================================
    // BASIC ATTRIBUTES
    // ============================================================
    [SectionHeader("Basic Attributes")]

    [ReadOnly]
    public string readOnlyField = "Cannot edit this";

    [LabelText("Custom Display Name")]
    public string renamedField = "LabelText in action";

    [HelpBox("This is a HelpBox with info message.", HelpBoxAttribute.MessageType.Info)]
    public int helpBoxField = 42;

    [Required("This reference is required!")]
    public GameObject requiredRef;

    [ABToolTip("GetTooltip")]
    public float tooltipField = 1.5f;
    private string GetTooltip() => $"Current value: {tooltipField}";

    // ============================================================
    // SHOW IF / HIDE IF (with value comparison)
    // ============================================================
    public enum DisplayMode { Simple, Advanced, Expert }

    [SectionHeader("Conditional Visibility")]
    public DisplayMode displayMode = DisplayMode.Simple;

    [ShowIf("displayMode", (int)DisplayMode.Advanced)]
    public string advancedOnlyField = "Visible in Advanced mode";

    [ShowIf("displayMode", (int)DisplayMode.Expert)]
    public string expertOnlyField = "Visible in Expert mode";

    public bool showOptional = true;

    [ShowIf("showOptional")]
    public string optionalField = "Toggle showOptional above";

    [HideIf("showOptional")]
    public string hiddenWhenOptional = "Hidden when showOptional is true";

    // ============================================================
    // ENABLE IF
    // ============================================================
    [SectionHeader("Enable If")]

    public bool enableEditing = false;

    [EnableIf("enableEditing")]
    public string grayedOutField = "Enable editing above to modify";

    [EnableIf("displayMode", (int)DisplayMode.Expert)]
    public float expertOnlySlider = 0.5f;

    // ============================================================
    // PROGRESS BAR
    // ============================================================
    [SectionHeader("Progress Bar")]

    [ProgressBar(0, 100, "Health", 0.2f, 0.8f, 0.2f)]
    public float health = 75f;

    [ProgressBar(0, 100, "Mana", 0.2f, 0.4f, 0.9f)]
    public float mana = 30f;

    [ProgressBar(0, 1)]
    public float normalizedProgress = 0.6f;

    // ============================================================
    // MIN MAX SLIDER
    // ============================================================
    [SectionHeader("MinMax Slider")]

    [MinMaxSlider(0f, 100f)]
    public Vector2 damageRange = new Vector2(10f, 50f);

    [MinMaxSlider(0f, 10f)]
    public Vector2 spawnDelay = new Vector2(1f, 3f);

    // ============================================================
    // SUFFIX LABEL
    // ============================================================
    [SectionHeader("Suffix Label")]

    [SuffixLabel("ms")]
    public float responseTime = 150f;

    [SuffixLabel("px")]
    public int borderWidth = 2;

    [SuffixLabel("%")]
    public float probability = 75f;

    // ============================================================
    // ENUM TOGGLE BUTTONS
    // ============================================================
    public enum Direction { Left, Right, Up, Down }

    [SectionHeader("Enum Toggle Buttons")]
    [EnumToggleButtons]
    public Direction moveDirection = Direction.Right;

    public enum Quality { Low, Medium, High, Ultra }

    [EnumToggleButtons]
    public Quality renderQuality = Quality.High;

    // ============================================================
    // GUI COLOR
    // ============================================================
    [SectionHeader("GUI Color Tint")]

    [GUIColor(1f, 0.8f, 0.8f)]
    public string redTintField = "Red tinted";

    [GUIColor(0.8f, 1f, 0.8f)]
    public string greenTintField = "Green tinted";

    [GUIColor(0.8f, 0.8f, 1f)]
    public string blueTintField = "Blue tinted";

    // ============================================================
    // DISABLE IN PLAY/EDITOR MODE
    // ============================================================
    [SectionHeader("Play/Editor Mode Control")]

    [DisableInPlayMode]
    public string editModeOnly = "Disabled during play";

    [DisableInEditorMode]
    public string playModeOnly = "Disabled in editor";

    // ============================================================
    // INLINE BUTTON
    // ============================================================
    [SectionHeader("Inline Button")]

    [InlineButton("ResetName", "Reset")]
    public string playerName = "Player1";

    [InlineButton("RandomizeHP", "Rand")]
    public int hitPoints = 100;

    private void ResetName() { playerName = "Player1"; }
    private void RandomizeHP() { hitPoints = Random.Range(1, 200); }

    // ============================================================
    // VALUE DROPDOWN
    // ============================================================
    [SectionHeader("Value Dropdown")]

    [ValueDropdown("GetWeaponNames")]
    public string selectedWeapon = "Sword";

    private List<string> GetWeaponNames() => new List<string> { "Sword", "Bow", "Staff", "Dagger", "Axe" };

    [ValueDropdown("difficulties")]
    public int difficultyLevel = 1;
    private List<int> difficulties = new List<int> { 1, 2, 3, 4, 5 };

    // ============================================================
    // ON VALUE CHANGED
    // ============================================================
    [SectionHeader("On Value Changed")]

    [OnValueChanged("OnVolumeChanged")]
    [Range(0f, 1f)]
    public float volume = 0.5f;

    private void OnVolumeChanged() { Debug.Log($"Volume changed to: {volume}"); }

    // ============================================================
    // VALIDATE INPUT
    // ============================================================
    [SectionHeader("Validate Input")]

    [ValidateInput("IsPositive", "Speed must be positive!", ValidateMessageType.Error)]
    public float speed = 10f;

    private bool IsPositive() => speed > 0;

    [ValidateInput("IsNotEmpty", "Name cannot be empty!", ValidateMessageType.Warning)]
    public string characterName = "Hero";

    private bool IsNotEmpty() => !string.IsNullOrEmpty(characterName);

    // ============================================================
    // INFO BOX
    // ============================================================
    [SectionHeader("Info Box")]

    [InfoBox("This is an informational message.")]
    public int infoField = 10;

    [InfoBox("Warning: This value affects performance!", InfoMessageType.Warning)]
    public int particleCount = 1000;

    // ============================================================
    // PROPERTY SPACE
    // ============================================================
    [SectionHeader("Property Space")]

    public float fieldBefore = 1f;

    [PropertySpace(20f)]
    public float fieldWithSpaceBefore = 2f;

    public float fieldAfter = 3f;

    // ============================================================
    // PROPERTY ORDER
    // ============================================================
    [SectionHeader("Property Order (check inspector order)")]

    [PropertyOrder(3)]
    public string thirdField = "Order 3";

    [PropertyOrder(1)]
    public string firstField = "Order 1";

    [PropertyOrder(2)]
    public string secondField = "Order 2";

    // ============================================================
    // BOX GROUP
    // ============================================================
    [SectionHeader("Box Group")]

    [BoxGroup("Movement")]
    public float moveSpeed = 5f;

    [BoxGroup("Movement")]
    public float jumpForce = 10f;

    [BoxGroup("Movement")]
    public float gravity = -9.81f;

    [BoxGroup("Combat")]
    public int attackPower = 25;

    [BoxGroup("Combat")]
    public float attackSpeed = 1.5f;

    [BoxGroup("Foldable Box", foldable: true)]
    public string foldableField1 = "This box can be folded";

    [BoxGroup("Foldable Box", foldable: true)]
    public int foldableField2 = 42;

    [BoxGroup("Foldable Box", foldable: true)]
    public float foldableField3 = 3.14f;

    // ============================================================
    // FOLDOUT GROUP
    // ============================================================
    [FoldoutGroup("Audio Settings", FoldoutGroupStyle.CenterLine)]
    public float masterVolume = 1f;
    [FoldoutGroup("Audio Settings", FoldoutGroupStyle.CenterLine)]
    public float musicVolume = 0.8f;
    [FoldoutGroup("Audio Settings", FoldoutGroupStyle.CenterLine)]
    public float sfxVolume = 0.9f;

    [FoldoutGroup("Debug Options", FoldoutGroupStyle.Line)]
    public bool showGizmos = true;

    [FoldoutGroup("Debug Options", FoldoutGroupStyle.Line)]
    public bool logPerformance = false;

    [FoldoutGroup("Network", FoldoutGroupStyle.CenterLine)]
    public string networkHost = "127.0.0.1";

    [FoldoutGroup("Network", FoldoutGroupStyle.CenterLine)]
    public int networkPort = 7777;

    [FoldoutGroup("Misc", FoldoutGroupStyle.Clean, 0.9f, 0.6f, 0.2f)]
    public string miscNote = "clean style";

    [FoldoutGroup("Misc", FoldoutGroupStyle.Clean, 0.9f, 0.6f, 0.2f)]
    public int miscCount = 42;

    // ============================================================
    // TAB GROUP
    // ============================================================
    [SectionHeader("Tab Group")]

    [TabGroup("Stats")]
    public int strength = 10;

    [TabGroup("Stats")]
    public int dexterity = 8;

    [TabGroup("Stats")]
    public int intelligence = 12;

    [TabGroup("Equipment")]
    public string weapon = "Iron Sword";

    [TabGroup("Equipment")]
    public string armor = "Leather Vest";

    [TabGroup("Equipment")]
    public string accessory = "Silver Ring";

    // ============================================================
    // HORIZONTAL GROUP
    // ============================================================
    [SectionHeader("Horizontal Group")]

    [HorizontalGroup("Position", 0.33f)]
    public float posX = 0f;

    [HorizontalGroup("Position", 0.33f)]
    public float posY = 0f;

    [HorizontalGroup("Position", 0.34f)]
    public float posZ = 0f;

    // ============================================================
    // PREVIEW FIELD
    // ============================================================
    [SectionHeader("Preview Field")]

    [PreviewField(80)]
    public Texture2D previewTexture;

    [PreviewField(80)]
    public Sprite previewSprite;

    [PreviewField(80)]
    public Mesh previewMesh;

    [PreviewField(80)]
    public GameObject previewPrefab;

    // ============================================================
    // INLINE EDITOR
    // ============================================================
    [SectionHeader("Inline Editor")]

    [InlineEditor]
    public ScriptableObject inlineScriptableObject;

    // ============================================================
    // TABLE LIST
    // ============================================================
    [System.Serializable]
    public struct InventoryItem
    {
        public string itemName;
        public int quantity;
        public float weight;
    }

    [SectionHeader("Table List")]
    [TableList]
    public List<InventoryItem> inventory = new List<InventoryItem>
    {
        new InventoryItem { itemName = "Health Potion", quantity = 5, weight = 0.3f },
        new InventoryItem { itemName = "Iron Sword", quantity = 1, weight = 3.5f },
        new InventoryItem { itemName = "Gold Coin", quantity = 100, weight = 0.01f }
    };

    // ============================================================
    // SHOW IN INSPECTOR (non-serialized)
    // ============================================================
    [SectionHeader("Show In Inspector")]
    [HideInInspector] public bool _showInInspectorSection;

    [ShowInInspector]
    public int CalculatedDPS => attackPower * (int)(1f / attackSpeed);

    [ShowInInspector]
    public string FullName => $"{characterName} (Lv.{strength + dexterity + intelligence})";

    [ShowInInspector(runtimeOnly: true)]
    public float TimeSinceStart => UnityEngine.Time.time;

    // ============================================================
    // EXISTING ATTRIBUTES SHOWCASE
    // ============================================================
    [SectionHeader("Other Existing Attributes")]

    [FreeRange(0f, 100f)]
    public float freeRangeValue = 50f;

    [ToggleButton("Enabled", "Disabled")]
    public bool toggleButton = true;

    [ToggleButton("Big Toggle ON", "Big Toggle OFF", big: true)]
    public bool bigToggleButton = false;

    [FolderPath]
    public string folderPath = "Assets/";

    [HideVariableName]
    public string noLabelField = "No variable name shown";

    // ============================================================
    // HORIZONTAL TOGGLE BUTTONS + CONDITIONAL (ShowIf on same line)
    // ============================================================
    [SectionHeader("Horizontal ToggleButtons + ShowIf")]

    [HorizontalGroup("PlayOptions")]
    [ToggleButton("Play On Enable")]
    public bool playOnEnable;

    [HorizontalGroup("PlayOptions")]
    [ShowIf("playOnEnable")]
    [ToggleButton("Ignore First Enable")]
    public bool ignoreFirstEnable;

    [HorizontalGroup("DebugToggles")]
    [ToggleButton("Show FPS")]
    public bool showFPS;

    [HorizontalGroup("DebugToggles")]
    [ToggleButton("Show Logs")]
    public bool showLogs;

    [HorizontalGroup("DebugToggles")]
    [ToggleButton("God Mode")]
    public bool godMode;

    // ============================================================
    // TAG FIELD / LAYER FIELD
    // ============================================================
    [SectionHeader("Tag & Layer Fields")]

    [TagField]
    public string targetTag = "Player";

    [LayerField]
    public int targetLayer;

    // ============================================================
    // INDENT
    // ============================================================
    [SectionHeader("Indent")]

    public string noIndent = "Level 0";

    [Indent(1)]
    public string indented1 = "Level 1";

    [Indent(2)]
    public string indented2 = "Level 2";

    // ============================================================
    // FILE PATH
    // ============================================================
    [SectionHeader("File Path")]

    [FilePath(Extensions = "png,jpg")]
    public string texturePath = "";

    [FilePath(AbsolutePath = true)]
    public string absoluteFilePath = "";

    // ============================================================
    // MULTILINE PROPERTY
    // ============================================================
    [SectionHeader("Multiline Property")]

    [MultilineProperty(4)]
    public string longDescription = "Write a long description here...";

    // ============================================================
    // ASSET ONLY / SCENE OBJECT ONLY
    // ============================================================
    [SectionHeader("Asset/Scene Object Restriction")]

    [AssetOnly]
    public GameObject assetOnlyRef;

    [SceneObjectOnly]
    public GameObject sceneOnlyRef;

    // ============================================================
    // SEARCHABLE ENUM
    // ============================================================
    public enum LargeEnum { Apple, Banana, Cherry, Date, Elderberry, Fig, Grape, Honeydew, Kiwi, Lemon, Mango, Nectarine, Orange, Papaya, Quince, Raspberry, Strawberry, Tangerine, Ugli, Vanilla, Watermelon }

    [SectionHeader("Searchable Enum")]

    [SearchableEnum]
    public LargeEnum fruit = LargeEnum.Apple;

    // ============================================================
    // SCENE FIELD
    // ============================================================
    [SectionHeader("Scene Field")]

    [SceneField]
    public string gameScene = "";

    // ============================================================
    // COLOR PALETTE
    // ============================================================
    [SectionHeader("Color Palette")]

    [ColorPalette("Vivid")]
    public Color vividColor = Color.red;

    [ColorPalette("Pastel")]
    public Color pastelColor = Color.white;

    [ColorPalette("#FF6B35", "#F7C59F", "#EFEFD0", "#004E89", "#1A659E")]
    public Color customPaletteColor = Color.white;

    // ============================================================
    // CONDITIONAL BOX GROUP
    // ============================================================
    [SectionHeader("Conditional Box Group")]

    public bool showNetworkSettings = true;

    [BoxGroup("Network Settings", ShowIf = "showNetworkSettings")]
    public string serverAddress = "127.0.0.1";

    [BoxGroup("Network Settings", ShowIf = "showNetworkSettings")]
    public int port = 7777;

    // ============================================================
    // LIST DRAWER SETTINGS
    // ============================================================
    [SectionHeader("List Drawer Settings")]

    [ListDrawerSettings(ElementLabel = "Waypoint $index", MinCount = 1, MaxCount = 10)]
    public List<Vector3> waypoints = new List<Vector3> { Vector3.zero, Vector3.one };

    [ListDrawerSettings(Draggable = false, ShowAddRemoveButtons = false)]
    public List<string> readOnlyList = new List<string> { "Fixed", "List" };

    // ============================================================
    // ON INSPECTOR INIT / DISPOSE
    // ============================================================
    [SectionHeader("Inspector Lifecycle")]
    public string lifecycleInfo = "Check console for init/dispose logs";

    [OnInspectorInit]
    private void OnInspectorOpened()
    {
        Debug.Log("[Showcase] Inspector opened (OnInspectorInit)");
    }

    [OnInspectorDispose]
    private void OnInspectorClosed()
    {
        Debug.Log("[Showcase] Inspector closed (OnInspectorDispose)");
    }

    // ============================================================
    // HIDE IN PLAY/EDITOR MODE
    // ============================================================
    [SectionHeader("Hide In Play/Editor Mode")]

    [HideInPlayMode]
    public string editorOnlyNote = "This is only visible in Editor mode";

    [HideInEditorMode]
    public string runtimeOnlyNote = "This is only visible in Play mode";

    // ============================================================
    // WRAP
    // ============================================================
    [SectionHeader("Wrap")]

    [Wrap(0f, 360f)]
    public float angle = 45f;

    [Wrap(0, 24)]
    public int hour = 12;

    // ============================================================
    // DELAYED PROPERTY
    // ============================================================
    [SectionHeader("Delayed Property")]

    [DelayedProperty]
    public string delayedName = "Press Enter to apply";

    [DelayedProperty]
    public float delayedValue = 3.14f;

    // ============================================================
    // DISPLAY AS STRING
    // ============================================================
    [SectionHeader("Display As String")]

    [DisplayAsString]
    public string buildVersion = "1.6.3-dev";

    [DisplayAsString]
    public int totalFrames = 999;

    [DisplayAsString(HideLabel = true, FontSize = 14)]
    public string statusMessage = "All systems operational";

    // ============================================================
    // INLINE BUTTONS
    // ============================================================
    [SectionHeader("Inline Buttons")]

    [InlineButton("GenerateGUID", "Generate")]
    public string guid = "";

    [InlineButton("RandomizeSpeed", "↻")]
    public float movementSpeed = 5f;

    [InlineButton("ClearNotes", "Clear", Mode = ButtonMode.AlwaysEnabled)]
    public string notes = "Some notes here";

    private void GenerateGUID() => guid = System.Guid.NewGuid().ToString();
    private void RandomizeSpeed() => movementSpeed = UnityEngine.Random.Range(0f, 100f);
    private void ClearNotes() => notes = "";

    // ============================================================
    // BUTTONS
    // ============================================================

    [Button("Log All Stats")]
    public void LogStats()
    {
        Debug.Log($"HP: {hitPoints}, ATK: {attackPower}, SPD: {speed}");
    }

    [Button("Reset Everything")]
    public void ResetAll()
    {
        health = 100f;
        mana = 100f;
        hitPoints = 100;
        speed = 10f;
        Debug.Log("All values reset!");
    }

    [Button(Mode = ButtonMode.EnabledInPlayMode)]
    public void PlayModeOnlyAction()
    {
        Debug.Log("This only works in play mode!");
    }

    // Horizontal group buttons (same line)
    [Button("▶ Action A", HorizontalGroup = "Actions")]
    public void ActionA() { Debug.Log("Action A"); }

    [Button("■ Action B", HorizontalGroup = "Actions")]
    public void ActionB() { Debug.Log("Action B"); }

    // Button with parameters
    [Button("Spawn Enemies")]
    public void SpawnEnemies(int count = 5, string enemyType = "Zombie", float spawnRadius = 10f)
    {
        Debug.Log($"Spawning {count} {enemyType}(s) in a {spawnRadius}m radius");
    }
}
