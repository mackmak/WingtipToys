using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WingtipToys.Models;


namespace WingtipToys.Logic
{
    public class ShoppingCartActions : IDisposable
    {
        public string ShoppingCartId { get; set; }

        private ProductContext _db = new ProductContext();


        public void AddToCart(int id)
        {
            ShoppingCartId = GetCartId();

            var cartItem = _db.CartItems.SingleOrDefault(
                cart => cart.CartId == ShoppingCartId
                && cart.ProductId == id);

            //first item in the cart, cart not created yet
            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    ItemId = Guid.NewGuid().ToString(),
                    ProductId = id,
                    CartId = ShoppingCartId,
                    Product = _db.Products.SingleOrDefault(
                        product => product.ProductID == id),
                    Quantity = 1,
                    DateCreated = DateTime.Now
                };

                _db.CartItems.Add(cartItem);
            }
            else//cart already created, adding products
            {
                cartItem.Quantity++;
            }

            _db.SaveChanges();
        }

        public string GetCartId()
        {
            if (HttpContext.Current.Session["CartId"] == null)
            {
                if (!string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name))
                {
                    HttpContext.Current.Session["CartId"] =
                        HttpContext.Current.User.Identity;
                }
                else
                {
                    HttpContext.Current.Session["CartId"] =
                        Guid.NewGuid().ToString();
                }
            }

            return HttpContext.Current.Session["CartId"].ToString();
        }

        public List<CartItem> GetCartItems()
        {
            ShoppingCartId = GetCartId();

            return _db.CartItems.Where(cart => cart.CartId ==
                ShoppingCartId).ToList();
        }

        public decimal GetTotal()
        {
            ShoppingCartId = GetCartId();

            decimal? total = decimal.Zero;

            total = (decimal?)_db.CartItems.Where(items => items.CartId == ShoppingCartId)
                .Select(items => items.Quantity * items.Product.UnitPrice).Sum();

            return total ?? decimal.Zero;
        }

        public ShoppingCartActions GetCart(HttpContext httpContext)
        {
            using (var shoppingCartActions = new ShoppingCartActions())
            {
                shoppingCartActions.ShoppingCartId = GetCartId();
                return shoppingCartActions;
            }
        }

        public void UpdateShoppingCartDatabase(String cartId,
            ShoppingCartUpdates[] CartItemUpdates)
        {
            using (var db = new WingtipToys.Models.ProductContext())
            {
                try
                {
                    var cart = GetCartItems();

                    foreach (var cartItem in cart)
                    {
                        //comparing items in the cart with the amount of updates to be done
                        for (int i = 0; i < CartItemUpdates.Count(); i++)
                        {
                            //when the items match against each other(cart and updates)
                            if (cartItem.Product.ProductID == CartItemUpdates[i].ProductId)
                            {
                                //if update is set for removal
                                if (CartItemUpdates[i].PurchaseQuantity < 1 ||
                                    CartItemUpdates[i].RemoveItem == true)
                                {
                                    RemoveItem(cartId, cartItem.ProductId);
                                }
                                else
                                {
                                    UpdateItem(cartId, cartItem.ProductId,
                                        CartItemUpdates[i].PurchaseQuantity);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw new Exception("ERROR: Unable to Update Cart Database - " +
                        ex.Message, ex);
                }
            }
        }

        public void RemoveItem(string cartID, int productID)
        {
            using (var _db = new ProductContext())
            {
                try
                {
                    var cartItem = _db.CartItems.SingleOrDefault(
                        item => item.CartId == cartID &&
                        item.Product.ProductID == productID);

                    if (cartItem != null)
                    {
                        _db.CartItems.Remove(cartItem);
                        _db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"ERROR: Unable to remove cart item of ID {cartID}"
                        + ex.Message, ex);
                }
            }
        }

        public void UpdateItem(string cartID, int productID, int quantity)
        {
            using (var _db = new ProductContext())
            {
                try
                {
                    var cartItem = _db.CartItems.SingleOrDefault(
                        item => item.CartId == cartID &&
                        item.Product.ProductID == productID);

                    if (cartItem != null)
                    {
                        cartItem.Quantity = quantity;
                        _db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"ERROR: Unable to update cart of ID {cartID}"
                        + ex.Message, ex);
                }
            }
        }

        public void EmptyCart()
        {
            ShoppingCartId = GetCartId();

            var cartItems = _db.CartItems.Where(
                cartItem => cartItem.CartId == ShoppingCartId);

            foreach (var item in cartItems)
            {
                _db.CartItems.Remove(item);
            }

            _db.SaveChanges();
        }

        public int GetCount()
        {
            ShoppingCartId = GetCartId();

            int? count = _db.CartItems.Where(items => items.CartId == ShoppingCartId)
                .Select(items => (int?)items.Quantity).Sum();

            return count ?? 0;

        }

        public void MigrateCart(string cartId, string userName)
        {
            var cartItems = _db.CartItems.Where(items => items.CartId == cartId);
            foreach (var item in cartItems)
            {
                item.CartId = userName;
            }

            HttpContext.Current.Session["CartId"] = userName;

            _db.SaveChanges();
        }

        public void Dispose()
        {
            if (_db != null)
            {
                _db.Dispose();
                _db = null;
            }
        }

        public struct ShoppingCartUpdates
        {
            public int ProductId;
            public int PurchaseQuantity;
            public bool RemoveItem;
        }
    }
}