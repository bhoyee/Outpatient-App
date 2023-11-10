// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


var selectedMonth = null;
var selectedDay = null;
var enteredYear = "";

function selectMonth(month, daysInMonth) {
    selectedMonth = month;
    $('.month-button').removeClass('selected');
    $(event.target).addClass('selected');

    // Move to the next step (Day Selection)
    $('.month-selection').hide();
    $('.day-selection').show();
    $('.back-button').show();

    // Populate days for the selected month
    var daysContainer = $('#daysContainer');
    daysContainer.empty();
    for (var day = 1; day <= daysInMonth; day++) {
        daysContainer.append('<button id="dayButton' + day + '" class="btn btn-info day-button m-2" onclick="selectDay(' + day + ')" onmouseover="hoverDay(' + day + ')" onmouseout="unhoverDay(' + day + ')">' + day + '</button>');
    }
}

function selectDay(day) {
    selectedDay = day;
    $('.day-button').removeClass('selected');
    $(event.target).addClass('selected');

    // Move to the next step (Year of Birth Input)
    $('.day-selection').hide();
    $('.year-input').show();
    $('.back-button').show();
}

function hoverDay(day) {
    $('#dayButton' + day).addClass('hovered');
}

function unhoverDay(day) {
    $('#dayButton' + day).removeClass('hovered');
}


function addToYear(number) {
    if (document.getElementById("yearInput").value.length < 4) {
        document.getElementById("yearInput").value += number;
        console.log("Year Input:", document.getElementById("yearInput").value);
        if (document.getElementById("yearInput").value.length == 4) {
            enteredYear = document.getElementById("yearInput").value; // Update enteredYear
            console.log("Year of birth entered, calling selectSurname");
            selectSurname(); // Call selectSurname without passing any arguments
        }
    }
}



function clearYearInput() {
    document.getElementById("yearInput").value = "";
    enteredYear = "";
}


function selectSurname() {
    console.log("Selecting surname...");

    // Hide the current step (Year of Birth Input)
    $('.year-input').hide();

    // Show the next step (Select the First Letter of Your Surname)
    $('.surname-selection').show();
    startTimer();
}

function selectSurnameLetter(letter) {
    console.log("Selected letter:", letter);

    // Display the selected letter in the text box
    document.getElementById("surnameInput").value = letter;

    // Move to the next step (Postcode Entry)
    $('.surname-selection').hide();
    $('.postcode-entry').show();
    startTimer();
}


function enterLetter(letter) {
    var postcodeInput = document.getElementById("postcodeInput");
    if (postcodeInput.value.length < 6) {
        postcodeInput.value += letter;

        if (postcodeInput.value.length == 6) {
            showSummary();
            startTimer();
        }
    }


}

function enterDigit(digit) {
    var postcodeInput = document.getElementById("postcodeInput");
    if (postcodeInput.value.length < 6) {
        postcodeInput.value += digit;
    }
}

function clearPostcodeInput() {
    document.getElementById("postcodeInput").value = "";
}

function showSummary() {
    var selectedMonthString = selectedMonth < 10 ? "0" + selectedMonth : selectedMonth.toString();
    var selectedDayString = selectedDay < 10 ? "0" + selectedDay : selectedDay.toString();
    var enteredYearString = enteredYear.toString();

    // Create a formatted date string in the 'dd/mm/yyyy' format
    var formattedDate = selectedDayString + "/" + selectedMonthString + "/" + enteredYearString.substring(0, enteredYearString.length - 2) + "**";

    var surname = document.getElementById("surnameInput").value;
    var postcode = document.getElementById("postcodeInput").value;

    // Hide the last two characters in the year and postcode with '**'
    var hiddenYear = enteredYearString.substring(0, enteredYearString.length - 2) + "**";
    var hiddenPostcode = postcode.substring(0, postcode.length - 2) + "**";

    document.getElementById("summaryDateOfBirth").textContent = formattedDate;
    document.getElementById("summarySurname").textContent = surname;
    document.getElementById("summaryPostcode").textContent = hiddenPostcode;

    // Hide the previous step and show the summary step
    $('.postcode-entry').hide();
    $('.appointment-summary').show();
    startTimer();
}



function goBack() {
    if ($('.appointment-summary').is(':visible')) { // If currently on Appointment Summary
        $('.appointment-summary').hide();
        $('.postcode-entry').show();
        $('.back-button').show();
    } else if ($('.postcode-entry').is(':visible')) {
        $('.postcode-entry').hide();
        $('.surname-selection').show();
    } else if ($('.surname-selection').is(':visible')) {
        $('.surname-selection').hide();
        $('.year-input').show();
    } else if ($('.year-input').is(':visible')) {
        $('.year-input').hide();
        $('.day-selection').show();
    } else if ($('.day-selection').is(':visible')) {
        $('.day-selection').hide();
        $('.month-selection').show();
        $('.back-button').hide();
    }
}

function bookNow() {
    // Show the pre-loading animation
    $('#loading').show();

    var dateOfBirth = selectedMonth + "/" + selectedDay + "/" + enteredYear;
    var surname = document.getElementById("surnameInput").value;
    var postcode = document.getElementById("postcodeInput").value;

    $.post('/Appointment/BookNow', {
        dateOfBirth: dateOfBirth,
        surname: surname,
        postcode: postcode
    })
        .done(function (data) {
            // Hide the pre-loading animation after the response
            $('#loading').hide();

            if (data.success) {
                // Patient found, show success message
                $('.appointment-success').show();
                $('.back-button').hide();
                startTimer();
                $('.appointment-failure').hide();
                $('.appointment-duplicate').hide();
                $('.appointment-summary').hide();

            } else {
                // Patient not found or invalid date format, show failure message
                if (data.message == "Patient already has an appointment on the specified date.") {
                    // Duplicate appointment found, show duplicate message
                    $('.appointment-duplicate').show();
                    $('.back-button').hide();
                    // Start a timer to automatically go back to home after 10 seconds
                    setTimeout(goHome, 10000);
                } else {
                    // Other failure message
                    $('.appointment-failure').show();
                    $('.back-button').hide();
                    setTimeout(goHome, 10000);
                }
                $('.appointment-success').hide();
                $('.appointment-summary').hide();

                // Display the custom error message on the front-end
                $('#errorMessage').text(data.message);
            }
        })
        .fail(function () {
            // Hide the pre-loading animation after the response
            $('#loading').hide();

            // Error occurred, show failure message
            $('.appointment-failure').show();
            $('.back-button').hide();
            setTimeout(goHome, 10000);
            $('.appointment-success').hide();
            $('.appointment-summary').hide();
            $('.appointment-duplicate').hide();

            // Display a generic error message on the front-end
            $('#errorMessage').text('An error occurred while booking the appointment. Please try again later.');
        });
}


function goHome() {
    window.location.href = '/Home/Index'; // Redirect to the home page
}

// Function to handle automatic redirection
function redirectToHome() {
    window.location.href = "/"; // Replace "/" with the actual URL to your home page
}

// Function to start the timer on a specific screen
function startTimer() {
    setTimeout(redirectToHome, 50000); // 15 seconds (15,000 milliseconds)
}
