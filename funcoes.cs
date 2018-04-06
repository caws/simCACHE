using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace simCACHE
{
    class funcoes
    {
        public string mapeamentoAssociativo(string caminhoArquivo, int tag, int set, int tamSet, Boolean LRU, Boolean FIFO, int word, int indiceGrid, int posCell) //terminar isso até quarta
        {
            string enderecoHex;
            Int64 enderecoBinario;
            int qtdSet = Convert.ToInt32(Math.Pow(2, set));

            int qtdHit = 0;
            int qtdMiss = 0;

            Int64[,] cache = new Int64[qtdSet, tamSet]; //numero de conjuntos + tags
            int[,] timeStampCache = new int[qtdSet, tamSet];

            for (int i = 0; i < qtdSet; i++) //numero do conjunto
            {
                timeStampCache[i,0] = 0;
                timeStampCache[i, 1] = 0;
            }

            System.IO.StreamReader file = new System.IO.StreamReader(caminhoArquivo); //abrindo o arquivo txt
          
            while ((enderecoHex = file.ReadLine()) != null)
            {             
                enderecoBinario = Convert.ToInt64("0x" + enderecoHex, 16);
                Int64 valorTag = enderecoBinario >> (set + word); //pega a tag
                Int64 setEscrita = (enderecoBinario - (valorTag << (set + word))) >> word; //pega o set

                int x; bool existe = false;
                    for (x = 0; x < tamSet; x++)
                        {
                            if (cache[setEscrita,x] == valorTag)
                            {
                                qtdHit++;
                                if (LRU == true)
                                {
                                    timeStampCache[setEscrita, x] = timeStampCache[setEscrita, x] + 1; //LRU
                                }
                                existe = true;
                                break;
                            }
                            else
                            {
                            //existe = false;
                            }
                        }
                    //MessageBox.Show(enderecoHex + "\n"+qtdHit.ToString());
                    if (existe == false)
                    {
                        if (LRU == true)//verifica o menos utilizado recentemente
                        {
                            Int64 indice = funcaoLRU(timeStampCache, tamSet, setEscrita);
                            cache[setEscrita, indice] = valorTag; //substitui bloco com a nova tag
                            timeStampCache[setEscrita, indice] =  1; //atualiza a timestamp
                        }
                        if (FIFO == true)//verifica o mais antigo a entrar na cache
                        {
                            int[,] indice = funcaoFIFO(timeStampCache, tamSet, setEscrita);
                            cache[setEscrita, indice[0, 0]] = valorTag; //substitui bloco com a nova tag
                            timeStampCache[setEscrita, indice[0, 0]] = indice[0, 1] + 1; //atualiza a timestamp
                        }
                        qtdMiss++;
                    }
            }
            file.Close();

            if (LRU == true)
            {
                //Form1._Form1.postaResultado("\nMapeamento Associativo "+tamSet.ToString()+"-way com LRU e cache de "+(qtdSet*tamSet*16).ToString()+" bytes concluída.");
                Form1._Form1.postaResultado("\nMapeamento Associativo " + tamSet.ToString() + "-way com LRU e cache de " + (qtdSet * tamSet * 16).ToString() + " bytes\nQuantidade de Hits:" + qtdHit.ToString() + "\nQuantidade de Misses:" + qtdMiss.ToString() + "\nQuantidade de endereços:" + (qtdHit + qtdMiss).ToString() + "\nPorcentagem de hits:" + porcentagem(qtdHit + qtdMiss, qtdHit) + "\nPorcentagem de misses:" + porcentagem(qtdHit + qtdMiss, qtdMiss));
                Form1._Form1.addcelulaGrid1(indiceGrid, posCell, qtdMiss);
            }
            if (FIFO == true)
            {
                //Form1._Form1.postaResultado("\nMapeamento Associativo " + tamSet.ToString() + "-way com FIFO cache de " + (qtdSet * tamSet * 16).ToString() + " bytes concluída.");
                Form1._Form1.postaResultado("\nMapeamento Associativo " + tamSet.ToString() + "-way com FIFO e cache de " + (qtdSet * tamSet * 16).ToString() + " bytes\nQuantidade de Hits:" + qtdHit.ToString() + "\nQuantidade de Misses:" + qtdMiss.ToString() + "\nQuantidade de endereços:" + (qtdHit + qtdMiss).ToString() + "\nPorcentagem de hits:" + porcentagem(qtdHit + qtdMiss, qtdHit) + "\nPorcentagem de misses:" + porcentagem(qtdHit + qtdMiss, qtdMiss));
                Form1._Form1.addcelulaGrid2(indiceGrid, posCell, qtdMiss);
            }
           return "Quantidade de Hits:" + qtdHit.ToString() + "\nQuantidade de Misses:" + qtdMiss.ToString();
        }

        public string mapeamentoDireto(string caminhoArquivo, int line, int tag, int word, int indiceGrid) 
        {
            string enderecoHex;
            Int64 enderecoBinario;
            int qtdMiss = 0;
            int qtdHits = 0;
            int qtdLinhasCache = Convert.ToInt32((Math.Pow(2, line))); //quantidade de linhas na cache

            Int64[][] cache = new Int64[qtdLinhasCache][]; // criando vetor com tag + dados
            for (int x = 0; x < qtdLinhasCache; x++)
            {
                cache[x] = new Int64[2]; //2 equivale ao campo tag + line
            }

            System.IO.StreamReader file = new System.IO.StreamReader(caminhoArquivo); //abrindo o arquivo txt
            while ((enderecoHex = file.ReadLine()) != null)
            {
                enderecoBinario = Convert.ToInt64("0x"+enderecoHex, 16);
                Int64 valorTag = enderecoBinario >> (line+word); //pega a tag
                Int64 linhaParaEscrita = (enderecoBinario - (valorTag << (line+word))) >> word; //pega a linha

               if (cache[linhaParaEscrita][0] == valorTag)
                {
                    qtdHits++;
                }
                else
                {
                    cache[linhaParaEscrita][0] = valorTag;
                    qtdMiss++;
                }

            }
            file.Close();
            Form1._Form1.postaResultado("Mapeamento Direto com cache de " + (qtdLinhasCache * 16).ToString() + " bytes.\nQuantidade de Hits:" + qtdHits.ToString() + "\nQuantidade de Misses:" + qtdMiss.ToString() + "\nQuantidade de endereços:" + (qtdHits + qtdMiss).ToString() + "\nPorcentagem de hits:" + porcentagem(qtdHits + qtdMiss, qtdHits) + "\nPorcentagem de misses:" + porcentagem(qtdHits + qtdMiss, qtdMiss));
            Form1._Form1.addcelulaGrid1(0, indiceGrid, qtdMiss);
            Form1._Form1.addcelulaGrid2(0, indiceGrid, qtdMiss);

            return "Quantidade de Hits:" + qtdHits.ToString() + "\nQuantidade de Misses:" + qtdMiss.ToString();
        }

        public int vlMaisAlto(int[,] timeStampCache, Int64 conjuntoDeLeitura, int tamSet)
        {
            int maior = timeStampCache[conjuntoDeLeitura,0];
            for (int i = 0; i <tamSet; i++)
            {
                if (timeStampCache[conjuntoDeLeitura,i] > maior)
                {
                    maior = timeStampCache[conjuntoDeLeitura, i];
                }
            }

            return maior;
        }

        public int[,] funcaoFIFO(int[,] timeStampCache, int tamSet, Int64 conjuntoDeLeitura)
        {
            int maisAntigo = timeStampCache[conjuntoDeLeitura, 0];
            int b; int indice = 0; int vl = 0;
            int[,] res = new int[1, 2]; ;

            for (b = 0; b < tamSet; b++)
            {
                if (timeStampCache[conjuntoDeLeitura, b] == 0)
                {
                    maisAntigo = timeStampCache[conjuntoDeLeitura, b];
                    indice = b;
                    vl = vlMaisAlto(timeStampCache, conjuntoDeLeitura, tamSet);
                    break;
                }
                else
                if (timeStampCache[conjuntoDeLeitura, b] < maisAntigo)
                {
                    maisAntigo = timeStampCache[conjuntoDeLeitura, b];
                    indice = b;
                }
            }
            vl = vlMaisAlto(timeStampCache, conjuntoDeLeitura, tamSet);            
            res[0, 0] = indice;
            res[0, 1] = vl;
            return res;
        }

        public int funcaoLRU(int[,] timeStampCache, int tamSet, Int64 conjuntoDeLeitura)
        {
            int maisAntigo = timeStampCache[conjuntoDeLeitura, 0];
            int cntrl = maisAntigo;
            int b; int indice = 0;

            for (b = 0; b < tamSet; b++)
            {
                if (timeStampCache[conjuntoDeLeitura, b] == 0)
                {
                    maisAntigo = timeStampCache[conjuntoDeLeitura, b];
                    indice = b;
                    break;
                }
                else
                if (timeStampCache[conjuntoDeLeitura, b] < maisAntigo)
                {
                   maisAntigo = timeStampCache[conjuntoDeLeitura, b];
                    indice = b;
                }
            }
           return indice;
        }
  
        public int potencia2(double x)
        {
            int y = 1; int pot = 0;
            while (y != x)
            {
                y = y * 2;
                pot++;
            }
            return pot;
        }
       
        public string porcentagem(int total, int valor)
        {
            double resultado = (100 * valor) / (double)total;

            return resultado.ToString() + "%";
        } 

    }
}
