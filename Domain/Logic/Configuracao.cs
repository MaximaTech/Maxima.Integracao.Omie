using Abstractions;
using Hangfire;
using Hangfire.Server;
using Maxima.Cliente.Omie.Data;
using Maxima.Cliente.Omie.Data.Models;
using Maxima.Cliente.Omie.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Logic
{
    public class Configuracao : IConfiguracao
    {
        private readonly OmieContext dbContext;


        public Configuracao(OmieContext context)
        {
            this.dbContext = context;
        }

        public async Task<bool> Salvar(ParametroModel config)
        {
            try
            {
                dbContext.Parametros
                    .Add(config);

                return (await dbContext.SaveChangesAsync() >= 1);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> Alterar(ParametroModel config)
        {
            try
            {
                var configBanco = await dbContext.Parametros
                    .FindAsync(config.Id);

                configBanco.Nome = config.Nome;
                configBanco.Valor = config.Valor;


                return await dbContext.SaveChangesAsync() >= 1;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> Excluir(int id)
        {
            try
            {
                var configBanco = await dbContext.Parametros
                  .FindAsync(id);

                if (configBanco != null)
                    dbContext.Parametros
                        .Remove(configBanco);

                return await dbContext.SaveChangesAsync() >= 1;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ParametroModel> Buscar(int id = 0, string nome = "")
        {
            try
            {

                if (id > 0)
                    return await dbContext.Parametros
                        .FindAsync(id);

                if (!string.IsNullOrEmpty(nome))
                    return await dbContext.Parametros
                        .Where(x => x.Nome.Contains(nome))
                        .FirstOrDefaultAsync();

                return null;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task ProximaEtapaConfig()
        {
            try
            {

                var etapa = await dbContext.Parametros
                    .Where(x => x.Nome.Contains(ConstantesEnum.EtapaConfig))
                    .FirstOrDefaultAsync();
                if (etapa == null)
                {
                    ParametroModel etapaModel = new() { Nome = ConstantesEnum.EtapaConfig, Valor = "1" };
                    dbContext.Parametros.Add(etapaModel);
                    await dbContext.SaveChangesAsync();
                }
                else
                {
                    _ = int.TryParse(etapa?.Valor, out var valorEnum);

                    valorEnum++;
                    if (valorEnum > (int)EnumEtapasConfig.Finalizado)
                        return;

                    etapa.Valor = valorEnum.ToString();
                    await dbContext.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string EtapaAtual()
        {
            try
            {
                var etapa = dbContext.Parametros
                    .Where(x => x.Nome.Contains(ConstantesEnum.EtapaConfig))
                    .FirstOrDefaultAsync();

                return etapa.Result.Valor;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> AutenticarOmie(string appKey, string appSecret)
        {
            using var httpClient = new HttpClient();
            using var request = new HttpRequestMessage(new HttpMethod("POST"), "https://app.omie.com.br/api/v1/geral/cidades/");
            request.Content = new StringContent("{\"call\":\"PesquisarCidades\",\"app_key\":\"" + appKey + "\",\"app_secret\":\"" + appSecret + "\",\"param\":[{\"pagina\":1,\"registros_por_pagina\":1}]}");
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            var response = await httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public string ConvertToCron(string horaText, string minutoText)
        {
            try
            {
                _ = int.TryParse(horaText, out var hora);
                _ = int.TryParse(minutoText, out var minuto);

                if ((minuto < 0 && minuto > 59) || (hora < 0 && hora > 23))
                    throw new Exception("Horario Invalido");

                if (hora == 0)
                {
                    return "0-59/" + minuto + " * * * *";
                }
                else
                {
                    var cronPart1 = (minuto != 0) ? $@"{minuto}" : $@"0";
                    var cronPart2 = (hora != 0) ? $@"0-23/{hora}" : $@"*";
                    return cronPart1 + " " + cronPart2 + " " + "* * *";
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task FinalizarCargaInicialOmie(PerformContext performContext, CancellationToken token)
        {
            await ProximaEtapaConfig();

            dbContext.Parametros.Add(new ParametroModel()
            {
                Nome = ConstantesEnum.CargaInicialFinalizada,
                Valor = true.ToString(),
            });

            await dbContext.SaveChangesAsync();

        }

        public async Task CargaInicialErro()
        {
            //await ProximaEtapaConfig();
            var etapa = await dbContext.Parametros
                    .Where(x => x.Nome.Contains(ConstantesEnum.EtapaConfig))
                    .FirstOrDefaultAsync();
            if (etapa == null)
            {
                ParametroModel etapaModel = new() { Nome = ConstantesEnum.EtapaConfig, Valor = ((int)EnumEtapasConfig.CargaInicialErro).ToString() };
                dbContext.Parametros.Add(etapaModel);
            }
            else
            {
                etapa.Valor = ((int)EnumEtapasConfig.CargaInicialErro).ToString();
            }
            await dbContext.SaveChangesAsync();


            dbContext.Parametros.Add(new ParametroModel()
            {
                Nome = ConstantesEnum.CargaInicialErro,
                Valor = true.ToString(),
            });

            await dbContext.SaveChangesAsync();
        }

        public async Task VoltarCargaInicial()
        {
            var etapa = await dbContext.Parametros
                    .Where(x => x.Nome.Contains(ConstantesEnum.EtapaConfig))
                    .FirstOrDefaultAsync();
            if (etapa == null)
            {
                ParametroModel etapaModel = new() { Nome = ConstantesEnum.EtapaConfig, Valor = ((int)EnumEtapasConfig.CargaInicial).ToString() };
                dbContext.Parametros.Add(etapaModel);
            }
            else
            {
                etapa.Valor = ((int)EnumEtapasConfig.CargaInicial).ToString();
            }
            await dbContext.SaveChangesAsync();

            var constante = await dbContext.Parametros
                    .Where(x => x.Nome.Contains(ConstantesEnum.CargaInicialErro) || x.Nome.Contains(ConstantesEnum.CargaInicial)).ToListAsync();

            if (constante.Any())
            {
                dbContext.Parametros.RemoveRange(constante);
                await dbContext.SaveChangesAsync();
            }
        }

    }
}
