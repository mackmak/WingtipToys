using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WingtipToys.Logic;
using WingtipToys.Models;
using System.Collections.Specialized;
using System.Collections;
using System.Web.ModelBinding;


namespace WingtipToys
{
    public partial class ShoppingCart : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            using (var shoppingCartActions = new ShoppingCartActions())
            {
                decimal cartTotal = 0;
                cartTotal = shoppingCartActions.GetTotal();
                if (cartTotal > 0)
                {
                    lblTotal.Text = String.Format("{0:c}", cartTotal);
                }
                else
                {
                    LabelTotalText.Text = "";
                    lblTotal.Text = "";
                    ShoppingCartTitle.InnerText = "Shopping Cart is Empty";
                    UpdateBtn.Visible = false;
                }
            }
        }

        public List<CartItem> GetShoppingCartItems()
        {
            var actions = new ShoppingCartActions();

            return actions.GetCartItems();
        }

        public List<CartItem> UpdateCartItems()
        {
            using(var shoppingCartActions = new ShoppingCartActions())
            {
                var cartId = shoppingCartActions.GetCartId();


                ShoppingCartActions.ShoppingCartUpdates[] cartUpdates = new 
                    ShoppingCartActions.ShoppingCartUpdates[CartGridView.Rows.Count];

                for(int i = 0; i < CartGridView.Rows.Count; i++)
                {
                    IOrderedDictionary rowValues = new OrderedDictionary();
                    rowValues = GetValues(CartGridView.Rows[i]);
                    cartUpdates[i].ProductId = Convert.ToInt32(rowValues["ProductID"]);

                    var cbRemove = new CheckBox();
                    cbRemove = (CheckBox)CartGridView.Rows[i].FindControl("Remove");
                    cartUpdates[i].RemoveItem = cbRemove.Checked;

                    var quantityTextBox = new TextBox();
                    quantityTextBox = (TextBox)CartGridView.Rows[i].FindControl("PurchaseQuantity");
                    cartUpdates[i].PurchaseQuantity = Convert.ToInt16(quantityTextBox.Text);
                }

                shoppingCartActions.UpdateShoppingCartDatabase(cartId, cartUpdates);
                CartGridView.DataBind();
                lblTotal.Text = string.Format("{0:c}", shoppingCartActions.GetTotal());

                return shoppingCartActions.GetCartItems();
            }
        }

        public static IOrderedDictionary GetValues(GridViewRow gridViewRow)
        {
            var values = new OrderedDictionary();
            foreach (DataControlFieldCell cell in gridViewRow.Cells)
            {
                if (cell.Visible)
                {
                    cell.ContainingField.ExtractValuesFromCell(values, cell, gridViewRow.RowState, true);
                }
            }

            return values;
        }

        protected void UpdateBtn_Click(object sender, EventArgs e)
        {
            UpdateCartItems();
        }
    }
}