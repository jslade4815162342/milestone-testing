# VR Bartending Simulator — Project Guide

**Course:** CPSC 4110 / 6110
**Team:** Lena Blankenbaker, Alexander Newby, Sofia Gray, Jude Slade
**Platform:** Meta Quest 3 (Android / OpenXR)
**Engine:** Unity + URP + XR Interaction Toolkit 3.x

---

## Project Overview

A VR bartending training simulator. The player stands behind a medieval-style tavern bar,
reads a drink recipe on a world-space canvas, grabs ingredient bottles, pours them into
a glass, then places the glass in a serving zone to validate the order. The loop repeats
with a new recipe.

**In scope for M3:**
- Bar environment (fixed layout — built by BarBuilder editor tool)
- Grab & pour mechanics (BottleInteractable → DrinkGlass)
- Recipe display UI (world-space canvas — RecipeDisplay.cs by Lena)
- Recipe validation (GameManager / RecipeManager)
- Ambient + SFX audio (BarAudioManager)
- Serve zone (ServingZone trigger on bar counter)

**Out of scope:**
- Liquid simulation → replaced by colour-fill cylinder inside the glass
- Multiplayer
- Complex physics (splashing, spilling)
- Hand-tracking (controller grip only)

---

## What Each Teammate Added

| Commit | Author | What it does |
|--------|--------|--------------|
| Added glassware assets… | Lena | Imported LowpolyDrinkGlasses pack; placed glasses in scene |
| Added VR recipe UI… | Lena | World-space Canvas + RecipeDisplay.cs (4 hardcoded recipes, Next button) |

---

## Runtime Script Summary

