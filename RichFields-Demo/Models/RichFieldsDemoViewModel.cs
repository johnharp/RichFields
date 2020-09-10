using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace RichFields_Demo.Models
{
    public class RichFieldsDemoViewModel
    {
        [Display(Name = "Example Percent Field")]
        public Int16? ExamplePercent { get; set; }

        [Display(Name = "Example Money (2-digit cents)")]
        public Decimal? ExampleMoney { get; set; }

        [Display(Name = "Example Int (16 bit)")]
        public Int16? ExampleInt { get; set; }

        [Display(Name = "Example Date")]
        public DateTime? ExampleDate { get; set; }

        [Display(Name = "Example Month/Year")]
        public DateTime? ExampleMonthYear { get; set; }

        [Display(Name = "Example Text")]
        public string ExampleText { get; set; }
    }
}