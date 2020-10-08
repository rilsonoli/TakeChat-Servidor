using System;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Collections;


namespace Negocio
{
    public class ChatServidorNegocio
    {
        public static ChatEntidade chatServicoEntidade { get; set; }

        public ChatServidorNegocio(string Ip)
        {
            chatServicoEntidade = new ChatEntidade(Ip);
        }

        /// <summary>
        /// Inclui um usuário ao hash de usuários
        /// </summary>
        /// <param name="tcpUsuario"></param>
        /// <param name="strUsername"></param>
        public static void IncluiUsuario(TcpClient tcpUsuario, string strUsername)
        {
            chatServicoEntidade.htUsuarios.Add(strUsername, tcpUsuario);
            chatServicoEntidade.htConexoes.Add(tcpUsuario, strUsername);
            EnviaMensagem("Administrador",chatServicoEntidade.htConexoes[tcpUsuario] + " entrou..",true);
        }

        /// <summary>
        /// Remove um usuário do hash de usuários
        /// </summary>
        /// <param name="tcpUsuario"></param>
        public static void RemoveUsuario(TcpClient tcpUsuario)
        {
            if (chatServicoEntidade.htConexoes[tcpUsuario] != null)
            {
                EnviaMensagem("Administrador",chatServicoEntidade.htConexoes[tcpUsuario] + " saiu...",true);

                // Removeo usuário da hash table
                chatServicoEntidade.htUsuarios.Remove(chatServicoEntidade.htConexoes[tcpUsuario]);
                chatServicoEntidade.htConexoes.Remove(tcpUsuario);
            }
        }
                
        /// <summary>
        /// Envia mensagens 
        /// </summary>
        /// <param name="Origem"></param>
        /// <param name="Mensagem"></param>
        /// <param name="msgAdm"></param>
        /// <param name="Destinatario"></param>
        /// <param name="privado"></param>
        public static void EnviaMensagem(string Origem, string Mensagem, bool msgAdm = false, string Destinatario = "", bool privado = false)
        {
            if (Mensagem.Trim() != "" && chatServicoEntidade.htUsuarios.Count > 0)
            {              

                if(!msgAdm)
                    Mensagem = $"{Origem} disse " + (privado ? "reservadamente " : "") + (!string.IsNullOrWhiteSpace(Destinatario) ? $"para {Destinatario}:" : ":") + Mensagem;
    
                
                TcpClient[] tcpClientes = new TcpClient[chatServicoEntidade.htUsuarios.Count];
                chatServicoEntidade.htUsuarios.Values.CopyTo(tcpClientes, 0);

                string usuarios = RetornaListaUsuarios(chatServicoEntidade.htUsuarios);

                for (int i = 0; i < tcpClientes.Length; i++)
                {
                    // Tenta enviar uma mensagem para cada cliente
                    try
                    {
                        // Se a mensagem estiver em branco ou a conexão for nula sai...
                        if (privado && tcpClientes[i] != ChatServidorNegocio.chatServicoEntidade.htUsuarios[Destinatario] && tcpClientes[i] != ChatServidorNegocio.chatServicoEntidade.htUsuarios[Origem])                       
                            continue;

                        // Envia a mensagem para o usuário atual no laço
                        var swSenderSender = new StreamWriter(tcpClientes[i].GetStream());
                        swSenderSender.WriteLine(usuarios + "|" + (msgAdm ? "Administrador:" : "" ) + Mensagem);
                        swSenderSender.Flush();
                        swSenderSender = null;
                    }
                    catch // Se houver um problema , o usuário não existe , então remove-o
                    {
                        RemoveUsuario(tcpClientes[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Inicia o serviço
        /// </summary>
        public void IniciaServico()
        {
            try
            {
                chatServicoEntidade.tlsCliente.Start();
                chatServicoEntidade.ServRodando = true;
                chatServicoEntidade.thrListener = new Thread(MantemServico);
                chatServicoEntidade.thrListener.Start();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Mantem o serviço rodando
        /// </summary>
        private void MantemServico()
        {
            while (chatServicoEntidade.ServRodando == true)
            {
                chatServicoEntidade.tcpCliente = chatServicoEntidade.tlsCliente.AcceptTcpClient();
                Conexao newConnection = new Conexao(chatServicoEntidade.tcpCliente);
            }
        }

        /// <summary>
        /// Retorna a lista de usuários
        /// </summary>
        /// <param name="usuariosHash"></param>
        /// <returns></returns>
        private static string RetornaListaUsuarios(Hashtable usuariosHash)
        {
            string usuarios = "";
            foreach (var item in usuariosHash.Keys)
            {
                usuarios += item.ToString() + ";";
            }

            return usuarios;
        }
    }
}
