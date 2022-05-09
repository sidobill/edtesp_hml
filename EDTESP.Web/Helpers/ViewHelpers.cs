using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using EDTESP.Infrastructure.CC.Util;
using EDTESP.Web.ViewModels.Financeiro;
using EDTESP.Web.ViewModels.Operacoes;

namespace EDTESP.Web.Helpers
{
    public static class ViewHelpers
    {
        public static string Content(this UrlHelper helper, string contentpath, bool absUrl)
        {
            var path = helper.Content(contentpath);
            var url = new Uri(HttpContext.Current.Request.Url, path);

            return absUrl ? url.AbsoluteUri : path;
        }

        public static MvcHtmlString EdtespValidationSummary(this HtmlHelper helper, string valMessage, object htmlAttrs)
        {
            if (helper.ViewData.ModelState.IsValid)
                return new MvcHtmlString("");

            var div = new TagBuilder("div");
            div.MergeAttributes(new RouteValueDictionary(htmlAttrs));
            div.AddCssClass("bg-red");
            div.AddCssClass("fg-white");
            div.AddCssClass("p-4");

            if (!string.IsNullOrEmpty(valMessage))
                div.InnerHtml += $"<p>{valMessage}</p>";

            foreach (var key in helper.ViewData.ModelState.Keys)
            {
                foreach (var err in helper.ViewData.ModelState[key].Errors)
                {
                    div.InnerHtml += $"<p>{err.ErrorMessage}</p>";
                }
            }

            return new MvcHtmlString(div.ToString(TagRenderMode.Normal));
        }
        
        public static MvcHtmlString MostraDocumento(this HtmlHelper html, string documento, string tipoPessoa)
        {
            var doc = documento.ClearNumber();

            if(string.IsNullOrEmpty(doc) || (doc.Length != 11 && doc.Length != 14))
                return new MvcHtmlString(documento);

            if(tipoPessoa == "F")
                return new MvcHtmlString($"{doc.Substring(0,3)}.{doc.Substring(3,3)}.{doc.Substring(6,3)}-{doc.Substring(9,2)}");

            return new MvcHtmlString($"{doc.Substring(0,2)}.{doc.Substring(2,3)}.{doc.Substring(5,3)}/{doc.Substring(8,4)}-{doc.Substring(12,2)}");
        }

