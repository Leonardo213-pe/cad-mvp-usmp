// Sección urgente del panel de central (HU11).
// - Estado inicial y resincronización por REST: GET /api/central/sos-pendientes (la BD es la fuente de verdad).
// - SOS nuevos por SignalR (evento "SosNuevo"); tras mostrarlos se envía el acuse "Presentado" (RNF02).
(() => {
    "use strict";

    const contenedor = document.querySelector(".central");
    const lista = document.getElementById("lista-sos");
    const sinSos = document.getElementById("sin-sos");
    const contador = document.getElementById("contador-sos");
    const estadoConexion = document.getElementById("estado-conexion");
    const alertaNueva = document.getElementById("alerta-nueva");
    const botonTurno = document.getElementById("iniciar-turno");
    const tituloOriginal = document.title;

    const sosPorId = new Map();
    const marcadores = new Map();
    let sinLeer = 0;
    let audio = null;

    // ---------- Mapa (Leaflet + teselas de OpenStreetMap con atribución visible) ----------
    const mapa = L.map("mapa").setView(
        [Number(contenedor.dataset.mapaLatitud), Number(contenedor.dataset.mapaLongitud)],
        Number(contenedor.dataset.mapaZoom));

    L.tileLayer("https://tile.openstreetmap.org/{z}/{x}/{y}.png", {
        maxZoom: 19,
        attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>'
    }).addTo(mapa);

    // ---------- Presentación ----------
    const hora = (iso) => new Date(iso).toLocaleTimeString("es-PE", { hour: "2-digit", minute: "2-digit", second: "2-digit" });

    function crearElemento(etiqueta, clase, texto) {
        const el = document.createElement(etiqueta);
        if (clase) el.className = clase;
        if (texto !== undefined) el.textContent = texto; // textContent: nunca se interpreta HTML recibido.
        return el;
    }

    function renderizarSos(sos, esNuevo) {
        sosPorId.set(sos.casoId, sos);

        const item = crearElemento("li", "sos" + (esNuevo ? " sos-nuevo" : ""));
        item.dataset.casoId = sos.casoId;

        const cabecera = crearElemento("div", "sos-cabecera");
        cabecera.append(crearElemento("strong", null, "SOS"), crearElemento("time", null, hora(sos.aceptadoEn)));
        item.append(cabecera, crearElemento("div", "sos-ciudadano", sos.ciudadano));
        item.append(crearElemento("div", "sos-dato", sos.telefono ? "Tel. " + sos.telefono : "Sin teléfono registrado"));

        if (sos.ubicacion) {
            item.append(crearElemento("div", "sos-dato",
                `Ubicación ±${Math.round(sos.ubicacion.precisionMetros)} m · ${hora(sos.ubicacion.obtenidaEn)}`));
            agregarMarcador(sos);
            item.addEventListener("click", () => enfocar(sos.casoId));
            item.tabIndex = 0;
        } else {
            item.append(crearElemento("div", "sos-dato sos-sin-ubicacion", "Ubicación no disponible"));
        }

        // Los nuevos arriba; el estado inicial ya viene ordenado del más antiguo al más reciente.
        lista.prepend(item);
        actualizarContador();
    }

    function agregarMarcador(sos) {
        const { latitud, longitud, precisionMetros } = sos.ubicacion;
        const marcador = L.marker([latitud, longitud], { title: "SOS " + hora(sos.aceptadoEn) }).addTo(mapa);
        const popup = crearElemento("div");
        popup.append(crearElemento("strong", null, "SOS · " + hora(sos.aceptadoEn)), crearElemento("br"),
            crearElemento("span", null, sos.ciudadano));
        marcador.bindPopup(popup);
        const circulo = L.circle([latitud, longitud], { radius: precisionMetros, color: "#c0392b", weight: 1, fillOpacity: 0.1 }).addTo(mapa);
        marcadores.set(sos.casoId, { marcador, circulo });
    }

    function enfocar(casoId) {
        const m = marcadores.get(casoId);
        if (!m) return;
        mapa.flyTo(m.marcador.getLatLng(), 17);
        m.marcador.openPopup();
        document.querySelectorAll(".sos-nuevo").forEach(el => el.classList.remove("sos-nuevo"));
        sinLeer = 0;
        document.title = tituloOriginal;
    }

    function actualizarContador() {
        const total = sosPorId.size;
        contador.textContent = total === 0 ? "Sin SOS pendientes" : `${total} SOS pendiente${total === 1 ? "" : "s"}`;
        sinSos.hidden = total > 0;
    }

    function limpiar() {
        lista.replaceChildren();
        marcadores.forEach(({ marcador, circulo }) => { marcador.remove(); circulo.remove(); });
        marcadores.clear();
        sosPorId.clear();
    }

    // ---------- Aviso visual y sonoro ----------
    botonTurno.addEventListener("click", () => {
        audio = new (window.AudioContext || window.webkitAudioContext)();
        botonTurno.textContent = "Turno iniciado · sonido activo";
        botonTurno.disabled = true;
    });

    function sonar() {
        if (!audio) return;
        const inicio = audio.currentTime;
        for (let i = 0; i < 3; i++) {
            const osc = audio.createOscillator();
            const vol = audio.createGain();
            osc.type = "square";
            osc.frequency.value = 880;
            vol.gain.value = 0.15;
            osc.connect(vol).connect(audio.destination);
            osc.start(inicio + i * 0.35);
            osc.stop(inicio + i * 0.35 + 0.2);
        }
    }

    function avisar() {
        sinLeer++;
        document.title = `(${sinLeer}) SOS nuevo · Panel CAD`;
        alertaNueva.hidden = false;
        alertaNueva.classList.remove("parpadeo");
        void alertaNueva.offsetWidth; // reinicia la animación
        alertaNueva.classList.add("parpadeo");
        setTimeout(() => { alertaNueva.hidden = true; }, 6000);
        sonar();
    }

    // ---------- Estado inicial y resincronización ----------
    async function cargarPendientes() {
        const respuesta = await fetch("/api/central/sos-pendientes", { cache: "no-store", credentials: "same-origin" });
        if (respuesta.status === 401) { window.location.href = "/cuentas/login?ReturnUrl=/central"; return; }
        if (!respuesta.ok) throw new Error("No se pudieron cargar los SOS pendientes: " + respuesta.status);
        const pendientes = await respuesta.json();
        limpiar();
        pendientes.forEach(sos => renderizarSos(sos, false));
    }

    // ---------- Tiempo real ----------
    function mostrarConexion(texto, clase) {
        estadoConexion.textContent = texto;
        estadoConexion.className = "conexion " + clase;
    }

    const conexion = new signalR.HubConnectionBuilder()
        .withUrl("/hubs/cad")
        // Sigue reintentando mientras el panel esté abierto (sección 9.8).
        .withAutomaticReconnect({ nextRetryDelayInMilliseconds: c => Math.min(1000 * 2 ** c.previousRetryCount, 15000) })
        .build();

    conexion.on("SosNuevo", async (sos) => {
        if (sosPorId.has(sos.casoId)) return;
        renderizarSos(sos, true);
        avisar();
        try {
            await conexion.invoke("Presentado", sos.casoId);
        } catch (e) {
            console.warn("No se pudo enviar el acuse de presentación", e);
        }
    });

    conexion.onreconnecting(() => mostrarConexion("Sin tiempo real · reconectando…", "conexion-error"));
    conexion.onreconnected(async () => {
        mostrarConexion("En tiempo real", "conexion-ok");
        await cargarPendientes(); // los eventos emitidos durante el corte se recuperan de la BD
    });
    conexion.onclose(() => {
        mostrarConexion("Sin tiempo real", "conexion-error");
        setTimeout(iniciar, 5000);
    });

    async function iniciar() {
        try {
            await conexion.start();
            mostrarConexion("En tiempo real", "conexion-ok");
            await cargarPendientes();
        } catch (e) {
            console.warn("Sin conexión con el servidor", e);
            mostrarConexion("Sin tiempo real · reintentando…", "conexion-error");
            setTimeout(iniciar, 5000);
        }
    }

    iniciar();
})();
