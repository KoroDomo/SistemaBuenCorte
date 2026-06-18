import { Navigate, useLocation } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

/**
 * Protege rutas privadas.
 * Si el usuario no está autenticado → redirige a /login.
 * Si se indica un rol requerido y el usuario no lo tiene → redirige a /no-autorizado.
 */
function RutaProtegida({ children, rolRequerido }) {
  const { sesion } = useAuth();
  const location = useLocation();

  if (!sesion) {
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  if (rolRequerido && sesion.rol !== rolRequerido) {
    return <Navigate to="/no-autorizado" replace />;
  }

  return children;
}

export default RutaProtegida;
