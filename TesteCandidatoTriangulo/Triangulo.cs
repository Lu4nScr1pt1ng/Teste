using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesteCandidatoTriangulo
{
    public class Triangulo
    {
        /// <summary>
        ///    6
        ///   3 5
        ///  9 7 1
        /// 4 6 8 4
        /// Um elemento somente pode ser somado com um dos dois elementos da próxima linha. Como o elemento 5 na Linha 2 pode ser somado com 7 e 1, mas não com o 9.
        /// Neste triangulo o total máximo é 6 + 5 + 7 + 8 = 26
        /// 
        /// Seu código deverá receber uma matriz (multidimensional) como entrada. O triângulo acima seria: [[6],[3,5],[9,7,1],[4,6,8,4]]
        /// </summary>
        /// <param name="dadosTriangulo"></param>
        /// <returns>Retorna o resultado do calculo conforme regra acima</returns>
        public int ResultadoTriangulo(string dadosTriangulo)
        {
            // var test = "[[6],[3, 5],[9, 7, 1],[4, 6, 8, 4]]";

            // convertendo string para um array

            // criando um array de string
            // removendo [ ] dos finais e dividindo pelo ],

            string[] arrString = dadosTriangulo.Trim('[', ']').Replace("],", "-").Split('-');

            // adicionando tamanho do array
            int[][] trianguloJaggedArray = new int[arrString.Length][];
            for (int i = 0; i < arrString.Length; i++)
            {
                // removendo [ ] das ponsta e dividindo por cada ,
                string[] indice = arrString[i].Trim('[', ']').Split(',');
                trianguloJaggedArray[i] = new int[indice.Length];
                for (int j = 0; j < indice.Length; j++)
                {
                    // confirmando e passando para intura e adicionado cada valor
                    trianguloJaggedArray[i][j] = int.Parse(indice[j]);
                }
            }

            int maxSum = 0;

            // começando pelo index do topo
            var indexAtual = 0;

            // primeiro valor é o topo do triangulo
            var topo = trianguloJaggedArray[0][indexAtual];
            maxSum += topo;

            // como já adicionamos o topo do triangulo agora começar pela segunda linha (index 1)
            for (var i = 1; i < trianguloJaggedArray.Length; i++)
            {
                var atual = trianguloJaggedArray[i][indexAtual];


                var proximo = trianguloJaggedArray[i][indexAtual + 1];


                if (atual > proximo)
                {
                    maxSum += atual;
                }
                else if (atual == proximo)
                {
                    maxSum += atual;
                }
                else
                {
                    indexAtual += 1;
                    maxSum += proximo;
                }
            }
            return maxSum;
        }
    }
}
