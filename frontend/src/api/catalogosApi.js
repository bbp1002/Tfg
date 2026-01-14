import axiosClient from "./axiosClient";

export const getCultivos = () =>
  axiosClient.get("/farm/cultivos");

export const getEcorregimenes = () =>
  axiosClient.get("/farm/ecorregimenes");