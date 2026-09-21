import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import api from "../services/api";
import "../styles/Products.css";

function Products() {
  const [products, setProducts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [search, setSearch] = useState("");
  const [cartMessage, setCartMessage] = useState("");
  const [addingProductId, setAddingProductId] = useState(null);

  const navigate = useNavigate();

  // --------------------------------------------------
  // LOAD PRODUCTS
  // --------------------------------------------------
  useEffect(() => {
    const loadProducts = async () => {
      try {
        setLoading(true);
        setError("");

        const response = await api.get("/Products");

        setProducts(response.data);
      } catch (error) {
        console.error("Error loading products:", error);

        setError(
          error.response?.data?.detail ||
            error.response?.data?.message ||
            "Unable to load products. Please make sure the backend API is running.",
        );
      } finally {
        setLoading(false);
      }
    };

    loadProducts();
  }, []);

  // --------------------------------------------------
  // ADD PRODUCT TO CART
  // --------------------------------------------------
  const handleAddToCart = async (product) => {
    setCartMessage("");

    const token = localStorage.getItem("token");

    if (!token) {
      setCartMessage("Please login before adding products to your cart.");

      return;
    }

    try {
      setAddingProductId(product.id);

      await api.post("/Cart", {
        productId: product.id,
        quantity: 1,
      });

      setCartMessage(`${product.name} added to cart successfully.`);
    } catch (error) {
      console.error("Error adding product to cart:", error);

      if (error.response?.status === 401) {
        setCartMessage("Your session has expired. Please login again.");
      } else {
        setCartMessage(
          error.response?.data?.detail ||
            error.response?.data?.message ||
            "Unable to add product to cart.",
        );
      }
    } finally {
      setAddingProductId(null);
    }
  };

  // --------------------------------------------------
  // FILTER PRODUCTS
  // --------------------------------------------------
  const filteredProducts = products.filter((product) =>
    product.name.toLowerCase().includes(search.toLowerCase()),
  );

  // --------------------------------------------------
  // LOADING
  // --------------------------------------------------
  if (loading) {
    return <div className="products-message">Loading products...</div>;
  }

  // --------------------------------------------------
  // ERROR
  // --------------------------------------------------
  if (error) {
    return (
      <div className="products-message">
        <p>{error}</p>
      </div>
    );
  }

  // --------------------------------------------------
  // PAGE
  // --------------------------------------------------
  return (
    <div className="products-page">
      <div className="products-header">
        <div>
          <h1>Products</h1>

          <p>Browse our available products</p>

          <input
            type="text"
            placeholder="Search products..."
            value={search}
            onChange={(event) => setSearch(event.target.value)}
            className="product-search"
          />
        </div>

        <div className="products-header-actions">
          <div className="product-count">
            {filteredProducts.length} Products
          </div>

          <button
            type="button"
            className="view-cart-button"
            onClick={() => navigate("/cart")}
          >
            View Cart
          </button>
        </div>
      </div>

      {/* CART SUCCESS / ERROR MESSAGE */}
      {cartMessage && <div className="cart-message">{cartMessage}</div>}

      {/* PRODUCTS */}
      {filteredProducts.length === 0 ? (
        <div className="products-message">No products found.</div>
      ) : (
        <div className="products-grid">
          {filteredProducts.map((product) => (
            <div className="product-card" key={product.id}>
              <div className="product-image-placeholder">📦</div>

              <div className="product-info">
                <h2>{product.name}</h2>

                <p className="product-description">
                  {product.description || "No description available"}
                </p>

                <div className="product-details">
                  <span className="product-price">
                    ₹{Number(product.price).toLocaleString("en-IN")}
                  </span>

                  <span
                    className={
                      product.stockQuantity > 0 ? "in-stock" : "out-of-stock"
                    }
                  >
                    {product.stockQuantity > 0
                      ? `${product.stockQuantity} in stock`
                      : "Out of stock"}
                  </span>
                </div>

                {/* ADD TO CART */}
                <button
                  type="button"
                  className="add-cart-button"
                  disabled={
                    product.stockQuantity <= 0 || addingProductId === product.id
                  }
                  onClick={() => handleAddToCart(product)}
                >
                  {product.stockQuantity <= 0
                    ? "Out of Stock"
                    : addingProductId === product.id
                      ? "Adding..."
                      : "Add to Cart"}
                </button>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}

export default Products;
