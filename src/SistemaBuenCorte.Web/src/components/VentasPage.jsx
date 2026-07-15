import { useEffect, useMemo, useState } from "react";
import { obtenerProductos } from "../services/productosApi";
import { registrarVenta } from "../services/ventasApi";
import { useAuth } from "../context/AuthContext";
import "./VentasPage.css";

const LIBRAS_POR_KG = 2.20462;

const formatoMoneda = new Intl.NumberFormat("es-DO", {
  style: "currency",
  currency: "DOP",
  minimumFractionDigits: 2,
});

function normalizarTipoVenta(tipoVenta) {
  return (tipoVenta || "").toString().trim().toLowerCase();
}

function esVentaPorPeso(producto) {
  return normalizarTipoVenta(producto.tipoVenta) === "peso";
}

function cantidadParaCalculo(item) {
  if (!item.esPeso) return Number(item.cantidad || 0);
  const cantidad = Number(item.cantidad || 0);
  return item.unidadPeso === "lb" ? cantidad / LIBRAS_POR_KG : cantidad;
}

function formatearCantidad(item) {
  if (!item.esPeso) return `${Number(item.cantidad || 0)} und`;
  return `${Number(item.cantidad || 0).toFixed(item.unidadPeso === "lb" ? 2 : 3)} ${
    item.unidadPeso === "lb" ? "lb" : "kg"
  }`;
}

function crearItemCarrito(producto, cantidad, unidadPeso) {
  const esPeso = esVentaPorPeso(producto);
  const item = {
    carritoId: `${producto.id}-${Date.now()}`,
    productoId: producto.id,
    nombre: producto.nombre,
    categoria: producto.categoria || "Sin categoria",
    tipoVenta: esPeso ? "Peso" : "Unidad",
    precio: Number(producto.precio || 0),
    cantidad: Number(cantidad || 0),
    unidadPeso: esPeso ? unidadPeso : "und",
    esPeso,
  };

  return {
    ...item,
    subtotal: item.precio * cantidadParaCalculo(item),
  };
}

