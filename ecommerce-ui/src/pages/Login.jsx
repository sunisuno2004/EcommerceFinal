import { useState } from "react";
import { useNavigate } from "react-router-dom";
import api from "../services/api";
import "../styles/Login.css";

function Login() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  const navigate = useNavigate();

  const handleLogin = async (event) => {
    event.preventDefault();

    setError("");
    setLoading(true);

    try {
      const response = await api.post("/Auth/login", {
        email,
        password,
      });

      console.log("Login response:", response.data);

      const token = response.data.token;
const user = response.data.user;

if (!token) {
  setError("Token was not returned by the server.");
  return;
}

localStorage.setItem("token", token);

if (user) {
  localStorage.setItem(
    "user",
    JSON.stringify(user)
  );
}

navigate("/products");
    } catch (error) {
      console.error("Login error:", error);

      setError(
  error.response?.data?.detail ||
    error.response?.data?.message ||
    "Login failed. Please check your email and password."
);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="login-page">
      <div className="login-card">
        <h1>E-Commerce</h1>

        <h2>Login</h2>

        <p className="login-subtitle">
          Sign in to continue
        </p>

        {error && (
          <div className="login-error">
            {error}
          </div>
        )}

        <form onSubmit={handleLogin}>
          <div className="form-group">
            <label>Email</label>

            <input
              type="email"
              value={email}
              placeholder="Enter your email"
              onChange={(event) =>
                setEmail(event.target.value)
              }
              required
            />
          </div>

          <div className="form-group">
            <label>Password</label>

            <input
              type="password"
              value={password}
              placeholder="Enter your password"
              onChange={(event) =>
                setPassword(event.target.value)
              }
              required
            />
          </div>

          <button
            type="submit"
            disabled={loading}
          >
            {loading ? "Logging in..." : "Login"}
          </button>
        </form>
      </div>
    </div>
  );
}

export default Login;