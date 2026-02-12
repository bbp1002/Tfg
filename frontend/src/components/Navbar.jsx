import { Link } from "react-router-dom";

export default function Navbar() {
  return (
    <nav
      style={{
        display: "flex",
        justifyContent: "center",
        gap: "25px",
        padding: "15px 290px",
        background: "rgba(255, 255, 255, 0.8)",
        backdropFilter: "blur(6px)",
        borderBottom: "1px solid #ddd",
        fontSize: "18px",
        fontWeight: "500"
      }}
    >
      <Link style={linkStyle} to="/">Home</Link>
      <Link style={linkStyle} to="/importar-pac">Importar PAC</Link>
      <Link style={linkStyle} to="/generar-propuesta">Propuesta IA</Link>
      <Link style={linkStyle} to="/exportar-propuesta">Exportar</Link>
    </nav>
  );
}

const linkStyle = {
  textDecoration: "none",
  color: "#333",
  padding: "8px 12px",
  borderRadius: "6px",
  transition: "0.2s",
};

linkStyle[":hover"] = {
  background: "#ddd",
};
