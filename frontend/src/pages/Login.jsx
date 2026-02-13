import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { login } from "../api/authApi";

export default function Login() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();

    try {
      const res = await login({ email, password });
      localStorage.setItem("jwt", res.data.token);

      navigate("/home"); // Redirige a Home

    } catch {
      alert("Error en login");
    }
  };

  return (
    <div style={centerBox}>
      <h2 style={title}>Iniciar sesión</h2>

      <form style={form} onSubmit={handleSubmit}>
        <label>Email</label>
        <input
          type="email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
        />

        <label>Contraseña</label>
        <input
          type="password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
        />

        <button type="submit" className="btn" style={{ width: "100%" }}>
          Entrar
        </button>
      </form>

      <p style={{ marginTop: "15px" , color: "black"}}>
        ¿No tienes cuenta?{" "}
        <a href="/register" style={{ color: "#1E3A8A", fontWeight: "600" }}>
          Regístrate aquí
        </a>
      </p>
    </div>
  );
}


const centerBox = {
  maxWidth: "400px",
  margin: "80px auto",
  padding: "30px",
  background: "white",
  borderRadius: "12px",
  boxShadow: "0 4px 20px rgba(0,0,0,0.15)",
  textAlign: "center"
};

const title = {
  color: "#1E3A8A",
  marginBottom: "20px"
};

const form = {
  display: "flex",
  flexDirection: "column",
  gap: "12px",
  color: "black",
  textAlign: "left"
};

