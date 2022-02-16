using System;
using System.Linq;
using System.Threading;
using Abstractions;
using Hangfire;
using Maxima.Cliente.Omie.Data;
using Maxima.Cliente.Omie.Domain.Entidades;
using Maxima.Cliente.Omie.Domain.Enums;
using Maxima.Cliente.Omie.Domain.Interfaces;
using Maxima.Cliente.Omie.Domain.Interfaces.Api;

namespace Maxima.Cliente.Omie.Domain.Logic
{
    public class JobsOmie : IJobs
    {
        private readonly IServiceProvider _services;
        private readonly OmieContext dbContext;

        public JobsOmie(OmieContext context, IServiceProvider serviceProvider)
        {
            this._services = serviceProvider;
            this.dbContext = context;
        }
        public JobsOmie()
        {

        }
        public void RecuperarJobsERPs()
        {
            RecurringJob.AddOrUpdate<IPedidoEnvioApiOmie>(x => x.ReceberPedidos(CancellationToken.None), RecuperarCronAsync(JobsCronEnumOmie.SincronizarPedidosNuvemAsync), timeZone: TimeZoneInfo.Local);
            RecurringJob.AddOrUpdate<IPedidoStatusApiOmie>(x => x.SincronizarStatusPedidosHistorico(CancellationToken.None), RecuperarCronAsync(JobsCronEnumOmie.SincronizarStatusPedido), timeZone: TimeZoneInfo.Local);
            RecurringJob.AddOrUpdate<IClienteApiOmie>(x => x.EnviarClientes(CancellationToken.None), RecuperarCronAsync(JobsCronEnumOmie.EnviarClientes), timeZone: TimeZoneInfo.Local);
            RecurringJob.AddOrUpdate<IClienteApiOmie>((x) => x.ReceberClientes(CancellationToken.None), RecuperarCronAsync(JobsCronEnumOmie.SincronizarClientes), timeZone: TimeZoneInfo.Local);
            RecurringJob.AddOrUpdate<IProdutoApiOmie>(x => x.EnviarProdutos(CancellationToken.None), RecuperarCronAsync(JobsCronEnumOmie.EnviarProdutos), timeZone: TimeZoneInfo.Local);
            RecurringJob.AddOrUpdate<IPrecoProdutoApiOmie>(x => x.EnviarPrecoProdutos(CancellationToken.None), RecuperarCronAsync(JobsCronEnumOmie.EnviarPrecoProdutos), timeZone: TimeZoneInfo.Local);
            RecurringJob.AddOrUpdate<IEstoqueApiOmie>(x => x.EnviarEstoque(CancellationToken.None), RecuperarCronAsync(JobsCronEnumOmie.EnviarEstoque), timeZone: TimeZoneInfo.Local);
            RecurringJob.AddOrUpdate<IPracaRegiaoApiOmie>(x => x.EnviarPracaRegiao(CancellationToken.None), RecuperarCronAsync(JobsCronEnumOmie.EnviarPracaRegiao), timeZone: TimeZoneInfo.Local);
            RecurringJob.AddOrUpdate<IProdutosPorFornecedorApiOmie>(x => x.EnviarProdutosPorFornecedor(CancellationToken.None), RecuperarCronAsync(JobsCronEnumOmie.EnviarProdutosPorFornecedor), timeZone: TimeZoneInfo.Local);
            RecurringJob.AddOrUpdate<IVendedorApiOmie>(x => x.EnviarVendedores(CancellationToken.None), RecuperarCronAsync(JobsCronEnumOmie.EnviarVendedores), timeZone: TimeZoneInfo.Local);
            RecurringJob.AddOrUpdate<IFornecedorApiOmie>(x => x.EnviarFornecedores(CancellationToken.None), RecuperarCronAsync(JobsCronEnumOmie.EnviarFornecedores), timeZone: TimeZoneInfo.Local);
            RecurringJob.AddOrUpdate<ITransportadoraApiOmie>(x => x.EnviarTransportadoras(CancellationToken.None), RecuperarCronAsync(JobsCronEnumOmie.EnviarTransportadora), timeZone: TimeZoneInfo.Local);
            RecurringJob.AddOrUpdate<IMeiosDePagamentosApiOmie>(x => x.EnviarMeiosPagamento(CancellationToken.None), RecuperarCronAsync(JobsCronEnumOmie.EnviarMeiosPagamento), timeZone: TimeZoneInfo.Local);
            RecurringJob.AddOrUpdate<IFormaPagamentoApiOmie>(x => x.EnviarFormaDePagamento(CancellationToken.None), RecuperarCronAsync(JobsCronEnumOmie.EnviarFormaDePagamento), timeZone: TimeZoneInfo.Local);
            RecurringJob.AddOrUpdate<IBancoApiOmie>((x) => x.EnviarBancos(CancellationToken.None), RecuperarCronAsync(JobsCronEnumOmie.EnviarBancos), timeZone: TimeZoneInfo.Local);
            RecurringJob.AddOrUpdate<ITipoAtividadeApiOmie>(x => x.EnviarTipoAtividade(CancellationToken.None), RecuperarCronAsync(JobsCronEnumOmie.EnviarTipoAtividade), timeZone: TimeZoneInfo.Local);
            RecurringJob.AddOrUpdate<ICidadeApiOmie>(x => x.EnviarCidades(CancellationToken.None), RecuperarCronAsync(JobsCronEnumOmie.EnviarCidade), timeZone: TimeZoneInfo.Local);
            RecurringJob.AddOrUpdate<IFilialApiOmie>(x => x.EnviarFiliais(CancellationToken.None), RecuperarCronAsync(JobsCronEnumOmie.EnviarFiliais), timeZone: TimeZoneInfo.Local);
            RecurringJob.AddOrUpdate<ITituloApiOmie>(x => x.EnviarTitulos(CancellationToken.None, false), RecuperarCronAsync(JobsCronEnumOmie.EnviarTitulos), timeZone: TimeZoneInfo.Local);
            //RecurringJob.AddOrUpdate<IPedidoHistoricoApiOmie>(x => x.HistoricoPedidos(CancellationToken.None), RecuperarCronAsync(JobsCronEnumOmie.SincronizarStatusPedido), timeZone: TimeZoneInfo.Local);
        }

