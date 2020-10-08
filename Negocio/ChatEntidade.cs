using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Negocio
{
    public class ChatEntidade
    {
        public Hashtable htUsuarios { get; set; }
        public Hashtable htConexoes { get; set; }
        public IPAddress enderecoIP { get; set; }
        public TcpClient tcpCliente { get; set; }       
        public TcpListener tlsCliente { get; set; }
        public bool ServRodando { get; set; }
        public Thread thrListener { get; set; }       

        public ChatEntidade()
        {
            htUsuarios = new Hashtable(100);
            htConexoes = new Hashtable(100);
            tcpCliente = new TcpClient();
        }

        public ChatEntidade(string Ip)
        {
            htUsuarios = new Hashtable(100);
            htConexoes = new Hashtable(100);
            enderecoIP = IPAddress.Parse(Ip);
            tlsCliente = new TcpListener(enderecoIP, 2502);
            tcpCliente = new TcpClient();
        }
    
    }
}
