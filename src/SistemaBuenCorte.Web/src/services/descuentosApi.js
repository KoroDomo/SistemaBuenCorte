// Servicio de conexión con la API de Descuentos.

import axios from "axios";

const API_BASE = process.env.REACT_APP_API_URL || "http://localhost:5097";
const API_URL = `${API_BASE.replace(/\/$/, '')}/api/descuentos`;

// GET /api/descuentos?nombre=&soloActivos=
export const obtenerDescuentos = async (nombre = "", soloActivos = null) => {
  const params = {};
  if (nombre) params.nombre = nombre;
  if (soloActivos !== null) params.soloActivos = soloActivos;
  const res = await axios.get(API_URL, { params });
  return res.data;
};

// GET /api/descuentos/{id}
export const obtenerDescuentoPorId = async (id) => {
  const res = await axios.get(`${API_URL}/${id}`);
  return res.data;
};

// POST /api/descuentos  — body: DescuentoCreateDto
export const crearDescuento = async (dto) => {
  const res = await axios.post(API_URL, dto);
  return res.data;
};

// PUT /api/descuentos/{id}  — body: DescuentoUpdateDto
export const actualizarDescuento = async (id, dto) => {
  const res = await axios.put(`${API_URL}/${id}`, dto);
  return res.data;
};

// DELETE /api/descuentos/{id}  — borrado lógico en el backend
export const eliminarDescuento = async (id) => {
  await axios.delete(`${API_URL}/${id}`);
};
