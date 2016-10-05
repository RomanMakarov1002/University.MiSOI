using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Misoi.Models
{
    public class LoadedImage
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public string Format { get; set; }
        public string Negative { get; set; }
        public string ImagePhysicalPath { get; set; }
        public string ImageFiltered { get; set; }
    }
}