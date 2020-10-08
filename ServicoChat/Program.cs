using Negocio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace ServicoChatApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Iniciando serviço....");
                        
            try
            {
                var ip = ConfigurationSettings.AppSettings["IPServidor"];

                if (string.IsNullOrEmpty(ip))
                {
                    Console.WriteLine("Não foi localizado o IP do servidor no arquivo de configuraçaõ App.config");
                }
                else
                {

                    // Cria uma nova instância do objeto ChatServidor
                    ChatServidorNegocio servicoChat = new ChatServidorNegocio(ip);

                    // Inicia o atendimento das conexões
                    servicoChat.IniciaServico();

                    // Mostra que nos iniciamos o atendimento para conexões
                    Console.WriteLine("Monitorando as conexões...\r\n");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro de conexão : " + ex.Message);
            }
        }
    }
}
