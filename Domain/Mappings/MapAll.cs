using AutoMapper;

namespace Maxima.Cliente.Omie.Domain.Mappings
{
    public partial class MapAll : Profile
    {

        public MapAll()
        {
            BancoMapping();
            CidadeMapping();
            ClienteMapping();
            DepartamentoMapping();
            FilialMapping();
            FornecedorMapping();
            EstoqueMapping();
            ProdutoMapping();
            PesquisaTituloMapping();
            VendedorMapping();
            TipoAtividadeMapping();
            TransportadoraMapping();
            PedidoMapping();
            FamiliaMapping();
            HistoricoPedidoMapping();
            TabelaDePrecos();
            FormaDePagamentoMapping();
            MeiosDePagamentoOmieMapping();
            PrecoProdutoOmieMapping();
            ClienteMaximaMap();
        }
    }
}