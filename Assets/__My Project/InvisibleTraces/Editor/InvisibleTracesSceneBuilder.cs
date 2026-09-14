#if UNITY_EDITOR
using InvisibleTraces;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class InvisibleTracesSceneBuilder
{
    private const string Root = "Assets/__My Project/InvisibleTraces";
    private const string ScenePath = Root + "/InvisibleTraces_Main.unity";
    private const string PlayerPath = "Assets/EZPZ Interaction Toolkit/Prefabs/Flat Screen Specific/EZPZ Player Flat Screen WASD.prefab";

    [MenuItem("DDES9902/Build Invisible Traces Prototype")]
    public static void Build()
    {
        EnsureFolders();
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        var palette = CreatePalette();
        CreateLighting();
        CreateRoom(palette);
        CreatePlayer();

        var managerObject = new GameObject("SYSTEM - Invisible Traces Training");
        var manager = managerObject.AddComponent<InvisibleTracesManager>();

        CreateInformationBoards(manager, palette);
        CreateActionStations(palette);
        CreatePreparationArea(palette);
        CreateTrainingItems(palette);
        CreateRiskStoryProps(palette);

        EditorSceneManager.SaveScene(scene, ScenePath);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("INVISIBLE TRACES BUILD COMPLETE: " + ScenePath);
    }

    private static void EnsureFolders()
    {
        EnsureFolder("Assets/__My Project", "InvisibleTraces");
        EnsureFolder(Root, "Scripts");
        EnsureFolder(Root, "Editor");
        EnsureFolder(Root, "Materials");
    }

    private static void EnsureFolder(string parent, string child)
    {
        if (!AssetDatabase.IsValidFolder(parent + "/" + child))
            AssetDatabase.CreateFolder(parent, child);
    }

    private static Material[] CreatePalette()
    {
        return new[]
        {
            Material("Floor", new Color(0.12f, 0.16f, 0.18f)),
            Material("Wall", new Color(0.72f, 0.76f, 0.77f)),
            Material("Counter", new Color(0.24f, 0.28f, 0.29f)),
            Material("SafeBlue", new Color(0.08f, 0.48f, 0.82f)),
            Material("HazardOrange", new Color(0.94f, 0.34f, 0.08f)),
            Material("ActionGreen", new Color(0.12f, 0.62f, 0.36f)),
            Material("Bread", new Color(0.82f, 0.62f, 0.32f)),
            Material("Vegetable", new Color(0.24f, 0.72f, 0.28f)),
            Material("White", new Color(0.92f, 0.94f, 0.94f)),
            Material("Dark", new Color(0.035f, 0.055f, 0.07f)),
            Material("Peanut", new Color(0.53f, 0.27f, 0.08f)),
            Material("Trace", new Color(0.95f, 0.12f, 0.72f))
        };
    }

    private static Material Material(string name, Color colour)
    {
        var path = Root + "/Materials/" + name + ".mat";
        var existing = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (existing != null)
        {
            existing.color = colour;
            return existing;
        }

        var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
        var material = new Material(shader) { name = name, color = colour };
        AssetDatabase.CreateAsset(material, path);
        return material;
    }

    private static void CreateLighting()
    {
        var lightObject = new GameObject("Directional Light");
        var light = lightObject.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.1f;
        lightObject.transform.rotation = Quaternion.Euler(48f, -32f, 0f);
        RenderSettings.ambientLight = new Color(0.35f, 0.37f, 0.4f);
    }

    private static void CreateRoom(Material[] p)
    {
        var environment = new GameObject("ENVIRONMENT - Shared Cafe Kitchen");
        Cube("Floor", new Vector3(0, -0.1f, 1), new Vector3(12, 0.2f, 12), p[0], environment.transform);
        Cube("Back Wall", new Vector3(0, 2.5f, 6), new Vector3(12, 5, 0.2f), p[1], environment.transform);
        Cube("Left Wall", new Vector3(-6, 2.5f, 1), new Vector3(0.2f, 5, 10), p[1], environment.transform);
        Cube("Right Wall", new Vector3(6, 2.5f, 1), new Vector3(0.2f, 5, 10), p[1], environment.transform);

        Cube("Preparation Counter", new Vector3(0, 0.75f, 1.6f), new Vector3(5.6f, 1.5f, 1.4f), p[2], environment.transform);
        Cube("Risk Counter", new Vector3(-4.25f, 0.75f, 3.7f), new Vector3(2.4f, 1.5f, 1.4f), p[2], environment.transform);
        Cube("Safe Equipment Counter", new Vector3(4.25f, 0.75f, 3.7f), new Vector3(2.4f, 1.5f, 1.4f), p[2], environment.transform);
    }

    private static void CreatePlayer()
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPath);
        if (prefab == null)
            throw new System.InvalidOperationException("EZPZ player prefab not found at " + PlayerPath);

        var player = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        player.name = "EZPZ PLAYER - Spatial Ray Interaction";
        player.transform.position = new Vector3(0, 0.05f, -3.8f);
        player.transform.rotation = Quaternion.identity;
    }

    private static void CreateInformationBoards(InvisibleTracesManager manager, Material[] p)
    {
        var board = Cube("ORDER AND FEEDBACK BOARD", new Vector3(0, 2.9f, 5.82f), new Vector3(5.8f, 2.5f, 0.12f), p[9]);
        manager.objectiveText = Text("Objective", "PEANUT-FREE ORDER", new Vector3(0, 3.65f, 5.72f), 0.24f, Color.white, TextAlignmentOptions.Center);
        manager.feedbackText = Text("Feedback", "Loading training scenario...", new Vector3(0, 2.92f, 5.71f), 0.14f, new Color(0.85f, 0.92f, 0.95f), TextAlignmentOptions.Center);
        manager.progressText = Text("Progress", "WORKTOP [TODO]   HANDS [TODO]", new Vector3(0, 2.18f, 5.71f), 0.13f, new Color(0.4f, 0.9f, 0.66f), TextAlignmentOptions.Center);
        manager.traceLegendText = Text("Trace Legend", "TRACE MODE: MAGENTA = PEANUT ALLERGEN PATH", new Vector3(0, 1.78f, 5.69f), 0.11f, new Color(1f, 0.25f, 0.8f), TextAlignmentOptions.Center);

        SetTextArea(manager.objectiveText, 5.2f, 0.55f);
        SetTextArea(manager.feedbackText, 5.2f, 0.9f);
        SetTextArea(manager.progressText, 5.2f, 0.5f);
        SetTextArea(manager.traceLegendText, 5.2f, 0.35f);
    }

    private static void CreateActionStations(Material[] p)
    {
        Station("CLEAN WORKTOP", new Vector3(-1.75f, 1.65f, 1.35f), TrainingActionStation.ActionType.CleanWorkstation, p[5], "Remove visible residue\nbefore preparation");
        Station("WASH HANDS", new Vector3(0, 1.65f, 1.35f), TrainingActionStation.ActionType.WashHands, p[3], "Control the hand-contact\npathway");
        Station("TRACE MODE", new Vector3(1.75f, 1.65f, 1.35f), TrainingActionStation.ActionType.ToggleTraceMode, p[11], "Reveal otherwise invisible\ncontamination");
        Station("RESET", new Vector3(4.65f, 1.55f, 5.4f), TrainingActionStation.ActionType.ResetTraining, p[4], "Try the procedure again");
    }

    private static void Station(string label, Vector3 position, TrainingActionStation.ActionType action, Material material, string detail)
    {
        var button = Cube(label, position, new Vector3(1.35f, 0.32f, 0.65f), material);
        var interactable = button.AddComponent<InteractableGeneral>();
        interactable.hoverText = label + "\n" + detail + "\nClick to activate";
        interactable.customTouchDistance = 8f;
        button.AddComponent<TrainingActionStation>().action = action;
        var text = Text(label + " Label", label, position + new Vector3(0, 0.18f, -0.34f), 0.11f, Color.white, TextAlignmentOptions.Center);
        SetTextArea(text, 1.25f, 0.28f);
    }

    private static void CreatePreparationArea(Material[] p)
    {
        var zone = Cube("SAFE PREPARATION ZONE", new Vector3(0, 1.56f, 2.0f), new Vector3(2.5f, 0.12f, 1.05f), p[3]);
        zone.GetComponent<BoxCollider>().isTrigger = true;
        zone.AddComponent<PreparationZone>();
        var rb = zone.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        var label = Text("Preparation Zone Label", "PLACE SAFE KNIFE + BREAD + VEGETABLES HERE", new Vector3(0, 1.67f, 1.48f), 0.095f, Color.white, TextAlignmentOptions.Center);
        SetTextArea(label, 2.5f, 0.3f);
    }

    private static void CreateTrainingItems(Material[] p)
    {
        Item("Contaminated Knife", "dirty_knife", "knife used for peanut butter", new Vector3(-4.35f, 1.65f, 3.5f), new Vector3(0.16f, 0.06f, 1.05f), p[4], true);
        Item("Clean Blue Knife", "clean_knife", "clean blue knife", new Vector3(3.95f, 1.65f, 3.45f), new Vector3(0.16f, 0.06f, 1.05f), p[3], false);
        Item("Bread", "bread", "sealed bread", new Vector3(4.45f, 1.72f, 3.6f), new Vector3(0.75f, 0.28f, 0.65f), p[6], false);
        Item("Vegetables", "vegetables", "washed vegetables", new Vector3(4.75f, 1.68f, 3.15f), new Vector3(0.65f, 0.22f, 0.55f), p[7], false);
        Item("Used Chopping Board", "dirty_board", "used chopping board", new Vector3(-3.9f, 1.58f, 3.8f), new Vector3(0.95f, 0.08f, 0.75f), p[10], true, false);
    }

    private static GameObject Item(string name, string id, string displayName, Vector3 position, Vector3 scale, Material material, bool contaminated, bool holdable = true)
    {
        var item = Cube(name, position, scale, material);
        var contaminable = item.AddComponent<ContaminableItem>();
        contaminable.itemId = id;
        contaminable.displayName = displayName;
        contaminable.startsContaminated = contaminated;

        if (holdable)
        {
            var body = item.AddComponent<Rigidbody>();
            body.mass = 0.5f;
            body.collisionDetectionMode = CollisionDetectionMode.Continuous;
            var h = item.AddComponent<Holdable>();
            h.hoverText = "Pick up " + displayName;
            h.heldText = "Carrying " + displayName + " - click to place";
            h.customHoldDistance = 1.3f;
            h.freezeRotation = true;
        }

        return item;
    }

    private static void CreateRiskStoryProps(Material[] p)
    {
        Cube("Peanut Butter Jar", new Vector3(-4.8f, 1.88f, 3.85f), new Vector3(0.38f, 0.6f, 0.38f), p[10]);
        var risk = Text("Risk Context", "PREVIOUS ORDER:\nPEANUT BUTTER TOAST", new Vector3(-4.25f, 2.55f, 5.75f), 0.13f, new Color(1f, 0.56f, 0.22f), TextAlignmentOptions.Center);
        SetTextArea(risk, 2.5f, 0.7f);
        var safe = Text("Safe Context", "SEALED INGREDIENTS\n+ CLEAN BLUE TOOL", new Vector3(4.2f, 2.55f, 5.75f), 0.13f, new Color(0.35f, 0.8f, 1f), TextAlignmentOptions.Center);
        SetTextArea(safe, 2.5f, 0.7f);
    }

    private static GameObject Cube(string name, Vector3 position, Vector3 scale, Material material, Transform parent = null)
    {
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = name;
        cube.transform.SetPositionAndRotation(position, Quaternion.identity);
        cube.transform.localScale = scale;
        if (parent != null) cube.transform.SetParent(parent);
        cube.GetComponent<Renderer>().sharedMaterial = material;
        return cube;
    }

    private static TextMeshPro Text(string name, string content, Vector3 position, float size, Color colour, TextAlignmentOptions alignment)
    {
        var go = new GameObject(name);
        go.transform.position = position;
        // TextMeshPro's front face already points toward the player at negative Z.
        // Rotating it 180 degrees mirrors every label in Game view.
        go.transform.rotation = Quaternion.identity;
        var text = go.AddComponent<TextMeshPro>();
        text.text = content;
        text.fontSize = size * 10f;
        text.color = colour;
        text.alignment = alignment;
        text.textWrappingMode = TextWrappingModes.Normal;
        return text;
    }

    private static void SetTextArea(TMP_Text text, float width, float height)
    {
        text.rectTransform.sizeDelta = new Vector2(width, height);
    }
}
#endif
