using System;
using System.Collections.Generic;
using System.Linq;


namespace WPF_POE_ST10311687
{
    public class Ingredient
    {
        public string Name { get; set; }
        public double Quantity { get; set; }
        public string UnitOfMeasurement { get; set; }
        private double ccalories { get; set; }
        public string FoodGroup { get; set; }

        /// <summary>
        /// Getters and setters for each ingredient
        /// </summary>       

        public double Calories
        {
            get
            {
                return ccalories;
            }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Calories cannot be less than 0");
                }
                ccalories = value;

            }
        }
    }
}


//============================================================ EndOfProgram ============================================================//