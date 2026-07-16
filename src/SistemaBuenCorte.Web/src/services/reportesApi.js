// Servicio de conexión con la API de Reportes.
// Cuando el backend esté listo, estas funciones reemplazan los mocks.

import axios from "axios";

const API_URL = "http://localhost:5097/api/reportes";

// GET /api/reportes/resumen
export const obtenerResumen = async () => {
  const res = await axios.get(`${API_URL}/resumen`);
  return res.data;
};

// GET /api/reportes/ventas-por-dia
export const obtenerVentasPorDia = async () => {
  const res = await axios.get(`${API_URL}/ventas-por-dia`);
  return res.data;
};

// GET /api/reportes/mas-vendidos
export const obtenerMasVendidos = async () => {
  const res = await axios.get(`${API_URL}/mas-vendidos`);
  return res.data;
};

// GET /api/reportes/historial
export const obtenerHistorial = async () => {
  const res = await axios.get(`${API_URL}/historial`);
  return res.data;
};
