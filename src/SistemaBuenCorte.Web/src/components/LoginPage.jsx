import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { loginApi } from "../services/authApi";
import "./LoginPage.css";

function LoginPage() {
  const [usuario, setUsuario] = useState("");
  const [contrasena, setContrasena] = useState("");
  const [mostrarPass, setMostrarPass] = useState(false);
  const [recordar, setRecordar] = useState(false);
  const [cargando, setCargando] = useState(false);
  const [error, setError] = useState("");

  const { login } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!usuario.trim() || !contrasena.trim()) {
      setError("Por favor complete todos los campos.");
      return;
    }
    setError("");
    setCargando(true);
    try {
      const datos = await loginApi(usuario.trim(), contrasena);
      login(datos);
      // Redirigir según rol: administradores -> dashboard, cajeros -> punto de venta
      const rol = (datos?.rol || datos?.Rol || "").toString().toLowerCase();
      if (rol === "administrador") {
        navigate("/dashboard");
      } else {
        navigate("/punto-venta");
      }
    } catch (err) {
      const msg =
        err?.response?.data?.mensaje || "Usuario o contraseña incorrectos.";
      setError(msg);
    } finally {
      setCargando(false);
    }
  };

  return (
    <div className="login-root">
      {/* ── Panel izquierdo ─────────────────────────────── */}
      <div className="login-left">
        <div className="login-brand">
          <span className="login-brand-logo">BC</span>
          <div>
            <p className="login-brand-nombre">El Buen Corte</p>
            <p className="login-brand-subtitulo">Sistema de Facturación y Gestión</p>
          </div>
        </div>

        <div className="login-hero">
          <h1 className="login-hero-titulo">
            Gestione las ventas<br />
            de su carnicería<br />
            <span className="login-hero-acento">en un solo lugar.</span>
          </h1>
          <p className="login-hero-desc">
            Registre ventas por peso o por unidad, calcule
            totales al instante, controle la caja y genere
            facturas en segundos.
          </p>
          <div className="login-hero-tags">
            <span>Cálculo automático</span>
            <span>Acceso por roles</span>
            <span>Acceso remoto</span>
          </div>
        </div>

        <p className="login-footer-copy">© 2026 · El Buen Corte · v1.0</p>
      </div>

      {/* ── Panel derecho ──────────────────────────────── */}
      <div className="login-right">
        <div className="login-card">
          <h2 className="login-card-titulo">Iniciar sesión</h2>
          <p className="login-card-subtitulo">
            Ingrese sus credenciales para acceder al sistema.
          </p>

          <form onSubmit={handleSubmit} noValidate>
            {/* Usuario */}
            <div className="login-campo">
              <label htmlFor="login-usuario">Usuario</label>
              <div className="login-input-wrap">
                <i className="ti ti-user" aria-hidden="true" />
                <input
                  id="login-usuario"
                  type="text"
                  autoComplete="username"
                  value={usuario}
                  onChange={(e) => setUsuario(e.target.value)}
                  placeholder="jcaceres"
                  disabled={cargando}
                />
              </div>
            </div>

            {/* Contraseña */}
            <div className="login-campo">
              <label htmlFor="login-contrasena">Contraseña</label>
              <div className="login-input-wrap">
                <i className="ti ti-lock" aria-hidden="true" />
                <input
                  id="login-contrasena"
                  type={mostrarPass ? "text" : "password"}
                  autoComplete="current-password"
                  value={contrasena}
                  onChange={(e) => setContrasena(e.target.value)}
                  placeholder="••••••••••"
                  disabled={cargando}
                />
                <button
                  type="button"
                  className="login-toggle-pass"
                  onClick={() => setMostrarPass((v) => !v)}
                  aria-label={mostrarPass ? "Ocultar contraseña" : "Mostrar contraseña"}
                >
                  <i className={`ti ${mostrarPass ? "ti-eye-off" : "ti-eye"}`} aria-hidden="true" />
                </button>
              </div>
            </div>

            {/* Recordar / Olvidé */}
            <div className="login-opciones">
              <label className="login-checkbox-label">
                <input
                  type="checkbox"
                  checked={recordar}
                  onChange={(e) => setRecordar(e.target.checked)}
                />
                <span className="login-checkmark">
                  {recordar && <i className="ti ti-check" aria-hidden="true" />}
                </span>
                Recordar este equipo
              </label>
              <button type="button" className="login-link">
                ¿Olvidó su contraseña?
              </button>
            </div>

            {/* Error */}
            {error && (
              <div className="login-error" role="alert">
                <i className="ti ti-alert-circle" aria-hidden="true" />
                {error}
              </div>
            )}

            {/* Submit */}
            <button
              type="submit"
              id="btn-iniciar-sesion"
              className="login-btn-submit"
              disabled={cargando}
            >
              {cargando ? (
                <>
                  <span className="login-spinner" /> Verificando...
                </>
              ) : (
                <>
                  Iniciar sesión <i className="ti ti-arrow-right" aria-hidden="true" />
                </>
              )}
            </button>
          </form>

          {/* Nota de roles */}
          <div className="login-nota">
            <i className="ti ti-info-circle" aria-hidden="true" />
            <p>
              El sistema asigna automáticamente las funciones
              disponibles según el <strong>rol</strong> del usuario:{" "}
              <strong>cajero</strong> o <strong>administrador</strong>.
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}

export default LoginPage;
