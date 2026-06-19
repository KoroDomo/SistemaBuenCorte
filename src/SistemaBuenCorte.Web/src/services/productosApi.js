// Servicio de conexión con la API de Productos.

import axios from "axios";

const API_BASE = process.env.REACT_APP_API_URL || "http://localhost:5097";
const API_URL = `${API_BASE.replace(/\/$/, '')}/api/productos`;

// GET /api/productos?nombre=&categoria=
export const obtenerProductos = async (nombre = "", categoria = "") => {
  const res = await axios.get(API_URL, { params: { nombre, categoria } });
  return res.data;
};

// GET /api/productos/{id}
export const obtenerProductoPorId = async (id) => {
  const res = await axios.get(`${API_URL}/${id}`);
  return res.data;
};

// POST /api/productos  — body: ProductoCreateDto
export const crearProducto = async (dto) => {
  const res = await axios.post(API_URL, dto);
  return res.data;
};

// PUT /api/productos/{id}  — body: ProductoUpdateDto
export const actualizarProducto = async (id, dto) => {
  const res = await axios.put(`${API_URL}/${id}`, dto);
  return res.data;
};

// DELETE /api/productos/{id}  — borrado lógico en el backend
export const eliminarProducto = async (id) => {
  await axios.delete(`${API_URL}/${id}`);
};
