import { useState, useEffect } from "react";
import "./ReportesPage.css";
import { obtenerResumen, obtenerVentasPorDia, obtenerMasVendidos, obtenerHistorial } from "../services/reportesApi";

const ITEMS_POR_PAGINA = 6;

function ReportesPage() {
  const [resumen, setResumen] = useState(null);
  const [ventasPorDia, setVentasPorDia] = useState([]);
  const [masVendidos, setMasVendidos] = useState([]);
  const [historial, setHistorial] = useState([]);
  const [cargando, setCargando] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    setCargando(true);
    Promise.all([
      obtenerResumen(),
      obtenerVentasPorDia(),
      obtenerMasVendidos(),
      obtenerHistorial(),
    ])
      .then(([res, dias, vendidos, hist]) => {
        setResumen(res);
        setVentasPorDia(dias);
        setMasVendidos(vendidos);
        setHistorial(hist);
      })
      .catch(() => setError("Error al cargar los reportes. Verifica que el backend esté corriendo."))
      .finally(() => setCargando(false));
  }, []);

  if (cargando) {
    return (
      <div className="reportes-page">
        <p className="estado-vacio">Cargando reportes...</p>
      </div>
    );
  }

  if (error) {
    return (
      <div className="reportes-page">
        <p className="estado-vacio" style={{ color: "#a32d2d" }}>{error}</p>
      </div>
    );
  }

  const maxVenta = Math.max(...ventasPorDia.map((v) => v.total), 1);
  const maxVendido = Math.max(...masVendidos.map((v) => v.cantidad), 1);
  const hoy = ventasPorDia[ventasPorDia.length - 1]?.dia;

  return (
    <div className="reportes-page">
      <header className="reportes-header">
        <div>
          <h1>Reportes de ventas</h1>
          <p className="reportes-subtitulo">Resumen del negocio</p>
        </div>
        <div className="reportes-header-acciones">
          <button className="btn-fecha">
            <i className="ti ti-calendar" aria-hidden="true"></i>
            {new Date().toLocaleDateString("es-DO", { day: "numeric", month: "short", year: "numeric" })}
          </button>
          <button className="btn-exportar">
            <i className="ti ti-download" aria-hidden="true"></i>
            Exportar
          </button>
        </div>
      </header>

      {/* Tarjetas de resumen */}
      <div className="tarjetas-grid">
        <div className="tarjeta">
          <p className="tarjeta-label">Ventas del día</p>
          <p className="tarjeta-valor">RD$ {resumen.ventasHoy.toLocaleString()}</p>
          <p className="tarjeta-variacion positiva">▲ {resumen.variacionDia}% vs. ayer</p>
        </div>
        <div className="tarjeta">
          <p className="tarjeta-label">Ventas del mes</p>
          <p className="tarjeta-valor">RD$ {resumen.ventasMes.toLocaleString()}</p>
          <p className="tarjeta-variacion positiva">▲ {resumen.variacionMes}% vs. mes ant.</p>
        </div>
        <div className="tarjeta">
          <p className="tarjeta-label">Facturas emitidas</p>
          <p className="tarjeta-valor">{resumen.facturasEmitidas.toLocaleString()}</p>
          <p className="tarjeta-subtexto">En el período</p>
        </div>
        <div className="tarjeta">
          <p className="tarjeta-label">Producto top</p>
          <p className="tarjeta-valor tarjeta-valor-sm">{resumen.productoTopNombre}</p>
          <p className="tarjeta-subtexto">
            {resumen.productoTopCantidad} {resumen.productoTopUnidad} vendidos
          </p>
        </div>
      </div>

      {/* Sección central: gráfico + más vendidos */}
      <div className="seccion-central">
        <div className="card grafico-card">
          <div className="grafico-header">
            <h2>Ventas por día</h2>
            <span className="grafico-periodo">Últimos 7 días (RD$)</span>
          </div>
          <div className="grafico-barras">
            {ventasPorDia.map((item) => (
              <div key={item.dia} className="barra-contenedor">
                <div className="barra-wrap">
                  <div
                    className={`barra ${item.dia === hoy ? "barra-hoy" : ""}`}
                    style={{ height: `${(item.total / maxVenta) * 100}%` }}
                    title={`RD$ ${item.total.toLocaleString()}`}
                  />
                </div>
                <span className={`barra-label ${item.dia === hoy ? "label-hoy" : ""}`}>
                  {item.dia}
                </span>
              </div>
            ))}
          </div>
        </div>

        <div className="card mas-vendidos-card">
          <h2>Más vendidos</h2>
          <div className="mas-vendidos-lista">
            {masVendidos.map((item) => (
              <div key={item.nombre} className="vendido-item">
                <div className="vendido-info">
                  <span className="vendido-nombre">{item.nombre}</span>
                  <span className="vendido-cantidad">{item.cantidad} {item.unidad}</span>
                </div>
                <div className="barra-progreso">
                  <div
                    className="barra-progreso-fill"
                    style={{ width: `${(item.cantidad / maxVendido) * 100}%` }}
                  />
                </div>
              </div>
            ))}
          </div>
        </div>
      </div>

      {/* Historial de ventas reciente */}
      <div className="card historial-card">
        <div className="historial-header">
          <h2>Historial de ventas reciente</h2>
          <button className="btn-ver-todo">Ver todo →</button>
        </div>
        <table className="historial-tabla">
          <thead>
            <tr>
              <th>Factura</th>
              <th>Fecha y hora</th>
              <th>Cajero</th>
              <th>Productos</th>
              <th className="col-derecha">Total</th>
            </tr>
          </thead>
          <tbody>
            {historial.map((venta) => (
              <tr key={venta.id}>
                <td className="factura-numero">{venta.numeroFactura}</td>
                <td>{venta.fechaHora}</td>
                <td>{venta.cajero}</td>
                <td>{venta.cantidadProductos} {venta.cantidadProductos === 1 ? "ítem" : "ítems"}</td>
                <td className="col-derecha">
                  RD$ {venta.total.toLocaleString("es-DO", { minimumFractionDigits: 2 })}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        {historial.length === 0 && (
          <p className="estado-vacio">No hay ventas registradas aún.</p>
        )}
      </div>
    </div>
  );
}

export default ReportesPage;
