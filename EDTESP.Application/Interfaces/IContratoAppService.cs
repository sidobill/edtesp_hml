using EDTESP.Domain.Entities;

namespace EDTESP.Application.Interfaces
{
    public interface IContratoAppService : IAppServiceBase<Contrato>
    {
        new void Insert(Contrato obj);

        void Aprovar(Contrato obj);

        void Reprovar(Contrato obj);

        void Suspender(Contrato obj, int motivoSuspensaoId, string observacao, int usuario);

        void Cancelar(Contrato obj, string observacaoCanc);

        void Reaprovar(Contrato obj);

        byte[] GerarViaContrato(int contratoId, string pasta = "");

        byte[] GerarCartaCancelamentoModelo1(int contratoId, string pasta = "", string contrato = "");

        byte[] GerarCartaCancelamentoModelo2(int contratoId, string pasta = "", string contrato = "");

        byte[] GerarCartaCancelamentoModelo3(int contratoId, string pasta = "", string contrato = "");

        byte[] GerarCartaCobranca(int contratoId, string pasta = "", int numeroContrato = 0);

        void EnviarContratoEmail(int contratoId, string pasta = "");

        void EnviarCartaCancelamento1(int contratoId, string pasta = "", string contrato = "");

        void EnviarCartaCancelamento2(int contratoId, string pasta = "", string contrato = "");

        void EnviarCartaCancelamento3(int contratoId, string pasta = "", string contrato = "");

        void EnviarCartaCobranca(int contratoId, string pasta = "", int numeroContrato = 0);

        byte[] GerarCartaCancelamentoModelo4(int contratoId, string pasta = "", string contrato = "");

        void EnviarCartaCancelamento4(int contratoId, string pasta = "", string contrato = "");
    }
}