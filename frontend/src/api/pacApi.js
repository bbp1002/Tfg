import axiosClient from "./axiosClient";

export const importarPac = (anio, archivo) => {
  const formData = new FormData();
  formData.append("Anio", anio);
  formData.append("ArchivoExcel", archivo);

  return axiosClient.post("/farm/importar-pac", formData, {
    headers: {
      "Content-Type": "multipart/form-data"
    }
  });
};


export const generarPropuesta = (data) =>
  axiosClient.post("/farm/generar-propuesta-ia", data);

export const exportarPropuesta = (anio, soloBorrador) =>
  axiosClient.get(`/farm/exportar-propuesta?anio=${anio}&soloBorrador=${soloBorrador}`, {
    responseType: "blob",
  });

export const verEnSigpac = (id) =>
  axiosClient.get(`/farm/ver-en-sigpac?id=${id}`);

export const asignarNombre = (parcelaId, nombre) =>
  axiosClient.put(`/farm/asignar-nombre?parcelaId=${parcelaId}&nombre=${nombre}`);
