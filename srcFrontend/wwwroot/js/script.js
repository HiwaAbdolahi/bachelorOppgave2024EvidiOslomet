//Camera functions

async function startCamera() {
    const video = document.getElementById('video');
    try {
        video.srcObject = await navigator.mediaDevices.getUserMedia({ video: true });
    } catch (err) {
        console.error('Error accessing camera:', err);
    }
}

async function captureImage() {
    const video = document.getElementById('video');
    const canvas = document.createElement('canvas');
    canvas.width = video.videoWidth;
    canvas.height = video.videoHeight;
    canvas.getContext('2d').drawImage(video, 0, 0, canvas.width, canvas.height);
    const imageData = canvas.toDataURL('image/jpeg');
    var base64ImageData = imageData.split(",")[1];
    await callRecognition(base64ImageData);
}

async function captureImageExit(){
    const video = document.getElementById('video');
    const canvas = document.createElement('canvas');
    canvas.width = video.videoWidth;
    canvas.height = video.videoHeight;
    canvas.getContext('2d').drawImage(video, 0, 0, canvas.width, canvas.height);
    const imageData = canvas.toDataURL('image/jpeg');
    var base64ImageData = imageData.split(",")[1];
    await callRecognitionExit(base64ImageData);
}


function displayResult(message) {
    $('#resultDiv').html(`<div class="mx-auto border border-dark alert alert-info fw-bold resultDiv" role="alert"><p class="fw-bold">${message}</p></div>`);
}

function displayResultExit(message) {
    $('#resultDivExit').html(`<div class="mx-auto border border-dark alert alert-info fw-bold resultDiv" role="alert"><p class="fw-bold">${message}</p></div>`);
}

if (window.location.pathname === '/') {
    window.onload = startCamera;
    setInterval(captureImage, 4000);
}
else if(window.location.pathname === '/Exit'){
    window.onload = startCamera;
    setInterval(captureImageExit, 4000);
}

//Infotable functions

function searchEmployees() {

    var searchText = document.getElementById("searchInput").value.toLowerCase();
    filterPartialView("partial_work", searchText);
    filterPartialView("partial_remote", searchText);
}

function filterPartialView(partialViewId, searchText) {
    var partialView = document.getElementById(partialViewId);
    var employees = partialView.getElementsByClassName("employee-name");

    for (var i = 0; i < employees.length; i++) {

        var employeeName = employees[i].innerText.toLowerCase();

        if (employeeName.includes(searchText)) {
            employees[i].closest('.cardContainer').style.display = "";
        } else {
            employees[i].closest('.cardContainer').style.display = "none";
        }
    }
}
