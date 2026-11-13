import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import api from "../services/api";
import "../styles/Cart.css";

function Cart() {
  const [cartItems, setCartItems] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [updatingProductId, setUpdatingProductId] = useState(null);

  const navigate = useNavigate();

  // --------------------------------------------------
  // ADD PRODUCT DETAILS TO CART ITEMS
  // --------------------------------------------------
  const enrichCartItems = async (items) => {
    const enrichedItems = await Promise.all(
      items.map(async (item) => {
        try {
          const productResponse = await api.get(`/Products/${item.productId}`);

          return {
            ...item,
            product: productResponse.data,
          };
        } catch (error) {
          console.error(`Unable to load product ${item.productId}:`, error);

          return item;
        }
      }),
    );

    return enrichedItems;
  };

  // --------------------------------------------------
  // LOAD CART WHEN PAGE OPENS
  // --------------------------------------------------
  useEffect(() => {
    let isMounted = true;

    const fetchCart = async () => {
      try {
        setError("");

        const response = await api.get("/Cart");

        const enrichedItems = await enrichCartItems(response.data);

        if (isMounted) {
          setCartItems(enrichedItems);
        }
      } catch (error) {
        console.error("Error loading cart:", error);

        if (!isMounted) {
          return;
        }

        if (error.response?.status === 401) {
          setError("Your session has expired. Please login again.");
        } else {
          setError(
            error.response?.data?.detail ||
              error.response?.data?.message ||
              "Unable to load your cart.",
          );
        }
      } finally {
        if (isMounted) {
          setLoading(false);
        }
      }
    };

    fetchCart();

    return () => {
      isMounted = false;
    };
  }, []);

  // --------------------------------------------------
  // REFRESH CART
  // --------------------------------------------------
  const refreshCart = async () => {
    const response = await api.get("/Cart");

    const enrichedItems = await enrichCartItems(response.data);

    setCartItems(enrichedItems);
  };

  // --------------------------------------------------
  // UPDATE QUANTITY
  // --------------------------------------------------
  const updateQuantity = async (productId, newQuantity) => {
    if (newQuantity < 1) {
      return;
    }

    try {
      setError("");
      setUpdatingProductId(productId);

      await api.put(`/Cart/${productId}`, {
        quantity: newQuantity,
      });

      await refreshCart();
    } catch (error) {
      console.error("Error updating cart:", error);

      setError(
        error.response?.data?.detail ||
          error.response?.data?.message ||
          "Unable to update cart quantity.",
      );
    } finally {
      setUpdatingProductId(null);
    }
  };

  // --------------------------------------------------
  // REMOVE PRODUCT
  // --------------------------------------------------
  const removeItem = async (productId) => {
    try {
      setError("");
      setUpdatingProductId(productId);

      await api.delete(`/Cart/${productId}`);

      await refreshCart();
    } catch (error) {
      console.error("Error removing cart item:", error);

      setError(
        error.response?.data?.detail ||
          error.response?.data?.message ||
          "Unable to remove product from cart.",
      );
    } finally {
      setUpdatingProductId(null);
    }
  };

  // --------------------------------------------------
  // CLEAR CART
  // --------------------------------------------------
  const clearCart = async () => {
    try {
      setError("");

      await api.delete("/Cart");

      setCartItems([]);
    } catch (error) {
      console.error("Error clearing cart:", error);

      setError(
        error.response?.data?.detail ||
          error.response?.data?.message ||
          "Unable to clear cart.",
      );
    }
  };

  // --------------------------------------------------
  // CALCULATE TOTAL
  // --------------------------------------------------
  const totalAmount = cartItems.reduce((total, item) => {
    const price = Number(item.unitPrice ?? item.product?.price ?? 0);

    return total + price * item.quantity;
  }, 0);

  // --------------------------------------------------
  // LOADING
  // --------------------------------------------------
  if (loading) {
    return <div className="cart-message-page">Loading cart...</div>;
  }

  // --------------------------------------------------
  // PAGE
  // --------------------------------------------------
  return (
    <div className="cart-page">
      <div className="cart-header">
        <div>
          <h1>Shopping Cart</h1>
          <p>Review the products in your cart</p>
        </div>

        <button type="button" onClick={() => navigate("/products")}>
          Continue Shopping
        </button>
      </div>

      {error && <div className="cart-error">{error}</div>}

      {cartItems.length === 0 ? (
        <div className="empty-cart">
          <h2>Your cart is empty</h2>

          <button type="button" onClick={() => navigate("/products")}>
            Browse Products
          </button>
        </div>
      ) : (
        <>
          <div className="cart-items">
            {cartItems.map((item) => {
              const productName =
                item.product?.name || `Product ${item.productId}`;

              const price = Number(item.unitPrice ?? item.product?.price ?? 0);

              return (
                <div className="cart-item" key={item.id}>
                  <div className="cart-product">
                    <div className="cart-product-icon">📦</div>

                    <div>
                      <h2>{productName}</h2>

                      <p>₹{price.toLocaleString("en-IN")}</p>
                    </div>
                  </div>

                  <div className="quantity-control">
                    <button
                      type="button"
                      disabled={
                        item.quantity <= 1 ||
                        updatingProductId === item.productId
                      }
                      onClick={() =>
                        updateQuantity(item.productId, item.quantity - 1)
                      }
                    >
                      -
                    </button>

                    <span>{item.quantity}</span>

                    <button
                      type="button"
                      disabled={updatingProductId === item.productId}
                      onClick={() =>
                        updateQuantity(item.productId, item.quantity + 1)
                      }
                    >
                      +
                    </button>
                  </div>

                  <div className="cart-subtotal">
                    ₹{(price * item.quantity).toLocaleString("en-IN")}
                  </div>

                  <button
                    type="button"
                    className="remove-button"
                    disabled={updatingProductId === item.productId}
                    onClick={() => removeItem(item.productId)}
                  >
                    Remove
                  </button>
                </div>
              );
            })}
          </div>

          <div className="cart-summary">
            <h2>Total: ₹{totalAmount.toLocaleString("en-IN")}</h2>

            <div className="cart-actions">
              <button type="button" onClick={clearCart}>
                Clear Cart
              </button>

              <button
                type="button"
                className="checkout-button"
                onClick={() => navigate("/checkout")}
              >
                Proceed to Checkout
              </button>
            </div>
          </div>
        </>
      )}
    </div>
  );
}

export default Cart;
