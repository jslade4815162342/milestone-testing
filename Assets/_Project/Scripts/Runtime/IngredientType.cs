/// <summary>
/// Every ingredient available in the simulator.
/// Add new values here to introduce new bottles.
/// Must match the IngredientType set on each BottleInteractable in the scene.
/// </summary>
public enum IngredientType
{
    // Spirits
    Vodka,
    Rum,
    Gin,
    Tequila,
    Whiskey,
    Bourbon,

    // Liqueurs
    TripleSec,
    BlueCuracao,
    Kahlua,

    // Juices & Mixers
    LimeJuice,
    LemonJuice,
    OrangeJuice,
    CranberryJuice,
    PineappleJuice,
    CoconutCream,
    SodaWater,
    TonicWater,
    GingerBeer,

    // Syrups
    SimpleSyrup,
    GrenadineSyrup,
    AgaveNectar,

    // Other
    Ice,
    Salt,
    Sugar,
    Mint,
    Vermouth,
}
