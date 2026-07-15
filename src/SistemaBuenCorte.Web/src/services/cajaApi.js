// Servicio de conexión con la API de Caja.

import axios from "axios";

const API_BASE = process.env.REACT_APP_API_URL || "http://localhost:5097";
const API_URL = `${API_BASE.replace(/\/$/, "")}/api/caja`;

// GET /api/caja
export const obtenerCajas = async () => {
    const res = await axios.get(API_URL);
    return res.data;
};

// GET /api/caja/{id}
export const obtenerCajaPorId = async (id) => {
    const res = await axios.get(`${API_URL}/${id}`);
    return res.data;
};

// GET /api/caja/abierta/{usuarioId}
export const obtenerCajaAbierta = async (usuarioId) => {
    try {
        const res = await axios.get(`${API_URL}/abierta/${usuarioId}`);
        return res.data;
    } catch (error) {
        // El backend devuelve 404 cuando el usuario no tiene una caja abierta.
        if (error.response?.status === 404) {
            return null;
        }

        throw error;
    }
};

// POST /api/caja/abrir
export const abrirCaja = async (dto) => {
    const res = await axios.post(`${API_URL}/abrir`, dto);
    return res.data;
};

// PUT /api/caja/{id}/cerrar
export const cerrarCaja = async (id, dto) => {
    const res = await axios.put(`${API_URL}/${id}/cerrar`, dto);
    return res.data;
};