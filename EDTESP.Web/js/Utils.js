function formatTitleReport(initialDate, finishDate, reportName, salesman) {

    var titulo = '';

    if (initialDate != "" && finishDate != "") {
        var dadosIni = initialDate.split(' ');
        var dadosFim = finishDate.split(' ');

        titulo = reportName + '                                                                                                                                                                                                                                                                           ' + salesman + '| Período ' + dadosIni[0] + ' à ' + dadosFim[0];
    }

    return titulo;
}