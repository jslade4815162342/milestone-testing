using UnityEngine;

/// <summary>
/// Central state machine for the bartending experience.
///
/// States:
///   WaitingForOrder → player reads recipe, picks up glass / bottles
///   MakingDrink     → player is actively pouring
///   Evaluating      → drink placed in serve zone; result displayed briefly
///
/// SETUP:
///   1. Create a GameObject named "GameManager".
///   2. Add this component + RecipeManager + BarAudioManager.
///   3. (Optional) assign Feedback Text — a world-space TextMeshPro that shows
///      "Correct!" / "Wrong ingredients!" after serving.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum State { WaitingForOrder, MakingDrink, Evaluating }
    public State CurrentState { get; private set; }

    [Header("Feedback")]
    [Tooltip("World-space TextMeshPro for result messages. Optional.")]
    public TMPro.TextMeshPro feedbackText;

    [Tooltip("How long the result message stays visible before the next round starts.")]
    public float resultDuration = 3f;

    [Header("Optional")]
    public ParticleSystem successParticles;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        SetFeedback("", Color.white);
        CurrentState = State.WaitingForOrder;
    }

    /// <summary>Called by DrinkGlass whenever an ingredient is poured.</summary>
    public void OnGlassUpdated(DrinkGlass glass)
    {
        if (CurrentState == State.WaitingForOrder)
            CurrentState = State.MakingDrink;
    }

    /// <summary>Called by ServingZone when the player places the glass down.</summary>
    public void OnDrinkServed(DrinkGlass glass)
    {
        if (CurrentState == State.Evaluating) return;
        CurrentState = State.Evaluating;

        bool correct = RecipeManager.Instance?.Validate(glass) ?? false;

        if (correct)
        {
            SetFeedback("✓  Perfect!", Color.green);
            BarAudioManager.Instance?.PlaySuccess();
            successParticles?.Play();
        }
        else
        {
            SetFeedback("✗  Check the recipe and try again!", Color.red);
            BarAudioManager.Instance?.PlayFail();
        }

        glass.Clear();
        Invoke(nameof(NextRound), resultDuration);
    }

    private void NextRound()
    {
        SetFeedback("", Color.white);
        RecipeManager.Instance?.NextRecipe();
        CurrentState = State.WaitingForOrder;
    }

    private void SetFeedback(string msg, Color color)
    {
        if (feedbackText == null) return;
        feedbackText.text  = msg;
        feedbackText.color = color;
    }
}
