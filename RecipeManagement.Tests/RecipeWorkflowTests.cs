using RecipeManagement.Core;

namespace RecipeManagement.Tests;

public sealed class RecipeWorkflowTests
{
    private static Recipe CreateRecipe(int id, string title, params string[] instructions)
    {
        return new Recipe
        {
            Id = id,
            Title = title,
            Ingredients = new List<string> { $"{title} ingredient 1", $"{title} ingredient 2" },
            Instructions = instructions.ToList()
        };
    }

    [Fact]
    public void EmptyStack_PeekReturnsNullAndRestoreReturnsFalse()
    {
        var manager = new RecipeManager(Array.Empty<Recipe>());

        Assert.Null(manager.PeekLastRemovedRecipe());
        Assert.False(manager.RestoreLastRemovedRecipe());
    }

    [Fact]
    public void EmptyQueue_PeekAndCompleteReturnNull()
    {
        var manager = new RecipeManager(Array.Empty<Recipe>());

        Assert.Null(manager.PeekNextInstruction());
        Assert.Null(manager.CompleteNextInstruction());
    }

    [Fact]
    public void AddRecipeToCookingPlan_DuplicateId_ReturnsFalse()
    {
        var manager = new RecipeManager(new[] { CreateRecipe(1, "Alpha", "Chop") });

        Assert.True(manager.AddRecipeToCookingPlan(1));
        Assert.False(manager.AddRecipeToCookingPlan(1));
        Assert.Equal(1, manager.CookingPlanCount);
    }

    [Fact]
    public void RemoveRecipe_WhileInCookingPlan_ReturnsFalse()
    {
        var manager = new RecipeManager(new[] { CreateRecipe(1, "Alpha", "Chop") });
        manager.AddRecipeToCookingPlan(1);

        Assert.False(manager.RemoveRecipe(1));
        Assert.NotNull(manager.FindRecipe(1));
    }

    [Fact]
    public void AddIngredients_MissingRecipe_ReturnsZero()
    {
        var manager = new RecipeManager(Array.Empty<Recipe>());

        Assert.Equal(0, manager.AddIngredientsToShoppingList(999));
        Assert.Equal(0, manager.ShoppingItemCount);
    }

    [Fact]
    public void FullWorkflow_RecipeMovesThroughPlanHistoryAndInstructions()
    {
        var manager = new RecipeManager(new[]
        {
            CreateRecipe(1, "Alpha", "Chop", "Boil"),
            CreateRecipe(2, "Beta", "Mix")
        });

        Assert.Equal(2, manager.RecipeCount);
        Assert.Equal(2, manager.AddIngredientsToShoppingList(1));
        Assert.Equal(2, manager.ShoppingItemCount);

        Assert.True(manager.AddRecipeToCookingPlan(1));
        Assert.True(manager.AddRecipeToCookingPlan(2));
        Assert.Equal(new[] { 1, 2 }, manager.GetCookingPlan());

        Assert.True(manager.RemoveRecipeFromCookingPlan(1));
        Assert.Equal(1, manager.PeekLastRemovedRecipe());
        Assert.True(manager.RestoreLastRemovedRecipe());
        Assert.Equal(new[] { 2, 1 }, manager.GetCookingPlan());

        Assert.True(manager.StartCooking(2));
        Assert.Equal("Mix", manager.PeekNextInstruction());
        Assert.Equal("Mix", manager.CompleteNextInstruction());
        Assert.Null(manager.CompleteNextInstruction());

        Assert.True(manager.RemoveRecipeFromCookingPlan(2));
        Assert.True(manager.RemoveRecipe(2));
        Assert.Equal(1, manager.RecipeCount);

        manager.ClearShoppingList();
        Assert.Equal(0, manager.ShoppingItemCount);
        Assert.Equal(new[] { 1 }, manager.GetCookingPlan());
    }
}
