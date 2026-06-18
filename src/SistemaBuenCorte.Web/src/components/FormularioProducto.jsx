import { useState, useEffect } from "react";
import "./FormularioProducto.css";

const FORM_INICIAL = {
  nombre: "",
  descripcion: "",
  categoria: "",
  tipoVenta: "peso",
  precio: "",
  stock: "",
  activo: true,
};

function FormularioProducto({ visible, productoEditar, onGuardar, onCerrar }) {
  const [form, setForm] = useState(FORM_INICIAL);
  const [errores, setErrores] = useState({});

  useEffect(() => {
    if (productoEditar) {
      setForm({
        nombre: productoEditar.nombre || "",
        descripcion: productoEditar.descripcion || "",
        categoria: productoEditar.categoria || "",
        tipoVenta: productoEditar.tipoVenta || "peso",
        precio: productoEditar.precio || "",
        stock: productoEditar.stock || "",
        activo: productoEditar.activo ?? true,
      });
    } else {
      setForm(FORM_INICIAL);
    }
    setErrores({});
  }, [productoEditar, visible]);

  if (!visible) return null;

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setForm({ ...form, [name]: type === "checkbox" ? checked : value });
    if (errores[name]) setErrores({ ...errores, [name]: null });
  };

  const validar = () => {
    const nuevosErrores = {};
    if (!form.nombre.trim()) nuevosErrores.nombre = "El nombre es requerido.";
    if (!form.categoria.trim()) nuevosErrores.categoria = "La categoría es requerida.";
    if (!form.precio || isNaN(form.precio) || Number(form.precio) <= 0)
      nuevosErrores.precio = "Ingresa un precio válido mayor a 0.";
    if (form.stock === "" || isNaN(form.stock) || Number(form.stock) < 0)
      nuevosErrores.stock = "Ingresa un stock válido (0 o más).";
    return nuevosErrores;
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    const nuevosErrores = validar();
    if (Object.keys(nuevosErrores).length > 0) {
      setErrores(nuevosErrores);
      return;
    }

    onGuardar({
      nombre: form.nombre.trim(),
      descripcion: form.descripcion.trim() || null,
      categoria: form.categoria.trim(),
      tipoVenta: form.tipoVenta === "peso" ? "Peso" : "Unidad",
      precio: Number(form.precio),
      stock: Number(form.stock),
      activo: Number(form.stock) > 0 ? form.activo : false,
    });
  };

  const handleOverlayClick = (e) => {
    if (e.target.classList.contains("modal-overlay")) onCerrar();
  };

  const esEdicion = !!productoEditar;

  return (
    <div className="modal-overlay" onClick={handleOverlayClick}>
      <div className="modal-contenedor">
        <div className="modal-header">
          <h2>{esEdicion ? "Editar producto" : "Nuevo producto"}</h2>
          <button className="btn-cerrar" onClick={onCerrar} aria-label="Cerrar">
            <i className="ti ti-x" aria-hidden="true"></i>
          </button>
        </div>

        <form onSubmit={handleSubmit} noValidate>
          <div className="modal-body">

            <div className={`campo ${errores.nombre ? "campo-error" : ""}`}>
              <label htmlFor="nombre">Nombre del producto</label>
              <input
                id="nombre" name="nombre" type="text"
                placeholder="Ej: Bistec de res"
                value={form.nombre} onChange={handleChange}
              />
              {errores.nombre && <span className="mensaje-error">{errores.nombre}</span>}
            </div>

            <div className="campo">
              <label htmlFor="descripcion">Descripción</label>
              <textarea
                id="descripcion" name="descripcion"
                placeholder="Descripción opcional del producto"
                value={form.descripcion} onChange={handleChange} rows={3}
              />
            </div>

            <div className={`campo ${errores.categoria ? "campo-error" : ""}`}>
              <label htmlFor="categoria">Categoría</label>
              <input
                id="categoria" name="categoria" type="text"
                placeholder="Ej: Res, Cerdo, Pollo, Embutidos"
                value={form.categoria} onChange={handleChange}
              />
              {errores.categoria && <span className="mensaje-error">{errores.categoria}</span>}
            </div>

            <div className="campo">
              <label>Tipo de venta</label>
              <div className="radio-grupo">
                <label className={`radio-opcion ${form.tipoVenta === "peso" ? "radio-activo" : ""}`}>
                  <input type="radio" name="tipoVenta" value="peso"
                    checked={form.tipoVenta === "peso"} onChange={handleChange} />
                  Por peso (kg)
                </label>
                <label className={`radio-opcion ${form.tipoVenta === "unidad" ? "radio-activo" : ""}`}>
                  <input type="radio" name="tipoVenta" value="unidad"
                    checked={form.tipoVenta === "unidad"} onChange={handleChange} />
                  Por unidad
                </label>
              </div>
            </div>

            <div className="campo-fila">
              <div className={`campo ${errores.precio ? "campo-error" : ""}`}>
                <label htmlFor="precio">Precio (RD$)</label>
                <input
                  id="precio" name="precio" type="number"
                  placeholder="0.00" min="0" step="0.01"
                  value={form.precio} onChange={handleChange}
                />
                {errores.precio && <span className="mensaje-error">{errores.precio}</span>}
              </div>

              <div className={`campo ${errores.stock ? "campo-error" : ""}`}>
                <label htmlFor="stock">
                  Stock ({form.tipoVenta === "peso" ? "kg" : "und"})
                </label>
                <input
                  id="stock" name="stock" type="number"
                  placeholder="0" min="0"
                  step={form.tipoVenta === "peso" ? "0.1" : "1"}
                  value={form.stock} onChange={handleChange}
                />
                {errores.stock && <span className="mensaje-error">{errores.stock}</span>}
              </div>
            </div>

            {esEdicion && (
              <div className="campo campo-activo">
                <label className="label-checkbox">
                  <input
                    type="checkbox" name="activo"
                    checked={form.activo} onChange={handleChange}
                  />
                  Producto activo
                </label>
              </div>
            )}

          </div>

          <div className="modal-footer">
            <button type="button" className="btn-cancelar" onClick={onCerrar}>
              Cancelar
            </button>
            <button type="submit" className="btn-guardar">
              {esEdicion ? "Guardar cambios" : "Crear producto"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

export default FormularioProducto;
