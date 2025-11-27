using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class LevelGenerator
{
    private const float CorridorLength = 20f;
    private const float CorridorWidth = 3f;
    private const float CorridorHeight = 3f;
    private const float WallThickness = 0.3f;

    [MenuItem("Tools/Generate Level_Main")]
    public static void GenerateLevelMain()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        CreateDirectionalLight();
        var player = CreatePlayerStart();
        CreateCorridor();

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/Level_Main.unity");
        Debug.Log("Level_Main.unity created successfully!");
    }

    private static void CreateDirectionalLight()
    {
        var lightGO = new GameObject("Directional Light");
        var light = lightGO.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 0.3f;
        light.color = new Color(0.6f, 0.7f, 0.9f); // Cold blue-ish color
        light.shadows = LightShadows.Soft;
        lightGO.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
    }

    private static GameObject CreatePlayerStart()
    {
        var player = new GameObject("PlayerStart");
        player.transform.position = new Vector3(0f, 1f, 0f);

        // Flashlight placeholder
        var flashlightGO = new GameObject("Flashlight");
        flashlightGO.transform.SetParent(player.transform);
        flashlightGO.transform.localPosition = new Vector3(0.3f, 0f, 0.5f);

        var flashlight = flashlightGO.AddComponent<Light>();
        flashlight.type = LightType.Spot;
        flashlight.intensity = 2f;
        flashlight.range = 15f;
        flashlight.spotAngle = 45f;
        flashlight.innerSpotAngle = 25f;
        flashlight.color = new Color(1f, 0.95f, 0.8f); // Warm flashlight color
        flashlight.shadows = LightShadows.Soft;

        return player;
    }

    private static void CreateCorridor()
    {
        var corridorParent = new GameObject("Corridor");

        // Floor
        CreatePrimitive("Floor", corridorParent.transform,
            new Vector3(0f, 0f, CorridorLength / 2f),
            new Vector3(CorridorWidth, WallThickness, CorridorLength),
            new Color(0.15f, 0.15f, 0.15f));

        // Ceiling
        CreatePrimitive("Ceiling", corridorParent.transform,
            new Vector3(0f, CorridorHeight, CorridorLength / 2f),
            new Vector3(CorridorWidth, WallThickness, CorridorLength),
            new Color(0.2f, 0.2f, 0.22f));

        // Left Wall
        CreatePrimitive("Wall_Left", corridorParent.transform,
            new Vector3(-CorridorWidth / 2f - WallThickness / 2f, CorridorHeight / 2f, CorridorLength / 2f),
            new Vector3(WallThickness, CorridorHeight, CorridorLength),
            new Color(0.25f, 0.22f, 0.2f));

        // Right Wall
        CreatePrimitive("Wall_Right", corridorParent.transform,
            new Vector3(CorridorWidth / 2f + WallThickness / 2f, CorridorHeight / 2f, CorridorLength / 2f),
            new Vector3(WallThickness, CorridorHeight, CorridorLength),
            new Color(0.25f, 0.22f, 0.2f));

        // Back Wall (behind player)
        CreatePrimitive("Wall_Back", corridorParent.transform,
            new Vector3(0f, CorridorHeight / 2f, -WallThickness / 2f),
            new Vector3(CorridorWidth + WallThickness * 2f, CorridorHeight, WallThickness),
            new Color(0.2f, 0.18f, 0.18f));

        // Front Wall (end of corridor)
        CreatePrimitive("Wall_Front", corridorParent.transform,
            new Vector3(0f, CorridorHeight / 2f, CorridorLength + WallThickness / 2f),
            new Vector3(CorridorWidth + WallThickness * 2f, CorridorHeight, WallThickness),
            new Color(0.2f, 0.18f, 0.18f));

        // Add doors along the corridor
        CreateDoor("Door_1", corridorParent.transform, 5f, true);
        CreateDoor("Door_2", corridorParent.transform, 10f, false);
        CreateDoor("Door_3", corridorParent.transform, 15f, true);
    }

    private static void CreateDoor(string name, Transform parent, float zPosition, bool leftSide)
    {
        float doorWidth = 1f;
        float doorHeight = 2.2f;
        float doorDepth = 0.1f;

        float xPos = leftSide
            ? -CorridorWidth / 2f - WallThickness / 2f + doorDepth / 2f
            : CorridorWidth / 2f + WallThickness / 2f - doorDepth / 2f;

        // Door frame (slightly larger than door)
        var frameGO = CreatePrimitive(name + "_Frame", parent,
            new Vector3(xPos, doorHeight / 2f + 0.05f, zPosition),
            new Vector3(doorDepth + 0.05f, doorHeight + 0.1f, doorWidth + 0.1f),
            new Color(0.12f, 0.1f, 0.08f));

        // Door panel
        var doorGO = CreatePrimitive(name, parent,
            new Vector3(xPos + (leftSide ? 0.02f : -0.02f), doorHeight / 2f, zPosition),
            new Vector3(doorDepth, doorHeight, doorWidth),
            new Color(0.35f, 0.25f, 0.15f));

        // Small light above door
        var doorLightGO = new GameObject(name + "_Light");
        doorLightGO.transform.SetParent(parent);
        doorLightGO.transform.position = new Vector3(
            leftSide ? -CorridorWidth / 2f + 0.3f : CorridorWidth / 2f - 0.3f,
            CorridorHeight - 0.3f,
            zPosition);

        var doorLight = doorLightGO.AddComponent<Light>();
        doorLight.type = LightType.Point;
        doorLight.intensity = 0.5f;
        doorLight.range = 3f;
        doorLight.color = new Color(1f, 0.6f, 0.3f); // Warm orange
    }

    private static GameObject CreatePrimitive(string name, Transform parent, Vector3 position, Vector3 scale, Color color)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        go.transform.SetParent(parent);
        go.transform.position = position;
        go.transform.localScale = scale;

        // Create a simple material
        var renderer = go.GetComponent<Renderer>();
        var material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        material.color = color;
        renderer.sharedMaterial = material;

        return go;
    }
}

