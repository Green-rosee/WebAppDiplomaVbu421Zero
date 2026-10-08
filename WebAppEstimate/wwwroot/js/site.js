// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
window.downloadFileFromBase64 = (fileName, base64Data) => {
    const link = document.createElement("a");

    link.download = fileName;
    link.href =
        "data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64," +
        base64Data;

    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
};