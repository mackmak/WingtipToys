using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WingtipToys.Logic;

namespace WingtipToys
{
    public partial class AddToCart : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string rawId = Request.QueryString["ProductID"];

            int productId;

            if(!string.IsNullOrEmpty(rawId) && int.TryParse(rawId, out productId))
            {
                using(var shoppingCartActions = new ShoppingCartActions())
                {
                    shoppingCartActions.AddToCart(Convert.ToInt16(rawId));
                }
            }
            else
            {
                Debug.Fail("ERROR: AddToCart MUST NOT be called without a productId");

                throw new Exception("AddToCart MUST NOT be called without a Product ID");
            }
            Response.Redirect("ShoppingCart.aspx");
        }

    }

}