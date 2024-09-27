$(document).ready(function () {

    console.log(Rol);
    const Toast = initToast();

    function formatDate(data) {
        const date = new Date(data);
        const day = String(date.getDate()).padStart(2, '0');
        const month = String(date.getMonth() + 1).padStart(2, '0');
        const year = date.getFullYear();
        return `${day}/${month}/${year}`;
    }

    function formatPrices(val) {
        return new Intl.NumberFormat('es-AR', {
            style: 'currency',
            currency: 'ARS'
        }).format(val);
    }

    $(".btn_activar").on("click", function () {
        const idContrato = $(this).data("id");
        console.log(idContrato);
        fireActiveModal(idContrato);
    });

    function fireActiveModal(idContrato) {
        if (isNumber(idContrato) && idContrato > 0) {
            Swal.fire({
                icon: 'warning',
                title: '¿Desea reactivar el contrato?',
                showDenyButton: true,
                showCancelButton: false,
                confirmButtonText: 'Activar',
                denyButtonText: `Cancelar`,
            }).then((result) => {
                if (result.isConfirmed) {
                    activeContrato(idContrato);
                }
            })
    }}

    function activeContrato(idContrato) {
        $.ajax({
            url: `/Contrato/Active/${idContrato}`,
            type: "GET",
        }).done(function () {
            window.location.reload();
        });
    }

    $(".btn_cancelar").on("click", function () {
        const idContrato = $(this).data("id");
        console.log(idContrato);
        fireCancelModal(idContrato);
    });

    function fireCancelModal(idContrato) {
        if (isNumber(idContrato) && idContrato > 0) {
            $.ajax({
                url: `/Contrato/Data/${idContrato}`,
                type: "GET",
            }).done(function (data) {
                console.log(data);
                if (data.fechaFinalizacion != null) {
                    Swal.fire({
                        icon: 'warning',
                        title: 'No se puede cancelar el contrato',
                        text: 'El contrato ya ha sido finalizado'
                    });
                } else if (data.fechaFinalizacion == null) {

                    const totalMeses = getMeses(data.fechaDesde, data.fechaHasta);
                    const totalAPagar = totalMeses * data.monto;
                    const totalMesesPagados = getMesesPagados(data);
                    const totalPagado = totalMesesPagados * data.monto;
                    const today = new Date();
                    let mesesHastaFinDecimal = Math.ceil(getMesesEnDecimal(data.fechaDesde, today));
                    if (mesesHastaFinDecimal < 0) {
                        mesesHastaFinDecimal = 0;
                    }
                    let totalAPagarFinal = mesesHastaFinDecimal * data.monto - totalPagado;
                    if (totalAPagarFinal < 0) {
                        totalAPagarFinal = 0;
                    }
                    const multa = calcularMulta(data.fechaDesde, data.fechaHasta, data.fechaFinalizacion, data.monto);
                    Swal.fire({
                        title: 'Cancelar Contrato nro. ' + idContrato,
                        html: `<div class="row">
                        <h4 class="text-danger">¿Seguro que desea anular el contrato antes de tiempo?</h4>
                        <h5 class="text-danger">¡Esta accion no se puede deshacer!</h5>
                                <div class="col-12">
                                    <p>Precio mensual: ${formatPrices(data.monto)}</p>
                                    <p>Meses pagados: ${totalMesesPagados} / ${totalMeses}</p>
                                    <p>Meses hasta la fecha : ${mesesHastaFinDecimal <= 0 ? "0 meses. (cancelando antes de inicio)" : mesesHastaFinDecimal}</p>
                                    <p>Multa: ${formatPrices(multa)}</p>
                                    <p>Cobro por meses faltantes: ${formatPrices(totalAPagarFinal)}</p>
                                    <hr>
                                    <p><strong>Total a pagar: ${formatPrices(totalAPagarFinal + multa)} </strong></p>
                                </div>
                                </div>`,
                        icon: 'warning',
                        showCancelButton: true,
                        confirmButtonColor: '#d33',
                        cancelButtonColor: '#3085d6',
                        cancelButtonText: 'No, volver',
                        confirmButtonText: 'Sí, anular contrato'
                    }).then((result) => {
                        if (result.isConfirmed) {
                            $.ajax({
                                url: `/Contrato/cancelar/${idContrato}`,
                                type: "POST"
                            }).done(function (data) {
                                if (data) {
                                    Swal.fire({
                                        icon: 'success',
                                        title: 'Contrato nro. ' + idContrato + ' cancelado',
                                        text: 'El contrato ha sido cancelado con exito'
                                    });
                                    location.reload();
                                }
                            }).fail(function (data) {
                                handleAjaxError(data, "Ocurrio un error al cancelar el contrato");
                            })
                        }
                    });
                }
            });

        }
    }

    $(".btn_pagar").on("click", function () {
        const idContrato = $(this).data("id");
        console.log(idContrato);
        fireMainModal(idContrato);
    });

    function getMesesPagados(contrato) {
        let mesesPagados = 0;
        if (contrato && contrato.pagos) {
            if (contrato.pagos.length > 0) {
                mesesPagados = contrato.pagos.filter(p => p.estado === true).length;
            }
        }
        return mesesPagados;
    }
    function getMeses(fechaDesde, fechaHasta) {
        let meses = 0;
        if (fechaDesde && fechaHasta) {
            fechaDesde = new Date(fechaDesde);
            fechaHasta = new Date(fechaHasta);
            meses = ((fechaHasta.getFullYear() - fechaDesde.getFullYear()) * 12) + (fechaHasta.getMonth() - fechaDesde.getMonth() + 1);
        }
        return meses;
    }

    function getMesesEnDecimal(fechaDesde, fechaFinalizacion) {
        fechaDesde = new Date(fechaDesde);
        fechaFinalizacion = new Date(fechaFinalizacion);
        const totalMonths = (fechaFinalizacion.getFullYear() - fechaDesde.getFullYear()) * 12 + fechaFinalizacion.getMonth() - fechaDesde.getMonth();
        let daysInStartMonth = new Date(fechaDesde.getFullYear(), fechaDesde.getMonth() + 1, 0).getDate();
        let daysInEndMonth = new Date(fechaFinalizacion.getFullYear(), fechaFinalizacion.getMonth() + 1, 0).getDate();

        let partialStartMonth = (daysInStartMonth - fechaDesde.getDate() + 1) / daysInStartMonth;
        let partialEndMonth = fechaFinalizacion.getDate() / daysInEndMonth;

        return totalMonths + partialStartMonth + partialEndMonth - 1;
    }



    function fireMainModal(idContrato, message = "", showMsg = false) {
        if (isNumber(idContrato) && idContrato > 0) {
            console.log("pago", idContrato);
            $.ajax({
                url: `/Contrato/Pagos/${idContrato}`,
                type: "GET",
            }).done(function (d) {
                console.log(d);
                const calculos = d.calculos
                const data = d.contrato
                let titulo = "";
                let subtitulo = "";
                const totalMesesPagados = calculos.mesesPagadosSinMulta;
                const totalMeses = getMeses(data.fechaDesde, data.fechaHasta);
                let totalAPagarFinal = calculos.faltanteAPagar;
                const totalPagado = calculos.totalPagado;
                const totalAPagar = calculos.totalAPagar;
                const mesesHastaFinDecimal = calculos.mesesHastaFinalizacion;
                const multa = calculos.multa;
                const precioMensual = data.monto;
                let multaData = "";
                let showMultaHint = false;
                if (calculos.contratoPagado) {
                    subtitulo = "Pagado"
                } else {
                    subtitulo = "No Pagado"
                }
                if (data.fechaFinalizacion == null) {
                    titulo = "Contrato Vigente"
                } else if (data.fechaFinalizacion < data.fechaHasta) {
                    showMultaHint = true;
                    titulo = "Contrato Cancelado"
                    const mesesRestantes = totalAPagarFinal - multa;
                    multaData = `
                        <p>Meses hasta la fecha : ${mesesHastaFinDecimal <= 0 ? "0 meses. (cancelado antes de inicio)" : Math.ceil(mesesHastaFinDecimal)}</p>
                        <p>Multa: ${formatPrices(multa)}</p>
                        <p>Cobro por meses faltantes: ${formatPrices(mesesRestantes<0?0:mesesRestantes)}</p>
                        `;

                } else if (data.fechaFinalizacion >= data.fechaHasta) {
                    titulo = "Contrato Finalizado"
                }

                console.log("totalMesesPagados", totalMesesPagados, "totalMeses", totalMeses, "totalPagado", totalPagado, "totalAPagar", totalAPagar);
                Swal.fire({
                    title: 'Pagos del Contrato n° ' + idContrato,
                    width: "80%",
                    heightAuto: false,
                    showCloseButton: true,
                    showCancelButton: true,
                    animation: false,
                    html: `<table id="pagosTable" class="table table-sm table-responsive table-hover table-striped display" width="100%">
                            <h3>${titulo}</h3>
                            <h4>${subtitulo}</h4>
                            <p>${formatDate(data.fechaDesde)} - ${formatDate(data.fechaHasta)} ${data.fechaFinalizacion ? ` (${formatDate(data.fechaFinalizacion)})` : ""}</p>
                            <h4 id="message" class="d-none"></h4>
                            <thead>
                                <tr>
                                    <th>#</th>
                                    <th>Descripción</th>
                                    <th>Importe</th>
                                    <th>Creación</th>
                                    <th>Activo</th>
                                    <th>Acciones</th>
                                </tr>
                            </thead>
                            <tbody>
                            </tbody>
                            </table>
                            <div class="row">
                                <p> Meses pagados: ${totalMesesPagados} / ${totalMeses}</p>
                                <p> Total Pagado${showMultaHint?"(Sin multa)":""}: ${formatPrices(totalPagado)} / ${formatPrices(totalAPagar)} (${formatPrices(precioMensual)}/mes)</p> 
                                ${multaData}
                                <hr>
                                <p><strong>Total a pagar: ${formatPrices(totalAPagarFinal)} </strong></p>
                            </div>
                            <hr>
                            <h3>Agregar nuevo Pago</h3>
                            <form id="form-pago">
                                <div class="row">
                                <div class="col-12 col-sm-6">
                                <input type="text" class="form-control" id="pagoImporte" placeholder="Importe" min="0" max="${totalAPagar}" value=${formatPrices(precioMensual)} disabled>
                                </div>
                                <div class="col-12 col-sm-6">
                                <input maxlength="100" type="text" class="form-control" id="pagoDetalle" placeholder="Descripción">
                                </div>    
                                </div>
                            </form>`,
                    confirmButtonText: 'Agregar nuevo Pago',
                    cancelButtonText: 'Cerrar',
                    didOpen: () => {
                        if (showMsg && message) {
                            $("#message").text(message).removeClass("d-none");
                            $("#message").addClass("text-success d-block");
                            setTimeout(() => $("#message").addClass("d-none"), 3000);
                        }
                        $("#pagosTable").DataTable({
                            data: data.pagos,
                            autoWidth: false,
                            responsive: true,
                            lengthMenu: [5, 10, 20, 40],
                            pageLength: 5,
                            columns: [
                                { data: "nroPago", responsivePriority: 1 },
                                { data: "detallePago", responsivePriority: 4 },
                                {
                                    data: null, responsivePriority: 3,
                                    render: function (data) {
                                        return formattedImport = new Intl.NumberFormat('es-AR', {
                                            style: 'currency',
                                            currency: 'ARS'
                                        }).format(data.importe);
                                    }, className: "text-nowrap"
                                },
                                {
                                    data: null, responsivePriority: 6,
                                    render: function (data) {
                                        return formatDate(data.fechaPago);
                                    }, className: "text-nowrap d-none d-sm-table-cell"
                                },
                                {
                                    data: null, responsivePriority: 5,
                                    render: function (data) {
                                        return data.estado ? "Si" : "No";
                                    }
                                },
                                {
                                    data: null, responsivePriority: 2,
                                    render: function (data) {
                                        let btns = `
                                            <button class="btn btn-info btn-sm btn_pago_editar" data-id-contrato="${idContrato}" title="Editar"><i class="bi bi-pencil-fill text-light"></i></button>`;
                                        if (data.estado && Rol == "Administrador") {
                                            btns +=
                                                `<button class="btn btn-danger btn-sm btn_pago_eliminar" data-id-contrato="${idContrato}" title="Eliminar"><i class="bi bi-trash-fill text-light"></i></button>`
                                        }
                                        return btns
                                    }
                                }
                            ],
                        });
                        initTableBtnListers(idContrato);
                    },
                    preConfirm: () => {
                        //const importeNuevo = $("#pagoImporte").val();
                        if (calculos.contratoPagado == false) {
                            const detalle = $("#pagoDetalle").val();
                            if (detalle.trim() == "") {
                                Swal.showValidationMessage("Por favor, ingrese una descripción")
                                return false;
                            }
                            if (detalle.length > 100) {
                                Swal.showValidationMessage("La descripción no debe superar los 100 caracteres")
                                return false;
                            }
                            return detalle;
                        } else {
                            Swal.showValidationMessage("Ya se ha completado la renta. No se pueden agregar más pagos.")
                            return false;
                        }
                    }
                }).then((result) => {
                    if (result.isConfirmed) {
                        const nuevoPago = result.value;
                        createPago(nuevoPago, idContrato)
                            .then(() => {
                                fireMainModal(idContrato, "¡Pago agregado correctamente!", true);
                            })
                            .catch((err) => {
                                console.log("Error al crear el pago", err);
                            });
                        console.log(nuevoPago);
                    }
                })
            }).fail(function (xhr) {
                handleAjaxError(xhr, "Ocurrio un error al pagar el contrato");
            });
        }

    }

    function initTableBtnListers() {
        $("#pagosTable").on("click", ".btn-sm", function () {
            const classes = $(this).attr("class");
            const idContrato = $(this).data("id-contrato");
            if (idContrato && idContrato > 0) {
                const data = $("#pagosTable").DataTable().row($(this).parents("tr")).data();
                if (classes.includes("btn_pago_editar")) {

                    fireEditModal(data, idContrato);
                }

                if (classes.includes("btn_pago_eliminar")) {
                    fireDeleteModal(data, idContrato);
                }
            }


        });
    }

    function fireDeleteModal(pago, idContrato,) {

        if (Rol != "Administrador") {
            throw new Error("No tiene permisos para realizar esta operación");
        }

        if (!pago || !idContrato) {
            return;
        }
        const idPago = pago.id;
        if (!isNumber(idPago) || idPago <= 0 || !isNumber(idContrato) || idContrato <= 0) {
            return;
        }
        Swal.fire({
            title: `¿Está seguro que desea eliminar el pago nro. ${pago.nroPago}?`,
            html:
                `<div class="row">
                 <h5 class="text-center text-danger">Esta acción descontará el importe del pago a la renta, lo que podría afectar el estado de finalización de la renta.</h5>
                 <p>Detalle: ${pago.detallePago}</p>
                 <p>Fecha de adición del pago: ${formatDate(pago.fechaPago)}</p>
                </div>`,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            cancelButtonText: 'Cancelar',
            confirmButtonText: 'Si, eliminar'
        }).then((result) => {
            if (result.isConfirmed) {
                deletePago(idPago, idContrato).then(() => {
                    fireMainModal(idContrato, "¡Pago eliminado correctamente!", true);
                }).catch((err) => {
                    console.log("Error al eliminar el pago", err);
                })
            } else {
                fireMainModal(idContrato);
            }
        })

    }

    function fireEditModal(pago, idContrato) {
        const idPago = pago.id;
        if (!isNumber(idPago) || idPago <= 0) {
            return;
        }
        Swal.fire({
            title: `Editando Pago nro. ${pago.nroPago}`,
            width: "80%",
            heightAuto: false,
            showCloseButton: true,
            showCancelButton: true,
            animation: false,
            html: `<form id="form-pago">
                    <div class="row m-3">
                    <p>Descripción actual: ${pago.detallePago}</p>
                    <p>Fecha de adisión del pago: ${formatDate(pago.fechaPago)}</p>
                    <div class="col-12 m-auto">
                    <input maxlength="100" type="text" class="form-control" id="pagoDetalle" placeholder="Nueva descripción" value="${pago.detallePago}">
                    </div>    
                    </div>
                </form>`,
            preConfirm: () => {

                const detalle = $("#pagoDetalle").val();

                if (detalle.trim() == "") {
                    Swal.showValidationMessage("Por favor, ingrese una descripción")
                    return false;
                }
                if (detalle.length > 100) {
                    Swal.showValidationMessage("La descripción no debe superar los 100 caracteres")
                    return false;
                }
                return detalle;

            },
        }).then((result) => {
            if (result.isConfirmed) {
                const nuevoPago = result.value;
                updatePago(idPago, nuevoPago, idContrato)
                    .then(() => {
                        fireMainModal(idContrato, "¡Pago editado correctamente!", true);
                    }).catch((err) => {
                        console.log("Error al editar el pago", err);
                    });
                console.log(nuevoPago);
            } else {
                fireMainModal(idContrato);
            }
        })
    }

    function deletePago(idPago, idContrato) {
        try {
            if (Rol != "Administrador") {
                throw new Error("No tiene permisos para realizar esta operación");
            }
            if (!isNumber(idContrato) || idContrato <= 0) {
                throw new Error("El ID del contrato no es valido");
            }
            if (!isNumber(idPago) || idPago <= 0) {
                throw new Error("El ID del pago no es valido");
            }
            return $.ajax({
                url: "/Contrato/Pagos/Eliminar",
                type: "POST",
                data: {
                    idPago: idPago,
                    idContrato: idContrato
                },
            }).done(function (data) {
                if (!data || !data || data < 0) {
                    return Promise.reject("Ocurrio un error al eliminar el pago");
                }
            }).fail(function (xhr) {
                handleAjaxError(xhr, "Ocurrio un error al eliminar el pago");
            })

        } catch (error) {
            Toast.fire({
                icon: 'Ocurrio un error inesperado',
                title: error.message
            })
            return Promise.reject(error);
        }
    }

    function updatePago(id, detalle, idContrato) {
        try {
            if (!isNumber(id) || id <= 0) {
                throw new Error("El ID del pago no es valido");
            }
            if (!detalle || detalle.trim() == "" || detalle.length > 100) {
                throw new Error("Por favor, complete todos los campos");
            }
            return $.ajax({
                url: "/Contrato/Pagos/Editar",
                type: "POST",
                data: {
                    idPago: id,
                    detalleNuevo: detalle,
                    idContrato: idContrato
                },

            }).done(function (data) {
                console.log(data);
                if (!data || !data || data < 0) {
                    return Promise.reject("Ocurrio un error al editar el pago");
                }
            }).fail(function (xhr) {
                handleAjaxError(xhr, "Ocurrio un error al editar el pago");
            });
        }
        catch (error) {
            Toast.fire({
                icon: 'Ocurrio un error inesperado',
                title: error.message
            })
            return Promise.reject(error);
        }
    }

    function createPago(detalle, idContrato) {

        try {
            if (!isNumber(idContrato) || idContrato <= 0) {
                throw new Error("El ID del contrato no es valido");
            }
            if (!detalle || detalle.trim() == "" || detalle.length > 100) {
                throw new Error("Por favor, complete todos los campos");
            }

            return $.ajax({
                url: "/Contrato/Pagos/Crear",
                type: "POST",
                data: {
                    detalle: detalle,
                    contratoId: idContrato
                },

            }).done(function (data) {
                if (data) {
                }
            }).fail(function (xhr) {
                handleAjaxError(xhr, "Ocurrio un error al crear el pago");
            });
        }
        catch (error) {
            return Promise.reject(error);
        }
    }

    const handleAjaxError = (xhr, generalMsg = "Ocurrio un error inesperado") => {
        let msg = generalMsg;
        try {
            const res = JSON.parse(xhr.responseText);
            if (res.exceptionType == "InvalidOperationException" || res.exceptionType == "KeyNotFoundException"
                || res.exceptionType == "ArgumentException"
            ) {
                msg = res.message;
            }
        } catch (error) {
            msg = xhr.responseText || msg;
        }
        Toast.fire({
            icon: 'error',
            title: msg
        })
        console.log(msg);
    }

})