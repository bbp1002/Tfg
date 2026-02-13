import { BrowserRouter, Routes, Route, useLocation } from "react-router-dom";
import Navbar from "./components/Navbar";

import Login from "./pages/Login";
import Register from "./pages/Register";
import ImportarPac from "./pages/ImportarPac";
import GenerarPropuesta from "./pages/GenerarPropuesta";
import ExportarPropuesta from "./pages/ExportarPropuesta";
import VerEnSigpac from "./pages/VerEnSigpac";
import AsignarNombre from "./pages/AsignarNombre";
import ParcelasConHistorico from "./pages/ParcelasConHistorico";

function Layout() {
  const location = useLocation();

  // Rutas donde NO queremos cabecera ni navbar
  const rutasSinLayout = ["/", "/register"];
  const ocultarLayout = rutasSinLayout.includes(location.pathname);

  return (
    <>
      {/* CABECERA (100% ancho SIEMPRE) */}
      {!ocultarLayout && (
        <header
          style={{
            width: "100%",
            position: "fixed",
            top: 0,
            left: 0,
            zIndex: 1000,
            background: "rgba(255, 255, 255, 0.8)",
            backdropFilter: "blur(6px)",
            borderBottom: "1px solid #ddd",
            display: "flex",
            justifyContent: "center",
            padding: "15px 20px"
          }}
        >
          <img src="/logo.png" alt="Logo" style={{ height: "80px" }} />
        </header>
      )}

      {/* NAVBAR (100% ancho SIEMPRE) */}
      {!ocultarLayout && <Navbar />}

      {/* CONTENIDO CENTRADO */}
      <div
        style={{
          marginTop: ocultarLayout ? "0px" : "165px", // cabecera + navbar
          maxWidth: "1200px",
          marginLeft: "auto",
          marginRight: "auto",
          padding: "0 20px"
        }}
      >
        <Routes>
          <Route path="/" element={<Login />} />
          <Route path="/register" element={<Register />} />
          <Route path="/importar-pac" element={<ImportarPac />} />
          <Route path="/generar-propuesta" element={<GenerarPropuesta />} />
          <Route path="/exportar-propuesta" element={<ExportarPropuesta />} />
          <Route path="/ver-en-sigpac" element={<VerEnSigpac />} />
          <Route path="/asignar-nombre" element={<AsignarNombre />} />
          <Route path="/home" element={<ParcelasConHistorico />} />
        </Routes>
      </div>
    </>
  );
}

export default function App() {
  return (
    <BrowserRouter>
      <Layout />
    </BrowserRouter>
  );
}

