import { useNavigate } from "react-router-dom";
import "../styles/Home.css";

function Home() {
  const navigate = useNavigate();

  const token = localStorage.getItem("token");

  return (
    <div className="home-page">
      <section className="hero-section">
        <div className="hero-content">
          <div className="hero-badge">✨ Smart Shopping Experience</div>

          <h1>
            Welcome to <span>E-Commerce Application</span>
          </h1>

          <p>
            Discover products, manage your cart, place orders, and track your
            purchases — all in one modern shopping experience.
          </p>

          <div className="hero-buttons">
            <button
              className="primary-btn"
              type="button"
              onClick={() => navigate("/products")}
            >
              Shop Now
            </button>

            {!token ? (
              <button
                className="secondary-btn"
                type="button"
                onClick={() => navigate("/login")}
              >
                Login
              </button>
            ) : (
              <button
                className="secondary-btn"
                type="button"
                onClick={() => navigate("/orders")}
              >
                View Orders
              </button>
            )}
          </div>
        </div>

        <div className="hero-card">
          <div className="hero-card-box">
            <div className="hero-icon">🛍️</div>
            <h3>Easy Shopping</h3>
            <p>Browse products and add them to your cart in seconds.</p>
          </div>

          <div className="hero-card-box">
            <div className="hero-icon">📦</div>
            <h3>Order Tracking</h3>
            <p>View your orders and payment details anytime.</p>
          </div>

          <div className="hero-card-box">
            <div className="hero-icon">💳</div>
            <h3>Secure Checkout</h3>
            <p>Place orders with different payment methods safely.</p>
          </div>
        </div>
      </section>

      <section className="features-section">
        <h2>Why Choose Our Store?</h2>

        <div className="features-grid">
          <div className="feature-card">
            <div className="feature-emoji">🚚</div>
            <h3>Fast Delivery</h3>
            <p>Quick and reliable order processing for a smooth experience.</p>
          </div>

          <div className="feature-card">
            <div className="feature-emoji">🛒</div>
            <h3>Simple Cart</h3>
            <p>Manage quantities, remove products, and checkout with ease.</p>
          </div>

          <div className="feature-card">
            <div className="feature-emoji">🔐</div>
            <h3>Secure Access</h3>
            <p>Protected login and authorized user-based operations.</p>
          </div>
        </div>
      </section>
    </div>
  );
}

export default Home;
