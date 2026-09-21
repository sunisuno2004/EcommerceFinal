import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import api from "../services/api";
import "../styles/Orders.css";

function Orders() {
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const navigate = useNavigate();

  useEffect(() => {
    let isMounted = true;

    const loadOrders = async () => {
      try {
        setLoading(true);
        setError("");

        const response = await api.get("/Orders");

        if (isMounted) {
          setOrders(response.data);
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

  if (loading) {
    return <div className="orders-message">Loading orders...</div>;
  }

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

                <span
                  className={`order-status ${
                    order.status ? order.status.toLowerCase() : ""
                  }`}
                >
                  {order.status}
                </span>
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
            </div>
          ))}
        </div>
      )}
    </div>
  );
}

export default Orders;
