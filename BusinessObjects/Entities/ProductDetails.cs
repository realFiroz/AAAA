﻿namespace Entities
{
    public class ProductDetails
    {
        public string  Name { get; set; }
        public string  SKUNumber { get; set; }
        public string  Specification { get; set; }
        public string  ImagePath { get; set; }
        public bool IsAvailable { get; set; }
        public double Price { get; set; }
        public double Discount { get; set; }
        public bool  IsPackaging { get; set; }
        public string  Category { get; set; }
    }
}
