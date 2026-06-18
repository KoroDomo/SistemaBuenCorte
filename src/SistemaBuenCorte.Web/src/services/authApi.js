import axios from "axios";

const API_URL = "http://localhost:5097/api/auth";

/**
 * POST /api/auth/login
 * Retorna { token, nombreUsuario, nombreCompleto, rol }
 */
export const loginApi = async (nombreUsuario, contrasena) => {
  const res = await axios.post(`${API_URL}/login`, {
    nombreUsuario,
    contrasena,
  });
  return res.data;
};
