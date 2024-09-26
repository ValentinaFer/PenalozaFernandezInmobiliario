
function formatPrice( value ) {
    return formattedImport = new Intl.NumberFormat('es-AR', {
        style: 'currency',
        currency: 'ARS'
    }).format(value);
}

function initToast(){
    return Toast = Swal.mixin({
        toast: true,
        position: 'top-end',
        showConfirmButton: false,
        timer: 3000,
        timerProgressBar: true,
        iconColor: 'white',
        customClass: {
            popup: 'colored-toast'
        },
        didOpen: (toast) => {
            toast.addEventListener('mouseenter', Swal.stopTimer)
            toast.addEventListener('mouseleave', Swal.resumeTimer)
        }
    });
}

/**
 * Adds and removes the specified classes.
 * @param {HTMLElement} element - element to modify classes for. 
 * @param {string} classToAdd - class to add to the element.
 * @param {string} classToRemove - class to remove from the element.
 */
function addRemoveClasses(element, classToAdd, classToRemove){
    element.classList.add(classToAdd);
    element.classList.remove(classToRemove);
}

/**
 * Checks if the value is in the range.
 * @param {*} value - value to check. 
 * @param {number} maxLength - maximum length of the value.
 * @param {number} minLength - minimum length of the value.
 * @returns true if the value is in the range, false otherwise.
 */
function isUnderRange(value, maxLength, minLength){
    if (value.length >= minLength && value.length <= maxLength) {
        return true;
    } else {
        return false;
    }
}
/**
 * Checks if the value is only letters.
 * @param {*} value - value to check.
 * @returns true if the value is only letters, false otherwise.
 */
function isOnlyLetters(value){
    if (/^[a-zA-ZÀ-ÖØ-öø-ÿ\s\']+$/.test(value)) {
        return true;
    } else {
        return false;
    }
}

/**
 * Checks if the value is a number.
 * @param {*} value - value to check.
 * @returns true if the value is a number, false otherwise.
 */
function isNumber(value){
    if (!isNaN(value)) {
        return true;
    } else {
        return false;
    }
}

/**
 * Checks if the value is empty. Checks for whitespaces.
 * @param {*} value - value to check. 
 * @returns true if the value is empty, false otherwise.
 */
function isEmpty(value) {
    if (value.trim() === "") {
        return true;
    }
    return false;
}

/**
 * Checks if the value has the correct email format: "xxx@yyy.zzz".
 * @param {*} value - value to check.
 * @returns true if the value has the correct email format, false otherwise.
 */
function isEmail(value){
    if (!/^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$/.test(value)) {
        return false;
    } else {
        return true;
    }
}

/**
 * Adds an error item list to the specified list with the specified classes and message to display, if it doesn't exist already.
 * @param {string} idList - id of the list.
 * @param {string} message - message to display.
 * @param {string[]} itemClasses - classes to add to the item.
 */
function addErrorItem(idList, message, itemClasses/*, itemClassReference*/){
    var list = document.querySelector("#"+idList);
    //var existingItems = list.querySelectorAll("."+itemClassReference);
    // if the item doesn't existf (!(existingItems.length > 0)) {
        var li = document.createElement("li");
        li.classList.add(...itemClasses);
        li.textContent = message;
        list.appendChild(li);
    //}
}

/**
 * Removes the custom errors list items from the specified list based on the specified classes.
 * @param {*} idList - id of the list to remove the items from.
 * @param {*} itemClassToRemove - referenced class to remove items.
 */
function removeCustomErrors(idList, itemClassToRemove){
    var list = document.getElementById(idList);
    var liList = list.querySelectorAll("."+itemClassToRemove);
    liList.forEach(function(li) {
       li.remove(); 
    });
}
