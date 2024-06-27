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
    /// 5 - read me file
    /// 7 - resize exit
    /// 8 - capital letters
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
           
        }

        // ------------------------- Error handling method -------------------------

        private void displayNoRecipe()
        {
            MessageBox.Show("There is no recipe in the list.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var recipeName in Recipes.Keys)
            {
                txtSpecificRecipe.Items.Add(recipeName); //Adding the recipe name to the combo box
            }

        }

        // ------------------------- Recipe name loader method -------------------------
        private void LoadRecipeNames()
        {
            txtSpecificRecipe.Items.Clear(); //Clearing all the current items in the combo box
            txtSpecificRecipeToScale.Items.Clear();
            txtResetRecipe.Items.Clear();
            foreach (var recipeName in Recipes.Keys)
            {
                txtSpecificRecipe.Items.Add(recipeName); //Adding recipe name the the combo boxes
                txtSpecificRecipeToScale.Items.Add(recipeName);
                txtResetRecipe.Items.Add(recipeName);
            }
        }

        // ------------------------- Calorie limiter method -------------------------
        private void ExceededCaloriesHandler(string message, double totalCalories)
        {
            string messageWithCalories = $"\t* Total calories (Calories quantify food energy intake): {totalCalories}\n" + message + "\n";

            lstDisplayDetails.Items.Add(messageWithCalories); //Adding the formated message to the list box
        }

        // ------------------------- Scale specific recipe method -------------------------
        private void scaleRecipeSpecificName(double scalingNumber)
        {
            lstDisplayDetails.Items.Clear();
            string recipeName1 = txtSpecificRecipeToScale.Text; //Getting the name of the recipe to scale from the combo box 
            if (Recipes.ContainsKey(recipeName1)) // Check if the recipe exists in the Recipes dictionary
            {
                MessageBoxResult result = MessageBox.Show($"Are you sure you want to scale the {recipeName1} recipe?", "Confirmation", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    Recipes[recipeName1].ScaleRecipe(scalingNumber); // Scale the recipe by the entered scaling number
                    lstDisplayDetails.Items.Add("The recipe has been scaled. Here is the updated recipe:\n");
                    lstDisplayDetails.Items.Add(Recipes[recipeName1].DisplayRecipe()); 
                    double totalCalories = Recipes[recipeName1].TotalCalories(); // Calculate the total calories of the scaled recipe
                    Recipes[recipeName1].notifyer(); // Notify any subscribers about the change in the recipe
                }

            }
            else
            {
                MessageBox.Show("There is no recipe with that name", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        // ------------------------- Add recipe button -------------------------
        private void btnAddRecipe_Click(object sender, RoutedEventArgs e)
        {
            addIngredient AddIngredient = new addIngredient();
            AddIngredient.ShowDialog(); //Take ingredient details from addIngredient

            allIngredients = AddIngredient.ingredients;
            allOriginalQuantities = AddIngredient.originalQuantities;
            allOriginalUnits = AddIngredient.originalUnits;
            allSteps = AddIngredient.steps;
            recipeName = AddIngredient.recipeName;

            if (recipeName != null)
            {
                recipe = new Recipe(recipeName, allIngredients, allOriginalQuantities, allOriginalUnits, allSteps);               

                if (!Recipes.ContainsKey(recipe.RecipeName)) //Check to see if the recipe name already exists
                {
                    Recipes.Add(recipe.RecipeName, recipe); //Adding the recipe
                    lstDisplayDetails.Items.Clear();
                    LoadRecipeNames(); //Method used to load recipe names
                }
                else
                {
                    MessageBox.Show("A recipe with this name already exists.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            
        }

        // ------------------------- Display all recipes button button -------------------------
        private void btnDisplayAllRecipe_Click(object sender, RoutedEventArgs e)
        {
            if (Recipes.Count == 0) //Check to see if recipes are in collection
            {
                displayNoRecipe();
            }
            else
            {
                lstDisplayDetails.Items.Clear(); //Clear list so user does not enter duplicates
                List<string> sortedKeys = new List<string>(Recipes.Keys);
                sortedKeys.Sort();

                foreach (string key in sortedKeys)
                {
                    Recipe recipe = Recipes[key];

                    recipe.ExceededCalories -= ExceededCaloriesHandler; // Unsubscribe from the ExceededCalories event to prevent multiple subscriptions


                    recipe.ExceededCalories += ExceededCaloriesHandler;  // Subscribe to the ExceededCalories event
                    lstDisplayDetails.Items.Add(recipe.DisplayRecipe());
                    recipe.notifyer(); 
                    lstDisplayDetails.Items.Add("-------------------------------------------------------" +
                        "-------------------------------------------------------------------------------");
                }
            }
        }

        // ------------------------- Display specific recipe button -------------------------
        private void btnDisplaySpecificRecipe_Click(object sender, RoutedEventArgs e)
        {
            if (Recipes.Count == 0)
            {
                displayNoRecipe();
            }
            else
            {
                lstDisplayDetails.Items.Clear();
                string recipeName = txtSpecificRecipe.Text; // Get the name of the specific recipe to display from the ComboBox
                if (Recipes.ContainsKey(recipeName))
                {
                    recipe.ExceededCalories -= ExceededCaloriesHandler; // Unsubscribe from the ExceededCalories event to prevent multiple subscriptions

                    recipe.ExceededCalories += ExceededCaloriesHandler; // Subscribe to the ExceededCalories event


                    lstDisplayDetails.Items.Add(Recipes[recipeName].DisplayRecipe()); 

                    double totalCalories = Recipes[recipeName].TotalCalories(); // Calculate the total calories of the recipe
                    Recipes[recipeName].notifyer();

                    var stepsWithCheckBox = Recipes[recipeName].Steps.Select(step => new { Step = step }).ToList();
                }
                else
                {
                    MessageBox.Show("Invalid recipe name please check the spelling of the recipe.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        // ------------------------- Scale recipe button -------------------------
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

        // ------------------------- Reset recipe button -------------------------
        private void btnResetRecipe_Click(object sender, RoutedEventArgs e)
        {
            if (Recipes.Count == 0)
            {
                displayNoRecipe();
            }
            else
            {
                string resetRecipeName = txtResetRecipe.Text; // Get the name of the recipe to reset from the ComboBox
                if (Recipes.ContainsKey(resetRecipeName)) // Check if the recipe exists in the Recipes dictionary
                {
                    MessageBoxResult result = MessageBox.Show($"Are you sure you want to reset the {resetRecipeName} recipe?", "Confirmation", MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.Yes)
                    {
                        lstDisplayDetails.Items.Clear();
                        bool success = Recipes[resetRecipeName].ResetRecipe(); // Attempt to reset the recipe to its original state
                        if (success)
                        {
                            lstDisplayDetails.Items.Add("The recipe has been reset. Here is the original recipe:");
                            lstDisplayDetails.Items.Add(Recipes[resetRecipeName].DisplayRecipe());
                            double totalCalories = Recipes[resetRecipeName].TotalCalories(); // Calculate the total calories of the reset recipe
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

        // ------------------------- Menu button -------------------------
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

        // ------------------------- Clear recipe button -------------------------
        private void btnClearRecipe_Click(object sender, RoutedEventArgs e)
        {
            if (Recipes.Count == 0)
            {
                displayNoRecipe();
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to clear all recipes? :", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                lstDisplayDetails.Items.Clear(); // Clear the list display to avoid duplicate entries

                if (result == MessageBoxResult.Yes)
                {
                    Recipes.Clear(); // Clear all ComboBoxes related to recipe
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

        // ------------------------- Exit button -------------------------
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); //Closes the application
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