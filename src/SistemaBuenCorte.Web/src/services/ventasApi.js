import axios from "axios";

const API_BASE = process.env.REACT_APP_API_URL || "http://localhost:5097";
const API_URL = `${API_BASE.replace(/\/$/, "")}/api/ventas`;

// POST /api/ventas
export const registrarVenta = async (dto) => {
  const respuesta = await axios.post(API_URL, dto);
  return respuesta.data;
};

// GET /api/ventas
export const obtenerVentas = async () => {
  const respuesta = await axios.get(API_URL);
  return respuesta.data;
};

// GET /api/ventas/{id}
export const obtenerVentaPorId = async (id) => {
  const respuesta = await axios.get(`${API_URL}/${id}`);
  return respuesta.data;
};
