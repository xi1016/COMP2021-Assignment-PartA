using RecipeManagement.Core;

namespace RecipeManagement.Tests;

public sealed class CookingSessionTests
{
    private static Recipe CreateRecipe(int id, params string[] instructions)
    {
        return new Recipe
        {
            Id = id,
            Title = $"Recipe {id}",
            Ingredients = new List<string> { "Ingredient" },
            Instructions = instructions.ToList()
        };
    }

    [Fact]
    public void StartCooking_MissingRecipe_ReturnsFalseAndLeavesQueueEmpty()
    {
        var manager = new RecipeManager(Array.Empty<Recipe>());

        Assert.False(manager.StartCooking(999));
        Assert.Equal(0, manager.PendingInstructionCount);
    }

    [Fact]
    public void StartCooking_RecipeWithoutInstructions_ReturnsFalse()
    {
        var manager = new RecipeManager(new[] { CreateRecipe(1) });

        Assert.False(manager.StartCooking(1));
        Assert.Equal(0, manager.PendingInstructionCount);
    }

    [Fact]
    public void StartCooking_LoadsInstructionsInOriginalOrder()
    {
        var manager = new RecipeManager(new[] { CreateRecipe(1, "Chop", "Fry", "Serve") });

        Assert.True(manager.StartCooking(1));
        Assert.Equal(3, manager.PendingInstructionCount);
        Assert.Equal(new[] { "Chop", "Fry", "Serve" }, DrainQueue(manager));
    }

    [Fact]
    public void StartCooking_SecondRecipe_ReplacesPreviousInstructions()
    {
        var manager = new RecipeManager(new[]
        {
            CreateRecipe(1, "Chop", "Fry"),
            CreateRecipe(2, "Mix")
        });

        manager.StartCooking(1);
        Assert.Equal(2, manager.PendingInstructionCount);

        Assert.True(manager.StartCooking(2));
        Assert.Equal(1, manager.PendingInstructionCount);
        Assert.Equal("Mix", manager.PeekNextInstruction());
    }

    [Fact]
    public void PeekNextInstruction_DoesNotRemoveFromQueue()
    {
        var manager = new RecipeManager(new[] { CreateRecipe(1, "Chop", "Fry") });
        manager.StartCooking(1);

        Assert.Equal("Chop", manager.PeekNextInstruction());
        Assert.Equal("Chop", manager.PeekNextInstruction());
        Assert.Equal(2, manager.PendingInstructionCount);
        Assert.Equal("Chop", manager.CompleteNextInstruction());
        Assert.Equal(1, manager.PendingInstructionCount);
    }

    [Fact]
    public void CompleteNextInstruction_RemovesOneItemPerCall()
    {
        var manager = new RecipeManager(new[] { CreateRecipe(1, "Chop", "Fry", "Serve") });
        manager.StartCooking(1);

        Assert.Equal("Chop", manager.CompleteNextInstruction());
        Assert.Equal("Fry", manager.CompleteNextInstruction());
        Assert.Equal("Serve", manager.CompleteNextInstruction());
        Assert.Null(manager.CompleteNextInstruction());
        Assert.Equal(0, manager.PendingInstructionCount);
    }

    private static IReadOnlyList<string> DrainQueue(RecipeManager manager)
    {
        var drained = new List<string>();

        while (manager.PendingInstructionCount > 0)
        {
            drained.Add(manager.CompleteNextInstruction()!);
        }

        return drained;
    }
}
