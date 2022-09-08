"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/relatorioHub").build();

connection.on("ReceiveMessage", function (message) {
    console.log(message);
});

connection.start().then(function () {
    console.log("Conectado!");
}).catch(function (err) {
    return console.error(err.toString());
});