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
    /// <summary>
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu : Window, INotifyPropertyChanged
    {
        private List<string> chosenRecipes = new List<string>();
        private Dictionary<string, Recipe> recipes;
        public event PropertyChangedEventHandler PropertyChanged;
        public SeriesCollection MyFoodGroup { get; set; }

        protected void OnPropertyChanged(string PropertyName)
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        public Menu(Dictionary<string, Recipe> recipes)
        {
            InitializeComponent();
            DataContext = this;
            this.recipes = recipes; 

            MyFoodGroup = new SeriesCollection();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (string recipeName in recipes.Keys)
            {
                cmbRecipeName.Items.Add(recipeName);
            }
        }

        private void Recipe_ExceededCalories(string message, double totalCalories)
        {
            string fullMessage = $"\t* Total calories (Calories quantify food energy intake.): {totalCalories}\n {message}";
            listMenuRecipe.Items.Add(fullMessage);
        }

        private void btnAddToMenu(object sender, RoutedEventArgs e)
        {
            listMenuRecipe.Items.Clear();
            string selectedRecipe = cmbRecipeName.SelectedItem?.ToString();

            if (selectedRecipe != null && recipes.ContainsKey(selectedRecipe))
            {
                if (!chosenRecipes.Contains(selectedRecipe))
                {
                    chosenRecipes.Add(selectedRecipe);
                    Recipe recipe = recipes[selectedRecipe];

                    recipe.ExceededCalories -= Recipe_ExceededCalories; 
                    recipe.ExceededCalories += Recipe_ExceededCalories; 
                }

                foreach (string item in chosenRecipes)
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

        private void btnCreatePieChart(object sender, RoutedEventArgs e)
        {
            Dictionary<string, int> foodGroupCounts = new Dictionary<string, int>
            {
                {"Fruit", 0}, {"Vegetable", 0}, {"Grains", 0}, {"Proteins", 0},
                {"Dairy", 0}, {"Fats and Oils", 0}, {"Sugar and Sweets", 0}, {"Others", 0}
            };

            foreach (string recipeName in chosenRecipes)
            {
                Recipe recipe = recipes[recipeName];

                foreach (Ingredient ingredient in recipe.Ingredients)
                {
                    string foodGroup = ingredient.FoodGroup;

                    if (foodGroupCounts.ContainsKey(foodGroup))
                    {
                        foodGroupCounts[foodGroup]++;
                    }
                }
            }

            int totalIngredients = foodGroupCounts.Values.Sum();

            MyFoodGroup.Clear();

            foreach (string foodGroup in foodGroupCounts.Keys)
            {
                MyFoodGroup.Add(new PieSeries
                {
                    Title = foodGroup,
                    Values = new ChartValues<int> { foodGroupCounts[foodGroup] },
                    DataLabels = true,
                    LabelPoint = chartPoint =>
                    {
                        double percentage = chartPoint.Y / totalIngredients;
                        return string.Format("{0:P}", percentage);
                    }
                });
            }
        }

        private void listMenuRecipe_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}

//============================================================ EndOfProgram ============================================================//