using System;
using System.Collections.Generic;
using System.Linq;
using ECC.Servicos.Abstrato;
using ECC.Dados.Repositorio;
using ECC.Dados.Infra;
using ECC.EntidadeAvisos;
using ECC.EntidadeEmail;
using ECC.EntidadePedido;
using ECC.EntidadePessoa;
using ECC.EntidadeUsuario;
using ECC.Servicos.ModelService;
using ECC.EntidadeParametroSistema;


namespace ECC.Servicos
{
    public class CalendarioFeriadoService : ICalendarioFeriadoService
    {

        #region Variaveis
       
        private readonly IEntidadeBaseRep<CalendarioFeriado> _calendarioFeriadoRep;
        private readonly IUtilService _utilService;
      
        #endregion

        #region Construtor
        public CalendarioFeriadoService(
           IEntidadeBaseRep<CalendarioFeriado> calendarioFeriadoRep,
            IUtilService utilService)
        {
           _calendarioFeriadoRep = calendarioFeriadoRep;
            _utilService = utilService;
        }
        #endregion

        #region CalendarioFeriadoService Implementacao

        public RetornoFeriadoMembro VerificaFeriadoMembro(DateTime data, Membro membro, string descEstado = null)
        {
            var estado = _utilService.RemoverAcentos(descEstado.Trim().Replace(" ", "_").ToUpper());
            var dataFeriado = new CalendarioFeriado();
            data = new DateTime(data.Year, data.Month, data.Day);
            var tipoFeriado = new TipoRetornoFeriado();

            dataFeriado = _calendarioFeriadoRep
                   .FirstOrDefault(x => x.DtEvento == data && x.Estado == string.Empty && x.Cidade == string.Empty);

            if (dataFeriado == null)
            {
                if (!string.IsNullOrEmpty(estado))
                {
                    dataFeriado = _calendarioFeriadoRep
                        .FirstOrDefault(x => x.DtEvento == data && x.Estado == estado);

                    if (dataFeriado != null)
                    {
                        var fornecedoresMembro = membro.MembroFornecedores
                         .Where(c => c.Fornecedor.Pessoa.Enderecos.Any(p => p.Estado.DescEstado != descEstado))
                         .Select(x => x.Fornecedor).ToList();

                        if (fornecedoresMembro.Count > 0)
                        {
                            var fornecedorRegiao = new List<Fornecedor>();

                            fornecedorRegiao = fornecedoresMembro
                           .Where(c => c.FornecedorRegiao
                           .Any(x => _utilService.RemoverAcentos(x.Cidade.Estado.DescEstado)
                           .ToUpper().Replace(" ", "_") == dataFeriado.Estado))
                           .ToList();

                            if (fornecedorRegiao.Count == 0)
                            {
                                fornecedorRegiao = fornecedoresMembro
                                    .Where(c => c.FornecedorRegiaoSemanal
                                    .Any(x => _utilService.RemoverAcentos(x.Cidade.Estado.DescEstado)
                                    .ToUpper().Replace(" ", "_") == dataFeriado.Estado))
                                    .ToList();
                            }

                            tipoFeriado = fornecedorRegiao.Count > 0 ?
                                TipoRetornoFeriado.NaoExisteFeriado :
                                TipoRetornoFeriado.Feriado;
                        }
                        else
                        {
                            tipoFeriado = TipoRetornoFeriado.Feriado;
                        }
                    }
                    else
                    {
                        int totalFornecedorSemFeriado = 0;

                        var fornecedoresAtendem = membro.MembroFornecedores
                            .Where(x => x.Fornecedor.FornecedorRegiao.Any(c => c.Cidade.Estado.DescEstado == descEstado))
                            .Select(f => f.Fornecedor).ToList();

                        var fornecedoresAtendemSem = membro.MembroFornecedores
                        .Where(x => x.Fornecedor.FornecedorRegiaoSemanal.Any(c => c.Cidade.Estado.DescEstado == descEstado))
                        .Select(f => f.Fornecedor)
                        .ToList();

                        if (fornecedoresAtendemSem.Count > 0)
                        {
                            fornecedoresAtendem.AddRange(fornecedoresAtendemSem);
                        }

                        if (fornecedoresAtendem.Count > 0)
                        {
                            var estadosFornecedores = fornecedoresAtendem.SelectMany(x => x.Pessoa.Enderecos)
                                .Select(c => _utilService.RemoverAcentos(c.Estado.DescEstado.ToUpper().Replace(" ", "_")))
                                .Distinct()
                                .ToList();

                            dataFeriado = _calendarioFeriadoRep
                                .FirstOrDefault(x => x.DtEvento == data && estadosFornecedores.Contains(x.Estado));

                            if (dataFeriado != null)
                            {
                                totalFornecedorSemFeriado =
                                    estadosFornecedores.Where(x => !dataFeriado.Estado.Equals(x)).ToList().Count;
                            }

                        }

                        if (totalFornecedorSemFeriado > 0)
                        {
                            tipoFeriado = TipoRetornoFeriado.NaoExisteFeriado;

                        }else if (totalFornecedorSemFeriado == 0 && dataFeriado == null)
                        {
                            tipoFeriado = TipoRetornoFeriado.NaoExisteFeriado;
                        }
                        else
                        {
                            tipoFeriado = TipoRetornoFeriado.FeriadoFornecedores;
                        }

                    }

                }
                else
                {
                    dataFeriado = _calendarioFeriadoRep
                      .FirstOrDefault(x => x.DtEvento == data);

                    tipoFeriado = TipoRetornoFeriado.Feriado;
                }
            }
            else
            {
                tipoFeriado = TipoRetornoFeriado.FeriadoNacional;
            }

            var retornoFeriadoMembro = new RetornoFeriadoMembro
            {
                CalendarioFeriado = dataFeriado,
                TipoRetornoFeriado = tipoFeriado
            };

            return retornoFeriadoMembro;
        }
        
        #endregion

    }

    public class RetornoFeriadoMembro
    {
        public TipoRetornoFeriado TipoRetornoFeriado { get; set; }

        public CalendarioFeriado CalendarioFeriado { get; set; }
    }


    public enum TipoRetornoFeriado
    {
        NaoExisteFeriado = 0,
        FeriadoNacional = 1,
        FeriadoFornecedores = 2,
        Feriado = 3

    }
}
