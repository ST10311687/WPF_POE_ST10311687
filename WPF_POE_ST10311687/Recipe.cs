using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_POE_ST10311687
{
    public class Recipe
    {
        public List<Ingredient> Ingredients = new List<Ingredient>();
        private List<double> OriginalQuantities = new List<double>();
        private List<string> OriginalUnits = new List<string>();
        private List<double> OriginalCalories = new List<double>();
        public List<string> Steps = new List<string>();
        public string RecipeName { get; set; }
        public delegate void ExceededCaloriesEventHandler(string message, double totalCalories); // Delegate for the ExceededCalories event handler
        public event ExceededCaloriesEventHandler ExceededCalories; // Event triggered when calories exceed a certain limit


        public Recipe() { }
        
        public Recipe (string recipeName, List<Ingredient> ingredients, List<double> originalQuantities, List<string> originalUnits, List<string> steps)
        {
            RecipeName = recipeName;
            Ingredients = ingredients;
            OriginalQuantities = originalQuantities;
            OriginalUnits = originalUnits;
            Steps = steps;

            foreach (Ingredient ingredient in Ingredients)
            {
                OriginalCalories.Add(ingredient.Calories);
            }
        }

        // ------------------------- Display recipe method -------------------------
        public string DisplayRecipe()
        {
            if (Ingredients.Count == 0) // Check if there are no ingredients in the recipe
            {
                return "No ingredients have been added.";
            }
            else
            {
                StringBuilder info = new StringBuilder($"Recipe Name: {RecipeName}\n"); // Initialize a StringBuilder to construct the recipe details

                string description = GenerateDescription(); // Generate a description for the recipe
                info.AppendLine(description);

                info.AppendLine("Ingredients:");
                foreach (Ingredient ingredient in Ingredients.OrderBy(i => i.Name))
                {
                    info.AppendLine($"\t• {ingredient.Quantity} {ingredient.UnitOfMeasurement} - {ingredient.Name} (Food Group: {ingredient.FoodGroup})");
                }

                info.AppendLine($"\nSteps for making {RecipeName}:");
                for (int i = 0; i < Steps.Count; i++) // Loop through each step and append it to the StringBuilder
                {
                    info.AppendLine($"\t{i + 1}. {Steps[i]}");
                }

                info.AppendLine("\nAdditional Information:");
                return info.ToString();
            }
        }

        // ------------------------- Scale recipe method -------------------------
        public void ScaleRecipe(double scalingNumber)
        {
            for (int i = 0; i < Ingredients.Count; i++) // Iterate through each ingredient in the Ingredients list
            {
                double quantity = Ingredients[i].Quantity; // Get the current quantity and unit of measurement of the ingredient
                string ufm = Ingredients[i].UnitOfMeasurement;

                quantity *= scalingNumber; // Scale the quantity by the scaling number

                if (ufm == "tablespoon" || ufm == "tablespoons") // Check if the unit of measurement is "tablespoon" or "tablespoons"
                {
                    if (quantity >= 16) // If the scaled quantity is 16 or more tablespoons, convert to cups
                    {
                        quantity /= 16;
                        quantity = Math.Round(quantity, 1);
                        ufm = "cup";
                    }
                }

                Ingredients[i].Quantity = quantity;
                Ingredients[i].UnitOfMeasurement = ufm;
                Ingredients[i].Calories *= scalingNumber;  // Scale the calories proportionally

            }
        }

        // ------------------------- Reset recipe method -------------------------
        public bool ResetRecipe()
        {
            if (Ingredients.Count == 0) // Check if there are no ingredients in the recipe

            {
                return false;
            }
            else
            {
                for (int i = 0; i < OriginalQuantities.Count; i++)
                {
                    Ingredients[i].Quantity = OriginalQuantities[i]; // Reset each ingredient's quantity, unit of measurement, and calories
                    Ingredients[i].UnitOfMeasurement = OriginalUnits[i];
                    Ingredients[i].Calories = OriginalCalories[i]; 
                }
                return true;
            }
        }

        // ------------------------- Total calories method -------------------------
        public double TotalCalories()
        {           
            double totalCalories = 0; // Initialize a variable to store the total calories
      
            foreach (Ingredient ingredient in Ingredients) // Iterate through each ingredient in the Ingredients list
            {               
                totalCalories += ingredient.Calories; // Add the calories of the current ingredient to the total
            }
            
            return totalCalories;
        }

        // ------------------------- Notifyer method -------------------------
        public void notifyer()
        {
            double totalCalories = TotalCalories();
            if (totalCalories > 300)
            {
                ExceededCalories?.Invoke($"\t* Alert!! Total calories for {RecipeName} exceeded 300! This is a high-calorie recipe. You might want to consider a lighter option.", totalCalories);
            }
            else if (totalCalories > 200)
            {
                ExceededCalories?.Invoke($"\t* Total calories for {RecipeName} are between 200 and 300. This is a moderately high-calorie recipe. Please keep in mind your daily intake.", totalCalories);
            }
            else if (totalCalories > 100)
            {
                ExceededCalories?.Invoke($"\t* Total calories for {RecipeName} are between 100 and 200. This is a moderate-calorie recipe. It's a balanced choice.", totalCalories);
            }
            else
            {
                ExceededCalories?.Invoke($"\t* Total calories for {RecipeName} are less than 100. This is a low-calorie recipe. Great choice for weight management.", totalCalories);
            }
        }

        // ------------------------- Description method -------------------------
        public string GenerateDescription()
        {
            StringBuilder description = new StringBuilder("This recipe for "); // Initialize a StringBuilder to construct the description
            description.Append(RecipeName); // Append the recipe name
            description.Append(" uses ");

            for (int i = 0; i < Ingredients.Count; i++) // Get the current ingredient
            {
                Ingredient ingredient = Ingredients[i];
                description.Append(ingredient.Quantity);
                description.Append(" ");
                description.Append(ingredient.UnitOfMeasurement);
                description.Append(" of ");
                description.Append(ingredient.Name);

                if (i < Ingredients.Count - 2) // Handle comma and "and" placement based on the number of ingredients
                {
                    description.Append(", "); // Add comma if more ingredients are coming
                }
                else if (i == Ingredients.Count - 2)
                {
                    description.Append(" and ");
                }
            }

            description.Append(" to create a delightful preparation.");
            return description.ToString(); // Return the constructed description as a string
        }

    }
}

//============================================================ EndOfProgram ============================================================//