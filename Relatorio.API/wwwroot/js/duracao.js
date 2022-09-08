function inserirDuracao(duracao){
    var input = document.getElementById("duracao");

    if(duracao === "Vendas")
    {
        input.value = 5;
    }
    else if(duracao === "Estoque"){
        input.value = 10;
    }
    else if(duracao === "Faturamento"){
        input.value = 15;
    } else {
        input.value = 0;
    }
}