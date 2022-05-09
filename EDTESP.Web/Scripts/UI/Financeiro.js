var urlVerTitulo;
var urlDonwloadBol;
var urlEnvioEmail;
var urlAlterarVencto;
var urlCancBol;
var urlBaixaTit;
var urlGerBol;
var urlDownloadBolVarios;
var urlMarcarComoNaoPago;

$(function () {
    $(".btnv").on('click', verTitulo);
    $(".btnDown").on('click', downloadBoleto);
    $(".btnEmail").on('click', enviarPorEmail);
    $(".btnAVenc").on('click', alterarVencto);
    $(".btnCancBol").on('click', cancelarBoleto);
    $(".btnBaixa").on('click', baixarTitulo);
    $(".btnGerar").on('click', gerarBoleto);
    $(".btnNaoPago").on('click', alterarNaoPago);

    $('.input-daterange').datepicker({
        language: 'pt-BR',
        autoclose: true
    });

    $("#dtIni, #dtFim").mask("99/99/9999");
});

function verTitulo() {
    var cid = $(this).data('id');
    showDialog(urlVerTitulo, cid, 'Dados do Título', 'w-50');
}

function downloadBoleto() {
    var cid = $(this).data('id');
    $.fileDownload(urlDonwloadBol + '?boletoId=' + cid,{});
}

function enviarPorEmail() {
    var cid = $(this).data('id');

    $.ajax({
        url: urlEnvioEmail,
        data: { boletoId: cid },
        dataType: 'JSON',
        beforeSend: function () {
            showActivity();
        },
        success: function (data) {
            if (data.error) {
                showError('Erro', 'Erro ao enviar o e-mail');
                console.log(data.msg);
                return;
            }

            closeActivity();
            showNotificationInfo('E-mail enviado com sucesso!');
        },
        error: function () {
            showError('Erro', 'Erro ao enviar o e-mail');
        }
    });
}

function alterarVencto() {
    var cid = $(this).data('id');
    var vc = $(this).data('vencto');
    var valorTotal = $(this).data('valor');

    var bd = '<input type="text" id="txtVencto" data-role="calendarpicker" value="' + vc + '" data-format="%d/%m/%Y" data-input-format="%d/%m/%Y" data-cls-calendar="compact" />';
    bd += '<input type="text" id="txtValorTit" value="' + valorTotal + '"/>';

    Metro.dialog.create({
        title: 'Alterar Vencto',
        content: bd,
        actions: [
            {
                caption: 'Salvar',
                cls: 'js-dialog-close primary',
                onclick: function () {
                    var nv = $("#txtVencto").val();
                    var valorParametro = $("#txtValorTit").val();

                    $.ajax({
                        url: urlAlterarVencto,
                        data: { id: cid, novoVencto: nv, valor: valorParametro},
                        dataType: 'JSON',
                        type: 'POST',
                        beforeSend: function() {
                            showActivity();
                        },
                        success: function (data) {
                            closeActivity();
                            if (data.error) {
                                showError('Erro', data.msg);
                                return;
                            }

                            document.forms[0].submit();
                        },
                        error: function() {
                            closeActivity();
                            showError('Erro', 'Erro ao executar a operação');
                        }
                    });
                }
            },
            {
                caption: 'Cancelar',
                cls: 'js-dialog-close'
            }
        ]
    });
}

function cancelarBoleto() {
    var cid = $(this).data('id');
    var titulo = $(this).data('tit');
    
    var bd = '<h3>ATENÇÃO</h3>'
        + '<h5>O Cancelamento no sistema não impede o pagamento do mesmo pelo cliente</h5>'
        + '<h5>Para boletos gerados via CNAB, deve-se cancelar também o boleto no Banco</h5>'
        + '<p>Deseja realmente cancelar o boleto?</p>';

    if (cid != "") {
        bd += '<input type="checkbox" id="chkNovoBoleto"><label>Gerar novo boleto</label>';
    }

    Metro.dialog.create({
        title: 'Cancelar Boleto',
        content: bd,
        clsDialog: 'bg-red fg-light w-50',
        clsAction: 'bg-white fg-dark',
        actions: [
            {
                caption: 'Confirmar',
                cls: 'js-dialog-close alert',
                onclick: function () {
                    var marcado = false;
                    var semBoleto = false;

                    if (cid != "") {
                        marcado = $("#chkNovoBoleto").is(":checked");
                    }
                    else {
                        cid = titulo;
                        semBoleto = true;
                    }

                    $.ajax({
                        url: urlCancBol,
                        data: { id: cid, novoBoleto: marcado, semBoleto: semBoleto},
                        dataType: 'JSON',
                        type: 'POST',
                        beforeSend: function () {
                            showActivity();
                        },
                        success: function (data) {
                            closeActivity();
                            if (data.error) {
                                showError('Erro', data.msg);
                                return;
                            }

                            document.forms[0].submit();
                        },
                        error: function () {
                            closeActivity();
                            showError('Erro', 'Erro ao executar a operação');
                        }
                    });
                }
            },
            {
                caption: 'Cancelar',
                cls: 'js-dialog-close success'
            }
        ]
    });
}