| Script | Purpose |
|--------|---------|
| `IngredientType.cs` | Enum of all ingredients |
| `DrinkRecipe.cs` | ScriptableObject — name, ingredients list, liquid colour |
| `BottleInteractable.cs` | Detects bottle tilt → pours ingredient into glass |
| `DrinkGlass.cs` | Tracks poured ingredients; drives liquid-fill visual |
| `ServingZone.cs` | Trigger on counter — submits drink to GameManager |
| `RecipeManager.cs` | Holds DrinkRecipe list; picks current recipe; validates |
| `GameManager.cs` | State machine: WaitingForOrder → MakingDrink → Evaluating |
| `BarAudioManager.cs` | SFX + ambient audio singleton |
| `RecipeDisplay.cs` | *(Lena's)* Drives the world-space recipe canvas text |
| `XROptimizationManager.cs` | Quest 3 performance settings (refresh rate, foveation) |

Editor tools live in `Assets/_Project/Scripts/Editor/`:
- `BarBuilder.cs` — `Tools → Bar Builder → Build Bar / Clear Bar`
- `SceneHierarchyOrganizer.cs` — `Tools → Organize Scene Hierarchy`
- `GitYAMLMergeSetup.cs` — auto-configures git YAML merge on startup

---

## How to Add a Drink Recipe

1. Right-click in Project → **Create → Bartending → Drink Recipe**
2. Name it (e.g. `Margarita`)
3. Fill in **Drink Name**, **Description**, and **Required Ingredients**
4. Drag into **RecipeManager → Recipes** in the Inspector

| Drink        | Ingredients |
|--------------|-------------|
| Margarita    | Tequila, TripleSec, LimeJuice, Salt |
| Cosmopolitan | Vodka, TripleSec, CranberryJuice, LimeJuice |
| Vodka Soda   | Vodka, SodaWater, Ice |
| Screwdriver  | Vodka, OrangeJuice |
| Cuba Libre   | Rum, SodaWater, LimeJuice |

---

## How to Set Up a Bottle

1. Place bottle model in scene.
2. Add **XRGrabInteractable**.
3. Add **BottleInteractable** → set **Ingredient Type**.
4. Add a non-trigger **Collider** for physics.
5. *(Optional)* Attach a Particle System to the spout → assign to **Pour Particles**.

## How to Set Up a Glass

1. Place glass model (from `LowpolyDrinkGlasses/Prefabs/`).
2. Add **XRGrabInteractable**.
3. Add **DrinkGlass** → set **Max Ingredients**.
4. Add a **Sphere Collider** → **Is Trigger ✓** — centre it just above the rim (r ≈ 0.12).
5. *(Optional)* Add a child Cylinder inside the glass (Y scale = 0) → assign to **Liquid Visual**.

## How to Set Up the Serve Zone

1. Create empty child on bar counter.
2. Add **Box Collider** → **Is Trigger ✓**.
3. Add **ServingZone** component.
4. *(Optional)* Assign a flat transparent quad as **Highlight Visual**.

## How to Build the Bar

```
Tools → Bar Builder → Build Bar
Tools → Bar Builder → Clear Bar   (remove and start over)
```

The script places bar counter sections (Bar_01_mod) with corner caps (Bar_01_corner)
along the back wall. Select the resulting **Bar** GameObject to reposition if needed.
Only the counter is placed — no extra props — so the scene stays clean.

---

## Scene Hierarchy Convention

```
--- MANAGEMENT ---
    GameManager          ← GameManager + RecipeManager + BarAudioManager
    EventSystem
--- ENVIRONMENT ---
    Room
    BarFurniture
        Bar              ← built by BarBuilder
--- XR ---
    XR Origin
    XR Interaction Manager
--- UI ---
    Canvas               ← Lena's world-space recipe canvas (RecipeDisplay)
    FeedbackText         ← TextMeshPro wired to GameManager.feedbackText
```

---

## Fixing Pink / Purple Objects

Pink = shader mismatch (Built-in RP or HDRP material in a URP project).

**Quick fix (one object):**
1. Select the object → Inspector → Materials → click the material.
2. Change **Shader** to `Universal Render Pipeline/Lit`.
3. Re-assign the Albedo/Base Map texture if it cleared.

**Batch fix:**
`Edit → Rendering → Materials → Convert Selected Built-in Materials to URP`
*(Select affected GameObjects first.)*

**Known problem packs:**
- `LeartesStudios/CoffeeShopInterior/HDRP/` — use the non-HDRP folder or convert.
- `EasyAssets/Korean_Style_Bar/` — needs Built-in → URP conversion.
- `BarProps/` — DAE imports can lose materials; re-apply from `Assets/BarProps/Materials/`.

---

## Build & Deploy to Quest

```
File → Build Settings
  Platform : Android
  Texture Compression : ASTC
  Run Device : Meta Quest 3

Player Settings → XR Plug-in Management → Android → OpenXR ✓
Player Settings → Other Settings → Minimum API Level : 29
```

1. Enable Developer Mode on headset (Meta app → headset settings).
2. Connect via USB-C.
3. **Build and Run** — Unity pushes the `.apk` directly.

---

## Known Issues (M3)

| Issue | Status | Notes |
|-------|--------|-------|
| No liquid simulation | By design | Colour-fill cylinder used instead |
| Pink materials (some packs) | Known | See "Fixing Pink / Purple Objects" |
| Player height not adjustable | Pending | Keep bar counter at ~1.0 m world Y |
| RecipeDisplay hardcoded | Acceptable for M3 | Integrate with RecipeManager in M4 |
| No tutorial / onboarding | Out of scope for M3 | Planned for M4 |

---

## Team Git Workflow

- `Assets/_Project/Scripts/Runtime/` — gameplay scripts
- `Assets/_Project/Scripts/Editor/` — editor tools only (not shipped in build)
- `Assets/_Project/ScriptableObjects/Recipes/` — DrinkRecipe assets
- `Assets/RecipeDisplay.cs` — Lena's recipe UI script (keep at root for now)
- Scene files committed via git — UnityYAMLMerge auto-configured (`GitYAMLMergeSetup.cs`)
- **Always pull before starting work** to avoid overwriting teammates' changes
