
function initInquilinoDatatable(idTable = "#tableInquilinos", selectable = false){

    $(idTable).DataTable({
        language: "es",
        responsive: true,
        processing: true,
        serverSide: true,
        lengthMenu: [5, 10, 15, 20],
        pageLength: 5,
        select: selectable?"single":false,
        ajax: {
            url: "/Contrato/Inquilinos",
            type: "GET",
        },
        order: [],
        columns: [
            { title: "#", data: "id", name: "IdInquilino", responsivePriority: 1, searchable: false, orderable: false, visible: false },
            {
                title: "Inquilino", data: null, render: function (data, type, row) {
                    return data.apellido + ", " + data.nombre + "."
                }, name: "Nombre", responsivePriority: 2
            },
            {
                title: "dni", data: null, render: function (data, type, row) {
                    return data.dni
                }, name: "dni", responsivePriority: 2
            },
            {
                title: "Acciones", data: null, responsivePriority: 2, searchable: false, orderable: false, render: function (data, type, row) {
                    return `<button class='btn btn-info btn-sm btnSeeDetails' data-id=${data.idInquilino}><i class='bi bi-person-vcard-fill text-light'></i></button>`
                }
            }
        ]
    })

}

function initInmuebleDatatable(idTable = "#tableInmuebles", selectable = false){
    $(idTable).DataTable({
        language: "es",
        searching: false,
        responsive: true,
        processing: true,
        serverSide: true,
        select: selectable?"single":false,
        ajax: {
            url: "/Contrato/GetInmuebles",
            type: "GET",
            data: function (d) {
                var dateFrom = $("#monthFrom").val();
                var dateTo = $("#monthTo").val();
                var usoInm = $("#usoInmueble").val();
                var tipoInm = $("#tipoInmueble").val();

                if (!dateFrom || !dateTo) {
                    dateFrom = "";
                    dateTo = "";
                } else {
                    let [year, month] = dateFrom.split("-");
                    dateFrom = new Date(year, month - 1, 1).toISOString().split('T')[0];
                    [year, month] = dateTo.split("-");
                    dateTo = new Date(year, month, 0).toISOString().split('T')[0];
                }

                if (usoInm == "Residencial" || usoInm == "Comercial") {
                    usoInm = new String(usoInm.toLowerCase());
                } else {
                    usoInm = new String("");
                }

                d.startDate = dateFrom;
                d.endDate = dateTo;
                d.usoInmueble = usoInm;
                console.log("Data", d);
            }
        },
        order: [],
        columns: [
            { title: "", data: "idInmueble", responsivePriority: 1, searchable: false, orderable: false },
            {
                title: "Tipo", data: null, render: function (data, type, row) {
                    console.log(data);
                    return data.tipo.tipo;
                }, name: "tipo", responsivePriority: 5
            },
            {
                title: "Ubicación", data: null, render: function (data, type, row) {
                    return `<a href="https://www.google.com/maps?q=${data.latitud},${data.longitud}&z=15&markers=${data.latitud},${data.longitud}" target="_blank"><i class="bi bi-geo-alt-fill"></i> ${data.direccion}</a>`;
                }, name: "direccion", responsivePriority: 6
            },
            {
                title: "Dueño", data: null, render: function (data, type, row) {
                    return data.duenio.apellido + " " + data.duenio.nombre
                }, name: "propietario", responsivePriority: 4
            },
            {
                title: "Precio", data: null, render: function (data, type, row) {
                    return new Intl.NumberFormat('es-AR', { style: 'currency', currency: 'ARS', minimumFractionDigits: 2 }).format(data.precio)
                }, name: "precio", responsivePriority: 3
            }]

    })
}

function initDataPickr(toIdInput = "#monthTo", fromIdInput = "#monthFrom") {

    if (toIdInput && fromIdInput) {
        $(fromIdInput)
        .prop("min", new Date(new Date().getFullYear(), new Date().getMonth() + 1, 1).toISOString().split('T')[0].slice(0, 7))
        .prop("max", new Date(new Date().getFullYear() + 2, new Date().getMonth(), 1).toISOString().split('T')[0].slice(0, 7))
        .on("change", function () {
            let valueFrom = $(this).val();
            
            if (valueFrom == "") {
                $(toIdInput).prop("min", new Date(new Date().getFullYear(), new Date().getMonth() + 1, 1).toISOString().split('T')[0].slice(0, 7));
                return;
            }
            const [year, month] = valueFrom.split("-");
            valueFrom = new Date(year, month - 1, 1);
            let valueTo = $(toIdInput).val();
            const [year2, month2] = valueTo.split("-");
            valueTo = year2 && month2 ? new Date(year2, month2 - 1, 1) : new Date();
            const today = new Date();
            today.setMonth(today.getMonth()+1) //minDate is + 1 month because next month is min
            today.setDate(1);
            if (valueFrom < today) { //in case of writing in the input directly
                valueFrom = today;
                $(fromIdInput).prop("value", today.toISOString().split('T')[0].slice(0, 7));
            }
            $(toIdInput).prop("min", new Date(valueFrom.getFullYear(), valueFrom.getMonth(), 1).toISOString().split('T')[0].slice(0, 7));
            if (valueTo < valueFrom) {
                $(toIdInput)
                    //.prop("min", new Date(valueFrom.getFullYear(), valueFrom.getMonth(), 1).toISOString().split('T')[0].slice(0, 7))
                    .prop("value", new Date(valueFrom.getFullYear(), valueFrom.getMonth(), 1).toISOString().split('T')[0].slice(0, 7));
            } else {
                $(toIdInput).prop("value", new Date(valueTo.getFullYear(), valueTo.getMonth(), 1).toISOString().split('T')[0].slice(0, 7));
            }
            console.log(valueFrom, valueTo);
        });

    $(toIdInput)
        .prop("min", new Date(new Date().getFullYear(), new Date().getMonth() + 1, 1).toISOString().split('T')[0].slice(0, 7))
        .prop("max", new Date(new Date().getFullYear() + 2, new Date().getMonth(), 1).toISOString().split('T')[0].slice(0, 7))
        .on("change", function () {
            let valueTo = $(this).val();
            if (valueTo == "") {
                return;
            }
            const [year, month] = valueTo.split("-");
            valueTo = new Date(year, month - 1, 1);
            let valueFrom = $(fromIdInput).val();
            const [year2, month2] = valueFrom.split("-");
            valueFrom = year2 && month2 ? new Date(year2, month2 - 1, 1) : new Date();
            const today = new Date();
            today.setMonth(today.getMonth()+2) //minDate is + 1 month because next month is min (and +1 because of setDate(0))
            today.setDate(0);
            if (valueFrom < today) { //in case of writing in the input directly
                valueFrom = today;
                $(toIdInput).prop("value", today.toISOString().split('T')[0].slice(0, 7));
            }
            if (valueTo < valueFrom) {
                $(toIdInput).prop("value", new Date(valueFrom.getFullYear(), valueFrom.getMonth(), 1).toISOString().split('T')[0].slice(0, 7));
            }
        });
    }
}