import axiosClient from "./axiosClient";

export const getParcelasConHistorico = () =>
  axiosClient.get("/farm/con-historico");

export const asignarNombre = (parcelaId, nombre) =>
  axiosClient.put(`/farm/asignar-nombre?parcelaId=${parcelaId}&nombre=${nombre}`);

export const verEnSigpac = (id) =>
  axiosClient.get(`/farm/ver-en-sigpac?id=${id}`);
