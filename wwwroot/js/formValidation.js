$(document).ready(function () {


    var fields = {
        name: false,
        last_name: false,
        email: false,
        phone: false,
        dni: false,
        address: false,
        isReady: function () {
            return this.name && this.last_name && this.email && this.phone && this.dni && this.address
        }
    };

    var messages = {
        name_empty: "Por favor, ingrese el nombre del " + entityName,
        name_invalid: "Por favor, ingrese un nombre válido",
        last_name_empty: "Por favor, ingrese el apellido del " + entityName,
        last_name_invalid: "Por favor, ingrese un apellido válido",
        email_empty: "Por favor, ingrese el correo electrónico del " + entityName,
        email_invalid: "Por favor, ingrese un correo electrónico válido",
        phone_empty: "Por favor, ingrese el teléfono del " + entityName,
        phone_invalid: "Por favor, ingrese un teléfono válido, solo números",
        dni_empty: "Por favor, ingrese el DNI del " + entityName,
        dni_invalid: "Por favor, ingrese un DNI válido, solo números",
        address_empty: "Por favor, ingrese la dirección del " + entityName,
    };

    var form = document.getElementById("myForm");
    var nameInput = document.getElementById("name");
    nameInput.focus();
    var lastNameInput = document.getElementById("last-name");
    var emailInput = document.getElementById("email");
    var phoneInput = document.getElementById("phone");
    var dniInput = document.getElementById("dni");
    var addressInput = document.getElementById("address");

    function triggerCheck() {
        var inputEvent = new Event("input", {
            bubbles: true,
            cancelable: true
        });
        nameInput.dispatchEvent(inputEvent);
        lastNameInput.dispatchEvent(inputEvent);
        emailInput.dispatchEvent(inputEvent);
        phoneInput.dispatchEvent(inputEvent);
        dniInput.dispatchEvent(inputEvent);
        addressInput.dispatchEvent(inputEvent);
    }

    form.addEventListener("submit", function (event) {
        event.preventDefault();
        console.log("processing...");
        if (fields.isReady()) {
            form.submit();
        } else {
            triggerCheck();
        }
    });

    //name input validation
    nameInput.addEventListener("input", function () {
        removeCustomErrors("name-feedback", "list-group-item");
        if (isEmpty(this.value)) {
            addErrorItem("name-feedback", messages.name_empty, ["text-danger", "list-group-item"]);
            addRemoveClasses(this, "is-invalid", "is-valid");
            fields.name = false;
        } else {
            if (!isOnlyLetters(this.value)) {
                addErrorItem("name-feedback", messages.name_invalid, ["text-danger", "list-group-item"]);
                addRemoveClasses(this, "is-invalid", "is-valid");
                fields.name = false;
            } else {
                addRemoveClasses(this, "is-valid", "is-invalid");
                fields.name = true;
            }
        }
    });

    //last name input validation
    lastNameInput.addEventListener("input", function () {
        removeCustomErrors("last-name-feedback", "list-group-item");
        if (isEmpty(this.value)) {
            addErrorItem("last-name-feedback", messages.last_name_empty, ["text-danger", "list-group-item"]);
            addRemoveClasses(this, "is-invalid", "is-valid");
            fields.last_name = false;
        } else {
            if (!isOnlyLetters(this.value)) {
                addErrorItem("last-name-feedback", messages.last_name_invalid, ["text-danger", "list-group-item"]);
                addRemoveClasses(this, "is-invalid", "is-valid");
                fields.last_name = false;
            } else {
                addRemoveClasses(this, "is-valid", "is-invalid");
                fields.last_name = true;
            }
        }
    });

    //email input validation
    emailInput.addEventListener("input", function () {
        removeCustomErrors("email-feedback", "list-group-item");
        if (isEmpty(this.value)) {
            addErrorItem("email-feedback", messages.email_empty, ["text-danger", "list-group-item"]);
            addRemoveClasses(this, "is-invalid", "is-valid");
            fields.email = false;
        } else {
            if (!isEmail(this.value)) {
                addErrorItem("email-feedback", messages.email_invalid, ["text-danger", "list-group-item"]);
                addRemoveClasses(this, "is-invalid", "is-valid");
                fields.email = false;
            } else {
                addRemoveClasses(this, "is-valid", "is-invalid");
                fields.email = true;
            }
        }
    });

    //phone input validation
    phoneInput.addEventListener("input", function () {
        removeCustomErrors("phone-feedback", "list-group-item");
        if (isEmpty(this.value)) {
            addErrorItem("phone-feedback", messages.phone_empty, ["text-danger", "list-group-item"]);
            addRemoveClasses(this, "is-invalid", "is-valid");
            fields.phone = false;
        } else {
            if (!isNumber(this.value)) {
                addErrorItem("phone-feedback", messages.phone_invalid, ["text-danger", "list-group-item"]);
                addRemoveClasses(this, "is-invalid", "is-valid");
                fields.phone = false;
            } else {
                addRemoveClasses(this, "is-valid", "is-invalid");
                fields.phone = true;
            }
        }
    });

    //dni input validation
    dniInput.addEventListener("input", function () {
        removeCustomErrors("dni-feedback", "list-group-item");
        if (isEmpty(this.value)) {
            addErrorItem("dni-feedback", messages.dni_empty, ["text-danger", "list-group-item"]);
            addRemoveClasses(this, "is-invalid", "is-valid");
            fields.dni = false;
        } else {
            if (!isNumber(this.value)) {
                addErrorItem("dni-feedback", messages.dni_invalid, ["text-danger", "list-group-item"]);
                addRemoveClasses(this, "is-invalid", "is-valid");
                fields.dni = false;
            } else {
                addRemoveClasses(this, "is-valid", "is-invalid");
                fields.dni = true;
            }
        }
    });

    //address input validation
    addressInput.addEventListener("input", function () {
        removeCustomErrors("address-feedback", "list-group-item");
        if (isEmpty(this.value)) {
            addErrorItem("address-feedback", messages.address_empty, ["text-danger", "list-group-item"]);
            addRemoveClasses(this, "is-invalid", "is-valid");
            fields.address = false;
        } else {
            addRemoveClasses(this, "is-valid", "is-invalid");
            fields.address = true;
        }
    });

    if (nameInput.value.trim() !== "" && lastNameInput.value.trim() !== "") { //si hay algo en el input(lo que quiere decir que se cargo la vista en modo update!) se hace una validacion "triggered manually"
        triggerCheck();
        console.log("checked");
    }
})