        public string RecuperarCronAsync(JobsCronEnumOmie jobEnum)
        {

            var cron = dbContext.JobsModels
                   .Where(x => x.Nome == jobEnum.ToString())
                   .FirstOrDefault();

            if (cron == null)
                return "0 23 * * *";
            else
                return cron.Valor;
        }

        [DisableConcurrentExecution(600)]
        public void CargaInicial()
        {

            var idPai = BackgroundJob.Enqueue(() => Console.WriteLine("Carga Inicial Inicializada"));
            idPai = BackgroundJob.ContinueJobWith<ICidadeApiOmie>(idPai, (x) => x.EnviarCidades(CancellationToken.None));
            idPai = BackgroundJob.ContinueJobWith<IFilialApiOmie>(idPai, (x) => x.EnviarFiliais(CancellationToken.None));
            idPai = BackgroundJob.ContinueJobWith(idPai, () => this.CargaInicialAfter(idPai));
        }

        public void CargaInicialAfter(string idPai)
        {
            if (dbContext.ControleDadosModels.Where(c => c.Tabela == ControleDadosEnum.FILIAIS).Any())
            {
                idPai = BackgroundJob.ContinueJobWith<ITipoAtividadeApiOmie>(idPai, (x) => x.EnviarTipoAtividade(CancellationToken.None));
                idPai = BackgroundJob.ContinueJobWith<IMeiosDePagamentosApiOmie>(idPai, (x) => x.EnviarMeiosPagamento(CancellationToken.None));
                idPai = BackgroundJob.ContinueJobWith<IFormaPagamentoApiOmie>(idPai, (x) => x.EnviarFormaDePagamento(CancellationToken.None));
                idPai = BackgroundJob.ContinueJobWith<IBancoApiOmie>(idPai, (x) => x.EnviarBancos(CancellationToken.None));
                idPai = BackgroundJob.ContinueJobWith<IVendedorApiOmie>(idPai, (x) => x.EnviarVendedores(CancellationToken.None));
                idPai = BackgroundJob.ContinueJobWith<IProdutosPorFornecedorApiOmie>(idPai, (x) => x.EnviarProdutosPorFornecedor(CancellationToken.None));
                idPai = BackgroundJob.ContinueJobWith<IFornecedorApiOmie>(idPai, (x) => x.EnviarFornecedores(CancellationToken.None));
                idPai = BackgroundJob.ContinueJobWith<IPracaRegiaoApiOmie>(idPai, (x) => x.EnviarPracaRegiao(CancellationToken.None));
                idPai = BackgroundJob.ContinueJobWith<ITransportadoraApiOmie>(idPai, (x) => x.EnviarTransportadoras(CancellationToken.None));
                idPai = BackgroundJob.ContinueJobWith<IClienteApiOmie>(idPai, (x) => x.EnviarClientes(CancellationToken.None));
                idPai = BackgroundJob.ContinueJobWith<IProdutoApiOmie>(idPai, (x) => x.EnviarProdutos(CancellationToken.None));
                // idPai = BackgroundJob.ContinueJobWith<IEstoqueApiOmie>(idPai, (x) => x.ListarEstoque(null));
                // idPai = BackgroundJob.ContinueJobWith<IPrecoProdutoApiOmie>(idPai, (x) => x.ListarPrecoProdutos(null));
                idPai = BackgroundJob.ContinueJobWith<ITituloApiOmie>(idPai, (x) => x.EnviarTitulos(CancellationToken.None, true));
                idPai = BackgroundJob.ContinueJobWith<IPedidoHistoricoApiOmie>(idPai, (x) => x.HistoricoPedidos(CancellationToken.None));
                idPai = BackgroundJob.ContinueJobWith<IConfiguracao>(idPai, (x) => x.FinalizarCargaInicialOmie(null, CancellationToken.None));
            }
            else
            {
                idPai = BackgroundJob.ContinueJobWith<IConfiguracao>(idPai, (x) => x.CargaInicialErro());
            }

        }
    }

}
