import { Link } from "react-router-dom";

export default function Navbar() {
  return (
    <nav style={{ padding: "10px", background: "#eee" }}>
      <Link to="/login">Login</Link> |{" "}
      <Link to="/register">Registro</Link> |{" "}
      <Link to="/importar-pac">Importar PAC</Link> |{" "}
      <Link to="/generar-propuesta">Propuesta IA</Link> |{" "}
      <Link to="/exportar-propuesta">Exportar</Link> |{" "}
      <Link to="/ver-en-sigpac">SIGPAC</Link> |{" "}
      <Link to="/asignar-nombre">Asignar nombre</Link>
    </nav>
  );
}
