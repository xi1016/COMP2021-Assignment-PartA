using RecipeManagement.Core;

namespace RecipeManagement.Tests;

public sealed class RecipeCatalogueTests
{
    [Fact]
    public void AddRecipe_AddsNewRecipe()
    {
        var manager = new RecipeManager(Array.Empty<Recipe>());
        var recipe = new Recipe { Id = 100, Title = "Test Soup" };

        Assert.True(manager.AddRecipe(recipe));
        Assert.Equal(1, manager.RecipeCount);
        Assert.Equal("Test Soup", manager.FindRecipe(100)?.Title);
    }

    [Fact]
    public void AddRecipe_DuplicateId_ReturnsFalse()
    {
        var manager = new RecipeManager(Array.Empty<Recipe>());
        manager.AddRecipe(new Recipe { Id = 100, Title = "Test Soup" });

        Assert.False(manager.AddRecipe(new Recipe { Id = 100, Title = "Another Soup" }));
        Assert.Equal(1, manager.RecipeCount);
    }

    [Fact]
    public void AddRecipe_NonPositiveId_ReturnsFalse()
    {
        var manager = new RecipeManager(Array.Empty<Recipe>());

        Assert.False(manager.AddRecipe(new Recipe { Id = 0, Title = "Bad Id" }));
    }

    [Fact]
    public void RemoveRecipe_ExistingId_RemovesAndReturnsTrue()
    {
        var manager = new RecipeManager(Array.Empty<Recipe>());
        manager.AddRecipe(new Recipe { Id = 100, Title = "Test Soup" });

        Assert.True(manager.RemoveRecipe(100));
        Assert.Null(manager.FindRecipe(100));
    }
}
