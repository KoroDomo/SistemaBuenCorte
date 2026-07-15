import { useEffect, useState } from "react";
import "./CajaPage.css";
import FormularioCaja from "./FormularioCaja";
import { useAuth } from "../context/AuthContext";
import {
    obtenerCajas,
    obtenerCajaAbierta,
    abrirCaja,
    cerrarCaja,
} from "../services/cajaApi";

const ITEMS_POR_PAGINA = 6;

function CajaPage() {
    const [cajas, setCajas] = useState([]);
    const [cajaAbierta, setCajaAbierta] = useState(null);
    const [cargando, setCargando] = useState(true);
    const [modalVisible, setModalVisible] = useState(false);
    const [tipoFormulario, setTipoFormulario] = useState("abrir");
    const [mensaje, setMensaje] = useState(null);
    const [paginaActual, setPaginaActual] = useState(1);

    const { sesion } = useAuth();

    const obtenerIdDesdeToken = (token) => {
        try {
            if (!token) return null;

            const base64Url = token.split(".")[1];
            const base64 = base64Url
                .replace(/-/g, "+")
                .replace(/_/g, "/");

            const payload = JSON.parse(atob(base64));

            return Number(payload.id) || null;
        } catch {
            return null;
        }
    };

    const usuarioId =
        Number(sesion?.id) > 0
            ? Number(sesion.id)
            : obtenerIdDesdeToken(sesion?.token);

    const mostrarMensaje = (texto, tipo = "exito") => {
        setMensaje({ texto, tipo });
        setTimeout(() => setMensaje(null), 3500);
    };

    const cargarDatos = async () => {
        setCargando(true);

        try {
            const listado = await obtenerCajas();
            setCajas(listado);

            if (usuarioId) {
                const abierta = await obtenerCajaAbierta(usuarioId);
                setCajaAbierta(abierta);
            }
        } catch {
            mostrarMensaje("Error al cargar la información de caja.", "error");
        } finally {
            setCargando(false);
        }
    };

    useEffect(() => {
        cargarDatos();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [usuarioId]);

    const handleAbrirModal = () => {
        setTipoFormulario("abrir");
        setModalVisible(true);
    };

    const handleCerrarModalCaja = () => {
        setTipoFormulario("cerrar");
        setModalVisible(true);
    };

    const handleCerrarModal = () => {
        setModalVisible(false);
    };

    const obtenerMensajeBackend = (error) => {
        const data = error.response?.data;

        if (typeof data === "string") return data;

        return (
            data?.mensaje ||
            data?.message ||
            data?.title ||
            "Ocurrió un error al procesar la operación."
        );
    };

    const handleGuardar = async (datos) => {
        try {
            if (tipoFormulario === "abrir") {
                const nuevaCaja = await abrirCaja(datos);
                setCajaAbierta(nuevaCaja);
                mostrarMensaje("Caja abierta correctamente.");
            } else {
                await cerrarCaja(cajaAbierta.id, datos);
                setCajaAbierta(null);
                mostrarMensaje("Caja cerrada correctamente.");
            }

            setModalVisible(false);
            await cargarDatos();
        } catch (error) {
            mostrarMensaje(obtenerMensajeBackend(error), "error");
        }
    };

    const totalPaginas = Math.max(
        1,
        Math.ceil(cajas.length / ITEMS_POR_PAGINA)
    );

    const inicio = (paginaActual - 1) * ITEMS_POR_PAGINA;
    const cajasPagina = cajas.slice(inicio, inicio + ITEMS_POR_PAGINA);

    const formatearFecha = (fecha) => {
        if (!fecha) return "—";

        return new Date(fecha).toLocaleString("es-DO", {
            dateStyle: "short",
            timeStyle: "short",
        });
    };

    return (
        <div className="caja-page">
            <header className="caja-header">
                <div>
                    <h1>Gestión de caja</h1>
                    <p className="caja-subtitulo">
                        Control de aperturas, cierres y movimientos por turno
                    </p>
                </div>

                {cajaAbierta ? (
                    <button
                        className="btn-cerrar-caja"
                        onClick={handleCerrarModalCaja}
                    >
                        <i className="ti ti-lock" aria-hidden="true"></i>
                        Cerrar caja
                    </button>
                ) : (
                    <button className="btn-abrir-caja" onClick={handleAbrirModal}>
                        <i className="ti ti-cash" aria-hidden="true"></i>
                        Abrir caja
                    </button>
                )}
            </header>

            {mensaje && (
                <div className={`mensaje-feedback mensaje-${mensaje.tipo}`}>
                    {mensaje.texto}
                </div>
            )}

            {!usuarioId && (
                <div className="mensaje-feedback mensaje-error">
                    No se pudo identificar el usuario autenticado.
                </div>
            )}

            <section className="caja-resumen">
                <div className="resumen-tarjeta">
                    <div className="resumen-icono">
                        <i className="ti ti-building-bank"></i>
                    </div>

                    <div>
                        <span>Estado actual</span>
                        <strong
                            className={
                                cajaAbierta ? "estado-abierta" : "estado-cerrada"
                            }
                        >
                            {cajaAbierta ? "Caja abierta" : "Caja cerrada"}
                        </strong>
                    </div>
                </div>

                <div className="resumen-tarjeta">
                    <div className="resumen-icono">
                        <i className="ti ti-wallet"></i>
                    </div>

                    <div>
                        <span>Monto de apertura</span>
                        <strong>
                            RD${" "}
                            {Number(cajaAbierta?.montoApertura || 0).toFixed(2)}
                        </strong>
                    </div>
                </div>

                <div className="resumen-tarjeta">
                    <div className="resumen-icono">
                        <i className="ti ti-chart-bar"></i>
                    </div>

                    <div>
                        <span>Ventas del turno</span>
                        <strong>
                            RD$ {Number(cajaAbierta?.totalVentas || 0).toFixed(2)}
                        </strong>
                    </div>
                </div>

                <div className="resumen-tarjeta">
                    <div className="resumen-icono">
                        <i className="ti ti-clock"></i>
                    </div>

                    <div>
                        <span>Fecha de apertura</span>
                        <strong>{formatearFecha(cajaAbierta?.fechaApertura)}</strong>
                    </div>
                </div>
            </section>

            <section className="historial-caja">
                <div className="historial-header">
                    <div>
                        <h2>Historial de cajas</h2>
                        <p>{cajas.length} turnos registrados</p>
                    </div>

                    <button className="btn-recargar" onClick={cargarDatos}>
                        <i className="ti ti-refresh"></i>
                        Actualizar
                    </button>
                </div>

                <div className="caja-tabla-contenedor">
                    {cargando ? (
                        <p className="estado-vacio">Cargando cajas...</p>
                    ) : cajas.length === 0 ? (
                        <p className="estado-vacio">
                            No existen turnos de caja registrados.
                        </p>
                    ) : (
                        <table className="caja-tabla">
                            <thead>
                                <tr>
                                    <th>ID</th>
                                    <th>Usuario</th>
                                    <th>Apertura</th>
                                    <th>Monto inicial</th>
                                    <th>Total ventas</th>
                                    <th>Monto cierre</th>
                                    <th>Estado</th>
                                </tr>
                            </thead>

                            <tbody>
                                {cajasPagina.map((caja) => (
                                    <tr key={caja.id}>
                                        <td>#{caja.id}</td>
                                        <td className="celda-nombre">
                                            {caja.nombreUsuario || `Usuario ${caja.usuarioId}`}
                                        </td>
                                        <td>{formatearFecha(caja.fechaApertura)}</td>
                                        <td>
                                            RD$ {Number(caja.montoApertura).toFixed(2)}
                                        </td>
                                        <td>
                                            RD$ {Number(caja.totalVentas || 0).toFixed(2)}
                                        </td>
                                        <td>
                                            {caja.montoCierreContado === null ||
                                                caja.montoCierreContado === undefined
                                                ? "—"
                                                : `RD$ ${Number(
                                                    caja.montoCierreContado
                                                ).toFixed(2)}`}
                                        </td>
                                        <td>
                                            <span
                                                className={`badge ${caja.estado?.toLowerCase() === "abierta"
                                                        ? "badge-caja-abierta"
                                                        : "badge-caja-cerrada"
                                                    }`}
                                            >
                                                {caja.estado}
                                            </span>
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    )}
                </div>

                {!cargando && cajas.length > 0 && (
                    <div className="paginacion">
                        <p>
                            Mostrando {inicio + 1}-
                            {Math.min(inicio + ITEMS_POR_PAGINA, cajas.length)} de{" "}
                            {cajas.length} cajas
                        </p>

                        <div className="paginacion-botones">
                            <button
                                className="btn-pagina"
                                onClick={() =>
                                    setPaginaActual((pagina) => Math.max(1, pagina - 1))
                                }
                                disabled={paginaActual === 1}
                            >
                                <i className="ti ti-chevron-left"></i>
                            </button>

                            {Array.from(
                                { length: totalPaginas },
                                (_, indice) => indice + 1
                            ).map((numero) => (
                                <button
                                    key={numero}
                                    className={`btn-pagina ${numero === paginaActual ? "activa" : ""
                                        }`}
                                    onClick={() => setPaginaActual(numero)}
                                >
                                    {numero}
                                </button>
                            ))}

                            <button
                                className="btn-pagina"
                                onClick={() =>
                                    setPaginaActual((pagina) =>
                                        Math.min(totalPaginas, pagina + 1)
                                    )
                                }
                                disabled={paginaActual === totalPaginas}
                            >
                                <i className="ti ti-chevron-right"></i>
                            </button>
                        </div>
                    </div>
                )}
            </section>

            <FormularioCaja
                visible={modalVisible}
                tipo={tipoFormulario}
                usuarioId={usuarioId}
                cajaActual={cajaAbierta}
                onGuardar={handleGuardar}
                onCerrar={handleCerrarModal}
            />
        </div>
    );
}

export default CajaPage;