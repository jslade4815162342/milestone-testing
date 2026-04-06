using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// One-click scene setup for the VR Bartending Simulator (milestone 2 scene).
///
/// Run in this order:
///   Tools ▶ Scene Setup ▶ Step 1 – Create GameManager
///   Tools ▶ Scene Setup ▶ Step 2 – Create Recipe Canvas
///   Tools ▶ Scene Setup ▶ Step 3 – Add Components to Glasses and Bottles
///   Tools ▶ Scene Setup ▶ Step 4 – Create Serving Zone
///   ── or ──
///   Tools ▶ Scene Setup ▶ Run All Steps
///
/// NOTE: The bar, glasses, and bottles are already in the scene placed by
/// the team. Steps here only ADD missing components — nothing is removed or moved.
/// </summary>
public static class SceneBootstrapper
{
    // Ingredient type assigned to each bottle by its GameObject name.
    // Adjust these if you rename bottles in the hierarchy.
    private static IngredientType GuessIngredient(string goName, int fallbackIndex)
    {
        var n = goName.ToLower();
        if (n.Contains("water"))  return IngredientType.SodaWater;
        if (n.Contains("wine"))   return IngredientType.Vodka;
        if (n.Contains("beer"))   return IngredientType.Rum;
        if (n.Contains("champ"))  return IngredientType.Tequila;

        // For generic "Bottle 12", "Bottle 14", etc. assign by order
        IngredientType[] defaults =
        {
            IngredientType.Vodka, IngredientType.Rum,
            IngredientType.Tequila, IngredientType.Gin,
            IngredientType.OrangeJuice, IngredientType.LimeJuice,
        };
        return defaults[fallbackIndex % defaults.Length];
    }

    // ── Run All ───────────────────────────────────────────────────────────────
    [MenuItem("Tools/Scene Setup/Run All Steps")]
    public static void RunAll()
    {
        CreateGameManager();
        CreateRecipeCanvas();
        AddComponentsToGlassesAndBottles();
        CreateServingZone();
        Debug.Log("[SceneBootstrapper] ✅ All steps done. " +
                  "Select GameManager and drag DrinkRecipe assets into RecipeManager → Recipes.");
    }

    // ── Step 1 ────────────────────────────────────────────────────────────────
    [MenuItem("Tools/Scene Setup/Step 1 - Create GameManager")]
    public static void CreateGameManager()
    {
        if (GameObject.Find("GameManager") != null)
        {
            Debug.Log("[SceneBootstrapper] GameManager already exists — skipping.");
            return;
        }

        var go = new GameObject("GameManager");
        Undo.RegisterCreatedObjectUndo(go, "Create GameManager");
        go.AddComponent<GameManager>();
        go.AddComponent<RecipeManager>();
        go.AddComponent<BarAudioManager>();

        Selection.activeGameObject = go;
        Debug.Log("[SceneBootstrapper] ✅ GameManager created. " +
                  "Select it → drag DrinkRecipe assets into RecipeManager → Recipes.");
    }

