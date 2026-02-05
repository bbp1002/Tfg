import { BrowserRouter, Routes, Route } from "react-router-dom";
import Navbar from "./components/Navbar";

import Login from "./pages/Login";
import Register from "./pages/Register";
import ImportarPac from "./pages/ImportarPac";
import GenerarPropuesta from "./pages/GenerarPropuesta";
import ExportarPropuesta from "./pages/ExportarPropuesta";
import VerEnSigpac from "./pages/VerEnSigpac";
import AsignarNombre from "./pages/AsignarNombre";
import ParcelasConHistorico from "./pages/ParcelasConHistorico";

function App() {
  return (
    <BrowserRouter>
    <header style={{
  display: "flex",
  justifyContent: "center",
  alignItems: "center",
  padding: "20px",
  background: "rgba(255,255,255,0.7)",
  backdropFilter: "blur(6px)"
}}>
  <img src="/logo.png" alt="Logo" style={{ height: "80px" }} />
</header>
      <Navbar />
      <div style={{ padding: "20px" }}>
        <Routes>
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />
          <Route path="/importar-pac" element={<ImportarPac />} />
          <Route path="/generar-propuesta" element={<GenerarPropuesta />} />
          <Route path="/exportar-propuesta" element={<ExportarPropuesta />} />
          <Route path="/ver-en-sigpac" element={<VerEnSigpac />} />
          <Route path="/asignar-nombre" element={<AsignarNombre />} />
          <Route path="/parcelas" element={<ParcelasConHistorico />} />
        </Routes>
      </div>
    </BrowserRouter>
  );
}

export default App;
