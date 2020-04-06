using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WingtipToys.Models;
using WingtipToys.Logic;

namespace WingtipToys.Login
{
    public partial class Admin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var productAction = Request.QueryString["ProductAction"];
            if (productAction == "add")
            {
                LabelAddStatus.Text = "Product Added";
            }

            if (productAction == "remove")
            {
                LabelRemoveStatus.Text = "Product Removed";
            }
        }



        private bool FileExtensionAccepted(string filePath)
        {
            var isFileOk = false;

            if (ProductImage.HasFile)
            {
                var fileExtension = System.IO.Path.GetExtension
                    (ProductImage.FileName).ToLower();

                string[] allowedExtensions = { ".gif", ".png", ".jpeg", ".jpg" };

                isFileOk = allowedExtensions.Contains(fileExtension);
            }

            return isFileOk;
        }

        protected void AddProductButton_Click(object sender, EventArgs e)
        {
            var path = Server.MapPath("~/Catalog/Images");

            if (FileExtensionAccepted(path))
            {
                try
                {
                    //Save uploaded (posted) file to images folder
                    ProductImage.PostedFile.SaveAs(path + ProductImage.FileName);

                    //Save uploaded (posted) file to Images/thumbs folder
                    ProductImage.PostedFile.SaveAs(path + "Thumbs/" + ProductImage.FileName);
                }
                catch (Exception ex)
                {
                    LabelAddStatus.Text = ex.Message;
                }
            }

            PersistData();
        }

        private void PersistData()
        {
            var wasProductAdded = new ProductActions().AddProduct(AddProductName.Text, AddProductDescription.Text,
            AddProductPrice.Text, DropDownAddCategory.SelectedValue, ProductImage.FileName);

            if (wasProductAdded)
            {
                RefreshPage("?ProductAction=add");
            }
            else
            {
                LabelAddStatus.Text = "File type not accepted";
            }
        }

        private void RefreshPage(string partialUrl)
        {

            var amountOfProducts = Request.Url.AbsoluteUri.Count() - Request.Url.Query.Count();
            var url = Request.Url.AbsoluteUri.Substring(0, amountOfProducts);

            Response.Redirect(url + partialUrl);
        }

        public IQueryable GetCategories()
        {
            return new ProductContext().Categories;
        }

        public IQueryable GetProducts()
        {
            return new ProductContext().Products;
        }

        protected void RemoveProductButton_Click(object sender, EventArgs e)
        {
            using(var _db = new ProductContext())
            {
                var productId = Convert.ToInt16(DropDownRemoveProduct.SelectedValue);
                var selectedProduct = _db.Products.First
                    (product => product.ProductID == productId);

                if(selectedProduct != null)
                {
                    _db.Products.Remove(selectedProduct);
                    _db.SaveChanges();

                    RefreshPage("?ProductAction=remove");
                }
                else
                {
                    LabelRemoveStatus.Text = "Product not found";
                }

            }
        }

    }
}