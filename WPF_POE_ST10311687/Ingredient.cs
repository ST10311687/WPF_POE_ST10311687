using System;
using System.Collections.Generic;
using System.Linq;


namespace WPF_POE_ST10311687
{
    public class Ingredient
    {
        public string Name { get; set; }
        public float Quantity { get; set; }
        public string Unit { get; set; }
        public int Calories { get; set; }
        public string FoodGroup { get; set; }

        /// <summary>
        /// Getters and setters for each ingredient
        /// </summary>
        /// <param name="name"></param>
        /// <param name="Quantity"></param>
        /// <param name="Unit"></param>
        public Ingredient(string name, float Quantity, String Unit, int calories, string foodGroup)
        {
            this.Name = name;
            this.Quantity = Quantity;
            this.Unit = Unit;
            this.Calories = calories;
            this.FoodGroup = foodGroup;
        }
    }
}