    // ── Step 2 ────────────────────────────────────────────────────────────────
    [MenuItem("Tools/Scene Setup/Step 2 - Create Recipe Canvas")]
    public static void CreateRecipeCanvas()
    {
        if (GameObject.Find("RecipeCanvas") != null)
        {
            Debug.Log("[SceneBootstrapper] RecipeCanvas already exists — skipping.");
            return;
        }

        // Canvas positioned at eye-level to the right of the XR Origin (1.661, 0.119, -8.87)
        var canvasGO = new GameObject("RecipeCanvas");
        Undo.RegisterCreatedObjectUndo(canvasGO, "Create Recipe Canvas");
        canvasGO.transform.position = new Vector3(4f, 1.55f, -8.15f);
        canvasGO.transform.rotation = Quaternion.Euler(0f, 180f, 0f); // faces the player

        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;

        var rt = canvasGO.GetComponent<RectTransform>();
        rt.sizeDelta  = new Vector2(600, 450);
        rt.localScale = Vector3.one * 0.0018f;

        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        // VR controller pointer support
        var trgType = System.Type.GetType(
            "UnityEngine.XR.Interaction.Toolkit.UI.TrackedDeviceGraphicRaycaster, " +
            "Unity.XR.Interaction.Toolkit");
        if (trgType != null)
        {
            canvasGO.AddComponent(trgType);
            canvasGO.GetComponent<GraphicRaycaster>().enabled = false;
        }

        // Dark background
        var bgGO = new GameObject("Background");
        bgGO.transform.SetParent(canvasGO.transform, false);
        var bgImg = bgGO.AddComponent<Image>();
        bgImg.color = new Color(0.05f, 0.03f, 0.01f, 0.88f);
        var bgRT = bgImg.rectTransform;
        bgRT.anchorMin = Vector2.zero;
        bgRT.anchorMax = Vector2.one;
        bgRT.offsetMin = bgRT.offsetMax = Vector2.zero;

        // Drink name title
        var titleGO  = new GameObject("RecipeTitle");
        titleGO.transform.SetParent(canvasGO.transform, false);
        var titleTMP = titleGO.AddComponent<TextMeshProUGUI>();
        titleTMP.text      = "Drink Name";
        titleTMP.fontSize  = 72;
        titleTMP.fontStyle = FontStyles.Bold;
        titleTMP.alignment = TextAlignmentOptions.Center;
        titleTMP.color     = new Color(1f, 0.85f, 0.4f);
        var titleRT = titleTMP.rectTransform;
        titleRT.anchoredPosition = new Vector2(0, 145);
        titleRT.sizeDelta        = new Vector2(560, 100);

        // Thin divider
        var divGO = new GameObject("Divider");
        divGO.transform.SetParent(canvasGO.transform, false);
        var divImg = divGO.AddComponent<Image>();
        divImg.color = new Color(1f, 0.85f, 0.4f, 0.5f);
        var divRT = divImg.rectTransform;
        divRT.anchoredPosition = new Vector2(0, 90);
        divRT.sizeDelta        = new Vector2(520, 3);

        // Ingredient list
        var ingredGO  = new GameObject("RecipeIngredients");
        ingredGO.transform.SetParent(canvasGO.transform, false);
        var ingredTMP = ingredGO.AddComponent<TextMeshProUGUI>();
        ingredTMP.text      = "- Ingredient 1\n- Ingredient 2";
        ingredTMP.fontSize  = 52;
        ingredTMP.alignment = TextAlignmentOptions.Left;
        ingredTMP.color     = Color.white;
        var ingredRT = ingredTMP.rectTransform;
        ingredRT.anchoredPosition = new Vector2(20, -30);
        ingredRT.sizeDelta        = new Vector2(540, 300);

        // Wire up RecipeDisplay (Lena's script)
        var rd = canvasGO.AddComponent<RecipeDisplay>();
        var so = new SerializedObject(rd);
        so.FindProperty("recipeTitle").objectReferenceValue       = titleTMP;
        so.FindProperty("recipeIngredients").objectReferenceValue = ingredTMP;
        so.ApplyModifiedProperties();

        Selection.activeGameObject = canvasGO;
        Debug.Log("[SceneBootstrapper] ✅ RecipeCanvas created. " +
                  "Move/rotate it in the scene view if the position feels off.");
    }

