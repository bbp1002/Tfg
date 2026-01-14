import axios from "axios";

const axiosClient = axios.create({
  baseURL: "https://localhost:7055/api", 
});

axiosClient.interceptors.request.use((config) => {
  const token = localStorage.getItem("jwt");
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

export default axiosClient;
