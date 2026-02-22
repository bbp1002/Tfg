import { useState } from "react";
import { importarPac } from "../api/pacApi";

export default function ImportarPac() {
  const [anio, setAnio] = useState(new Date().getFullYear());
  const [archivo, setArchivo] = useState(null);

  const enviar = async () => {
    if (!archivo) {
      alert("Debes seleccionar un archivo Excel");
      return;
    }

    try {
      const res = await importarPac(anio, archivo);
      alert("Importación realizada correctamente");
    } catch (err) {
      console.error(err);
      alert("Error al importar la PAC");
    }
  };

  return (
    <div style={centerBox}>
      <h2>Importar PAC</h2>

      <label>Año de campaña</label>
      <input
        type="number"
        value={anio}
        onChange={(e) => setAnio(parseInt(e.target.value))}
      />

      <label>Archivo Excel</label>
      <input
        type="file"
        accept=".xlsx,.xls"
        onChange={(e) => setArchivo(e.target.files[0])}
      />

      <button onClick={enviar}>Importar</button>
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