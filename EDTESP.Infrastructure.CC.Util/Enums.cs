namespace EDTESP.Infrastructure.CC.Util
{
    public enum EnumStatus
    {
        Inativo = 0,
        Ativo = 1
    }

    public enum EnumStatusContrato
    {
        EmAnalise = 0,
        Aprovado = 1,
        Reprovado = 2,
        Cancelado = 3,
        Suspenso = 4,
        Encerrado = 5
    }

    public enum EnumTipoBaixa
    {
        Manual = 1,
        Automatica = 2,
        Cnab = 3
    }

    public struct TipoPessoa
    {
        public const string Juridica = "J";
        public const string Fisica = "F";
    }

    public struct ModalidadeGeracaoBoleto
    {
        public const string Api = "A";
        public const string Cnab = "C";
    }
}