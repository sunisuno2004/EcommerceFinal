import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import api from "../services/api";
import "../styles/Checkout.css";

function Checkout() {
  const [cartItems, setCartItems] = useState([]);
  const [paymentMethod, setPaymentMethod] = useState("UPI");

  const [loading, setLoading] = useState(true);
  const [processing, setProcessing] = useState(false);

  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");

  const navigate = useNavigate();

  // --------------------------------------------------
  // LOAD CART
  // --------------------------------------------------
  useEffect(() => {
    let isMounted = true;

    const fetchCart = async () => {
      try {
        setError("");

        const response = await api.get("/Cart");

        if (isMounted) {
          setCartItems(response.data);
        }
      } catch (error) {
        console.error("Error loading checkout cart:", error);

        if (!isMounted) {
          return;
        }

        if (error.response?.status === 401) {
          setError("Your session has expired. Please login again.");
        } else {
          setError(
            error.response?.data?.detail ||
              error.response?.data?.message ||
              "Unable to load checkout information.",
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
  // TOTAL
  // --------------------------------------------------
  const totalAmount = cartItems.reduce((total, item) => {
    const price = Number(item.unitPrice ?? item.product?.price ?? 0);

    return total + price * item.quantity;
  }, 0);

  // --------------------------------------------------
  // PLACE ORDER + PAYMENT
  // --------------------------------------------------
  const handleCheckout = async () => {
    if (cartItems.length === 0) {
      setError("Your cart is empty. Add products before checkout.");

      return;
    }

    try {
      setProcessing(true);
      setError("");
      setSuccess("");

      // STEP 1:
      // Create order from logged-in user's cart.
      const orderResponse = await api.post("/Orders");

      const order = orderResponse.data;

      if (!order?.id) {
        throw new Error("Order ID was not returned by the server.");
      }

      // STEP 2:
      // Process payment for the newly created order.
      const paymentResponse = await api.post(`/Payment/${order.id}`, {
        paymentMethod,
      });

      setSuccess(
        paymentResponse.data?.message || "Payment successful. Order confirmed.",
      );

      // Order creation clears the cart on the backend.
      setCartItems([]);

      // Go to My Orders after a short success message.
      setTimeout(() => {
        navigate("/orders");
      }, 1500);
    } catch (error) {
      console.error("Checkout error:", error);

      setError(
        error.response?.data?.detail ||
          error.response?.data?.message ||
          error.message ||
          "Checkout failed. Please try again.",
      );
    } finally {
      setProcessing(false);
    }
  };

  // --------------------------------------------------
  // LOADING
  // --------------------------------------------------
  if (loading) {
    return <div className="checkout-message">Loading checkout...</div>;
  }

  // --------------------------------------------------
  // PAGE
  // --------------------------------------------------
  return (
    <div className="checkout-page">
      <div className="checkout-header">
        <div>
          <h1>Checkout</h1>

          <p>Review your order and select a payment method</p>
        </div>

        <button
          type="button"
          onClick={() => navigate("/cart")}
          disabled={processing}
        >
          Back to Cart
        </button>
      </div>

      {error && <div className="checkout-error">{error}</div>}

      {success && <div className="checkout-success">{success}</div>}

      {cartItems.length === 0 && !success ? (
        <div className="checkout-empty">
          <h2>Your cart is empty</h2>

          <button type="button" onClick={() => navigate("/products")}>
            Browse Products
          </button>
        </div>
      ) : (
        !success && (
          <div className="checkout-content">
            {/* ORDER SUMMARY */}
            <div className="checkout-section">
              <h2>Order Summary</h2>

              {cartItems.map((item) => {
                const productName =
                  item.product?.name || `Product ${item.productId}`;

                const price = Number(
                  item.unitPrice ?? item.product?.price ?? 0,
                );

                return (
                  <div className="checkout-item" key={item.id}>
                    <div>
                      <h3>{productName}</h3>

                      <p>Quantity: {item.quantity}</p>
                    </div>

                    <strong>
                      ₹{(price * item.quantity).toLocaleString("en-IN")}
                    </strong>
                  </div>
                );
              })}

              <div className="checkout-total">
                <span>Total</span>

                <strong>₹{totalAmount.toLocaleString("en-IN")}</strong>
              </div>
            </div>

            {/* PAYMENT */}
            <div className="checkout-section">
              <h2>Payment Method</h2>

              <label className="payment-option">
                <input
                  type="radio"
                  name="paymentMethod"
                  value="UPI"
                  checked={paymentMethod === "UPI"}
                  onChange={(event) => setPaymentMethod(event.target.value)}
                />

                <span>UPI</span>
              </label>

              <label className="payment-option">
                <input
                  type="radio"
                  name="paymentMethod"
                  value="Card"
                  checked={paymentMethod === "Card"}
                  onChange={(event) => setPaymentMethod(event.target.value)}
                />

                <span>Credit / Debit Card</span>
              </label>

              <label className="payment-option">
                <input
                  type="radio"
                  name="paymentMethod"
                  value="CashOnDelivery"
                  checked={paymentMethod === "CashOnDelivery"}
                  onChange={(event) => setPaymentMethod(event.target.value)}
                />

                <span>Cash on Delivery</span>
              </label>

              <button
                type="button"
                className="place-order-button"
                disabled={processing || cartItems.length === 0}
                onClick={handleCheckout}
              >
                {processing ? "Processing..." : "Place Order"}
              </button>
            </div>
          </div>
        )
      )}
    </div>
  );
}

export default Checkout;
