using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gelo_Sao_Mateus_EMISSOR_NF
{
    public class Data
    {
        public List<Cliente> Companies { get; set; }
    }
    public class Cliente
    {
        public string CNPJ { get; set; }
        public string IE { get; set; }
        public string NOME_FANTASIA { get; set; }
        public string LOGRADOURO { get; set; }
        public string NUMERO { get; set; }
        public string BAIRRO { get; set; }
        public string VALOR { get; set; }
        public object OBS { get; set; }
    }
    public class ObservaçãoCNPJ
    {
        public string CNPJ;
        public object OBS;

        public ObservaçãoCNPJ(string CNPJ, object OBS)
        {
            this.CNPJ= CNPJ;
            this.OBS= OBS;
        }
    }
}
    