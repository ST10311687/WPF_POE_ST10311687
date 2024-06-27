using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using LiveCharts;
using LiveCharts.Wpf;

namespace WPF_POE_ST10311687
{
    public partial class Menu : Window, INotifyPropertyChanged
    {
        private List<string> chosenRecipes = new List<string>();
        private Dictionary<string, Recipe> recipes; // Dictionary to store recipes
        public event PropertyChangedEventHandler PropertyChanged; // Event for property changed notifications
        public SeriesCollection MyFoodGroup { get; set; }

        protected void OnPropertyChanged(string PropertyName)
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        public Menu(Dictionary<string, Recipe> recipes)
        {
            InitializeComponent(); // Initializes the components defined in the XAML.
            DataContext = this; // Sets the data context for data binding.
            this.recipes = recipes; 

            MyFoodGroup = new SeriesCollection(); // Initializes the SeriesCollection for food groups.
        }

        // ------------------------- Window loaded method -------------------------
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (string recipeName in recipes.Keys) // Adds each recipe name from the recipes dictionary to the combo box.
            {
                cmbRecipeName.Items.Add(recipeName);
            }
        }

        // ------------------------- Calories exceeded method -------------------------
        private void Recipe_ExceededCalories(string message, double totalCalories)
        {
            string fullMessage = $"\t* Total calories (Calories quantify food energy intake.): {totalCalories}\n {message}";
            listMenuRecipe.Items.Add(fullMessage); // Adds the message to the list of menu recipes.
        }

        // ------------------------- Menu button -------------------------
        private void btnAddToMenu(object sender, RoutedEventArgs e)
        {
            listMenuRecipe.Items.Clear();
            string selectedRecipe = cmbRecipeName.SelectedItem?.ToString(); // Gets the selected recipe from the combo box.


            if (selectedRecipe != null && recipes.ContainsKey(selectedRecipe)) // Checks if a recipe is selected and exists in the recipes dictionary.
            {
                if (!chosenRecipes.Contains(selectedRecipe))
                {
                    chosenRecipes.Add(selectedRecipe);
                    Recipe recipe = recipes[selectedRecipe];

                    recipe.ExceededCalories -= Recipe_ExceededCalories; 
                    recipe.ExceededCalories += Recipe_ExceededCalories; 
                }

                foreach (string item in chosenRecipes) // Adds each chosen recipe to the list and notifies if any calorie limits are exceeded.
                {
                    Recipe recipe = recipes[item];
                    listMenuRecipe.Items.Add(recipe.DisplayRecipe());
                    recipe.notifyer();
                }
            }
            else
            {
                MessageBox.Show("Invalid recipe name. Please check the spelling of the recipe.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ------------------------- Pie chart button -------------------------
        private void btnCreatePieChart(object sender, RoutedEventArgs e)
        {
            Dictionary<string, int> foodGroupCounts = new Dictionary<string, int> // Initializes a dictionary to count ingredients by food group.
            {
                {"Fruit", 0}, {"Vegetable", 0}, {"Grains", 0}, {"Proteins", 0},
                {"Dairy", 0}, {"Fats and Oils", 0}, {"Sugar and Sweets", 0}, {"Others", 0}
            };

            foreach (string recipeName in chosenRecipes) // Counts the ingredients in each chosen recipe by food group.
            {
                Recipe recipe = recipes[recipeName];

                foreach (Ingredient ingredient in recipe.Ingredients)
                {
                    string foodGroup = ingredient.FoodGroup;

                    if (foodGroupCounts.ContainsKey(foodGroup)) // Increments the count for the corresponding food group.
                    {
                        foodGroupCounts[foodGroup]++;
                    }
                }
            }

            int totalIngredients = foodGroupCounts.Values.Sum();

            MyFoodGroup.Clear();

            foreach (string foodGroup in foodGroupCounts.Keys) // Adds each food group and its count to the pie chart series.
            {
                MyFoodGroup.Add(new PieSeries
                {
                    Title = foodGroup, // Sets the title of the pie chart slice.
                    Values = new ChartValues<int> { foodGroupCounts[foodGroup] }, // Sets the value for the pie chart slice.
                    DataLabels = true, // Enables data labels for the pie chart slice.
                    LabelPoint = chartPoint =>
                    {
                        double percentage = chartPoint.Y / totalIngredients; // Calculates the percentage of the total ingredients for the food group.
                        return string.Format("{0:P}", percentage); // Formats the percentage as a string.
                    }
                });
            }
        }       
    }
}

//============================================================ EndOfProgram ============================================================//