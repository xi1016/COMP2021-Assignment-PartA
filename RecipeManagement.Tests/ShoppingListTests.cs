using RecipeManagement.Core;

namespace RecipeManagement.Tests;

public sealed class ShoppingListTests
{
    private static Recipe CreateRecipe(int id, string title, params string[] ingredients)
    {
        return new Recipe
        {
            Id = id,
            Title = title,
            Ingredients = ingredients.ToList(),
            Instructions = new List<string> { "Cook" }
        };
    }

    [Fact]
    public void AddIngredients_CopiesIngredientsInOriginalOrder()
    {
        var manager = new RecipeManager(new[]
        {
            CreateRecipe(1, "Omelette", "Eggs", "Butter", "Chives")
        });

        int added = manager.AddIngredientsToShoppingList(1);

        Assert.Equal(3, added);
        Assert.Equal(new[] { "Eggs", "Butter", "Chives" }, manager.GetShoppingList());
    }

    [Fact]
    public void AddIngredients_ForTwoRecipes_AppendsInCallOrder()
    {
        var manager = new RecipeManager(new[]
        {
            CreateRecipe(1, "Omelette", "Eggs", "Butter"),
            CreateRecipe(2, "Salad", "Lettuce")
        });

        manager.AddIngredientsToShoppingList(1);
        manager.AddIngredientsToShoppingList(2);

        Assert.Equal(new[] { "Eggs", "Butter", "Lettuce" }, manager.GetShoppingList());
    }

    [Fact]
    public void GetShoppingList_ReturnsSnapshotThatDoesNotAffectInternalList()
    {
        var manager = new RecipeManager(new[] { CreateRecipe(1, "Omelette", "Eggs") });
        manager.AddIngredientsToShoppingList(1);

        IReadOnlyList<string> snapshot = manager.GetShoppingList();

        Assert.Single(snapshot);
        Assert.Single(manager.GetShoppingList());
    }

    [Fact]
    public void ClearShoppingList_EmptiesTheList()
    {
        var manager = new RecipeManager(new[] { CreateRecipe(1, "Omelette", "Eggs", "Butter") });
        manager.AddIngredientsToShoppingList(1);

        manager.ClearShoppingList();

        Assert.Equal(0, manager.ShoppingItemCount);
        Assert.Empty(manager.GetShoppingList());
    }
}
