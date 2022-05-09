function showInfo(msgTitle, message, okCallback) {
    Metro.dialog.create({
        title: msgTitle,
        clsTitle: 'bg-cobalt fg-white',
        content: message,
        actions: [
            {
                caption: 'Ok',
                cls: 'success js-dialog-close',
                onclick: okCallback
            }
        ]
    });
}

function showActivity() {
    Metro.dialog.open('#dlgAct');
}

function closeActivity() {
    Metro.dialog.close('#dlgAct');
}

function showError(msgTitle, message) {
    Metro.dialog.create({
        title: msgTitle,
        clsTitle: 'bg-red fg-white',
        content: message,
        actions: [
            {
                caption: 'Ok',
                cls: 'alert js-dialog-close'
            }
        ]
    });
}

function showConfirm(urlajax, iditem) {
    Metro.dialog.create({
        title: 'Confirma a acão?',
        clsTitle: 'bg-red fg-white',
        content: 'Tem certeza que deseja remover?',
        actions: [
            {
                caption: 'Sim',
                cls: 'alert js-dialog-close',
                onclick: function() {
                    $.ajax({
                        url: urlajax,
                        data: { id: iditem },
                        type: 'POST',
                        dataType: 'JSON',
                        success: function (data) {
                            if (!data.error) {
                                window.location.reload();
                            } else {
                                showError('Erro', data.message);
                            }
                        },
                        error: function (x, s, e) {
                            showError('Erro', 'Erro ao executar operação');
                            console.log(e);
                        }
                    });
                }
            },
            {
                caption: 'Não',
                cls: 'success js-dialog-close'
            }
        ]
    });
}

function showDialog(urlajax, iditem, title, clsd) {
    $.ajax({
        url: urlajax,
        data: { id: iditem },
        type: 'GET',
        success: function (html) {
            Metro.dialog.create({
                removeOnClose: true,
                title: title,
                clsTitle: 'bg-gray',
                clsDialog: clsd,
                clsContent: 'overflow',
                content: html,
                actions: [
                    {
                        caption: 'Ok',
                        cls: 'alert js-dialog-close'
                    }
                ]
            });
        },
        error: function (x, s, e) {
            showError('Erro', 'Erro ao executar operação');
            console.log(e);
        }
    });

    
}

function showNotificationInfo(message, title, interval) {
    var tt = title == undefined ? 'Sucesso' : title;
    var int = interval == undefined ? 5000 : interval;

    Metro.notify.create(message, tt,
        {
            cls: 'info',
            timeout: int
        });
}

function clearNumber(number) {
    return number.replace(/\.|\-/g, '');
}

function SelecionaRibbonTab(tab) {
    $("nav[data-role='ribbonmenu'] .tabs-holder li").removeClass("active");

    var litb = $("nav[data-role='ribbonmenu'] .tabs-holder li[data-rb-tab='"+tab+"']");

    if (litb.length == 0)
        litb = $("nav[data-role='ribbonmenu'] .tabs-holder li:not(.static)").first();

    litb.addClass("active");
    var stb = litb.find("a").attr("href");

    $("nav[data-role='ribbonmenu'] .content-holder .section").removeClass("active");
    $("nav[data-role='ribbonmenu'] .content-holder").find(stb).addClass("active");
}

function jsonDate2Date(strdate) {
    return new Date(parseInt(strdate.substr(6)));
}