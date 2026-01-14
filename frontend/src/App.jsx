import { BrowserRouter, Routes, Route } from "react-router-dom";
import Navbar from "./components/Navbar";

import Login from "./pages/Login";
import Register from "./pages/Register";
import ImportarPac from "./pages/ImportarPac";
import GenerarPropuesta from "./pages/GenerarPropuesta";
import ExportarPropuesta from "./pages/ExportarPropuesta";
import VerEnSigpac from "./pages/VerEnSigpac";
import AsignarNombre from "./pages/AsignarNombre";

function App() {
  return (
    <BrowserRouter>
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
        </Routes>
      </div>
    </BrowserRouter>
  );
}

export default App;