    // ── Step 3 ────────────────────────────────────────────────────────────────
    /// <summary>
    /// Walks every child of the "Glasses" parent and adds the right component:
    ///   • Name contains "Glass" or "Cup"  → DrinkGlass + sphere trigger collider
    ///   • Name contains "Bottle" or "Bottled" → BottleInteractable
    /// Does NOT touch existing XRGrabInteractable or Rigidbody — those are already correct.
    /// </summary>
    [MenuItem("Tools/Scene Setup/Step 3 - Add Components to Glasses and Bottles")]
    public static void AddComponentsToGlassesAndBottles()
    {
        var glassesParent = GameObject.Find("Glasses");
        if (glassesParent == null)
        {
            Debug.LogWarning("[SceneBootstrapper] No 'Glasses' object found. " +
                             "Make sure milestone 2 is the active scene.");
            return;
        }

        int glassCount  = 0;
        int bottleCount = 0;
        int bottleIndex = 0;

        foreach (Transform child in glassesParent.transform)
        {
            var go   = child.gameObject;
            var name = go.name;

            bool isGlass  = name.Contains("Glass") || name.Contains("Cup");
            bool isBottle = name.Contains("Bottle") || name.Contains("Bottled");

            // ── GLASS ─────────────────────────────────────────────────────────
            if (isGlass)
            {
                // DrinkGlass tracks what's been poured in
                if (go.GetComponent<DrinkGlass>() == null)
                    Undo.AddComponent<DrinkGlass>(go);

                // Trigger sphere collider above the rim — detects tilted bottles
                bool hasTrigger = false;
                foreach (var col in go.GetComponents<Collider>())
                    if (col.isTrigger) { hasTrigger = true; break; }

                if (!hasTrigger)
                {
                    var sphere       = Undo.AddComponent<SphereCollider>(go);
                    sphere.isTrigger = true;
                    sphere.radius    = 0.12f;
                    sphere.center    = new Vector3(0f, 0.1f, 0f); // just above the rim
                }

                glassCount++;
                Debug.Log($"[SceneBootstrapper]  Glass setup: {name}");
            }

            // ── BOTTLE ────────────────────────────────────────────────────────
            else if (isBottle)
            {
                var bi = go.GetComponent<BottleInteractable>();
                if (bi == null)
                {
                    bi = Undo.AddComponent<BottleInteractable>(go);
                    bi.ingredientType = GuessIngredient(name, bottleIndex);
                }

                bottleCount++;
                bottleIndex++;
                Debug.Log($"[SceneBootstrapper]  Bottle setup: {name} → {bi.ingredientType}");
            }
        }

        Debug.Log($"[SceneBootstrapper] ✅ Done — {glassCount} glasses, {bottleCount} bottles set up.\n" +
                  "⚠  Check each bottle in the Inspector: select it → BottleInteractable → " +
                  "change Ingredient Type if the auto-assigned one is wrong.");
    }

    // ── Step 4 ────────────────────────────────────────────────────────────────
    [MenuItem("Tools/Scene Setup/Step 4 - Create Serving Zone")]
    public static void CreateServingZone()
    {
        if (GameObject.Find("ServingZone") != null)
        {
            Debug.Log("[SceneBootstrapper] ServingZone already exists — skipping.");
            return;
        }

        // Place at the far right of the bar counter so the player slides the
        // finished drink there to "serve" it.
        var go = new GameObject("ServingZone");
        Undo.RegisterCreatedObjectUndo(go, "Create Serving Zone");

        // Use roughly the bar counter's world position — adjust X/Z if needed
        go.transform.position = new Vector3(7.2f, 1.3f, -7.8f);

        var col       = go.AddComponent<BoxCollider>();
        col.isTrigger = true;
        col.size      = new Vector3(0.6f, 0.4f, 0.6f);

        go.AddComponent<ServingZone>();

        // Green highlight quad so the player can see the serve spot
        var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        Undo.RegisterCreatedObjectUndo(quad, "Serving Zone Highlight");
        quad.name = "Highlight";
        quad.transform.SetParent(go.transform, false);
        quad.transform.localPosition = new Vector3(0f, -0.19f, 0f);
        quad.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        quad.transform.localScale    = new Vector3(0.55f, 0.55f, 1f);
        Object.DestroyImmediate(quad.GetComponent<MeshCollider>());

        var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        if (mat != null)
        {
            mat.color = new Color(0.1f, 1f, 0.1f, 0.4f);
            if (mat.HasProperty("_Surface")) mat.SetFloat("_Surface", 1);
            mat.renderQueue = 3000;
            quad.GetComponent<Renderer>().material = mat;
        }

        // Wire the highlight quad into the ServingZone component
        var sz = go.GetComponent<ServingZone>();
        var szSO = new SerializedObject(sz);
        szSO.FindProperty("highlightVisual").objectReferenceValue = quad;
        szSO.ApplyModifiedProperties();

        Selection.activeGameObject = go;
        Debug.Log("[SceneBootstrapper] ✅ ServingZone placed at (7.2, 1.3, -7.8). " +
                  "A green square marks the serve spot on the bar. " +
                  "Drag it in the scene view if it needs repositioning.");
    }
}
