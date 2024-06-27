using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPF_POE_ST10311687
{
    /// <summary>
    /// Interaction logic for addIngredient.xaml
    /// </summary>
    public partial class addIngredient : Window
    {
        public List<Ingredient> ingredients { get; }
        public List<double> originalQuantities { get; }
        public List<string> originalUnits { get; }
        public List<string> steps { get; }
        public string recipeName { set; get; }

        int ingredientCount = 0;
        int stepsCount = 0;

        public addIngredient()
        {
            InitializeComponent(); // Initializes the components defined in the XAML.

            ingredients = new List<Ingredient>();
            originalQuantities = new List<double>();
            originalUnits = new List<string>();
            steps = new List<string>();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {       
            cmbFoodGroup.Items.Add("Fruit"); // Add options to the food group combo box
            cmbFoodGroup.Items.Add("Vegetable");
            cmbFoodGroup.Items.Add("Grains");
            cmbFoodGroup.Items.Add("Proteins");
            cmbFoodGroup.Items.Add("Dairy");
            cmbFoodGroup.Items.Add("Fats and Oils");
            cmbFoodGroup.Items.Add("Sugar and Sweets");
            cmbFoodGroup.Items.Add("Others");
        }

        private void Button_Ingredient(object sender, RoutedEventArgs e)
        {
            recipeName = string.Empty; // Initialize variables to store ingredient and recipe details
            string name = string.Empty;
            double quantity = 0.0;
            string unitOfMeasurement = string.Empty;
            double calories = 0.0;
            string foodGroup = string.Empty;

            recipeName = txtRecipeName.Text; // Get the recipe name from the text box.
            Recipe recipe = new Recipe();

            if (recipeName.Length > 0)
            {
                ingredientCount++; // Increment the ingredient count.

                try
                {
                    name = txtIngredientName.Text; // Get the ingredient name from the text box.


                    if (!double.TryParse(txtQuantity.Text, out quantity)) // Try to parse the quantity from the text box.
                    {
                        ingredientCount--; // Decrement the ingredient count if parsing fails.
                        throw new FormatException("Invalid quantity value. Please enter a numeric value.");
                    }
                    originalQuantities.Add(quantity);

                    unitOfMeasurement = txtUnitOfMeasurement.Text; // Get the unit of measurement from the text box.
                    originalUnits.Add(unitOfMeasurement);

                    if (unitOfMeasurement == "tablespoon" || unitOfMeasurement == "tablespoons") // Convert tablespoons to cups if necessary.
                    {
                        if (quantity >= 16)
                        {
                            quantity /= 16;
                            quantity = Math.Round(quantity, 1);
                            unitOfMeasurement = "cup";
                        }
                    }

                    if (!double.TryParse(txtCalories.Text, out calories)) // Try to parse the calories from the text box.
                    {
                        ingredientCount--;
                        throw new FormatException("Invalid calories value. Please enter a numeric value.");
                    }
                     
                    foodGroup = cmbFoodGroup.SelectedItem?.ToString(); // Get the selected food group from the combo box.

                    if (string.IsNullOrEmpty(foodGroup)) // Ensure a food group is selected.
                    {
                        ingredientCount--;
                        throw new Exception("Please select a food group.");
                    }
                }
                catch (FormatException ex)
                {
                    MessageBox.Show(ex.Message, "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                ingredients.Add(new Ingredient { Name = name, Quantity = quantity, UnitOfMeasurement = unitOfMeasurement, Calories = calories, FoodGroup = foodGroup }); // Add the new ingredient to the list of ingredients.
                recipe = new Recipe(recipeName, ingredients, originalQuantities, originalUnits, steps);

                lstDisplay.Items.Add($"Ingredient No {ingredientCount} for {recipeName}\n\tName: {name}\n\tQuantity: {quantity}\n\tUnit Of Measurement: {unitOfMeasurement}\n\tCalories: {calories}\n\tFood Group: {foodGroup}");

                recipe.ExceededCalories += (message, totalCalories) => // Add an event handler for the ExceededCalories event.
                {
                    if (totalCalories > 300)
                    {
                        lstDisplay.Items.Add(message);
                        lstDisplay.Items.Add($"Total calories: {totalCalories}");
                    }
                    else
                    {
                        lstDisplay.Items.Add(message);
                        lstDisplay.Items.Add($"Total calories: {totalCalories}");
                    }
                };

                txtIngredientName.Clear(); // Clear the input fields.
                txtQuantity.Clear();
                txtUnitOfMeasurement.Clear();
                txtCalories.Clear();
            }
            else
            {
                MessageBox.Show("Please enter recipe.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ingredientCount = 0; // Reset the ingredient count.
            }
        }

        private void Button_Step(object sender, RoutedEventArgs e)
        {
            string strSteps = "";

            if (ingredientCount == 0) // Check if there are any ingredients added.
            {
                MessageBox.Show("Please add Ingredient First Before you add step.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                stepsCount = 0; // Reset the steps count.
            }
            else
            {
                strSteps = txtList.Text; // Get the step details from the text box.

                if (strSteps.Length == 0) // Check if the step details are provided.
                {
                    MessageBox.Show("Please add steps.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    stepsCount = 0;
                }
                else
                {
                    stepsCount++; // Increment the steps count.
                    steps.Add(strSteps); // Add the step to the list of steps.
                    lstDisplay.Items.Add($"Step {stepsCount}:\n{strSteps}"); // Display the step details in the list.
                    txtList.Clear();
                    txtList.Focus();
                }
            }
        }

        private void Button_AddRecipe(object sender, RoutedEventArgs e)
        {
            if (ingredients.Count == 0) // Check if there are any ingredients added to the recipe.
            {
                MessageBox.Show("Please add Recipe.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                double totalCalories = 0;
                foreach (var ingredient in ingredients) // Calculate the total calories of the recipe by summing up the calories of each ingredient.
                {
                    totalCalories += ingredient.Calories;
                }

                if (totalCalories > 300) // Check if the total calories exceed 300.
                {
                    MessageBox.Show($"Recipe added Successfully with high calories! Total Calories: {totalCalories}. This is a high-calorie recipe. You might want to consider a lighter option.", "High Calories Alert.", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {

                    MessageBox.Show("Recipe added Successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                DialogResult = true; // Set the dialog result to true indicating a successful operation.

                this.Close();
            }
        }
    }
}


//============================================================ EndOfProgram ============================================================//