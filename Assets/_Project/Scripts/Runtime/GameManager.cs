using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum State { WaitingForOrder, MakingDrink, Evaluating }
    public State CurrentState { get; private set; }

    [Header("Feedback")]
    public TMPro.TextMeshPro feedbackText;
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

    public void OnGlassUpdated(DrinkGlass glass)
    {
        if (CurrentState == State.WaitingForOrder)
            CurrentState = State.MakingDrink;
    }

    public void OnDrinkServed(DrinkGlass glass)
    {
        if (CurrentState == State.Evaluating) return;
        CurrentState = State.Evaluating;

        bool correct = RecipeManager.Instance?.Validate(glass) ?? false;

        if (correct)
        {
            SetFeedback("✓  Perfect!", Color.green);
            successParticles?.Play();
        }
        else
        {
            SetFeedback("✗  Check the recipe and try again!", Color.red);
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