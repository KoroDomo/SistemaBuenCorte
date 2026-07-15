import { useEffect, useState } from "react";
import "./FormularioCaja.css";
import "./FormularioProducto.css";


function FormularioCaja({
    visible,
    tipo,
    usuarioId,
    cajaActual,
    onGuardar,
    onCerrar,
}) {
    const [monto, setMonto] = useState("");
    const [error, setError] = useState("");

    useEffect(() => {
        setMonto("");
        setError("");
    }, [visible, tipo]);

    if (!visible) return null;

    const esApertura = tipo === "abrir";

    const validar = () => {
        if (monto === "" || Number.isNaN(Number(monto))) {
            return "Debes ingresar un monto válido.";
        }

        if (Number(monto) < 0) {
            return "El monto no puede ser negativo.";
        }

        return "";
    };

    const handleSubmit = (e) => {
        e.preventDefault();

        const mensajeError = validar();

        if (mensajeError) {
            setError(mensajeError);
            return;
        }

        if (esApertura) {
            onGuardar({
                usuarioId: Number(usuarioId),
                montoApertura: Number(monto),
            });
        } else {
            onGuardar({
                montoCierreContado: Number(monto),
            });
        }
    };

    const handleOverlayClick = (e) => {
        if (e.target.classList.contains("modal-overlay")) {
            onCerrar();
        }
    };

    const montoEsperado =
        Number(cajaActual?.montoApertura || 0) +
        Number(cajaActual?.totalVentas || 0);

    return (
        <div className="modal-overlay" onClick={handleOverlayClick}>
            <div className="modal-contenedor modal-caja">
                <div className="modal-header">
                    <h2>{esApertura ? "Abrir caja" : "Cerrar caja"}</h2>

                    <button
                        type="button"
                        className="btn-cerrar"
                        onClick={onCerrar}
                        aria-label="Cerrar"
                    >
                        <i className="ti ti-x" aria-hidden="true"></i>
                    </button>
                </div>

                <form onSubmit={handleSubmit} noValidate>
                    <div className="modal-body">
                        {!esApertura && cajaActual && (
                            <div className="resumen-cierre">
                                <div>
                                    <span>Monto de apertura</span>
                                    <strong>
                                        RD$ {Number(cajaActual.montoApertura).toFixed(2)}
                                    </strong>
                                </div>

                                <div>
                                    <span>Total de ventas</span>
                                    <strong>
                                        RD$ {Number(cajaActual.totalVentas || 0).toFixed(2)}
                                    </strong>
                                </div>

                                <div className="monto-esperado">
                                    <span>Monto esperado</span>
                                    <strong>RD$ {montoEsperado.toFixed(2)}</strong>
                                </div>
                            </div>
                        )}

                        <div className={`campo ${error ? "campo-error" : ""}`}>
                            <label htmlFor="monto">
                                {esApertura
                                    ? "Monto inicial de apertura"
                                    : "Monto contado al cierre"}
                            </label>

                            <input
                                id="monto"
                                name="monto"
                                type="number"
                                min="0"
                                step="0.01"
                                placeholder="0.00"
                                value={monto}
                                onChange={(e) => {
                                    setMonto(e.target.value);
                                    setError("");
                                }}
                                autoFocus
                            />

                            {error && <span className="mensaje-error">{error}</span>}
                        </div>
                    </div>

                    <div className="modal-footer">
                        <button
                            type="button"
                            className="btn-cancelar"
                            onClick={onCerrar}
                        >
                            Cancelar
                        </button>

                        <button type="submit" className="btn-guardar">
                            {esApertura ? "Abrir caja" : "Confirmar cierre"}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
}

export default FormularioCaja;