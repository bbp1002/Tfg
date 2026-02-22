import { useState, useEffect } from "react";
import { generarPropuesta } from "../api/pacApi";
import { getCultivos, getEcorregimenes } from "../api/catalogosApi";
import { useNavigate } from "react-router-dom";

export default function GenerarPropuesta() {
  const [anioCampania, setAnioCampania] = useState(new Date().getFullYear());
  const [cultivos, setCultivos] = useState([]);
  const [ecorregimenes, setEcorregimenes] = useState([]);

  const [cultivosSeleccionados, setCultivosSeleccionados] = useState([]);
  const [ecorregimenesSeleccionados, setEcorregimenesSeleccionados] = useState([]);
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();



  const [respuesta, setRespuesta] = useState(null);

  // Cargar catálogos al entrar en la página
  useEffect(() => {
    async function cargarDatos() {
      try {
        const resEco = await getEcorregimenes();
        const resCultivos = await getCultivos();
        setEcorregimenes(resEco.data);
        setCultivos(resCultivos.data);
      } catch (err) {
        console.error("Error cargando catálogos", err);
      }
    }
    cargarDatos();
  }, []);

  const toggleCultivo = (c) => { 
    setCultivosSeleccionados(prev => 
        prev.includes(c) ? prev.filter(x => x !== c) : [...prev, c] 
    ); 
  }; 
  const toggleEco = (e) => { 
    setEcorregimenesSeleccionados(prev => 
        prev.includes(e) ? prev.filter(x => x !== e) : [...prev, e] 
    ); 
  };

const enviar = async () => {
  try {
    setLoading(true); //Activar pantalla de carga

    const body = {
      AnioCampania: anioCampania,
      EcorregimenesObjetivo: ecorregimenesSeleccionados,
      CultivosPermitidos: cultivosSeleccionados
    };

    const res = await generarPropuesta(body);
    // Mostrar mensaje
    const mensaje =
      typeof res.data === "string"
        ? res.data
        : res.data.mensaje || "Propuesta generada correctamente";

    alert(mensaje);

    // Redirigir después de aceptar
    navigate("/exportar-propuesta");
    console.log(res.data);
  } catch (err) {
    console.error(err);
    alert("Error al generar propuesta");
  } finally {
    setLoading(false); //Desactivar la pantalla de carga
  }
};

if (loading) {
  return (
    <div style={loadingScreen}>
      <div style={loadingBox}>
        <div className="spinner"></div>
        <h3>Generando propuesta...</h3>
        <p>Este proceso puede tardar hasta 3 minutos.</p>
        <p>No cierre la página.</p>
      </div>
    </div>
  );
}


  return (
    <div style={centerBox}>
      <h2>Generar propuesta IA</h2>

<h3>Año de campaña</h3>
<input
  type="number"
  value={anioCampania}
  onChange={(e) => setAnioCampania(parseInt(e.target.value))}
  min="2020"
  max="2100"
  style={{ width: "120px", padding: "5px", marginBottom: "20px" }}
/>

<h3>Cultivos permitidos</h3>
{cultivos.map(c => (
  <label key={c} style={{ display: "block" }}>
    <input
      type="checkbox"
      checked={cultivosSeleccionados.includes(c)}
      onChange={() =>
        setCultivosSeleccionados(prev =>
          prev.includes(c)
            ? prev.filter(x => x !== c)
            : [...prev, c]
        )
      }
    />
    {c}
  </label>
))}


<h3>Ecorregímenes objetivo</h3>
{ecorregimenes.map(e => (
  <label key={e} style={{ display: "block" }}>
    <input
      type="checkbox"
      checked={ecorregimenesSeleccionados.includes(e)}
      onChange={() =>
        setEcorregimenesSeleccionados(prev =>
          prev.includes(e)
            ? prev.filter(x => x !== e)
            : [...prev, e]
        )
      }
    />
    {e}
  </label>
))}

      <button style={{ marginTop: "20px" }} onClick={enviar}>
        Generar propuesta
      </button>

      {respuesta && (
        <div style={{ marginTop: "30px", padding: "10px", border: "1px solid #ccc" }}>
          <h3>Resultado</h3>
          <p><strong>{respuesta.mensaje}</strong></p>
          <p>Fecha: {respuesta.fechaGeneracion}</p>
        </div>
      )}
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

const loadingScreen = {
  position: "fixed",
  top: 0,
  left: 0,
  width: "100vw",
  height: "100vh",
  backgroundColor: "rgba(0,0,0,0.6)",
  display: "flex",
  justifyContent: "center",
  alignItems: "center",
  zIndex: 9999, 
  color: "black"
};

const loadingBox = {
  background: "white",
  padding: "40px",
  borderRadius: "12px",
  textAlign: "center",
  width: "350px",
  color: "black"
};