function VentasPage() {
  const { sesion } = useAuth();

  const [productos, setProductos] = useState([]);
  const [cargando, setCargando] = useState(true);
  const [error, setError] = useState("");
  const [busqueda, setBusqueda] = useState("");
  const [categoriaActiva, setCategoriaActiva] = useState("Todas");
  const [cantidades, setCantidades] = useState({});
  const [unidadesPeso, setUnidadesPeso] = useState({});
  const [carrito, setCarrito] = useState([]);
  const [descuento, setDescuento] = useState("");
  const [resumenVenta, setResumenVenta] = useState(null);
  const [procesandoVenta, setProcesandoVenta] = useState(false);

  useEffect(() => {
    setCargando(true);
    obtenerProductos()
      .then((data) => {
        setProductos(Array.isArray(data) ? data : []);
        setError("");
      })
      .catch(() => {
        setProductos([]);
        setError("No se pudieron cargar los productos para vender.");
      })
      .finally(() => setCargando(false));
  }, []);

  const productosDisponibles = useMemo(() => {
    return productos.filter((producto) => producto.activo && Number(producto.stock || 0) > 0);
  }, [productos]);

  const categorias = useMemo(() => {
    const unicas = new Set(productosDisponibles.map((producto) => producto.categoria || "Sin categoria"));
    return ["Todas", ...Array.from(unicas).sort((a, b) => a.localeCompare(b))];
  }, [productosDisponibles]);

  const productosFiltrados = useMemo(() => {
    const texto = busqueda.trim().toLowerCase();
    return productosDisponibles.filter((producto) => {
      const coincideTexto =
        producto.nombre.toLowerCase().includes(texto) ||
        (producto.categoria || "").toLowerCase().includes(texto);
      const coincideCategoria =
        categoriaActiva === "Todas" || (producto.categoria || "Sin categoria") === categoriaActiva;
      return coincideTexto && coincideCategoria;
    });
  }, [busqueda, categoriaActiva, productosDisponibles]);

  const totales = useMemo(() => {
    const subtotal = carrito.reduce((total, item) => total + item.subtotal, 0);
    const porcentajeDescuento = Math.min(100, Math.max(0, Number(descuento || 0)));
    const montoDescuento = subtotal * (porcentajeDescuento / 100);
    return {
      subtotal,
      porcentajeDescuento,
      montoDescuento,
      total: subtotal - montoDescuento,
    };
  }, [carrito, descuento]);

  const obtenerCantidadProducto = (producto) => {
    const valor = cantidades[producto.id];
    if (valor !== undefined) return valor;
    return esVentaPorPeso(producto) ? "1" : "1";
  };

  const obtenerUnidadPesoProducto = (producto) => unidadesPeso[producto.id] || "lb";

  const actualizarCantidad = (productoId, valor) => {
    setCantidades((actual) => ({ ...actual, [productoId]: valor }));
  };

  const actualizarUnidadPeso = (productoId, valor) => {
    setUnidadesPeso((actual) => ({ ...actual, [productoId]: valor }));
  };

  const agregarProducto = (producto) => {
    const cantidad = Number(obtenerCantidadProducto(producto));
    if (!cantidad || cantidad <= 0) return;

    const unidadPeso = obtenerUnidadPesoProducto(producto);
    const item = crearItemCarrito(producto, cantidad, unidadPeso);
    setCarrito((actual) => [...actual, item]);
    setResumenVenta(null);
  };

  const quitarProducto = (carritoId) => {
    setCarrito((actual) => actual.filter((item) => item.carritoId !== carritoId));
  };

  const finalizarVenta = async () => {
    if (carrito.length === 0 || procesandoVenta) return;

    const usuarioId = Number(sesion?.id ?? sesion?.Id);

    if (!usuarioId) {
      setError("No se pudo identificar el usuario de la sesión.");
      return;
    }

    const productosVendidos = [...carrito];

    const solicitud = {
      usuarioId,
      metodoPago: "Efectivo",
      porcentajeDescuento: totales.porcentajeDescuento,
      nombreCliente: null,
      detalles: carrito.map((item) => ({
        productoId: item.productoId,
        cantidad: Number(cantidadParaCalculo(item).toFixed(3)),
      })),
    };

    setProcesandoVenta(true);
    setError("");

    try {
      const venta = await registrarVenta(solicitud);

      const fechaVenta = venta?.fecha ?? venta?.Fecha ?? new Date().toISOString();
      const numeroFactura = venta?.numeroFactura ?? venta?.NumeroFactura ?? "";
      const subtotal = Number(venta?.subtotal ?? venta?.Subtotal ?? 0);
      const montoDescuento = Number(
        venta?.montoDescuento ?? venta?.MontoDescuento ?? 0
      );
      const total = Number(venta?.total ?? venta?.Total ?? 0);

      setResumenVenta({
        id: venta?.id ?? venta?.Id,
        numeroFactura,
        fecha: new Date(fechaVenta).toLocaleString("es-DO", {
          dateStyle: "medium",
          timeStyle: "short",
        }),
        productos: productosVendidos,
        subtotal,
        porcentajeDescuento: totales.porcentajeDescuento,
        montoDescuento,
        total,
      });

      setCarrito([]);
      setDescuento("");

      try {
        const productosActualizados = await obtenerProductos();
        setProductos(
          Array.isArray(productosActualizados) ? productosActualizados : []
        );
      } catch {
        // La venta ya fue registrada; solo falló refrescar la lista visual.
      }
    } catch (err) {
      const erroresApi = err?.response?.data?.errores;
      const mensajeApi = err?.response?.data?.mensaje;

      setError(
        Array.isArray(erroresApi)
          ? erroresApi.join(" ")
          : mensajeApi || "No se pudo registrar la venta."
      );
    } finally {
      setProcesandoVenta(false);
    }
  };

  return (
    <div className="ventas-page">
      <header className="ventas-header">
        <div>
          <h1>Ventas</h1>
          <p className="ventas-subtitulo">
            Registre ventas utilizando productos reales del inventario
          </p>
        </div>
      </header>

      {error && (
        <div className="ventas-alerta" role="alert">
          <i className="ti ti-alert-circle" aria-hidden="true"></i>
          {error}
        </div>
      )}

      <div className="ventas-grid">
        <section className="ventas-panel ventas-panel--catalogo">
          <div className="ventas-panel-header">
            <div>
              <h2>Productos disponibles</h2>
              <span>{productosDisponibles.length} productos listos para vender</span>
            </div>
          </div>

          <div className="ventas-toolbar">
            <div className="ventas-buscador">
              <i className="ti ti-search" aria-hidden="true"></i>
              <input
                type="text"
                placeholder="Buscar por producto o categoria..."
                value={busqueda}
                onChange={(e) => setBusqueda(e.target.value)}
              />
            </div>
            <div className="ventas-categorias" aria-label="Categorias">
              {categorias.map((categoria) => (
                <button
                  key={categoria}
                  type="button"
                  className={`ventas-categoria ${categoriaActiva === categoria ? "activa" : ""}`}
                  onClick={() => setCategoriaActiva(categoria)}
                >
                  {categoria}
                </button>
              ))}
            </div>
          </div>

          <div className="ventas-productos">
            {cargando ? (
              <p className="ventas-estado">Cargando productos...</p>
            ) : productosFiltrados.length === 0 ? (
              <p className="ventas-estado">No hay productos disponibles con ese filtro.</p>
            ) : (
              productosFiltrados.map((producto) => {
                const esPeso = esVentaPorPeso(producto);
                const unidadPeso = obtenerUnidadPesoProducto(producto);
                return (
                  <article className="ventas-producto" key={producto.id}>
                    <div className="ventas-producto-info">
                      <div>
                        <h3>{producto.nombre}</h3>
                        <p>{producto.categoria || "Sin categoria"}</p>
                      </div>
                      <span className={`ventas-badge ${esPeso ? "ventas-badge--peso" : "ventas-badge--unidad"}`}>
                        {esPeso ? "Peso" : "Unidad"}
                      </span>
                    </div>

                    <div className="ventas-producto-meta">
                      <span>{formatoMoneda.format(Number(producto.precio || 0))}</span>
                      <small>
                        Stock:{" "}
                        {esPeso
                          ? `${Number(producto.stock || 0).toFixed(1)} kg`
                          : `${Number(producto.stock || 0)} und`}
                      </small>
                    </div>

                    <div className="ventas-producto-acciones">
                      <input
                        type="number"
                        min="0"
                        step={esPeso ? "0.01" : "1"}
                        value={obtenerCantidadProducto(producto)}
                        onChange={(e) => actualizarCantidad(producto.id, e.target.value)}
                        aria-label={`Cantidad para ${producto.nombre}`}
                      />
                      {esPeso ? (
                        <select
                          value={unidadPeso}
                          onChange={(e) => actualizarUnidadPeso(producto.id, e.target.value)}
                          aria-label={`Unidad de peso para ${producto.nombre}`}
                        >
                          <option value="lb">lb</option>
                          <option value="kg">kg</option>
                        </select>
                      ) : (
                        <span className="ventas-unidad-fija">und</span>
                      )}
                      <button type="button" onClick={() => agregarProducto(producto)}>
                        <i className="ti ti-plus" aria-hidden="true"></i>
                        Agregar
                      </button>
                    </div>
                  </article>
                );
              })
            )}
          </div>
        </section>

        <aside className="ventas-panel ventas-panel--carrito">
          <div className="ventas-panel-header">
            <div>
              <h2>Carrito de venta</h2>
              <span>{carrito.length} producto(s)</span>
            </div>
          </div>

          {carrito.length === 0 ? (
            <p className="ventas-estado ventas-estado--carrito">
              Agrega productos para construir una venta simulada.
            </p>
          ) : (
            <div className="ventas-carrito-lista">
              {carrito.map((item) => (
                <div className="ventas-carrito-item" key={item.carritoId}>
                  <div className="ventas-carrito-superior">
                    <div>
                      <h3>{item.nombre}</h3>
                      <p>{item.tipoVenta} | {item.categoria}</p>
                    </div>
                    <button
                      type="button"
                      className="ventas-btn-icono"
                      onClick={() => quitarProducto(item.carritoId)}
                      aria-label={`Quitar ${item.nombre}`}
                    >
                      <i className="ti ti-trash" aria-hidden="true"></i>
                    </button>
                  </div>
                  <div className="ventas-carrito-detalle">
                    <span>{formatoMoneda.format(item.precio)}</span>
                    <span>{formatearCantidad(item)}</span>
                    <strong>{formatoMoneda.format(item.subtotal)}</strong>
                  </div>
                </div>
              ))}
            </div>
          )}

          <div className="ventas-totales">
            <div className="ventas-descuento">
              <label htmlFor="ventas-descuento">Descuento (%)</label>
              <input
                id="ventas-descuento"
                type="number"
                min="0"
                max="100"
                value={descuento}
                onChange={(e) => setDescuento(e.target.value)}
                placeholder="0"
              />
            </div>

            <div className="ventas-total-linea">
              <span>Subtotal</span>
              <strong>{formatoMoneda.format(totales.subtotal)}</strong>
            </div>
            <div className="ventas-total-linea">
              <span>Descuento</span>
              <strong>{formatoMoneda.format(totales.montoDescuento)}</strong>
            </div>
            <div className="ventas-total-linea ventas-total-linea--final">
              <span>Total</span>
              <strong>{formatoMoneda.format(totales.total)}</strong>
            </div>

            <button
              type="button"
              className="ventas-finalizar"
              disabled={carrito.length === 0 || procesandoVenta}
              onClick={finalizarVenta}
            >
              <i className="ti ti-check" aria-hidden="true"></i>
              {procesandoVenta ? "Registrando..." : "Finalizar venta"}
            </button>
          </div>
        </aside>
      </div>

      {resumenVenta && (
        <section className="ventas-panel ventas-resumen">
          <div className="ventas-panel-header">
            <div>
              <h2>
                Venta registrada
                {resumenVenta.numeroFactura
                  ? ` · ${resumenVenta.numeroFactura}`
                  : ""}
              </h2>
              <span>{resumenVenta.fecha}</span>
            </div>
          </div>

          <div className="ventas-resumen-aviso">
            <i className="ti ti-info-circle" aria-hidden="true"></i>
            La venta fue guardada, el inventario fue actualizado y la factura fue generada.
          </div>

          <div className="ventas-resumen-tabla-contenedor">
            <table className="ventas-resumen-tabla">
              <thead>
                <tr>
                  <th>Producto</th>
                  <th>Tipo</th>
                  <th>Precio</th>
                  <th>Cantidad</th>
                  <th>Subtotal</th>
                </tr>
              </thead>
              <tbody>
                {resumenVenta.productos.map((item) => (
                  <tr key={item.carritoId}>
                    <td>{item.nombre}</td>
                    <td>{item.tipoVenta}</td>
                    <td>{formatoMoneda.format(item.precio)}</td>
                    <td>{formatearCantidad(item)}</td>
                    <td>{formatoMoneda.format(item.subtotal)}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          <div className="ventas-resumen-totales">
            <span>Subtotal: {formatoMoneda.format(resumenVenta.subtotal)}</span>
            <span>
              Descuento ({resumenVenta.porcentajeDescuento}%):{" "}
              {formatoMoneda.format(resumenVenta.montoDescuento)}
            </span>
            <strong>Total: {formatoMoneda.format(resumenVenta.total)}</strong>
          </div>
        </section>
      )}
    </div>
  );
}

export default VentasPage;
