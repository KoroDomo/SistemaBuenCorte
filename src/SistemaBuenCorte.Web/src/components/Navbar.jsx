import { useAuth } from "../context/AuthContext";
import "./Navbar.css";

function Navbar({ onMenuToggle }) {
  const { sesion } = useAuth();

  const iniciales = sesion?.nombreCompleto
    ? sesion.nombreCompleto
        .split(" ")
        .slice(0, 2)
        .map((n) => n[0])
        .join("")
        .toUpperCase()
    : "?";

  // Fecha actual en formato "04 jun 2026"
  const fecha = new Date().toLocaleDateString('es-ES', { day: '2-digit', month: 'short', year: 'numeric' }).replace('.', '');

  return (
    <header className="navbar">
      <div className="navbar-izquierda">
        <button
          className="navbar-menu-btn"
          onClick={onMenuToggle}
          aria-label="Abrir menú"
        >
          <i className="ti ti-menu-2" aria-hidden="true" />
        </button>
      </div>

      <div className="navbar-derecha">
        <span className="navbar-fecha" style={{ fontSize: '13px', color: '#5f5e5a', fontWeight: '500', marginRight: '8px' }}>
          {fecha}
        </span>
        {/* Notificaciones (placeholder para futuro) */}
        <button className="navbar-icono-btn" aria-label="Notificaciones" title="Notificaciones">
          <i className="ti ti-bell" aria-hidden="true" />
        </button>

        {/* Perfil */}
        <div className="navbar-perfil">
          <span className="navbar-perfil-avatar">{iniciales}</span>
          <div className="navbar-perfil-info">
            <span className="navbar-perfil-nombre">{sesion?.nombreCompleto}</span>
            <span className="navbar-perfil-rol">{sesion?.rol}</span>
          </div>
        </div>
      </div>
    </header>
  );
}

export default Navbar;
