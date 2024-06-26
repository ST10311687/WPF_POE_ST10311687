using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Linq;
using System.Collections.Generic;


namespace WPF_POE_ST10311687
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// Things to do 
    /// 1 - add comments
    /// 2 - add more references
    /// 3 - add menu
    /// 4 - fix text when a recipe has been added
    /// 5 - read me file
    /// 6 - change colors
    /// 7 - resize exit
    /// 8 - capital letters
    /// 9 - 
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Ingredient> allIngredients;
        private List<double> allOriginalQuantities;
        private List<string> allOriginalUnits;
        private List<string> allSteps;
        private string recipeName;
        Recipe recipe;
        private static Dictionary<string, Recipe> Recipes = new Dictionary<string, Recipe>();

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += Window_Loaded;
            anotherRecipeLable.Visibility = Visibility.Collapsed;
        }

        private void displayNoRecipe()
        {
            MessageBox.Show("There is no recipe in the list.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var recipeName in Recipes.Keys)
            {
                txtSpecificRecipe.Items.Add(recipeName);
            }

        }

        private void LoadRecipeNames()
        {
            txtSpecificRecipe.Items.Clear();
            txtSpecificRecipeToScale.Items.Clear();
            txtResetRecipe.Items.Clear();
            foreach (var recipeName in Recipes.Keys)
            {
                txtSpecificRecipe.Items.Add(recipeName);
                txtSpecificRecipeToScale.Items.Add(recipeName);
                txtResetRecipe.Items.Add(recipeName);
            }
        }

        private void ExceededCaloriesHandler(string message, double totalCalories)
        {
            string messageWithCalories = $"\t* Total calories (Calories quantify food energy intake): {totalCalories}\n" + message + "\n";

            lstDisplayDetails.Items.Add(messageWithCalories);
        }

        private void scaleRecipeSpecificName(double scalingNumber)
        {
            lstDisplayDetails.Items.Clear();
            string recipeName1 = txtSpecificRecipeToScale.Text;
            if (Recipes.ContainsKey(recipeName1))
            {
                MessageBoxResult result = MessageBox.Show($"Are you sure you want to scale the {recipeName1} recipe?", "Confirmation", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    Recipes[recipeName1].ScaleRecipe(scalingNumber);
                    lstDisplayDetails.Items.Add("The recipe has been scaled. Here is the updated recipe:\n");
                    lstDisplayDetails.Items.Add(Recipes[recipeName1].DisplayRecipe());
                    double totalCalories = Recipes[recipeName1].TotalCalories();
                    Recipes[recipeName1].notifyer();
                }

            }
            else
            {
                MessageBox.Show("There is no recipe with that name", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void btnAddRecipe_Click(object sender, RoutedEventArgs e)
        {
            addIngredient AddIngredient = new addIngredient();
            AddIngredient.ShowDialog();

            allIngredients = AddIngredient.ingredients;
            allOriginalQuantities = AddIngredient.originalQuantities;
            allOriginalUnits = AddIngredient.originalUnits;
            allSteps = AddIngredient.steps;
            recipeName = AddIngredient.recipeName;

            if (recipeName != null)
            {
                recipe = new Recipe(recipeName, allIngredients, allOriginalQuantities, allOriginalUnits, allSteps);               

                if (!Recipes.ContainsKey(recipe.RecipeName))
                {
                    Recipes.Add(recipe.RecipeName, recipe);
                    lstDisplayDetails.Items.Clear();
                    LoadRecipeNames();
                }
                else
                {
                    MessageBox.Show("A recipe with this name already exists.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            anotherRecipeLable.Visibility = Visibility.Visible;
        }

        private void btnDisplayAllRecipe_Click(object sender, RoutedEventArgs e)
        {
            if (Recipes.Count == 0)
            {
                displayNoRecipe();
            }
            else
            {
                lstDisplayDetails.Items.Clear();
                List<string> sortedKeys = new List<string>(Recipes.Keys);
                sortedKeys.Sort();

                foreach (string key in sortedKeys)
                {
                    Recipe recipe = Recipes[key];

                    recipe.ExceededCalories -= ExceededCaloriesHandler;

                    recipe.ExceededCalories += ExceededCaloriesHandler;
                    lstDisplayDetails.Items.Add(recipe.DisplayRecipe());
                    recipe.notifyer(); 
                    lstDisplayDetails.Items.Add("-------------------------------------------------------" +
                        "-------------------------------------------------------------------------------");
                }
            }
        }

        private void btnDisplaySpecificRecipe_Click(object sender, RoutedEventArgs e)
        {
            if (Recipes.Count == 0)
            {
                displayNoRecipe();
            }
            else
            {
                lstDisplayDetails.Items.Clear();
                string recipeName = txtSpecificRecipe.Text;
                if (Recipes.ContainsKey(recipeName))
                {
                    recipe.ExceededCalories -= ExceededCaloriesHandler;

                    recipe.ExceededCalories += ExceededCaloriesHandler;

                    lstDisplayDetails.Items.Add(Recipes[recipeName].DisplayRecipe());
                    double totalCalories = Recipes[recipeName].TotalCalories();
                    Recipes[recipeName].notifyer();

                    var stepsWithCheckBox = Recipes[recipeName].Steps.Select(step => new { Step = step }).ToList();
                }
                else
                {
                    MessageBox.Show("Invalid recipe name please check the spelling of the recipe.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        

        private void btnScaleRecipe_Click(object sender, RoutedEventArgs e)
        {
            if (Recipes.Count == 0)
            {
                displayNoRecipe();
            }
            else
            {
                if (rdoZeroPointFive.IsChecked == true)
                {
                    scaleRecipeSpecificName(0.5);
                }
                else if (rdoTwo.IsChecked == true)
                {
                    scaleRecipeSpecificName(2);
                }
                else if (rdoThree.IsChecked == true)
                {
                    scaleRecipeSpecificName(3);
                }
                else
                {
                    MessageBox.Show("Please choose one number.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
        }

        private void btnResetRecipe_Click(object sender, RoutedEventArgs e)
        {
            if (Recipes.Count == 0)
            {
                displayNoRecipe();
            }
            else
            {
                string resetRecipeName = txtResetRecipe.Text;
                if (Recipes.ContainsKey(resetRecipeName))
                {
                    MessageBoxResult result = MessageBox.Show($"Are you sure you want to reset the {resetRecipeName} recipe?", "Confirmation", MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.Yes)
                    {
                        lstDisplayDetails.Items.Clear();
                        bool success = Recipes[resetRecipeName].ResetRecipe();
                        if (success)
                        {
                            lstDisplayDetails.Items.Add("The recipe has been reset. Here is the original recipe:");
                            lstDisplayDetails.Items.Add(Recipes[resetRecipeName].DisplayRecipe());
                            double totalCalories = Recipes[resetRecipeName].TotalCalories();
                            Recipes[resetRecipeName].notifyer();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("There is no recipe with that name", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
        }

        private void btnaddMenu_Click(object sender, RoutedEventArgs e)
        {
            if (Recipes.Count == 0)
            {
                displayNoRecipe();
            }
            else
            {
                lstDisplayDetails.Items.Clear();   
                Menu menu = new Menu(Recipes);
                menu.Show();
            }
        }

        private void btnClearRecipe_Click(object sender, RoutedEventArgs e)
        {
            if (Recipes.Count == 0)
            {
                displayNoRecipe();
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to clear all recipes? :", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                lstDisplayDetails.Items.Clear();

                if (result == MessageBoxResult.Yes)
                {
                    Recipes.Clear();
                    txtSpecificRecipe.Items.Clear();
                    txtSpecificRecipeToScale.Items.Clear();
                    txtResetRecipe.Items.Clear();
                    MessageBox.Show("All recipes have been cleared.", "Success", MessageBoxButton.OK);
                }
                else
                {
                    MessageBox.Show("You cancled to clear the recipe.", "Cancled", MessageBoxButton.OK);
                }
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

#region Reference List
/* Troelsen, A.and Japikse, P. 2024. Pro C# 9 with .NET 5.
New York: Apress.

Koshy, B., 2019. Stack overflow. [Online] 
Available at: https://stackoverflow.com/questions/2019402/when-why-to-use-delegates
[Accessed 28 May 2024].
*/

#endregion
//============================================================ EndOfProgram ============================================================//