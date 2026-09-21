import { useState } from "react";
import { useNavigate } from "react-router-dom";
import api from "../services/api";
import "../styles/Register.css";

function Register() {
  const [fullName, setFullName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] =
    useState("");

  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [loading, setLoading] = useState(false);

  const navigate = useNavigate();

  const handleRegister = async (event) => {
    event.preventDefault();

    setError("");
    setSuccess("");

    if (password.length < 6) {
      setError(
        "Password must contain at least 6 characters."
      );
      return;
    }

    if (password !== confirmPassword) {
      setError("Passwords do not match.");
      return;
    }

    setLoading(true);

    try {
      const response = await api.post(
        "/Auth/register",
        {
          fullName,
          email,
          password,
        }
      );

      setSuccess(
        response.data.message ||
          "Registration successful."
      );

      setFullName("");
      setEmail("");
      setPassword("");
      setConfirmPassword("");

      setTimeout(() => {
        navigate("/login");
      }, 1200);
    } catch (error) {
      console.error(
        "Registration error:",
        error
      );

      setError(
        error.response?.data?.detail ||
          error.response?.data?.message ||
          "Registration failed. Please try again."
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="register-page">
      <div className="register-card">
        <h1>E-Commerce</h1>

        <h2>Create Account</h2>

        <p className="register-subtitle">
          Register to start shopping
        </p>

        {error && (
          <div className="register-error">
            {error}
          </div>
        )}

        {success && (
          <div className="register-success">
            {success}
          </div>
        )}

        <form onSubmit={handleRegister}>
          <div className="form-group">
            <label>Full Name</label>

            <input
              type="text"
              value={fullName}
              placeholder="Enter your full name"
              onChange={(event) =>
                setFullName(event.target.value)
              }
              maxLength={100}
              required
            />
          </div>

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
              placeholder="Minimum 6 characters"
              onChange={(event) =>
                setPassword(event.target.value)
              }
              minLength={6}
              required
            />
          </div>

          <div className="form-group">
            <label>Confirm Password</label>

            <input
              type="password"
              value={confirmPassword}
              placeholder="Enter password again"
              onChange={(event) =>
                setConfirmPassword(
                  event.target.value
                )
              }
              minLength={6}
              required
            />
          </div>

          <button
            type="submit"
            disabled={loading}
          >
            {loading
              ? "Creating Account..."
              : "Register"}
          </button>
        </form>

        <p className="login-link">
          Already have an account?{" "}
          <button
            type="button"
            className="link-button"
            onClick={() =>
              navigate("/login")
            }
          >
            Login
          </button>
        </p>
      </div>
    </div>
  );
}

export default Register;