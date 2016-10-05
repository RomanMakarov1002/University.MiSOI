using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Misoi.Models
{
    public class ImageChart
    {
        public int PixelsCount { get; set; }
        public int[] Brightness { get; set; }
        public double[] BrightnessPersentage { get; set; }
        public double MaxPersentage { get; set; }
    }
}