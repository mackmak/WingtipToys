using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WingtipToys.Models;

namespace WingtipToys.Logic
{
    public class ProductActions
    {
        public bool AddProduct(string ProductName, string ProductDesc, string ProductPrice, 
            string ProductCategory, string ProductImagePath)
        {
            var product = new Product
            {
                ProductName = ProductName,
                Description = ProductDesc,
                UnitPrice = Convert.ToDouble(ProductPrice),
                ImagePath = ProductImagePath,
                CategoryID = Convert.ToInt32(ProductCategory)
            };

            using(var _db = new ProductContext())
            {
                _db.Products.Add(product);
                _db.SaveChanges();
            }

            return true;

        }
    }
}