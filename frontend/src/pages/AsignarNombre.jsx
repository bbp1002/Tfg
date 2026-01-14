import { useState } from "react";
import { asignarNombre } from "../api/pacApi";

export default function AsignarNombre() {
  const [parcelaId, setParcelaId] = useState("");
  const [nombre, setNombre] = useState("");
  const [resultado, setResultado] = useState(null);

  const enviar = async () => {
    try {
      const ok = await asignarNombre(parcelaId, nombre);
      setResultado(ok.status === 200);
    } catch {
      setResultado(false);
    }
  };

  return (
    <div>
      <h2>Asignar nombre a parcela</h2>

      <input
        type="number"
        placeholder="ID parcela"
        value={parcelaId}
        onChange={(e) => setParcelaId(e.target.value)}
      />

      <input
        type="text"
        placeholder="Nombre"
        value={nombre}
        onChange={(e) => setNombre(e.target.value)}
      />

      <button onClick={enviar}>Guardar</button>

      {resultado !== null && (
        <p style={{ marginTop: "20px", color: resultado ? "green" : "red" }}>
          {resultado ? "Nombre asignado correctamente" : "Error al asignar nombre"}
        </p>
      )}
    </div>
  );
}
