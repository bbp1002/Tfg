import { useState } from "react";
import { verEnSigpac } from "../api/pacApi";

export default function VerEnSigpac() {
  const [id, setId] = useState("");
  const [respuesta, setRespuesta] = useState(null);

  const buscar = async () => {
    try {
      const res = await verEnSigpac(id);
      setRespuesta(res.data);
    } catch {
      alert("Error al consultar SIGPAC");
    }
  };

  return (
    <div>
      <h2>Ver en SIGPAC</h2>

      <input
        type="number"
        placeholder="ID parcela"
        value={id}
        onChange={(e) => setId(e.target.value)}
      />

      <button onClick={buscar}>Buscar</button>

      {respuesta && (
        <div style={{ marginTop: "20px" }}>
          <p>Parcela: {respuesta.parcelaId}</p>
          <a href={respuesta.urlVisor} target="_blank">
            Abrir en SIGPAC
          </a>
        </div>
      )}
    </div>
  );
}
