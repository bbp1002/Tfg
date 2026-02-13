import { useState } from "react";
import { registerUser } from "../api/authApi";

export default function Register() {
  const [nombre, setNombre] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  const [resultado, setResultado] = useState(null);

  const enviar = async () => {
    try {
      const res = await registerUser({
        nombre,
        email,
        password
      });

      if (res.status === 200 || res.status === 201) {
        setResultado("ok");
      } else {
        setResultado("error");
      }
    } catch (err) {
      console.error(err);
      setResultado("error");
    }
  };

  return (
    <div style={centerBox}>
      <h2>Registro</h2>

      <div style={{ marginTop: "20px" , color: "black"}}>
        <label>Nombre:</label>
        <input
          type="text"
          value={nombre}
          onChange={(e) => setNombre(e.target.value)}
        />
      </div>

      <div style={{ marginTop: "20px" , color: "black"}}>
        <label>Email:</label>
        <input
          type="email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
        />
      </div>

      <div style={{ marginTop: "20px" , color: "black"}}>
        <label>Contraseña:</label>
        <input
          type="password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
        />
      </div>

      <button style={{ marginTop: "20px" }} onClick={enviar}>
        Registrarse
      </button>

      {resultado === "ok" && (
        <p style={{ color: "green", marginTop: "20px" }}>
          Usuario registrado correctamente.
        </p>
      )}

      {resultado === "error" && (
        <p style={{ color: "red", marginTop: "20px" }}>
          Error al registrar el usuario.
        </p>
      )}
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
  textAlign: "center",
  color: "black"
};