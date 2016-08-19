using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zintegrowany_system_przeciwpozarowy
{
    public class Logs
    {
        public int Id { get; set; }

        [Display(Name = "Data")]
        [Required]
        public DateTime Date { get; set; }

        [Display(Name = "Akcja")]
        [Required]
        public string Action { get; set; }

        [Display(Name = "Co")]
        [Required]
        public string What { get; set; }

        [Display(Name = "Komu")]
        [Required]
        public string WhatId { get; set; }

    }
}
