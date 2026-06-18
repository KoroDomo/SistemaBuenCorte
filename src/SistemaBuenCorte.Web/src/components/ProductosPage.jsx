import { useState, useEffect } from "react";
import "./ProductosPage.css";
import FormularioProducto from "./FormularioProducto";
import { obtenerProductos, crearProducto, actualizarProducto, eliminarProducto } from "../services/productosApi";

const ITEMS_POR_PAGINA = 6;

function ProductosPage() {
  const [productos, setProductos] = useState([]);
  const [cargando, setCargando] = useState(true);
  const [busqueda, setBusqueda] = useState("");
  const [paginaActual, setPaginaActual] = useState(1);
  const [modalVisible, setModalVisible] = useState(false);
  const [productoEditar, setProductoEditar] = useState(null);
  const [mensaje, setMensaje] = useState(null);

  useEffect(() => {
    setCargando(true);
    obtenerProductos()
      .then(setProductos)
      .catch(() => mostrarMensaje("Error al cargar productos.", "error"))
      .finally(() => setCargando(false));
  }, []);

  const mostrarMensaje = (texto, tipo = "exito") => {
    setMensaje({ texto, tipo });
    setTimeout(() => setMensaje(null), 3000);
  };

  const cargarProductos = async () => {
    const data = await obtenerProductos();
    setProductos(data);
  };

  const productosFiltrados = productos.filter((p) => {
    const texto = busqueda.toLowerCase();
    return (
      p.nombre.toLowerCase().includes(texto) ||
      (p.categoria && p.categoria.toLowerCase().includes(texto))
    );
  });

  const totalPaginas = Math.max(1, Math.ceil(productosFiltrados.length / ITEMS_POR_PAGINA));
  const inicio = (paginaActual - 1) * ITEMS_POR_PAGINA;
  const productosPagina = productosFiltrados.slice(inicio, inicio + ITEMS_POR_PAGINA);

  const handleBusqueda = (e) => {
    setBusqueda(e.target.value);
    setPaginaActual(1);
  };

  const handleNuevoProducto = () => {
    setProductoEditar(null);
    setModalVisible(true);
  };

  const handleEditar = (producto) => {
    setProductoEditar(producto);
    setModalVisible(true);
  };

  const handleCerrarModal = () => {
    setModalVisible(false);
    setProductoEditar(null);
  };

  const handleGuardar = async (productoFinal) => {
    try {
      if (productoEditar) {
        await actualizarProducto(productoEditar.id, productoFinal);
        mostrarMensaje("Producto actualizado correctamente.");
      } else {
        await crearProducto(productoFinal);
        mostrarMensaje("Producto creado correctamente.");
      }
      await cargarProductos();
    } catch {
      mostrarMensaje("Error al guardar el producto.", "error");
    }
    handleCerrarModal();
  };

  const handleEliminar = async (producto) => {
    if (window.confirm(`¿Seguro que quieres eliminar "${producto.nombre}"?`)) {
      try {
        await eliminarProducto(producto.id);
        setProductos((prev) => prev.filter((p) => p.id !== producto.id));
        mostrarMensaje("Producto eliminado.", "error");
      } catch {
        mostrarMensaje("Error al eliminar el producto.", "error");
      }
    }
  };

  const productosSinStock = productos.filter((p) => !p.activo || p.stock === 0).length;

  return (
    <div className="productos-page">
      <header className="productos-header">
        <div>
          <h1>Gestión de productos</h1>
          <p className="productos-subtitulo">
            {productos.filter((p) => p.activo).length} productos activos · {productosSinStock} sin stock
          </p>
        </div>
      </header>

      {mensaje && (
        <div className={`mensaje-feedback mensaje-${mensaje.tipo}`}>
          {mensaje.texto}
        </div>
      )}

      <div className="productos-toolbar">
        <div className="buscador">
          <i className="ti ti-search" aria-hidden="true"></i>
          <input
            type="text"
            placeholder="Buscar producto..."
            value={busqueda}
            onChange={handleBusqueda}
          />
        </div>
        <button className="btn-filtrar">
          <i className="ti ti-filter" aria-hidden="true"></i>
          Filtrar
        </button>
        <button className="btn-nuevo" onClick={handleNuevoProducto}>
          <i className="ti ti-plus" aria-hidden="true"></i>
          Nuevo producto
        </button>
      </div>

      <div className="productos-tabla-contenedor">
        {cargando ? (
          <p className="estado-vacio">Cargando productos...</p>
        ) : productosFiltrados.length === 0 ? (
          <p className="estado-vacio">No se encontraron productos.</p>
        ) : (
          <table className="productos-tabla">
            <thead>
              <tr>
                <th>Producto</th>
                <th>Categoría</th>
                <th>Tipo de venta</th>
                <th>Precio</th>
                <th>Stock</th>
                <th>Estado</th>
                <th className="col-acciones">Acciones</th>
              </tr>
            </thead>
            <tbody>
              {productosPagina.map((producto) => (
                <tr key={producto.id}>
                  <td className="celda-nombre">{producto.nombre}</td>
                  <td>{producto.categoria || "—"}</td>
                  <td>
                    <span className={`badge badge-tipo-${producto.tipoVenta}`}>
                      {producto.tipoVenta === "peso" || producto.tipoVenta === "Peso"
                        ? "Por peso (kg)"
                        : "Por unidad"}
                    </span>
                  </td>
                  <td>RD$ {Number(producto.precio).toFixed(2)}</td>
                  <td className={producto.stock === 0 ? "stock-cero" : undefined}>
                    {producto.tipoVenta === "peso" || producto.tipoVenta === "Peso"
                      ? `${Number(producto.stock).toFixed(1)} kg`
                      : `${producto.stock} und`}
                  </td>
                  <td>
                    <span className={`badge ${producto.activo && producto.stock > 0 ? "badge-estado-activo" : "badge-estado-sinStock"}`}>
                      {producto.activo && producto.stock > 0 ? "Activo" : "Sin stock"}
                    </span>
                  </td>
                  <td className="col-acciones">
                    <button
                      className="btn-icono"
                      aria-label={`Editar ${producto.nombre}`}
                      onClick={() => handleEditar(producto)}
                    >
                      <i className="ti ti-edit" aria-hidden="true"></i>
                    </button>
                    <button
                      className="btn-icono"
                      aria-label={`Eliminar ${producto.nombre}`}
                      onClick={() => handleEliminar(producto)}
                    >
                      <i className="ti ti-trash" aria-hidden="true"></i>
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      {!cargando && productosFiltrados.length > 0 && (
        <div className="paginacion">
          <p>
            Mostrando {inicio + 1}-{Math.min(inicio + ITEMS_POR_PAGINA, productosFiltrados.length)} de {productosFiltrados.length} productos
          </p>
          <div className="paginacion-botones">
            <button
              className="btn-pagina"
              onClick={() => setPaginaActual((p) => Math.max(1, p - 1))}
              disabled={paginaActual === 1}
              aria-label="Página anterior"
            >
              <i className="ti ti-chevron-left" aria-hidden="true"></i>
            </button>
            {Array.from({ length: totalPaginas }, (_, i) => i + 1).map((numero) => (
              <button
                key={numero}
                className={`btn-pagina ${numero === paginaActual ? "activa" : ""}`}
                onClick={() => setPaginaActual(numero)}
              >
                {numero}
              </button>
            ))}
            <button
              className="btn-pagina"
              onClick={() => setPaginaActual((p) => Math.min(totalPaginas, p + 1))}
              disabled={paginaActual === totalPaginas}
              aria-label="Página siguiente"
            >
              <i className="ti ti-chevron-right" aria-hidden="true"></i>
            </button>
          </div>
        </div>
      )}

      <FormularioProducto
        visible={modalVisible}
        productoEditar={productoEditar}
        onGuardar={handleGuardar}
        onCerrar={handleCerrarModal}
      />
    </div>
  );
}

export default ProductosPage;
