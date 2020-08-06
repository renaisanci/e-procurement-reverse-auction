using System;
using System.Collections;
using System.Collections.Generic;
using Gerencianet.SDK.Requests;
using Gerencianet.SDK.Responses;
using Newtonsoft.Json;

namespace Gerencianet.SDK
{   
    /// <summary>
    /// API para integração com a GerenciaNet
    /// </summary>
    public sealed class GerencianetAPI
    {
        #region [ Attributes ]

        private readonly dynamic _endpoints;

        #endregion

        #region [ Properties ]


        #endregion

        #region [ Constructors ]

        public GerencianetAPI()
        {
            this._endpoints = new Endpoints(Credentials.Default.ClientId, Credentials.Default.ClientSecret, Credentials.Default.Sandbox);
        }

        #endregion

        #region [ Methods ]

        #region [ Errors ]
        /// <summary>
        /// Método de retorno de Erros para transações efetuadas no servidor da GerenciaNet
        /// </summary>
        /// <param name="code">Passar código de erro para restorno da mensagem de erro.</param>
        /// <returns></returns>
        public string MessageError(int code)
        {
            switch (code)
            {
                case 3500000:
                    return "Erro interno do servidor.";
                case 3500001:
                    return "A aplicação fornecida não tem permissão para utilizar este endpoint.";
                case 3500002:
                    return "O parâmetro[data] é obrigatório.";
                case 3500007:
                    return "O tipo de pagamento informado na propriedade[type] não está disponível.";
                case 3500008:
                    return "Requisição não autorizada.";
                case 3500010:
                    return "A propriedade[% s] informada não existe.";
                case 3500011:
                    return "% s";
                case 3500016:
                    return "A transação deve possuir um cliente antes de ser paga.";
                case 3500021:
                    return "A propriedade[installments] não pode ser superior a 1 para assinaturas.";
                case 3500023:
                    return "A propriedade[% s] é obrigatória.";
                case 3500030:
                    return "Esta transação já possui uma forma de pagamento definida.";
                case 3500032:
                    return "O plano não pode ser removido pois possui transações associadas.";
                case 3500034:
                    return "% s";
                case 3500036:
                    return "A forma de pagamento da transação não é boleto bancário.";
                case 3500037:
                    return "A propriedade[% s] informada é inválida.";
                case 3500038:
                    return "Apenas transações com status[waiting] ou[unpaid] podem ser atualizadas.";
                case 3500039:
                    return "A propriedade[send_to] é obrigatória para solicitação de envio pelos correios.";
                case 3500040:
                    return "Apenas assinaturas com status[new] ou[active] podem ser canceladas.";
                case 3500041:
                    return "A propriedade[repeats] deve ser maior ou igual a dois.";
                case 3500042:
                    return "O parâmetro[data] deve ser um JSON.";
                case 3500043:
                    return "Apenas transações com status[new], [link], [waiting] ou[unpaid] podem ser canceladas.";
                case 3500044:
                    return "A transação não pode ser paga pois possui o status[% s].";
                case 3500050:
                    return "O identificador de conta fornecido é inválido.";
                case 3500051:
                    return "É necessário chamar a função $gn.ready.";
                case 3500052:
                    return "A função de callback é obrigatória.";
                case 3500053:
                    return "O parâmetro[total] é obrigatório e deve ser um inteiro.";
                case 3500054:
                    return "O parâmetro[brand] é obrigatório.";
                case 3500055:
                    return "O parâmetro[card_data] deve ser um objeto.";
                case 3500056:
                    return "O parâmetro[brand] informado é inválido.As opções válidas são: 'visa', 'mastercard', 'amex', 'diners', 'discover', 'jcb', 'elo' or 'aura'.";
                case 3500057:
                    return "O parâmetro[number] é obrigatório.";
                case 3500058:
                    return "O parâmetro[number] informado é inválido.";
                case 3500059:
                    return "O parâmetro[cvv] é obrigatório.";
                case 3500060:
                    return "O parâmetro[expiration_month] é obrigatório.";
                case 3500061:
                    return "O parâmetro[expiration_year] é obrigatório.";
                case 3500062:
                    return "O parâmetro[expiration_month] informado é inválido.";
                case 3500063:
                    return "O parâmetro[available_payment_forms] é obrigatório.";
                case 3500064:
                    return "O parâmetro[available_payment_forms] informado é inválido.As opções disponíveis são: 'banking_billet' and 'credit_card'.";
                case 3500065:
                    return "Os parâmetros são obrigatórios.";
                case 3500066:
                    return "Apenas transações com status[waiting] podem ser atualizadas.";
                case 3500068:
                    return "Apenas carnês com status[active] podem ser atualizados.";
                case 3500070:
                    return "Apenas cobranças com status[waiting] que não foram pagas com cartão de crédito podem ser atualizadas.";
                case 3500073:
                    return "Não é possível aplicar desconto para cobranças que utilizam Marketplace.";
                case 3500077:
                    return "A propriedade[% s] já existe.";
                case 3500078:
                    return "Já existe uma aplicação chamada '%s'.";
                case 3500079:
                    return "A aplicação '%s' não pode ser removida pois possui cobranças associadas.";
                case 3500080:
                    return "Essa cobrança já teve um link de pagamento definido.";
                case 3500081:
                    return "A cobrança não possui status[% s].";
                case 4600001:
                    return "Pagamento não encontrado.";
                case 4600002:
                    return "";
                case 4600005:
                    return "O saldo da conta é insuficiente para efetuar a transação.";
                case 4600007:
                    return "A data do vencimento do boleto deve ser maior ou igual que a data atual.";
                case 4600008:
                    return "A data de vencimento mínima para emissão com envio pelos correios é inválida.";
                case 4600022:
                    return "";
                case 4600026:
                    return "";
                case 4600028:
                    return "";
                case 4600029:
                    return "";
                case 4600032:
                    return "";
                case 4600033:
                    return "Conta bloqueada para realizar emissões.";
                case 4600034:
                    return "A transação ultrapassa o limite de emissão da conta.";
                case 4600035:
                    return "Serviço indisponível para a conta.Por favor, solicite que o recebedor entre em contato com o suporte Gerencianet.";
                case 4600037:
                    return "O valor da emissão é superior ao limite operacional da conta.Por favor, solicite que o recebedor entre em contato com o suporte Gerencianet.";
                case 4600059:
                    return "O número máximo de folhas por carnê é 12.";
                case 4600060:
                    return "A data informada é inválida.";
                case 4600073:
                    return "";
                case 4600100:
                    return "Timeout: Não foi possível validar os dados enviados.Por favor, tente novamente mais tarde.";
                case 4600111:
                    return "";
                case 4600142:
                    return "Transação não processada por conter incoerência nos dados cadastrais.";
                case 4600148:
                    return "";
                case 4600164:
                    return "";
                case 4600196:
                    return "";
                case 4600197:
                    return "";
                case 4600198:
                    return "";
                case 4600202:
                    return "O envio de CPF e / ou CNPJ é obrigatório.";
                case 4600204:
                    return "";
                case 4600209:
                    return "Limite de emissões diárias excedido.Por favor, solicite que o recebedor entre em contato com o suporte Gerencianet.";
                case 4600210:
                    return "não é possível emitir três emissões idênticas.Por favor, entre em contato com nosso suporte para orientações sobre o uso correto dos serviços Gerencianet.";
                case 4600212:
                    return "Número de telefone já associado a outro CPF. Não é possível cadastrar o mesmo telefone para mais de um CPF.";
                case 4600218:
                    return "E - mail já associado a outro CPF.Não é possível cadastrar o mesmo e - mail para mais de um CPF.";
                case 4600219:
                    return "";
                case 4600222:
                    return "Recebedor e cliente não podem ser a mesma pessoa.";
                case 4600224:
                    return "Problemas na validação da autorização do App.Por favor, solicite que o recebedor entre em contato com o suporte Gerencianet";
                case 4600254:
                    return "";
                case 4600257:
                    return "";
                case 4600268:
                    return "";
                case 4600270:
                    return "";
                case 4600271:
                    return "";
                case 4600272:
                    return "";
                case 4600309:
                    return "";
                case 4600329:
                    return "";
                case 4600378:
                    return "";
                case 4600379:
                    return "";
                case 4600380:
                    return "";
                case 4600439:
                    return "O cancelamento de todas as lâminas só pode ser feito para carnês e a partir da primeira lâmina";
                case 4699999:
                    return "";
                default:
                    return "";
            }
        }

