using System.Collections.Generic;
using RecipeManagement.Core;

namespace RecipeManagement.Tests;

/// <summary>
/// Example tests from the assignment specification. Add your own tests as you work.
/// </summary>
public sealed class RecipeManagerTests
{
    [Fact]
    public void Constructor_BuildsRecipeDictionary()
    {
        var manager = CreateManager();
        Assert.Equal(2, manager.RecipeCount);
        Assert.Equal("Recipe A", manager.FindRecipe(10)?.Title);
    }

    [Fact]
    public void InstructionsAreCompletedInFileOrder()
    {
        var manager = CreateManager();
        Assert.True(manager.StartCooking(10));
        Assert.Equal("First step", manager.PeekNextInstruction());
        Assert.Equal("First step", manager.CompleteNextInstruction());
        Assert.Equal("Second step", manager.PeekNextInstruction());
    }

    [Fact]
    public void RemovedRecipesAreRestoredLastInFirstOut()
    {
        var manager = CreateManager();
        manager.AddRecipeToCookingPlan(10);
        manager.AddRecipeToCookingPlan(20);
        manager.RemoveRecipeFromCookingPlan(10);
        manager.RemoveRecipeFromCookingPlan(20);
        Assert.Equal(20, manager.PeekLastRemovedRecipe());
        Assert.True(manager.RestoreLastRemovedRecipe());
        Assert.Equal(new[] { 20 }, manager.GetCookingPlan());
    }
    [Fact]
    public void ShoppingList_CollectsAndClears()
    {
        var manager = CreateManager();
        Assert.Equal(1, manager.AddIngredientsToShoppingList(10));
        Assert.Equal(1, manager.ShoppingItemCount);
        Assert.Contains("1 apple", manager.GetShoppingList());
        manager.ClearShoppingList();
        Assert.Equal(0, manager.ShoppingItemCount);
    }

    [Fact]
    public void AddRecipe_DuplicateIdReturnsFalse()
    {
        var manager = CreateManager();
        var result = manager.AddRecipe(new Recipe { Id = 10, Title = "Dup" });
        Assert.False(result);
    }

    [Fact]
    public void SavedRecipe_AddAndRemove()
    {
        var manager = CreateManager();
        Assert.True(manager.AddSavedRecipe(10));
        Assert.True(manager.IsRecipeSaved(10));
        Assert.False(manager.AddSavedRecipe(10));
        Assert.True(manager.RemoveSavedRecipe(10));
        Assert.False(manager.IsRecipeSaved(10));
    }

    [Fact]
    public void SearchByTitle_FindsMatchingRecipes()
    {
        var manager = CreateManager();
        var results = manager.SearchByTitle("Recipe A");
        Assert.Single(results);
        Assert.Equal("Recipe A", results[0].Title);
    }
 

    private static RecipeManager CreateManager()
    {
        return new RecipeManager(new[]
        {
            new Recipe
            {
                Id = 10,
                Title = "Recipe A",
                Ingredients = new() { "1 apple" },
                Instructions = new() { "First step", "Second step" }
            },
            new Recipe
            {
                Id = 20,
                Title = "Recipe B"
            }
        });
    }
}
