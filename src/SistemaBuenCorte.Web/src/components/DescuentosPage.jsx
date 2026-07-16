import { useState, useEffect } from "react";
// Reutilizamos los estilos de la página de productos para mantener
// consistencia visual (tabla, toolbar, badges, paginación) sin duplicar CSS.
import "./ProductosPage.css";
import FormularioDescuento from "./FormularioDescuento";
import { obtenerDescuentos, crearDescuento, actualizarDescuento, eliminarDescuento } from "../services/descuentosApi";

const ITEMS_POR_PAGINA = 6;

function DescuentosPage() {
  const [descuentos, setDescuentos] = useState([]);
  const [cargando, setCargando] = useState(true);
  const [busqueda, setBusqueda] = useState("");
  const [paginaActual, setPaginaActual] = useState(1);
  const [modalVisible, setModalVisible] = useState(false);
  const [descuentoEditar, setDescuentoEditar] = useState(null);
  const [mensaje, setMensaje] = useState(null);

  useEffect(() => {
    setCargando(true);
    obtenerDescuentos()
      .then(setDescuentos)
      .catch(() => mostrarMensaje("Error al cargar descuentos.", "error"))
      .finally(() => setCargando(false));
  }, []);

  const mostrarMensaje = (texto, tipo = "exito") => {
    setMensaje({ texto, tipo });
    setTimeout(() => setMensaje(null), 3000);
  };

  const cargarDescuentos = async () => {
    const data = await obtenerDescuentos();
    setDescuentos(data);
  };

  const descuentosFiltrados = descuentos.filter((d) =>
    d.nombre.toLowerCase().includes(busqueda.toLowerCase())
  );

  const totalPaginas = Math.max(1, Math.ceil(descuentosFiltrados.length / ITEMS_POR_PAGINA));
  const inicio = (paginaActual - 1) * ITEMS_POR_PAGINA;
  const descuentosPagina = descuentosFiltrados.slice(inicio, inicio + ITEMS_POR_PAGINA);

  const handleBusqueda = (e) => {
    setBusqueda(e.target.value);
    setPaginaActual(1);
  };

  const handleNuevoDescuento = () => {
    setDescuentoEditar(null);
    setModalVisible(true);
  };

  const handleEditar = (descuento) => {
    setDescuentoEditar(descuento);
    setModalVisible(true);
  };

  const handleCerrarModal = () => {
    setModalVisible(false);
    setDescuentoEditar(null);
  };

  const handleGuardar = async (descuentoFinal) => {
    try {
      if (descuentoEditar) {
        await actualizarDescuento(descuentoEditar.id, descuentoFinal);
        mostrarMensaje("Descuento actualizado correctamente.");
      } else {
        await crearDescuento(descuentoFinal);
        mostrarMensaje("Descuento creado correctamente.");
      }
      await cargarDescuentos();
    } catch (err) {
      const errores = err?.response?.data?.errores;
      mostrarMensaje(
        Array.isArray(errores) ? errores.join(" ") : "Error al guardar el descuento.",
        "error"
      );
    }
    handleCerrarModal();
  };

  const handleEliminar = async (descuento) => {
    if (window.confirm(`¿Seguro que quieres desactivar "${descuento.nombre}"?`)) {
      try {
        await eliminarDescuento(descuento.id);
        await cargarDescuentos();
        mostrarMensaje("Descuento desactivado.", "error");
      } catch {
        mostrarMensaje("Error al desactivar el descuento.", "error");
      }
    }
  };

  const activos = descuentos.filter((d) => d.activo).length;

  const formatoValor = (descuento) =>
    descuento.tipo === "Porcentaje"
      ? `${Number(descuento.valor).toFixed(0)} %`
      : `RD$ ${Number(descuento.valor).toFixed(2)}`;

  return (
    <div className="productos-page">
      <header className="productos-header">
        <div>
          <h1>Gestión de descuentos</h1>
          <p className="productos-subtitulo">
            {activos} descuentos activos · {descuentos.length - activos} inactivos
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
            placeholder="Buscar descuento..."
            value={busqueda}
            onChange={handleBusqueda}
          />
        </div>
        <button className="btn-nuevo" onClick={handleNuevoDescuento}>
          <i className="ti ti-plus" aria-hidden="true"></i>
          Nuevo descuento
        </button>
      </div>

      <div className="productos-tabla-contenedor">
        {cargando ? (
          <p className="estado-vacio">Cargando descuentos...</p>
        ) : descuentosFiltrados.length === 0 ? (
          <p className="estado-vacio">No se encontraron descuentos.</p>
        ) : (
          <table className="productos-tabla">
            <thead>
              <tr>
                <th>Nombre</th>
                <th>Tipo</th>
                <th>Valor</th>
                <th>Estado</th>
                <th className="col-acciones">Acciones</th>
              </tr>
            </thead>
            <tbody>
              {descuentosPagina.map((descuento) => (
                <tr key={descuento.id}>
                  <td className="celda-nombre">{descuento.nombre}</td>
                  <td>
                    <span className="badge">
                      {descuento.tipo === "Porcentaje" ? "Porcentaje" : "Monto fijo"}
                    </span>
                  </td>
                  <td>{formatoValor(descuento)}</td>
                  <td>
                    <span className={`badge ${descuento.activo ? "badge-estado-activo" : "badge-estado-sinStock"}`}>
                      {descuento.activo ? "Activo" : "Inactivo"}
                    </span>
                  </td>
                  <td className="col-acciones">
                    <button
                      className="btn-icono"
                      aria-label={`Editar ${descuento.nombre}`}
                      onClick={() => handleEditar(descuento)}
                    >
                      <i className="ti ti-edit" aria-hidden="true"></i>
                    </button>
                    <button
                      className="btn-icono"
                      aria-label={`Desactivar ${descuento.nombre}`}
                      onClick={() => handleEliminar(descuento)}
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

      {!cargando && descuentosFiltrados.length > 0 && (
        <div className="paginacion">
          <p>
            Mostrando {inicio + 1}-{Math.min(inicio + ITEMS_POR_PAGINA, descuentosFiltrados.length)} de {descuentosFiltrados.length} descuentos
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

      <FormularioDescuento
        visible={modalVisible}
        descuentoEditar={descuentoEditar}
        onGuardar={handleGuardar}
        onCerrar={handleCerrarModal}
      />
    </div>
  );
}

export default DescuentosPage;
