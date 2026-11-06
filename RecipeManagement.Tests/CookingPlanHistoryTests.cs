using RecipeManagement.Core;

namespace RecipeManagement.Tests;

public sealed class CookingPlanHistoryTests
{
    private static Recipe CreateRecipe(int id)
    {
        return new Recipe
        {
            Id = id,
            Title = $"Recipe {id}",
            Ingredients = new List<string> { "Ingredient" },
            Instructions = new List<string> { "Instruction" }
        };
    }

    [Fact]
    public void RemoveRecipeFromCookingPlan_WhenNotPlanned_ReturnsFalseAndKeepsHistoryEmpty()
    {
        var manager = new RecipeManager(new[] { CreateRecipe(1) });

        Assert.False(manager.RemoveRecipeFromCookingPlan(1));
        Assert.Equal(0, manager.CookingPlanCount);
        Assert.Equal(0, manager.RemovedRecipeCount);
        Assert.Null(manager.PeekLastRemovedRecipe());
    }

    [Fact]
    public void RemoveRecipeFromCookingPlan_WhenPlanned_PushesIdOntoHistory()
    {
        var manager = new RecipeManager(new[] { CreateRecipe(1), CreateRecipe(2) });
        manager.AddRecipeToCookingPlan(1);
        manager.AddRecipeToCookingPlan(2);

        Assert.True(manager.RemoveRecipeFromCookingPlan(1));

        Assert.Equal(new[] { 2 }, manager.GetCookingPlan());
        Assert.Equal(1, manager.RemovedRecipeCount);
        Assert.Equal(1, manager.PeekLastRemovedRecipe());
    }

    [Fact]
    public void RemoveRecipeFromCookingPlan_MultipleRemovals_PeekReturnsMostRecent()
    {
        var manager = new RecipeManager(new[] { CreateRecipe(1), CreateRecipe(2) });
        manager.AddRecipeToCookingPlan(1);
        manager.AddRecipeToCookingPlan(2);

        manager.RemoveRecipeFromCookingPlan(1);
        manager.RemoveRecipeFromCookingPlan(2);

        Assert.Equal(2, manager.PeekLastRemovedRecipe());
        Assert.Equal(2, manager.RemovedRecipeCount);
    }

    [Fact]
    public void RestoreLastRemovedRecipe_WhenRecipeWasDeleted_ReturnsFalse()
    {
        var manager = new RecipeManager(new[] { CreateRecipe(1) });
        manager.AddRecipeToCookingPlan(1);
        manager.RemoveRecipeFromCookingPlan(1);

        Assert.True(manager.RemoveRecipe(1));

        Assert.False(manager.RestoreLastRemovedRecipe());
        Assert.Empty(manager.GetCookingPlan());
        Assert.Equal(0, manager.RemovedRecipeCount);
    }

    [Fact]
    public void RestoreLastRemovedRecipe_WhenRecipeIsAlreadyPlanned_ReturnsFalse()
    {
        var manager = new RecipeManager(new[] { CreateRecipe(1) });
        manager.AddRecipeToCookingPlan(1);
        manager.RemoveRecipeFromCookingPlan(1);
        manager.AddRecipeToCookingPlan(1);

        Assert.False(manager.RestoreLastRemovedRecipe());
        Assert.Equal(new[] { 1 }, manager.GetCookingPlan());
        Assert.Equal(0, manager.RemovedRecipeCount);
    }

    [Fact]
    public void RestoreLastRemovedRecipe_AppendsToEndOfPlan()
    {
        var manager = new RecipeManager(new[] { CreateRecipe(1), CreateRecipe(2), CreateRecipe(3) });
        manager.AddRecipeToCookingPlan(1);
        manager.AddRecipeToCookingPlan(2);
        manager.RemoveRecipeFromCookingPlan(1);

        manager.AddRecipeToCookingPlan(3);
        Assert.Equal(new[] { 2, 3 }, manager.GetCookingPlan());

        Assert.True(manager.RestoreLastRemovedRecipe());
        Assert.Equal(new[] { 2, 3, 1 }, manager.GetCookingPlan());
    }

    [Fact]
    public void AddRecipeToCookingPlan_MissingRecipe_ReturnsFalse()
    {
        var manager = new RecipeManager(Array.Empty<Recipe>());

        Assert.False(manager.AddRecipeToCookingPlan(999));
        Assert.Empty(manager.GetCookingPlan());
    }
}
