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
            InitializeComponent();

            ingredients = new List<Ingredient>();
            originalQuantities = new List<double>();
            originalUnits = new List<string>();
            steps = new List<string>();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Add options to the food group combo box
            cmbFoodGroup.Items.Add("Fruit");
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
            recipeName = string.Empty;
            string name = string.Empty;
            double quantity = 0.0;
            string unitOfMeasurement = string.Empty;
            double calories = 0.0;
            string foodGroup = string.Empty;

            recipeName = txtRecipeName.Text;
            Recipe recipe = new Recipe();

            if (recipeName.Length > 0)
            {
                ingredientCount++;

                try
                {
                    name = txtIngredientName.Text;

                    if (!double.TryParse(txtQuantity.Text, out quantity))
                    {
                        ingredientCount--;
                        throw new FormatException("Invalid quantity value. Please enter a numeric value.");
                    }
                    originalQuantities.Add(quantity);

                    unitOfMeasurement = txtUnitOfMeasurement.Text;
                    originalUnits.Add(unitOfMeasurement);

                    if (unitOfMeasurement == "tablespoon" || unitOfMeasurement == "tablespoons")
                    {
                        if (quantity >= 16)
                        {
                            quantity /= 16;
                            quantity = Math.Round(quantity, 1);
                            unitOfMeasurement = "cup";
                        }
                    }

                    if (!double.TryParse(txtCalories.Text, out calories))
                    {
                        ingredientCount--;
                        throw new FormatException("Invalid calories value. Please enter a numeric value.");
                    }

                    foodGroup = cmbFoodGroup.SelectedItem?.ToString();

                    if (string.IsNullOrEmpty(foodGroup))
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

                ingredients.Add(new Ingredient { Name = name, Quantity = quantity, UnitOfMeasurement = unitOfMeasurement, Calories = calories, FoodGroup = foodGroup });
                recipe = new Recipe(recipeName, ingredients, originalQuantities, originalUnits, steps);

                lstDisplay.Items.Add($"Ingredient No {ingredientCount} for {recipeName}\n\tName: {name}\n\tQuantity: {quantity}\n\tUnit Of Measurement: {unitOfMeasurement}\n\tCalories: {calories}\n\tFood Group: {foodGroup}");

                recipe.ExceededCalories += (message, totalCalories) =>
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

                txtIngredientName.Clear();
                txtQuantity.Clear();
                txtUnitOfMeasurement.Clear();
                txtCalories.Clear();
            }
            else
            {
                MessageBox.Show("Please enter recipe.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ingredientCount = 0;
            }
        }

        private void Button_Step(object sender, RoutedEventArgs e)
        {
            string strSteps = "";

            if (ingredientCount == 0)
            {
                MessageBox.Show("Please add Ingredient First Before you add step.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                stepsCount = 0;
            }
            else
            {
                strSteps = txtList.Text;

                if (strSteps.Length == 0)
                {
                    MessageBox.Show("Please add steps.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    stepsCount = 0;
                }
                else
                {
                    stepsCount++;
                    steps.Add(strSteps);
                    lstDisplay.Items.Add($"Step {stepsCount}:\n{strSteps}");
                    txtList.Clear();
                    txtList.Focus();
                }
            }
        }

        private void Button_AddRecipe(object sender, RoutedEventArgs e)
        {
            if (ingredients.Count == 0)
            {
                MessageBox.Show("Please add Recipe.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                double totalCalories = 0;
                foreach (var ingredient in ingredients)
                {
                    totalCalories += ingredient.Calories;
                }

                if (totalCalories > 300)
                {
                    MessageBox.Show($"Recipe added Successfully with high calories! Total Calories: {totalCalories}. This is a high-calorie recipe. You might want to consider a lighter option.", "High Calories Alert.", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {

                    MessageBox.Show("Recipe added Successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                DialogResult = true;
                this.Close();
            }
        }
    }
}


//============================================================ EndOfProgram ============================================================//