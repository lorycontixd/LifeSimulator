using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Lore.Game.Managers
{
    public class CookingManager : BaseManager
    {
        public List<Ingredient> allIngredients = new List<Ingredient>();
        public List<Recipe> recipes = new List<Recipe>();
        public Dictionary<Ingredient, int> ingredients = new Dictionary<Ingredient, int>();


        public override void Start()
        {
            base.Start();
        }

        public bool BuyIngredienet(Ingredient ingredient)
        {
            if (ingredients.ContainsKey(ingredient))
            {
                ingredients[ingredient] += 1;
            }
            else
            {
                ingredients.Add(ingredient, 1);
            }
            return true;
        }
        public bool UseIngredient(Ingredient ingredient)
        {
            if (ingredients.ContainsKey(ingredient))
            {
                ingredients[ingredient] -= 1;
                return true;
            }
            return false;
        }
        public bool HasIngredient(Ingredient ingredient)
        {
            return ingredients.ContainsKey(ingredient);
        }
        public bool HasIngredient(string name)
        {
            return ingredients.Keys.Any(i  => i.Name == name);
        }
        public List<Recipe> GetCookableRecipes()
        {
            List<Recipe> results = new List<Recipe>();
            foreach(Recipe recipe in recipes)
            {
                foreach(Ingredient ingredient in recipe.Ingredients)
                {
                    if (!ingredients.ContainsKey(ingredient))
                    {
                        continue;
                    }
                    results.Add(recipe);
                }
            }
            return results;
        }
        public Recipe GetRandomCookableRecipe()
        {
            var recipes = GetCookableRecipes();
            if (recipes.Count <= 0) { return null; }
            return recipes[UnityEngine.Random.Range(0, recipes.Count)];
        }
    }
}
