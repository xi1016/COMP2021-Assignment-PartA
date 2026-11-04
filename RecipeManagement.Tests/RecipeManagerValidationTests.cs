using RecipeManagement.Core;

namespace RecipeManagement.Tests;

public sealed class RecipeManagerValidationTests
{
    private static Recipe CreateRecipe(int id, string title)
    {
        return new Recipe
        {
            Id = id,
            Title = title,
            Ingredients = new List<string> { "Ingredient" },
            Instructions = new List<string> { "Instruction" }
        };
    }

    [Fact]
    public void Constructor_NullCollection_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new RecipeManager(null!));
    }

    [Fact]
    public void Constructor_NullEntryInCollection_ThrowsArgumentException()
    {
        var recipes = new List<Recipe> { CreateRecipe(1, "Soup"), null! };

        Assert.Throws<ArgumentException>(() => new RecipeManager(recipes));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Constructor_NonPositiveId_ThrowsArgumentException(int id)
    {
        var recipes = new[] { CreateRecipe(id, "Soup") };

        Assert.Throws<ArgumentException>(() => new RecipeManager(recipes));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_BlankTitle_ThrowsArgumentException(string title)
    {
        var recipes = new[] { CreateRecipe(1, title) };

        Assert.Throws<ArgumentException>(() => new RecipeManager(recipes));
    }

    [Fact]
    public void Constructor_DuplicateId_ThrowsArgumentException()
    {
        var recipes = new[]
        {
            CreateRecipe(1, "Soup"),
            CreateRecipe(1, "Stew")
        };

        Assert.Throws<ArgumentException>(() => new RecipeManager(recipes));
    }

    [Fact]
    public void Constructor_ValidRecipes_KeepsEveryEntry()
    {
        var recipes = new[]
        {
            CreateRecipe(1, "Soup"),
            CreateRecipe(2, "Stew"),
            CreateRecipe(3, "Pie")
        };

        var manager = new RecipeManager(recipes);

        Assert.Equal(3, manager.RecipeCount);
        Assert.Equal("Stew", manager.FindRecipe(2)?.Title);
        Assert.Equal("Pie", manager.FindRecipe(3)?.Title);
    }

    [Fact]
    public void AddRecipe_NullRecipe_ThrowsArgumentNullException()
    {
        var manager = new RecipeManager(Array.Empty<Recipe>());

        Assert.Throws<ArgumentNullException>(() => manager.AddRecipe(null!));
        Assert.Equal(0, manager.RecipeCount);
    }
}
