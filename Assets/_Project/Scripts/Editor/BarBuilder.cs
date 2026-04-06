using UnityEditor;
using UnityEngine;

/// <summary>
/// Builds a clean bar counter along the back wall of the tavern.
///
/// Menu:  Tools ▶ Bar Builder ▶ Build Bar
///        Tools ▶ Bar Builder ▶ Clear Bar
///
/// Keeps it simple — just the counter (corner caps + straight sections).
/// No lamps, cupboards, or stools so the scene stays uncluttered.
/// Adjust the world-space constants below if the counter needs to move.
/// </summary>
public static class BarBuilder
{
    // ── Prefab paths ──────────────────────────────────────────────────────────
    private const string MOD_PATH    = "Assets/MedievalTavernPack/Prefabs/Furniture/Bar_01_mod.prefab";
    private const string CORNER_PATH = "Assets/MedievalTavernPack/Prefabs/Furniture/Bar_01_corner.prefab";

    // ── Layout (world-space) ──────────────────────────────────────────────────
    // Room is ~10 × 10 units; floor at Y ≈ 0.06.
    // XR Origin (player) is at Z ≈ -8.87 facing +Z (toward the room).
    // Bar sits in front of the player at Z = -7.8, giving ~0.77 m of space
    // behind the counter for the bartender to move around.
    private const float FLOOR_Y       =  0.06f;
    private const float BAR_Z         = -7.8f;   // ~1 m in front of XR Origin
    private const float BAR_START_X   =  2.0f;   // left edge
    private const float BAR_END_X     =  8.0f;   // right edge
    private const float SECTION_WIDTH =  1.0f;   // Bar_01_mod is ~1 u wide

    // ─────────────────────────────────────────────────────────────────────────
    [MenuItem("Tools/Bar Builder/Build Bar")]
    public static void BuildBar()
    {
        // Don't double-build
        if (GameObject.Find("Bar") != null)
        {
            bool rebuild = EditorUtility.DisplayDialog(
                "Bar already exists",
                "A 'Bar' GameObject is already in the scene.\nClear it and rebuild?",
                "Clear & Rebuild", "Cancel");
            if (!rebuild) return;
            ClearBar();
        }

        // Load prefabs
        var modPrefab    = AssetDatabase.LoadAssetAtPath<GameObject>(MOD_PATH);
        var cornerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(CORNER_PATH);

        if (modPrefab == null || cornerPrefab == null)
        {
            Debug.LogError("[BarBuilder] Could not find MedievalTavernPack bar prefabs. " +
                           "Check that the pack is imported and paths match.");
            return;
        }

        // Root container — child of BarFurniture if it exists
        var barRoot = new GameObject("Bar");
        Undo.RegisterCreatedObjectUndo(barRoot, "Build Bar");

        var furniture = GameObject.Find("BarFurniture");
        if (furniture != null)
            barRoot.transform.SetParent(furniture.transform, worldPositionStays: true);

        // ── Left corner cap ───────────────────────────────────────────────────
        Place(cornerPrefab, barRoot.transform,
              new Vector3(BAR_START_X, FLOOR_Y, BAR_Z),
              Quaternion.Euler(0f, -90f, 0f));

        // ── Straight sections ─────────────────────────────────────────────────
        int count = Mathf.RoundToInt((BAR_END_X - BAR_START_X) / SECTION_WIDTH) - 1;
        for (int i = 1; i <= count; i++)
        {
            float x = BAR_START_X + i * SECTION_WIDTH;
            Place(modPrefab, barRoot.transform,
                  new Vector3(x, FLOOR_Y, BAR_Z),
                  Quaternion.identity);
        }

        // ── Right corner cap ──────────────────────────────────────────────────
        Place(cornerPrefab, barRoot.transform,
              new Vector3(BAR_END_X, FLOOR_Y, BAR_Z),
              Quaternion.Euler(0f, 0f, 0f));

        // Select & highlight in hierarchy
        Selection.activeGameObject = barRoot;
        EditorGUIUtility.PingObject(barRoot);

        Debug.Log($"[BarBuilder] Bar built — {barRoot.transform.childCount} pieces. " +
                  "If the counter clips the wall or faces the wrong way, select the 'Bar' " +
                  "object and nudge its Z or rotate 180° on Y.");
    }

    // ─────────────────────────────────────────────────────────────────────────
    [MenuItem("Tools/Bar Builder/Clear Bar")]
    public static void ClearBar()
    {
        var bar = GameObject.Find("Bar");
        if (bar == null) { Debug.Log("[BarBuilder] No 'Bar' found."); return; }
        Undo.DestroyObjectImmediate(bar);
        Debug.Log("[BarBuilder] Bar cleared.");
    }

    // ── Helper ────────────────────────────────────────────────────────────────
    private static void Place(GameObject prefab, Transform parent, Vector3 pos, Quaternion rot)
    {
        var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        Undo.RegisterCreatedObjectUndo(go, "Build Bar");
        go.transform.position = pos;
        go.transform.rotation = rot;
        go.transform.SetParent(parent, worldPositionStays: true);
    }
}
