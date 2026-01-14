import axiosClient from "./axiosClient";

export const login = (data) =>
  axiosClient.post("/auth/login", data);

export const registerUser = (data) =>
  axiosClient.post("/auth/register", data);
