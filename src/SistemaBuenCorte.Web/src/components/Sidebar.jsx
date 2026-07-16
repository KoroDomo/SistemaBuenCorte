import { NavLink, useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import "./Sidebar.css";

const MENU_ADMIN = [
  { section: "GESTIÓN" },
  { to: "/productos", icon: "ti-box", label: "Productos" },
  { to: "/ventas", icon: "ti-menu-2", label: "Ventas" },
  { to: "/caja", icon: "ti-cash", label: "Caja" },
  { to: "/descuentos", icon: "ti-discount", label: "Descuentos" },
  { section: "ANÁLISIS" },
  { to: "/reportes", icon: "ti-chart-bar", label: "Reportes" },
];

const MENU_CAJERO = [
  { section: "OPERACIÓN" },
  { to: "/punto-venta", icon: "ti-shopping-cart", label: "Punto de venta" },
  { to: "/historial", icon: "ti-menu-2", label: "Historial de ventas" },
  { to: "/caja", icon: "ti-cash", label: "Caja" },
];

function Sidebar({ collapsed, onToggle }) {
  const { sesion, logout } = useAuth();
  const navigate = useNavigate();

  const menu = sesion?.rol === "Administrador" ? MENU_ADMIN : MENU_CAJERO;

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  const iniciales = sesion?.nombreCompleto
    ? sesion.nombreCompleto
        .split(" ")
        .slice(0, 2)
        .map((n) => n[0])
        .join("")
        .toUpperCase()
    : "?";

  return (
    <aside className={`sidebar ${collapsed ? "sidebar--collapsed" : ""}`}>
      {/* Brand */}
      <div className="sidebar-brand">
        <span className="sidebar-brand-logo">BC</span>
        {!collapsed && (
          <div className="sidebar-brand-texto">
            <span className="sidebar-brand-nombre">El Buen Corte</span>
            <span className="sidebar-brand-sub">{sesion?.rol === "Administrador" ? "ADMINISTRACIÓN" : "PUNTO DE VENTA"}</span>
          </div>
        )}
        <button
          className="sidebar-toggle"
          onClick={onToggle}
          aria-label={collapsed ? "Expandir menú" : "Colapsar menú"}
        >
          <i className={`ti ${collapsed ? "ti-layout-sidebar-right" : "ti-layout-sidebar-left"}`} aria-hidden="true" />
        </button>
      </div>

      {/* Navegación */}
      <nav className="sidebar-nav" aria-label="Menú principal">
        <ul>
          {menu.map((item, index) => {
            if (item.section) {
              return (
                !collapsed && (
                  <li key={`section-${index}`} className="sidebar-section-title">
                    {item.section}
                  </li>
                )
              );
            }
            return (
              <li key={item.to}>
                <NavLink
                  to={item.to}
                  className={({ isActive }) =>
                    `sidebar-item ${isActive ? "sidebar-item--active" : ""}`
                  }
                  title={collapsed ? item.label : undefined}
                >
                  <i className={`ti ${item.icon}`} aria-hidden="true" />
                  {!collapsed && <span>{item.label}</span>}
                </NavLink>
              </li>
            );
          })}
        </ul>
      </nav>

      {/* Usuario & logout */}
      <div className="sidebar-footer">
        <div className="sidebar-user">
          <span className="sidebar-avatar">{iniciales}</span>
          {!collapsed && (
            <div className="sidebar-user-info">
              <span className="sidebar-user-nombre">{sesion?.nombreCompleto}</span>
              <span className="sidebar-user-rol">{sesion?.rol}</span>
            </div>
          )}
        </div>
        {/* El logout puede ir en otro lugar o mantenerse aquí minimalista. El mockup no muestra botón de logout explícito aquí, pero lo mantendremos por usabilidad. */}
        <button
          className="sidebar-logout"
          onClick={handleLogout}
          title="Cerrar sesión"
          aria-label="Cerrar sesión"
        >
          <i className="ti ti-logout" aria-hidden="true" />
        </button>
      </div>
    </aside>
  );
}

export default Sidebar;