        #endregion

        #region [ Transaction ]
        /// <summary>
        /// Método para criar uma transação
        /// </summary>
        /// <param name="transaction">Objeto da classe TransactionRequest que é obrigatório.</param>
        /// <returns></returns>
        public TransactionResponse CreateTransaction(TransactionRequest transaction)
        {

            //var body = JsonConvert.SerializeObject(transaction);
            var response = this._endpoints.CreateCharge(null, transaction);

            int code = response.code;

            if (code != 200)
            {
                string json = response;
                throw GnException.Build(json, code);
            }

            var data = JsonConvert.DeserializeObject<TransactionResponse>(response.data.ToString());
            //var data = response.data.ToString();

            return data;
        }

        #endregion

        #region [ Payment ]

        private object CreatePayment(int chargerId, Payment payment)
        {
            var param = new { id = chargerId };


            //var body = new { payment = JsonConvert.SerializeObject(payment) };

            var tipo = new TipoPagamento
            {
                Payment = payment
            };

            var response = this._endpoints.PayCharge(param, tipo);


            return response.data;
        }

        /// <summary>
        /// Método para gerar um boleto
        /// </summary>
        /// <param name="chargerId">Passar o chargerId retornado ao criar a transação.</param>
        /// <param name="bankingBillet">Passar Obejeto BankingBillet para criar o boleto.</param>
        /// <returns></returns>
        public BankingBilletResponse CreateBankingBillet(int chargerId, Payment payment)
        {
            var response = this.CreatePayment(chargerId, payment);
            
            var data = JsonConvert.DeserializeObject<BankingBilletResponse>(response.ToString());

            return data;
        }