        public static MvcHtmlString MostraOptsContrato(this HtmlHelper html, ContratoViewModel model)
        {
            var uh = new UrlHelper(html.ViewContext.RequestContext);

            var tb = new TagBuilder("div");
            tb.AddCssClass("toolbar");

            var btnv = new TagBuilder("button");
            btnv.AddCssClass("tool-button");
            btnv.AddCssClass("bg-lightGray");
            btnv.AddCssClass("btnv");
            btnv.Attributes.Add("data-id", model.ContratoId.ToString());
            btnv.Attributes.Add("title","Ver Dados");
            btnv.InnerHtml += "<span class=\"mif-eye\"></span>";
            tb.InnerHtml += btnv;

            var btne = new TagBuilder("a");
            btne.AddCssClass("tool-button");
            btne.AddCssClass("bg-yellow");
            btne.AddCssClass("btnEdit");
            btne.Attributes.Add("href", uh.Action("Editar", "Contratos", new { id = model.ContratoId }));
            btne.Attributes.Add("data-id", model.ContratoId.ToString());
            btne.Attributes.Add("title", "Editar");
            btne.InnerHtml += "<span class=\"mif-pencil\"></span>";

            var btna = new TagBuilder("button");
            btna.AddCssClass("tool-button");
            btna.AddCssClass("bg-darkBlue fg-light");
            btna.AddCssClass("btnAprv");
            btna.Attributes.Add("data-id", model.ContratoId.ToString());
            btna.Attributes.Add("title", "Aprovar");
            btna.InnerHtml += "<span class=\"mif-checkmark\"></span>";

            var btnr = new TagBuilder("button");
            btnr.AddCssClass("tool-button");
            btnr.AddCssClass("bg-crimson fg-light");
            btnr.AddCssClass("btnRepr");
            btnr.Attributes.Add("data-id", model.ContratoId.ToString());
            btnr.Attributes.Add("title", "Reprovar");
            btnr.InnerHtml += "<span class=\"mif-cross\"></span>";

            var btnd = new TagBuilder("button");
            btnd.AddCssClass("tool-button");
            btnd.AddCssClass("secondary");
            btnd.AddCssClass("btnDown");
            btnd.Attributes.Add("data-id", model.ContratoId.ToString());
            btnd.Attributes.Add("title", "Baixar PDF");
            btnd.InnerHtml += "<span class=\"mif-file-download\"></span>";

            var btnm = new TagBuilder("button");
            btnm.AddCssClass("tool-button");
            btnm.AddCssClass("secondary");
            btnm.AddCssClass("btnEmail");
            btnm.Attributes.Add("data-id", model.ContratoId.ToString());
            btnm.Attributes.Add("title", "Enviar p/ E-Mail");
            btnm.InnerHtml += "<span class=\"mif-mail\"></span>";

            var btns = new TagBuilder("button");
            btns.AddCssClass("tool-button");
            btns.AddCssClass("warning");
            btns.AddCssClass("btnSusp");
            btns.Attributes.Add("data-id", model.ContratoId.ToString());
            btns.Attributes.Add("title", "Suspender Contrato");
            btns.InnerHtml += "<span class=\"mif-minus\"></span>";

            var btni = new TagBuilder("button");
            btni.AddCssClass("tool-button");
            btni.AddCssClass("secondary");
            btni.AddCssClass("btnCart");
            btni.Attributes.Add("data-id", model.ContratoId.ToString());
            btni.Attributes.Add("title", "Cartas de Cancelamento");
            btni.InnerHtml += "<span class=\"mif-file-empty\"></span>";

            var btnra = new TagBuilder("button");
            btnra.AddCssClass("tool-button");
            btnra.AddCssClass("bg-darkBlue fg-light");
            btnra.AddCssClass("btnReap");
            btnra.Attributes.Add("data-href", uh.Action("Reaprovar","Contratos", new { id = model.ContratoId }));
            btnra.Attributes.Add("data-id", model.ContratoId.ToString());
            btnra.Attributes.Add("title", "Reaprovar Contrato");
            btnra.InnerHtml += "<span class=\"mif-checkmark\"></span>";

            var btnExc = new TagBuilder("button");
            btnExc.AddCssClass("tool-button");
            btnExc.AddCssClass("bg-red fg-light");
            btnExc.AddCssClass("btnExc");
            btnExc.Attributes.Add("data-href", uh.Action("EmAnalise", "Contratos", new { id = model.ContratoId }));
            btnExc.Attributes.Add("data-id", model.ContratoId.ToString());
            btnExc.Attributes.Add("title", "Excluir");
            btnExc.InnerHtml += "<span class=\"mif-minus\"></span>";

            var btnvs = new TagBuilder("button");
            btnvs.AddCssClass("tool-button");
            btnvs.AddCssClass("success");
            btnvs.AddCssClass("btnVoltarSuspenso");
            btnvs.Attributes.Add("data-href", uh.Action("VoltarSuspenso", "Contratos", new { id = model.ContratoId }));
            btnvs.Attributes.Add("data-id", model.ContratoId.ToString());
            btnvs.Attributes.Add("title", "Voltar para aprovado");
            btnvs.InnerHtml += "<span class=\"mif-checkmark\"></span>";

            var btnEditVend = new TagBuilder("button");
            btnEditVend.AddCssClass("tool-button");
            btnEditVend.AddCssClass("bg-yellow");
            btnEditVend.AddCssClass("btnEditarDados");
            btnEditVend.Attributes.Add("data-id", model.ContratoId.ToString());
            btnEditVend.Attributes.Add("title", "Editar");
            btnEditVend.InnerHtml += "<span class=\"mif-pencil\"></span>";

            var btnEditParc = new TagBuilder("button");
            btnEditParc.AddCssClass("tool-button");
            btnEditParc.AddCssClass("bg-purple");
            btnEditParc.AddCssClass("btnEditarParcela");
            btnEditParc.Attributes.Add("data-id", model.ContratoId.ToString());
            btnEditParc.Attributes.Add("title", "Editar forma de pagamento títulos");
            btnEditParc.InnerHtml += "<span class=\"mif-pencil\"></span>";

            if (model.StatusContratoId == (int) EnumStatusContrato.EmAnalise)
            {   
                tb.InnerHtml += btne;
                tb.InnerHtml += btna;
                tb.InnerHtml += btnr;
            }

            if (model.StatusContratoId == (int) EnumStatusContrato.Aprovado)
            {
                if (model.PermiteDownload)
                    tb.InnerHtml += btnd;

                if (model.PermiteEnviarEmail)
                    tb.InnerHtml += btnm;

                tb.InnerHtml += btns;
                tb.InnerHtml += btnExc;
                tb.InnerHtml += btnEditVend;
                tb.InnerHtml += btnEditParc;
            }

            if (model.StatusContratoId == (int) EnumStatusContrato.Suspenso)
            {
                //tb.InnerHtml += btnra;
                tb.InnerHtml += btnvs;
            }

            if (model.PermiteCartaCancelamento)
                tb.InnerHtml += btni;

            return new MvcHtmlString(tb.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString MostraOptsTitulo(this HtmlHelper html, TituloViewModel titulo)
        {
            var tb = new TagBuilder("div");
            tb.AddCssClass("toolbar");

            var btnv = new TagBuilder("button");
            btnv.AddCssClass("tool-button");
            btnv.AddCssClass("bg-lightGray");
            btnv.AddCssClass("btnv");
            btnv.Attributes.Add("data-id", titulo.TituloId.ToString());
            btnv.Attributes.Add("title", "Ver Dados");
            btnv.InnerHtml += "<span class=\"mif-eye\"></span>";
            tb.InnerHtml += btnv;

            var btna = new TagBuilder("button");
            btna.AddCssClass("tool-button");
            btna.AddCssClass("dark");
            btna.AddCssClass("btnAVenc");
            btna.Attributes.Add("data-id", titulo.TituloId.ToString());
            btna.Attributes.Add("data-vencto", titulo.DataVenctoReal.ToString("dd/MM/yyyy"));
            btna.Attributes.Add("data-valor", titulo.Valor.ToString());
            btna.Attributes.Add("title", "Alterar Vencto.");
            btna.InnerHtml += "<span class=\"mif-calendar\"></span>";

            var btnc = new TagBuilder("button");
            btnc.AddCssClass("tool-button");
            btnc.AddCssClass("alert");
            btnc.AddCssClass("btnCancBol");
            btnc.Attributes.Add("data-id", titulo.UltimoBoleto?.BoletoId.ToString());
            btnc.Attributes.Add("data-tit", titulo.TituloId.ToString());
            btnc.Attributes.Add("title", "Cancelar Boleto");
            btnc.InnerHtml += "<span class=\"mif-cross\"></span>";

            var btnd = new TagBuilder("button");
            btnd.AddCssClass("tool-button");
            btnd.AddCssClass("secondary");
            btnd.AddCssClass("btnDown");
            btnd.Attributes.Add("data-id", titulo.UltimoBoleto?.BoletoId.ToString());
            btnd.Attributes.Add("title", "Baixar PDF");
            btnd.InnerHtml += "<span class=\"mif-file-download\"></span>";

            var btnm = new TagBuilder("button");
            btnm.AddCssClass("tool-button");
            btnm.AddCssClass("secondary");
            btnm.AddCssClass("btnEmail");
            btnm.Attributes.Add("data-id", titulo.UltimoBoleto?.BoletoId.ToString());
            btnm.Attributes.Add("title", "Enviar p/ E-Mail");
            btnm.InnerHtml += "<span class=\"mif-mail\"></span>";

            var btnb = new TagBuilder("button");
            btnb.AddCssClass("tool-button");
            btnb.AddCssClass("yellow");
            btnb.AddCssClass("btnBaixa");
            btnb.Attributes.Add("data-id", titulo.TituloId.ToString());
            btnb.Attributes.Add("title", "Marcar c/ Pago");
            btnb.InnerHtml += "<span class=\"mif-checkmark\"></span>";

            var btng = new TagBuilder("button");
            btng.AddCssClass("tool-button");
            btng.AddCssClass("success");
            btng.AddCssClass("btnGerar");
            btng.Attributes.Add("data-id", titulo.TituloId.ToString());
            btng.Attributes.Add("title", "Gerar Boleto");
            btng.InnerHtml += "<span class=\"mif-plus\"></span>";

            var nbtnb = new TagBuilder("button");
            nbtnb.AddCssClass("tool-button");
            nbtnb.AddCssClass("alert");
            nbtnb.AddCssClass("btnNaoPago");
            nbtnb.Attributes.Add("data-id", titulo.TituloId.ToString());
            nbtnb.Attributes.Add("title", "Marcar c/ não Pago");
            nbtnb.InnerHtml += "<span class=\"mif-checkmark\"></span>";

            if (titulo.EmAberto)
            {
                if (!titulo.GeraBoleto || (titulo.PossuiBoleto && !titulo.UltimoBoleto.EnviadoAoBanco))
                {
                    tb.InnerHtml += btna;
                }


                if (titulo.GeraBoleto && !titulo.PossuiBoleto)
                {
                    tb.InnerHtml += btng;
                    //tb.InnerHtml += btng;
                }

                if (titulo.GeraBoleto && titulo.PossuiBoletoGerado)
                {
                    tb.InnerHtml += btnd;
                    tb.InnerHtml += btnm;
                }

                tb.InnerHtml += btnc;

                if (titulo.PermissaoParaMarcarComoPago)
                    tb.InnerHtml += btnb;
            }
            else
            {
                if (!titulo.Suspenso && titulo.PermissaoParaMarcarComoPago && titulo.DataBaixa.HasValue && !titulo.DataCancelamento.HasValue)
                    tb.InnerHtml += nbtnb;
            }

            return new MvcHtmlString(tb.ToString(TagRenderMode.Normal));
        }
    }
}