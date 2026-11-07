using System;
using System.Collections.Generic;

namespace RecipeManagement.Core;

/// <summary>
/// Implement this class using the five Part A collections as private fields:
/// Dictionary&lt;int, Recipe&gt;, List&lt;string&gt;, LinkedList&lt;int&gt;,
/// Stack&lt;int&gt; and Queue&lt;string&gt;.
/// </summary>
public sealed class RecipeManager : IRecipeManager
{
private readonly Dictionary<int, Recipe> _recipes = new();
private readonly List<string> _shoppingList = new();
private readonly LinkedList<int> _cookingPlan = new();
private readonly Stack<int> _removedRecipes = new();
private readonly Queue<string> _instructions = new();
private readonly HashSet<int> _savedRecipes = new();



    public RecipeManager(IEnumerable<Recipe> recipes)
    {
        // TODO Part A: validate recipes and build Dictionary<int, Recipe>.
    foreach (Recipe recipe in recipes)
{
    if (recipe is null)
    {
        throw new ArgumentException("A recipe in the supplied list is null.");
    }
    if (recipe.Id <= 0)
    {
        throw new ArgumentException($"Recipe ID must be a positive number: {recipe.Id}.");
    }
    if (string.IsNullOrWhiteSpace(recipe.Title))
    {
        throw new ArgumentException($"Recipe {recipe.Id} has a blank title.");
    }
    if (_recipes.ContainsKey(recipe.Id))
    {
        throw new ArgumentException($"Duplicate recipe ID: {recipe.Id}.");
    }
    _recipes.Add(recipe.Id, recipe);
}

    }

    public int RecipeCount => _recipes.Count;
    public int ShoppingItemCount => _shoppingList.Count;
    public int CookingPlanCount => _cookingPlan.Count;
    public int PendingInstructionCount => _instructions.Count;
    public int RemovedRecipeCount => _removedRecipes.Count;


    public bool AddRecipe(Recipe recipe)
    {
        if (recipe.Id <= 0 || string.IsNullOrWhiteSpace(recipe.Title))
        {
            return false;
        }
        if (_recipes.ContainsKey(recipe.Id))
        {
            return false;
        }
        _recipes.Add(recipe.Id, recipe);
        return true;
    }


    public Recipe? FindRecipe(int recipeId)
    {
        return _recipes.GetValueOrDefault(recipeId);
    }


    public bool RemoveRecipe(int recipeId)
    {
        if (!_recipes.ContainsKey(recipeId) || _cookingPlan.Contains(recipeId))
        {
            return false;
        }
        return _recipes.Remove(recipeId);
    }


    public int AddIngredientsToShoppingList(int recipeId)
    {
        if (!_recipes.TryGetValue(recipeId, out Recipe? recipe))
        {
            return 0;
        }
        foreach (string ingredient in recipe.Ingredients)
        {
            _shoppingList.Add(ingredient);
        }
        return recipe.Ingredients.Count;
    }


    public IReadOnlyList<string> GetShoppingList()
    {
        return _shoppingList.ToArray();
    }


    public void ClearShoppingList()
    {
        _shoppingList.Clear();
    }


    public bool AddRecipeToCookingPlan(int recipeId)
    {
        if (!_recipes.ContainsKey(recipeId) || _cookingPlan.Contains(recipeId))
        {
            return false;
        }
        _cookingPlan.AddLast(recipeId);
        return true;
    }


    public bool RemoveRecipeFromCookingPlan(int recipeId)
    {
        if (!_cookingPlan.Remove(recipeId))
        {
            return false;
        }
        _removedRecipes.Push(recipeId);
        return true;
    }


    public bool RestoreLastRemovedRecipe()
    {
        if (_removedRecipes.Count == 0)
        {
            return false;
        }
        int recipeId = _removedRecipes.Pop();
        if (!_recipes.ContainsKey(recipeId) || _cookingPlan.Contains(recipeId))
        {
            return false;
        }
        _cookingPlan.AddLast(recipeId);
        return true;
    }


    public int? PeekLastRemovedRecipe()
    {
        return _removedRecipes.Count == 0 ? null : _removedRecipes.Peek();
    }


    public IReadOnlyList<int> GetCookingPlan()
    {
        return _cookingPlan.ToArray();
    }


    public bool StartCooking(int recipeId)
    {
        if (!_recipes.TryGetValue(recipeId, out Recipe? recipe) || recipe.Instructions.Count == 0)
        {
            return false;
        }
        _instructions.Clear();
        foreach (string instruction in recipe.Instructions)
        {
            _instructions.Enqueue(instruction);
        }
        return true;
    }


    public string? PeekNextInstruction()
    {
        return _instructions.Count == 0 ? null : _instructions.Peek();
    }


    public string? CompleteNextInstruction()
    {
        return _instructions.Count == 0 ? null : _instructions.Dequeue();
    }


    public IReadOnlyList<Recipe> SearchByTitle(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return _recipes.Values.ToList();
        }
        return _recipes.Values
            .Where(r => r.Title.Contains(searchText, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }


    public IReadOnlyList<Recipe> SearchByIngredient(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return new List<Recipe>();
        }
        return _recipes.Values
            .Where(r => r.Ingredients.Any(
                ing => ing.Contains(searchText, StringComparison.OrdinalIgnoreCase)))
            .ToList();
    }


    public IReadOnlyList<Recipe> GetHighestProteinRecipes(int count)
    {
        if (count <= 0)
        {
            return new List<Recipe>();
        }
        return _recipes.Values
            .Where(r => r.Nutrition is not null && r.Nutrition.ProteinG.HasValue)
            .OrderByDescending(r => r.Nutrition!.ProteinG!.Value)
            .Take(count)
            .ToList();
    }


    public bool AddSavedRecipe(int recipeId)
    {
        if (!_recipes.ContainsKey(recipeId))
        {
            return false;
        }
        return _savedRecipes.Add(recipeId);
    }



    public bool RemoveSavedRecipe(int recipeId)
    {
        return _savedRecipes.Remove(recipeId);
    }


    public bool IsRecipeSaved(int recipeId)
    {
        return _savedRecipes.Contains(recipeId);
    }


    public IReadOnlyList<int> GetSavedRecipes()
    {
        return _savedRecipes.ToList();
    }

}
