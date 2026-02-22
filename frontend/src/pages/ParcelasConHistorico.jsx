
import { useEffect, useState } from "react";
import {
  getParcelasConHistorico,
  asignarNombre,
  verEnSigpac
} from "../api/parcelasApi";

export default function ParcelasConHistorico() {
  const [filas, setFilas] = useState([]);

  useEffect(() => {
    async function cargar() {
      const res = await getParcelasConHistorico();

      // Aplanar: una fila por recinto
      const data = res.data.flatMap(p =>
        p.recintos.map(r => {
          const ultimosTres = r.historico.slice(0, 3);
          return {
            parcelaId: p.parcelaId,
            nombre: p.nombre,
            provincia: p.provincia,
            municipio: p.municipio,
            poligono: p.poligono,
            numeroParcela: p.numeroParcela,
            superficieParcela: p.superficieTotal,
            recintoId: r.recintoId,
            superficieRecinto: r.superficie,
            historico: ultimosTres
          };
        })
      );

      setFilas(data);
    }
    cargar();
  }, []);

  const cambiarNombreLocal = (parcelaId, nuevoNombre) => {
    setFilas(prev =>
      prev.map(f =>
        f.parcelaId === parcelaId ? { ...f, nombre: nuevoNombre } : f
      )
    );
  };

  const guardarNombre = async (parcelaId, nombre) => {
    try {
      await asignarNombre(parcelaId, nombre);
    } catch (e) {
      console.error(e);
      alert("Error guardando el nombre");
    }
  };

  const abrirSigpac = async (parcelaId) => {
    try {
      const res = await verEnSigpac(parcelaId);
      const url = res.data.visorUrl;

      if (!url) {
        alert("La API no devolvió URL de SIGPAC");
        return;
      }

      window.open(url, "_blank");
    } catch (e) {
      console.error(e);
      alert("No se pudo abrir SIGPAC");
    }
  };

  return (
    <div style={{ maxWidth: "100%", overflowX: "auto" }}>
      <h2>Parcelas y Recintos</h2>

      <table style={{ width: "100%", borderCollapse: "collapse", background: "white" }}>
        <thead>
          <tr style={{ background: "#2c3e50", color: "white" }}>
            <th style={th}>Parcela ID</th>
            <th style={th}>Nombre</th>
            <th style={th}>Provincia</th>
            <th style={th}>Municipio</th>
            <th style={th}>Polígono</th>
            <th style={th}>Parcela</th>
            <th style={th}>Sup. Parcela</th>
            <th style={th}>Recinto ID</th>
            <th style={th}>Sup. Recinto</th>
            <th style={th}>Últ. Año</th>
            <th style={th}>Últ. Cultivo</th>
            <th style={th}>SIGPAC</th>
            <th style={th}>Guardar</th>
          </tr>
        </thead>

        <tbody>
          {filas.map((f, idx) => (
            <tr key={idx} style={{ borderBottom: "1px solid #ddd" }}>
              <td style={td}>{f.parcelaId}</td>

              <td style={td}>
                <input
                  type="text"
                  value={f.nombre || ""}
                  onChange={(e) => cambiarNombreLocal(f.parcelaId, e.target.value)}
                  style={{ width: "150px" }}
                />
              </td>

              <td style={td}>{f.provincia}</td>
              <td style={td}>{f.municipio}</td>
              <td style={td}>{f.poligono}</td>
              <td style={td}>{f.numeroParcela}</td>
              <td style={td}>{f.superficieParcela} ha</td>
              <td style={td}>{f.recintoId}</td>
              <td style={td}>{f.superficieRecinto} ha</td>
              <td style={td}> {f.historico.map(h => ( <div key={h.anioCampania}> {h.anioCampania} </div> ))} </td> 
              <td style={td}> {f.historico.map(h => ( <div key={h.anioCampania}> {h.cultivo} </div> ))} </td>

              <td style={td}>
                <button onClick={() => abrirSigpac(f.parcelaId)}>
                  SIGPAC
                </button>
              </td>

              <td style={td}>
                <button onClick={() => guardarNombre(f.parcelaId, f.nombre)}>
                  Guardar
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

const th = {
  padding: "10px",
  border: "1px solid #444",
  textAlign: "left",
  fontSize: "14px",
  background: "#1f2937",   // gris oscuro
  color: "white"
};

const td = {
  padding: "8px",
  border: "1px solid #ccc",
  fontSize: "14px",
  background: "rgba(255,255,255,0.9)",  // blanco translúcido
  color: "#111"                         // texto oscuro
};
  