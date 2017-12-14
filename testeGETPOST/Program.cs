using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace testeGETPOST
{
    class Program
    {
        static void Main(string[] args)
        {
            string filename = System.Environment.GetEnvironmentVariable("OUTPUT_PATH");
            TextWriter tw = new StreamWriter("teste.txt", true);
            int res;
            string _strSiglaPais;

            Console.WriteLine("Informe a sigla do país");
            _strSiglaPais = Console.ReadLine();

            int _QtdePopulacao;

            Console.WriteLine("Informe a quantidade da população a ser verificada");
            _QtdePopulacao = Convert.ToInt32(Console.ReadLine());

            res = getCountries(_strSiglaPais, _QtdePopulacao);
            tw.WriteLine(res);

            tw.Flush();
            tw.Close();
        }

        /// <summary>
        /// Metodo para executação de URL com chamada do metodo GET da API, onde o seu retorno é tratado
        /// e transofrmado em objeto do tipo json e somente a partir de então é realizado a busca e tratamento das informações
        /// </summary>
        /// <param name="strSiglaPais">sigla do pais a ser pesquisado</param>
        /// <param name="intPopulacao">quantidade de população a ser verificada se é maior que</param>
        /// <returns></returns>
        static int getCountries(string strSiglaPais, int intPopulacao)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://jsonmock.hackerrank.com/api/countries/search?name=" + strSiglaPais);//
            request.Method = "Get";
            request.KeepAlive = true;
            request.ContentType = "appication/json";
            //request.Headers.Add("Content-Type", "appication/json");
            request.ContentType = "application/x-www-form-urlencoded";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string myResponse = "";

            // faz a leitura do stream de retorno da API, e armazena na variavel em formato string
            // utiliza-se o using para garantir liberação do objeto pelo destrutor
            using (System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream()))
            {
                myResponse = sr.ReadToEnd();
            }

            // TRansforma o JSON que esta em formato do tipo string em formato Objeto JSON
            JToken token = JObject.Parse(myResponse);

            int page = (int)token.SelectToken("page");
            int totalPages = (int)token.SelectToken("total_pages");
            int per_page = (int)token.SelectToken("per_page");

            // Faz a soma da população de todos os registros retornados no JSON e aramazena na variavel
            int totalPopulacao = token["data"].Sum(m => (int)m.SelectToken("population"));
            int qtdePaises = 0;

            // Percorre o JSON de resposta para verificar a população
            for (int i = 0; i < per_page; i++)
            {
                var t = (int)token["data"][i].SelectToken("population");

                if (t > intPopulacao)
                {
                    qtdePaises++;
                }
            }

            Console.ReadLine();

            // Retorno com a qtde. de países que tem população superior ao informado para pesquisa
            return (qtdePaises);

        }
    }
}