function baixarTitulo() {
    var cid = $(this).data('id');

    var bd = '<p>Deseja realmente marcar este titulo como pago?</p>'
        + '<label for="txtVencto">Data da Baixa</label>'
        + '<input type="text" id="txtVencto" data-role="calendarpicker" value="' + new Date().toLocaleDateString() +'" data-format="%d/%m/%Y" data-input-format="%d/%m/%Y" />';

    Metro.dialog.create({
        title: 'Baixar Título',
        content: bd,
        clsTitle: 'bg-yellow fg-dark',
        clsAction: 'bg-white fg-dark',
        actions: [
            {
                caption: 'Confirmar',
                cls: 'js-dialog-close alert',
                onclick: function () {
                    var nv = $("#txtVencto").val();
                    
                    $.ajax({
                        url: urlBaixaTit,
                        data: { id: cid, dataBaixa: nv },
                        dataType: 'JSON',
                        type: 'POST',
                        beforeSend: function () {
                            showActivity();
                        },
                        success: function (data) {
                            closeActivity();
                            if (data.error) {
                                showError('Erro', data.msg);
                                return;
                            }

                            document.forms[0].submit();
                        },
                        error: function () {
                            closeActivity();
                            showError('Erro', 'Erro ao executar a operação');
                        }
                    });
                }
            },
            {
                caption: 'Cancelar',
                cls: 'js-dialog-close success'
            }
        ]
    });
}

function alterarNaoPago() {
    var cid = $(this).data('id');

    var bd = '<p>Deseja realmente cancelar esse pagamento?</p>';

    Metro.dialog.create({
        title: 'Marcar como não pago',
        content: bd,
        clsTitle: 'bg-yellow fg-dark',
        clsAction: 'bg-white fg-dark',
        actions: [
            {
                caption: 'Confirmar',
                cls: 'js-dialog-close alert',
                onclick: function () {
                    $.ajax({
                        url: urlMarcarComoNaoPago,
                        data: { id: cid },
                        dataType: 'JSON',
                        type: 'POST',
                        beforeSend: function () {
                            showActivity();
                        },
                        success: function (data) {
                            closeActivity();
                            if (data.error) {
                                showError('Erro', data.msg);
                                return;
                            }

                            document.forms[0].submit();
                        },
                        error: function () {
                            closeActivity();
                            showError('Erro', 'Erro ao executar a operação');
                        }
                    });
                }
            },
            {
                caption: 'Cancelar',
                cls: 'js-dialog-close success'
            }
        ]
    });
}

function gerarBoleto() {
    var cid = $(this).data('id');

    $.ajax({
        url: urlGerBol,
        data: { id: cid },
        dataType: 'JSON',
        type: 'POST',
        beforeSend: function () {
            showActivity();
        },
        success: function (data) {
            closeActivity();
            if (data.error) {
                showError('Erro', data.msg);
                return;
            }

            document.forms[0].submit();
        },
        error: function () {
            closeActivity();
            showError('Erro', 'Erro ao executar a operação');
        }
    });
}

function downloadVariosBoletos() {
    var idBoletos = "";

    $("input[name='boletoId']:checked").each(function () {
        idBoletos += $(this).data('id') + ",";
    });

    if (idBoletos == "") {
        showError('Erro', 'Selecione no mínimo um boleto para fazer o dowload');
    }
    else {
        $.fileDownload(urlDownloadBolVarios + '?boletos=' + idBoletos, {});
    }
}