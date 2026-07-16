import { useState, useEffect } from "react";
// Reutilizamos los estilos del formulario de productos para mantener
// consistencia visual (modal, campos, botones) sin duplicar CSS.
import "./FormularioProducto.css";

const FORM_INICIAL = {
  nombre: "",
  tipo: "porcentaje",
  valor: "",
  activo: true,
};

function FormularioDescuento({ visible, descuentoEditar, onGuardar, onCerrar }) {
  const [form, setForm] = useState(FORM_INICIAL);
  const [errores, setErrores] = useState({});

  useEffect(() => {
    if (descuentoEditar) {
      setForm({
        nombre: descuentoEditar.nombre || "",
        tipo: descuentoEditar.tipo === "MontoFijo" ? "montoFijo" : "porcentaje",
        valor: descuentoEditar.valor ?? "",
        activo: descuentoEditar.activo ?? true,
      });
    } else {
      setForm(FORM_INICIAL);
    }
    setErrores({});
  }, [descuentoEditar, visible]);

  if (!visible) return null;

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setForm({ ...form, [name]: type === "checkbox" ? checked : value });
    if (errores[name]) setErrores({ ...errores, [name]: null });
  };

  const validar = () => {
    const nuevosErrores = {};
    if (!form.nombre.trim()) nuevosErrores.nombre = "El nombre es requerido.";
    else if (form.nombre.trim().length > 80)
      nuevosErrores.nombre = "El nombre no puede superar los 80 caracteres.";

    if (form.valor === "" || isNaN(form.valor) || Number(form.valor) <= 0) {
      nuevosErrores.valor = "Ingresa un valor válido mayor a 0.";
    } else if (form.tipo === "porcentaje" && Number(form.valor) > 100) {
      nuevosErrores.valor = "Un porcentaje no puede superar el 100%.";
    }
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
      tipo: form.tipo === "montoFijo" ? "MontoFijo" : "Porcentaje",
      valor: Number(form.valor),
      activo: form.activo,
    });
  };

  const handleOverlayClick = (e) => {
    if (e.target.classList.contains("modal-overlay")) onCerrar();
  };

  const esEdicion = !!descuentoEditar;

  return (
    <div className="modal-overlay" onClick={handleOverlayClick}>
      <div className="modal-contenedor">
        <div className="modal-header">
          <h2>{esEdicion ? "Editar descuento" : "Nuevo descuento"}</h2>
          <button className="btn-cerrar" onClick={onCerrar} aria-label="Cerrar">
            <i className="ti ti-x" aria-hidden="true"></i>
          </button>
        </div>

        <form onSubmit={handleSubmit} noValidate>
          <div className="modal-body">

            <div className={`campo ${errores.nombre ? "campo-error" : ""}`}>
              <label htmlFor="nombre">Nombre del descuento</label>
              <input
                id="nombre" name="nombre" type="text"
                placeholder="Ej: Descuento empleado"
                value={form.nombre} onChange={handleChange}
              />
              {errores.nombre && <span className="mensaje-error">{errores.nombre}</span>}
            </div>

            <div className="campo">
              <label>Tipo de descuento</label>
              <div className="radio-grupo">
                <label className={`radio-opcion ${form.tipo === "porcentaje" ? "radio-activo" : ""}`}>
                  <input type="radio" name="tipo" value="porcentaje"
                    checked={form.tipo === "porcentaje"} onChange={handleChange} />
                  Porcentaje (%)
                </label>
                <label className={`radio-opcion ${form.tipo === "montoFijo" ? "radio-activo" : ""}`}>
                  <input type="radio" name="tipo" value="montoFijo"
                    checked={form.tipo === "montoFijo"} onChange={handleChange} />
                  Monto fijo (RD$)
                </label>
              </div>
            </div>

            <div className={`campo ${errores.valor ? "campo-error" : ""}`}>
              <label htmlFor="valor">
                {form.tipo === "porcentaje" ? "Valor (%)" : "Valor (RD$)"}
              </label>
              <input
                id="valor" name="valor" type="number"
                placeholder={form.tipo === "porcentaje" ? "Ej: 10" : "Ej: 50.00"}
                min="0"
                max={form.tipo === "porcentaje" ? "100" : undefined}
                step={form.tipo === "porcentaje" ? "1" : "0.01"}
                value={form.valor} onChange={handleChange}
              />
              {errores.valor && <span className="mensaje-error">{errores.valor}</span>}
            </div>

            {esEdicion && (
              <div className="campo campo-activo">
                <label className="label-checkbox">
                  <input
                    type="checkbox" name="activo"
                    checked={form.activo} onChange={handleChange}
                  />
                  Descuento activo
                </label>
              </div>
            )}

          </div>

          <div className="modal-footer">
            <button type="button" className="btn-cancelar" onClick={onCerrar}>
              Cancelar
            </button>
            <button type="submit" className="btn-guardar">
              {esEdicion ? "Guardar cambios" : "Crear descuento"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

export default FormularioDescuento;
