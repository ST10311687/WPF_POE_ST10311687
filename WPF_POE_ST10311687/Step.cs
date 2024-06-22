using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_POE_ST10311687
{
    public class Step
    {
        public string Description { get; set; }
        public Step(string description)
        {
            Description = description;
        }
    }
}