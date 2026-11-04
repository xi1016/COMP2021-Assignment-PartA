# Recipe Management System — Student Starter

Starter repository for Parts A and B. Implement `RecipeManager` in Core; the Application menu and JSON loader are supplied.

## What is supplied

- `RecipeManagement.Core/Models/` — recipe model classes
- `RecipeManagement.Core/RecipeLoader.cs` — reads `data/recipes.json`
- `RecipeManagement.Core/IRecipeManager.cs` — public API
- `RecipeManagement.Application/` — console menu (options labelled PartA / PartB)
- `RecipeManagement.Tests/` — example tests
- `data/recipes.json` — recipe dataset

## What you implement

**Part A** — `RecipeManager.cs` using:

- `Dictionary<int, Recipe>`
- `List<string>`
- `LinkedList<int>`
- `Stack<int>`
- `Queue<string>`

**Part B** — LINQ searches, protein report, saved-recipe collection, `Design.md`, and more tests.

## Build and run

Open `StudentPackage/RecipeManagement.sln`:

```bash
dotnet build
dotnet test
dotnet run --project RecipeManagement.Application -- data/recipes.json
```

Until you implement `RecipeManager`, menu options print a **Not implemented** message.

## AI acknowledgement

AI acknowledgement: I used WorkBuddy to help me understand the required behaviours of the five collection types (Dictionary, List, LinkedList, Stack, Queue), how `LinkedList.Remove` and `Stack.Pop`/`Peek` differ, and how to interpret compiler and test failures. I did not copy or adapt AI-generated code or other material into my submission. I developed the submitted solution myself based on my understanding of the course material.

## Implementation notes (Part A)

- `Dictionary<int, Recipe>` holds the catalogue, keyed by recipe ID.
- `List<string>` holds the shopping list, appended in ingredient order.
- `LinkedList<int>` holds the cooking plan in the order recipes were added.
- `Stack<int>` records removed plan entries so the most recent removal is restored first.
- `Queue<string>` holds cooking instructions and is refilled by `StartCooking`.

`GetShoppingList` and `GetCookingPlan` return copies so callers cannot mutate the internal collections.

## Test summary (Part A)

- `RecipeManagerTests.cs` — supplied example tests (kept unchanged).
- `RecipeCatalogueTests.cs` — add/find/remove behaviour, duplicate, missing and non-positive IDs.
- `RecipeManagerValidationTests.cs` — constructor rejection of null, null entries, non-positive IDs, blank titles and duplicate IDs; `AddRecipe(null)`.
- `ShoppingListTests.cs` — ingredient order, append across recipes, snapshot reads and clearing.
- `RecipeWorkflowTests.cs` — empty stack, empty queue, duplicate plan entry, remove-while-planned, and an end-to-end component interaction test.

29 tests pass, 0 fail (`dotnet test`).
