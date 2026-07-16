import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import { AuthProvider } from "./context/AuthContext";
import LoginPage from "./components/LoginPage";
import AppLayout from "./components/AppLayout";
import RutaProtegida from "./components/RutaProtegida";
import ProductosPage from "./components/ProductosPage";
import VentasPage from "./components/VentasPage";
import CajaPage from "./components/CajaPage";
import ReportesPage from "./components/ReportesPage";
import DescuentosPage from "./components/DescuentosPage";

/* ──────────────────────────────────────────────
   Páginas placeholder para rutas futuras
   (se reemplazarán con los módulos reales)
────────────────────────────────────────────── */
function PlaceholderPage({ nombre }) {
  return (
    <div style={{
      padding: "40px 32px",
      fontFamily: "Inter, sans-serif",
      color: "#2c2c2a"
    }}>
      <h1 style={{ fontSize: 22, fontWeight: 500, marginBottom: 8 }}>
        {nombre}
      </h1>
      <p style={{ color: "#888780", fontSize: 14 }}>
        Este módulo está en desarrollo.
      </p>
    </div>
  );
}

function NoAutorizado() {
  return (
    <div style={{
      display: "flex",
      flexDirection: "column",
      alignItems: "center",
      justifyContent: "center",
      height: "100vh",
      fontFamily: "Inter, sans-serif",
      gap: 12,
      color: "#2c2c2a"
    }}>
      <span style={{ fontSize: 48 }}>🚫</span>
      <h1 style={{ margin: 0 }}>Acceso no autorizado</h1>
      <p style={{ color: "#888780" }}>
        No tienes permisos para acceder a esta sección.
      </p>
    </div>
  );
}

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <Routes>
          {/* Pública */}
          <Route path="/login" element={<LoginPage />} />
          <Route path="/no-autorizado" element={<NoAutorizado />} />

          {/* Privadas — requieren sesión */}
          <Route
            element={
              <RutaProtegida>
                <AppLayout />
              </RutaProtegida>
            }
          >
            {/* Dashboard → redirige a reportes */}
            <Route
              path="/dashboard"
              element={<Navigate to="/reportes" replace />}
            />

            {/* Reportes → página principal al entrar (solo admin) */}
            <Route
              path="/reportes"
              element={
                <RutaProtegida rolRequerido="Administrador">
                  <ReportesPage />
                </RutaProtegida>
              }
            />

            {/* Productos → accesible por ambos roles */}
            <Route
              path="/productos"
              element={
                <RutaProtegida>
                  <ProductosPage />
                </RutaProtegida>
              }
            />

            {/* Descuentos → solo admin */}
            <Route
              path="/descuentos"
              element={
                <RutaProtegida rolRequerido="Administrador">
                  <DescuentosPage />
                </RutaProtegida>
              }
            />

            {/* Ventas → ambos roles */}
            <Route
              path="/ventas"
              element={<VentasPage />}
            />

            {/* Caja → ambos roles */}
            <Route
              path="/caja"
              element={
                <RutaProtegida>
                  <CajaPage />
                </RutaProtegida>
              }
            />

            {/* Punto de Venta → ambos roles */}
            <Route
              path="/punto-venta"
              element={<VentasPage />}
            />

            {/* Historial de ventas → ambos roles */}
            <Route
              path="/historial"
              element={<PlaceholderPage nombre="Historial de ventas" />}
            />
          </Route>

          {/* Al entrar → va a reportes directamente */}
          <Route path="/" element={<Navigate to="/reportes" replace />} />
          <Route path="*" element={<Navigate to="/login" replace />} />
        </Routes>
      </AuthProvider>
    </BrowserRouter>
  );
}

export default App;
