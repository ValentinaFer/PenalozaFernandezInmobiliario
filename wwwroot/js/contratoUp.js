$(document).ready(function () {

    const Toast = initToast();

    if (Pagos > 0) {
        Swal.fire({
            icon: 'warning',
            title: '¡Ya existen pagos realizados para este Contrato!',
            html: `<div class="row text-center"><h4>Por favor, verifique la información de los pagos realizados</h4>.
                <p>Si desea cambiar el inmueble, por favor, elimine los pagos realizados antes.</p>
                </div>`,

        })

        $("#btn_update")
            .prop("disabled", true)

        initInquilinoDatatable("#tableInquilinos", true);
        $("#tableInquilinos").DataTable().on("select", function (e, dt, type, indexes) {
            $("#btn_update").prop("disabled", false);
        }).on("deselect", function (e, dt, type, indexes) {
            $("#btn_update").prop("disabled", true);
        });

        $("#btn_update")
            .on("click", function () {
                console.log("click");
                const inquilinoSelected = $("#tableInquilinos").DataTable().row({ selected: true }).data();
                console.log(inquilinoSelected);
                if (inquilinoSelected && inquilinoSelected.id) {
                    $.ajax({
                        url: `/Contrato/Update/Inquilino`,
                        type: "POST",
                        data: {
                            idInquilino: inquilinoSelected.id,
                            idContrato: IdContratoEditando,
                        }
                    }).done(function (d) {
                        Swal.fire({
                            icon: 'success',
                            title: '¡Inquilino del Contrato actualizado exitosamente!',
                        })
                        $("#btn_update").prop("disabled", true);
                    }).fail(function (e) {
                        console.log(e);
                        let msg = "¡Ocurrio un error inesperado!";
                        if (e.responseJSON) {   
                            msg = e.responseJSON.message
                            ;
                        }
                        Toast.fire({
                            icon: 'error',
                            title: '¡No se pudo actualizar el inquilino del Contrato!',
                            text : msg.length < 50 ? msg : msg.substring(0, 50) + "..."
                        })
                    });
                }

            });

    } else {


        $(".steps_btn").click(function () {
            const step = $(this).data("step"); //current step
            const action = $(this).data("action"); //next || previous
            handleStep(step, action);
        });

        function checkSelected(tableId) {
            const selectedItem = $(tableId).DataTable().rows('.selected').data()[0];
            if (tableId === "#tableInquilinos") {
                $(".mandatory-inquilino").removeClass("text-danger text-success");
                $(".icon-inquilino").find("i").remove();
                if (!selectedItem) {
                    $(".mandatory-inquilino").addClass("text-danger");
                    $(".icon-inquilino").prepend('<i class="bi bi-exclamation-circle"></i> ');
                    return false;
                } else if (selectedItem && selectedItem.id) {
                    $(".mandatory-inquilino").addClass("text-success");
                    $(".icon-inquilino").prepend("<i class='bi bi-check-circle'></i> ");
                    return true;
                }
            }
            if (tableId === "#tableInmuebles") {
                $(".mandatory-inmueble").removeClass("text-danger text-success");
                $(".icon-inmueble").find("i").remove();
                const inmuebleData = $("#tableInmuebles").DataTable().rows('.selected').data()[0];
                console.log(inmuebleData);
                if (!inmuebleData) {
                    $(".mandatory-inmueble").addClass("text-danger");
                    $(".icon-inmueble").prepend('<i class="bi bi-exclamation-circle"></i> ');
                    return false;
                } else if (inmuebleData && inmuebleData.idInmueble) {
                    $(".mandatory-inmueble").addClass("text-success");
                    $(".icon-inmueble").prepend("<i class='bi bi-check-circle'></i> ");
                    return true;
                }
            }
            return false;
        }

        initInquilinoDatatable("#tableInquilinos", true);

        $("#tableInquilinos").DataTable()
            .on("select", function (e, dt, type, indexes) {
                checkSelected("#tableInquilinos");
            }).on("deselect", function (e, dt, type, indexes) {
                checkSelected("#tableInquilinos");
            });


        if (Pagos == 0) {
            initInmuebleDatatable("#tableInmuebles", true);
            $("#tableInmuebles").DataTable().on("select", function (e, dt, type, indexes) {
                checkSelected("#tableInmuebles");
            }).on("deselect", function (e, dt, type, indexes) {
                checkSelected("#tableInmuebles");
            });
        }


        function handleStep(step, action) {
            console.log(step, action);
            if (action === "next") {
                switch (step) {
                    case 1: //debe haber seleccionado el inquilino
                        break;
                    case 2: //debe haber seleccionado el inmueble
                        //debe haber seleccionado inquilino e inmueble para pasar al step 3
                        handleShowReview();


                        break;
                    case 3: //creando
                        updateContrato();
                        return;
                        break;
                    default:
                        return;
                        break;
                }
                $(`#step_${step}`).removeClass("d-block").addClass("d-none");
                $(`#step_${step + 1}`).removeClass("d-none").addClass("d-block");
            } else if (action === "previous") {
                if (step === 2 || step === 3) {
                    $(`#step_${step}`).removeClass("d-block").addClass("d-none");
                    $(`#step_${step - 1}`).removeClass("d-none").addClass("d-block");
                }
            }
        }

        initDataPickr("#monthTo", "#monthFrom");

        $("#monthFrom").on("change", function () {
            updateTermRenting();
        });

        $("#monthTo").on("change", function () {
            updateTermRenting();
        });

        function getNextMonth() {
            const date = new Date();
            date.setMonth(date.getMonth() + 1);
            return date.toISOString().split('T')[0].slice(0, 7);
        }

        function getNextTwoYears() {
            const date = new Date();
            date.setFullYear(date.getFullYear() + 2);
            return date;
        }

        function getFormattedDate(date) {
            const months = ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"];
            const month = months[date.getMonth()];
            const year = date.getFullYear();
            return `${month} de ${year}`;
        }

        function updateTermRenting() {
            let valueFrom = $("#monthFrom").val();
            let valueTo = $("#monthTo").val();
            if (valueFrom == "" || valueTo == "") {
                return;
            }
            valueFrom = new Date(valueFrom);
            valueTo = new Date(valueTo);

            let stringToShow = "";
            let start = new Date(valueFrom);
            let end = new Date(valueTo);
            let years = (end.getFullYear() - start.getFullYear());
            let months = (end.getMonth() - start.getMonth()) + 1;

            if (months < 0) {
                years--;
                months += 12;
            }

            if (years > 0) {
                stringToShow += years + (years > 1 ? " años " : " año ");
            }
            if (months > 0) {
                stringToShow += months + (months > 1 ? " meses " : " mes ");
            }

            $("#duracionAlquiler").text(stringToShow);
        }

        $("#btn_search_inmueble").on("click", function () {
            $("#tableInmuebles").DataTable().ajax.reload();
        });

        function getDates() {
            const [year, month] = $("#monthFrom").val().split("-");
            const [year2, month2] = $("#monthTo").val().split("-");
            if (year && month && year2 && month2) {
                return {
                    from: new Date(year, month - 1, 1),
                    to: new Date(year2, month2, 0)
                };
            }
            return null;
        }

        function validarInput(input) {
            if (!input.value) {
                input.classList.add("is-invalid");
                input.classList.remove("is-valid");
            } else {
                input.classList.add("is-valid");
                input.classList.remove("is-invalid");
            }
        }

        function handleShowReview() {
            const inqSelected = checkSelected("#tableInquilinos");
            const inmSelected = checkSelected("#tableInmuebles");
            const datesSelected = getDates();
            let inquilinoId = null
            if (inqSelected) {
                inquilinoId = $("#tableInquilinos").DataTable().rows({ selected: true }).data()[0].id;
            }
            let inmuebleId = null
            if (inmSelected) {
                inmuebleId = $("#tableInmuebles").DataTable().rows({ selected: true }).data()[0].idInmueble;
            } else {
                inmuebleId = idInmuebleEditando;
            }


            const from = flatpickr("#monthFromReview", {
                plugins: [
                    new monthSelectPlugin({
                        altInput: true,
                        shorthand: true,
                        ariaDateFormat: "F \\de Y",
                        dateFormat: "Y-m",
                        altFormat: "F \\de Y", //for some reason the format is not working(using formatDate instead)
                    })
                ],
                locale: "es",
                minDate: getNextMonth(),
                maxDate: getNextTwoYears(),
                formatDate: function (date) {
                    return getFormattedDate(date);
                },
            });

            const to = flatpickr("#monthToReview", {
                plugins: [
                    new monthSelectPlugin({
                        altInput: true,
                        shorthand: true, // Use abbreviated month names
                        ariaDateFormat: "F \\de Y",
                        dateFormat: "Y-m", // Set the format to Year-Month (e.g., 2024-10)
                        altFormat: "F \\de Y", // Show the full month name and year (e.g., October de 2024)
                    })
                ],
                locale: "es",
                maxDate: getNextTwoYears(),
                formatDate: function (date) {
                    return getFormattedDate(date);
                }
            });

            if (IdContratoEditando > 0 & Pagos == 0) {
                $.ajax({
                    //trae meses ocupados por fuera del contrato x de x inmueble
                    url: `/Contrato/Inmueble/GetOccupiedMonths/${IdContratoEditando}/${inmuebleId}`,
                    type: "GET",
                })
                    .done(function (data) { //date: array of occupied months(Contratos) [{fechaDesde:, fechaHasta:}]
                        if (data != null) {
                            console.log(data);
                            let datesRange = data.map((date) => {
                                return { from: date.fechaDesde, to: date.fechaHasta };
                            })
                            //for some reason UI is not updating correctly with this (minDate and maxDate), it takes a few interations to update correctly
                            from.set("disable", datesRange);
                            to.set("disable", datesRange);
                            from.set("onChange", function (selectedDates, dateStr, instance) {
                                try {
                                    if (selectedDates.length > 0) {
                                        const selectedFrom = selectedDates[0];

                                        const closestAfterDisabledRange = datesRange.find((date) => {
                                            const fromD = new Date(date.from);
                                            const toD = new Date(date.to);
                                            return selectedFrom < fromD && selectedFrom < toD;
                                        })
                                        if (closestAfterDisabledRange != null && closestAfterDisabledRange != undefined) {
                                            let maxDminusMonth = new Date(closestAfterDisabledRange.from);
                                            maxDminusMonth.setMonth(maxDminusMonth.getMonth());
                                            maxDminusMonth.setDate(0);
                                            const maxD = maxDminusMonth
                                            console.log("Min", selectedFrom, ", max", maxD);
                                            to.set("minDate", selectedFrom);
                                            to.set("maxDate", maxD);
                                        } else {
                                            console.log("Min, max setted for To", selectedFrom, getNextTwoYears());
                                            to.set("minDate", selectedFrom);
                                            to.set("maxDate", new Date(getNextTwoYears()));
                                            to.set("maxDate", getNextTwoYears());
                                        }
                                        to.setDate(selectedFrom);
                                        to.redraw();
                                        from.redraw();
                                        validarInput(from.input);
                                    };
                                } catch (error) {
                                    console.log(error);
                                }

                            });
                            to.set("onChange", function (selectedDates, dateStr, instance) {
                                validarInput(to.input);
                            });
                            to.set("onClose", function (selectedDates, dateStr, instance) {
                                validarInput(to.input);
                                console.log("to:", instance);
                            });
                            from.set("onClose", function (selectedDates, dateStr, instance) {
                                validarInput(from.input);
                            })

                        }
                        if (datesSelected != null) {
                            if (datesSelected.from < datesSelected.to) {
                                from.setDate(datesSelected.from);
                                to.setDate(datesSelected.to);
                            }
                        }
                    })
                    .fail(function (jqXHR, textStatus, errorThrown) {
                        console.log(textStatus, errorThrown);
                        Swal.fire({
                            icon: 'error',
                            title: '¡Algo salio mal al cargar los meses ocupados!',
                            text: "Ocurrio un error al cargar los meses ocupados, por favor intente de nuevo más tarde o contacte con un administrador.",
                        })
                        $("#step_3").addClass("d-none");
                        $("#step_3").removeClass("d-block");
                        $("#step_2").removeClass("d-none");
                        $("#step_2").addClass("d-block");
                        return;
                    });
            }

            console.log(inmuebleId, inquilinoId, datesSelected);
        }

        function updateContrato() {
            const inmuebleSelected = $("#tableInmuebles").DataTable().rows({ selected: true }).data()[0];
            const inquilinoSelected = $("#tableInquilinos").DataTable().rows({ selected: true }).data()[0];
            let dateFrom = null;
            let dateTo = null;

            if (Pagos == 0) {
                const from = document.querySelector("#monthFromReview")._flatpickr;
                const to = document.querySelector("#monthToReview")._flatpickr;
                console.log(from, to);
                dateFrom = new Date(from.currentYear, from.currentMonth, 1);
                dateTo = new Date(to.currentYear, to.currentMonth + 1, 0);
                console.log(inmuebleSelected, inquilinoSelected, dateFrom, dateTo);
            }

            const newInmuebleId = inmuebleSelected ? inmuebleSelected.idInmueble : idInmuebleEditando;
            const newInquilinoId = inquilinoSelected ? inquilinoSelected.id : IdInquilinoEditando;
            const newDateFrom = dateFrom ? dateFrom : null;
            const newDateTo = dateTo ? dateTo : null;
            if (newInmuebleId != null && newInquilinoId != null && newDateFrom != null && newDateTo != null) {
                if (Pagos == 0) {
                    if (isNumber(newInmuebleId) && isNumber(newInquilinoId)) {
                        if (newDateFrom == "" || newDateTo == "") {
                            Toast.fire({
                                icon: 'error',
                                title: '¡Debes seleccionar una fecha!',
                                text: "Por favor, ingrese una fecha de inicio y fin válida.",
                            })
                            return;
                        } else {
                            if (newDateFrom > newDateTo) {
                                Toast.fire({
                                    icon: 'error',
                                    title: '¡Fecha de inicio no puede ser mayor a la fecha de fin!',
                                    text: "Por favor, ingrese una fecha de inicio y fin válida.",
                                })
                                return;
                            }
                        }
                    }
                }
                Swal.fire({
                    title: 'Cargando...',
                    html: "<p>Editando inmueble...</p>",
                    willOpen: () => {
                        Swal.showLoading();
                        let formData = new FormData();
                        formData.append("inquilinoId", newInquilinoId);
                        formData.append("inmuebleId", newInmuebleId);
                        formData.append("dateFrom", newDateFrom ? newDateFrom.toISOString().split('T')[0] : "");
                        formData.append("dateTo", newDateTo ? newDateTo.toISOString().split('T')[0] : "");
                        formData.append("idContrato", IdContratoEditando);
                        formData.append("idInmuebleOld", idInmuebleEditando);
                        formData.append("idInquilinoOld", IdInquilinoEditando);
                        $.ajax({
                            url: "/Contrato/Update",
                            type: "POST",
                            processData: false,
                            contentType: false,
                            data: formData,
                        }).done(function (data) {
                            Swal.hideLoading();
                            console.log(data);
                            Swal.fire({
                                icon: 'success',
                                title: `Contrato n° ${data}`,
                                text: '¡Inmueble actualizado con éxito!',
                            });
                        }).fail(function (xhr) {
                            Swal.close();
                            handleAjaxError(xhr, "Ocurrio un error al crear el contrato.");
                            console.log("Error en ajax al crear el contrato", xhr);
                        });
                    }
                })
            } else {
                Toast.fire({
                    icon: 'error',
                    title: 'Oops.. Algo salio mal al cargar el inmueble o el inquilino!'
                })
                $("#step_3").addClass("d-none");
                $("#step_3").removeClass("d-block");
                $("#step_2").addClass("d-none");
                $("#step_2").removeClass("d-block");
                $("#step_1").removeClass("d-none");
                $("#step_1").addClass("d-block");
                return;
            }
        }


        const handleAjaxError = (xhr, generalMsg = "Ocurrio un error inesperado") => {
            let msg = generalMsg;
            try {
                const res = JSON.parse(xhr.responseText);
                if (res.exceptionType == "InvalidOperationException") {
                    msg = res.message;
                    Swal.fire({
                        icon: 'error',
                        title: msg
                    })
                    return;
                } else if (res.exceptionType == "Fuera de disponibilidad") {
                    msg = res.message;
                    Swal.fire({
                        icon: 'error',
                        title: "El inmueble se encuentra fuera de disponibilidad para las fechas seleccionadas. Por favor, elija otras fechas.",
                    })
                    return;
                } {
                    msg = res.message || msg;
                }
            } catch (error) {
                msg = xhr.responseText || msg;
            }
            Toast.fire({
                icon: 'error',
                title: msg
            })

        }
    }
})

