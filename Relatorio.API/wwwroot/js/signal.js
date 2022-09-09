"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/relatorioHub").build();

connection.on("ReceiveMessage", function (id) {
    document.getElementById(`relatorio-item-color-${id}`).className = "btn btn-success disabled";

    document.getElementById(`relatorio-item-status-${id}`).innerHTML = "CONCLUÍDO";
});

connection.start().then(function () {
    console.log("Conectado!");
}).catch(function (err) {
    return console.error(err.toString());
});