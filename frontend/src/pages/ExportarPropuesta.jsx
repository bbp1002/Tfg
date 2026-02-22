import { useState } from "react";
import { exportarPropuesta } from "../api/pacApi";

export default function ExportarPropuesta() {
  const [anio, setAnio] = useState(new Date().getFullYear());
  const [soloBorrador, setSoloBorrador] = useState(true);

  const descargar = async () => {
    try {
      const res = await exportarPropuesta(anio, soloBorrador);

      const url = window.URL.createObjectURL(new Blob([res.data]));
      const link = document.createElement("a");
      link.href = url;
      link.setAttribute("download", `propuesta_${anio}.xlsx`);
      document.body.appendChild(link);
      link.click();

      alert("Descarga completada");
    } catch {
      alert("Error al exportar");
    }
  };

  return (
    <div style={centerBox}>
      <h2>Exportar propuesta</h2>

      Año       
      <input
        type="number"
        value={anio}
        onChange={(e) => setAnio(e.target.value)}
      />

      <label>
        <input
          type="checkbox"
          checked={soloBorrador}
          onChange={(e) => setSoloBorrador(e.target.checked)}
        />
        Solo borrador
      </label>

      <button onClick={descargar}>Descargar Excel</button>
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