        #endregion

        #region [ Plan ]
                
        public dynamic CreatePlan(Plan plan)
        {
            var body = JsonConvert.SerializeObject(plan);

            var response = this._endpoints.CreatePlan(null, body);

            return response;
        }

        public dynamic CreateSubscription(int idPlan, TransactionRequest transaction)
        {
            var param = new { id = idPlan };

            var body = JsonConvert.SerializeObject(transaction);

            var response = this._endpoints.CreateSubscription(param, body); ;

            return response;
        }

        public dynamic CreateSubscriptionPayment(int idSubscription, Payment payment)
        {
            var param = new { id = idSubscription };

            var body = new { payment = JsonConvert.SerializeObject(payment) };

            var response = this._endpoints.PaySubscription(param, payment);

            return response;
        }

        #endregion

        #region [Notification]

        /// <summary>
        /// Método que envia Token para GerenciaNet e recebe o Status da Fatura.
        /// </summary>
        /// <param name="notification">Passar Token de notificação para receber Status da Fatura.</param>
        /// <returns></returns>
        public NotificationsResponse ReturnStatusFatura(string notification)
        {
            var param = new {token = notification};

            var response = _endpoints.GetNotification(param);

            var ultimoObjetoArray = response.data;

            var objeto = ultimoObjetoArray[ultimoObjetoArray.Count - 1];

            return JsonConvert.DeserializeObject<NotificationsResponse>(objeto.ToString());

        }

        #endregion

        #region [Cancel Transaction]

        /// <summary>
        /// Método para Cancelamento de uma Transação
        /// </summary>
        /// <param name="cancelTransaction">Passar objeto CancelTransaction com a propriedade ChargerId para cancelar transação.</param>
        /// <returns></returns>
        public object CancelTransaction(int chargerId)
        {
            var param = new { id = chargerId };

            var response = _endpoints.CancelCharge(param);

            return response.code;
        }

        #endregion

        #endregion
    }
}
