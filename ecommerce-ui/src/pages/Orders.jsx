import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import api from "../services/api";
import "../styles/Orders.css";

function Orders() {
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const navigate = useNavigate();

  // --------------------------------------------------
  // ADD PRODUCT + PAYMENT DETAILS TO ORDERS
  // --------------------------------------------------
  const enrichOrders = async (ordersData) => {
    const enrichedOrders = await Promise.all(
      ordersData.map(async (order) => {
        // Load product details
        const enrichedItems = await Promise.all(
          (order.orderItems || []).map(async (item) => {
            try {
              const productResponse = await api.get(
                `/Products/${item.productId}`,
              );

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

        // Load payment details
        let payment = null;

        try {
          const paymentResponse = await api.get(`/Payments/order/${order.id}`);

          payment = paymentResponse.data;
        } catch (error) {
          // A missing payment should not break the whole Orders page
          console.warn(`Unable to load payment for order ${order.id}:`, error);
        }

        return {
          ...order,
          orderItems: enrichedItems,
          payment,
        };
      }),
    );

    return enrichedOrders;
  };

  // --------------------------------------------------
  // LOAD ORDERS
  // --------------------------------------------------
  useEffect(() => {
    let isMounted = true;

    const loadOrders = async () => {
      try {
        setLoading(true);
        setError("");

        const response = await api.get("/Orders");

        const enrichedOrders = await enrichOrders(response.data);

        if (isMounted) {
          setOrders(enrichedOrders);
        }
      } catch (error) {
        console.error("Error loading orders:", error);

        if (!isMounted) {
          return;
        }

        if (error.response?.status === 401) {
          setError("Your session has expired. Please login again.");
        } else {
          setError(
            error.response?.data?.detail ||
              error.response?.data?.message ||
              "Unable to load your orders.",
          );
        }
      } finally {
        if (isMounted) {
          setLoading(false);
        }
      }
    };

    loadOrders();

    return () => {
      isMounted = false;
    };
  }, []);

  // --------------------------------------------------
  // LOADING
  // --------------------------------------------------
  if (loading) {
    return <div className="orders-message">Loading orders...</div>;
  }

  // --------------------------------------------------
  // PAGE
  // --------------------------------------------------
  return (
    <div className="orders-page">
      <div className="orders-header">
        <div>
          <h1>My Orders</h1>
          <p>View your order history</p>
        </div>

        <button type="button" onClick={() => navigate("/products")}>
          Continue Shopping
        </button>
      </div>

      {error && <div className="orders-error">{error}</div>}

      {!error && orders.length === 0 ? (
        <div className="empty-orders">
          <h2>No orders yet</h2>

          <p>Your completed orders will appear here.</p>

          <button type="button" onClick={() => navigate("/products")}>
            Browse Products
          </button>
        </div>
      ) : (
        <div className="orders-list">
          {orders.map((order) => (
            <div className="order-card" key={order.id}>
              <div className="order-card-header">
                <div>
                  <h2>Order #{order.id}</h2>

                  <p>
                    {order.orderDate
                      ? new Date(order.orderDate).toLocaleString("en-IN")
                      : "Order date unavailable"}
                  </p>
                </div>

                <div>
                  <div>
                    <strong>Order Status: </strong>

                    <span
                      className={`order-status ${
                        order.status ? order.status.toLowerCase() : ""
                      }`}
                    >
                      {order.status || "Unknown"}
                    </span>
                  </div>

                  <div style={{ marginTop: "8px" }}>
                    <strong>Payment Status: </strong>

                    <span
                      className={`payment-status ${
                        order.payment?.status
                          ? order.payment.status.toLowerCase()
                          : ""
                      }`}
                    >
                      {order.payment?.status || "Not Available"}
                    </span>
                  </div>
                </div>
              </div>

              <div className="order-items">
                {order.orderItems?.map((item) => {
                  const productName =
                    item.product?.name || `Product ${item.productId}`;

                  const price = Number(item.unitPrice ?? 0);

                  return (
                    <div className="order-item" key={item.id}>
                      <div>
                        <h3>{productName}</h3>

                        <p>Quantity: {item.quantity}</p>

                        <p>Unit Price: ₹{price.toLocaleString("en-IN")}</p>
                      </div>

                      <strong>
                        ₹{(price * item.quantity).toLocaleString("en-IN")}
                      </strong>
                    </div>
                  );
                })}
              </div>

              <div className="order-total">
                <span>Total Amount</span>

                <strong>
                  ₹{Number(order.totalAmount ?? 0).toLocaleString("en-IN")}
                </strong>
              </div>

              {order.payment && (
                <div
                  style={{
                    marginTop: "12px",
                    paddingTop: "12px",
                    borderTop: "1px solid #ddd",
                  }}
                >
                  <p>
                    <strong>Payment Method:</strong>{" "}
                    {order.payment.paymentMethod}
                  </p>

                  {order.payment.transactionReference && (
                    <p>
                      <strong>Transaction Reference:</strong>{" "}
                      {order.payment.transactionReference}
                    </p>
                  )}
                </div>
              )}
            </div>
          ))}
        </div>
      )}
    </div>
  );
}

export default Orders;
