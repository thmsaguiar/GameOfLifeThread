using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Windows;
using System.Diagnostics;

namespace GameOfLife
{
    class Gol
    {
        public Stopwatch time = new Stopwatch();
        private static int qtd, inicioI, fimI, tam,inicioJ, fimJ;        
        private static List<List<char>> geracaoI = new List<List<char>>();
        private static List<List<char>> geracaoF = new List<List<char>>();

        //GETS E SETS
        public int Qtd
        {
          get { return qtd; }
          set { qtd = value; }
        }
        public int InicioI
        {
            get { return inicioI; }
            set { inicioI = value; }
        }
        public int FimI
        {
            get { return fimI; }
            set { fimI = value; }
        }
        public int InicioJ
        {
            get { return inicioJ; }
            set { inicioJ = value; }
        }
        public int FimJ
        {
            get { return fimJ; }
            set { fimJ = value; }
        }
        public int Tam
        {
            get { return tam; }
            set { tam = value; }
        }
        public List<List<char>> GeracaoI
        {
            get { return geracaoI; }
            set { geracaoI = value; }
        }
        public List<List<char>>  GeracaoF
        {
            get { return geracaoF; }
            set { geracaoF = value; }
        }

        public void estadoCel()
        {           
            //VERIFICANDO VIZINHOS
            for (int i = inicioI; i < fimI; i++)
            {
                for (int j = inicioJ; j < fimJ; j++)
                {
                    int viz = 0;
                    if (geracaoI[i - 1][j - 1] == '1')
                    {
                        viz++;
                    }
                    if (geracaoI[i - 1][j] == '1')
                    {
                        viz++;
                    }
                    if (geracaoI[i - 1][j + 1] == '1')
                    {
                        viz++;
                    }
                    if (geracaoI[i][j - 1] == '1')
                    {
                        viz++;
                    }
                    if (geracaoI[i][j + 1] == '1')
                    {
                        viz++;
                    }
                    if (geracaoI[i + 1][j - 1] == '1')
                    {
                        viz++;
                    }
                    if (geracaoI[i + 1][j] == '1')
                    {
                        viz++;
                    }
                    if (geracaoI[i + 1][j + 1] == '1')
                    {
                        viz++;
                    }
                    //DETERMINANDO ESTADO DAS CÉLULAS - Se estão vivas(1) ou mortas(0)
                    if (geracaoI[i][j] == '1')
                    {
                        if (viz < 2 || viz > 3)
                        {
                            geracaoF[i][j] = '0';
                        }
                        else
                        {
                            geracaoF[i][j] = '1';
                        }
                    }
                    else
                    {
                        if (viz == 3)
                        {
                            geracaoF[i][j] = '1';
                        }
                        else
                        {
                            geracaoF[i][j] = '0';
                        }
                    }

                }
            }
        }

        public void result(List<List<char>> geracao)
        {
            //CRIANDO NOVO ARQUIVO QUE CONTERÁ A GERACAO FINAL
            string dir = "Geracao.txt";
            using (StreamWriter writer = new StreamWriter(dir))
            {
                for (int i = 1; i < geracao.Count - 1; i++)
                {
                    for (int j = 1; j < geracao[0].Count - 1; j++)
                    {
                        writer.Write(geracao[i][j]);

                    }
                    writer.WriteLine("");
                }
            }
            //VERIFICA SE FOI CRIADO O NOVO ARQUIVO
            if (System.IO.File.Exists(dir))
            {
                MessageBoxResult conf = MessageBox.Show("Nova geração criada com sucesso!", "Confirmação");
            }
            else
            {
                MessageBoxResult conf = MessageBox.Show("ERROR: Arquivo novo não foi criado", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void iniciandoMatriz()
        {
            //INICIADO A MATRIZ GERCAÇÃOF COM 0                                  
            for (int j = 0, l = 0; j < tam + 2; j++, l = 0)
            {
                List<char> linha = new List<char>();
                while (l < tam + 2)
                {
                    linha.Add('0');
                    l++;
                }
                geracaoF.Add(linha);
            }
        }

        public void geracao()
        {
            //INICIANDO VARIÁVEIS PARA PECORRER A MATRIZ DO INICIO AO FIM
            inicioI = inicioJ= 1;
            fimI = fimJ = tam + 1;           

            iniciandoMatriz();//Inicia a matriz final com '0'
            //Fazendo as gerações
            for (int i = 0; i < qtd; i++)
            {
                time.Start();// Começa a contar o tempo 

                estadoCel();

                time.Stop();// Para de contar o tempo

                //FAZENDO A TROCA DE VALORES
                for (int k = 0; k < geracaoI.Count; k++)
                {
                    for (int j = 0; j < geracaoI[0].Count; j++)
                    {
                        geracaoI[k][j] = geracaoF[k][j];
                    }
                }
            }

        }

        public void geracao2T()
        {
            //DETERMINANDO OS VALORES DE INCIO E FIM DO CONTADOR(j)
            inicioJ = 1;
            fimJ = tam + 1;
            //INICIANDO MATRIZ GERACAOF COM '0'
            iniciandoMatriz();
            //CRIA UM OBJETO DO TIPO GOL(Sigla de Game Of Life)
            Gol jogoG = new Gol();
            //RODA AS GERAÇÕES
            for (int i = 0; i < qtd; i++)
            {                
                //ADICIONA OS VALORES PARA AS VARIÁVEIS     
                jogoG.GeracaoI = geracaoI;
                jogoG.GeracaoF = geracaoF;

                //DETERMINANDO OS VALORES DE INICIO E FIM DO CONTADOR(i)
                jogoG.InicioI = 1;
                jogoG.FimI = (tam + 2) / 2;

                //CRIANDO AS THREADS
                Thread mythread = new Thread(jogoG.estadoCel);
                Thread mythread2 = new Thread(jogoG.estadoCel);

                time.Start();// Começa a contar o tempo 

                //Iniciando a primeira Thread
                mythread.Start();
                mythread.Join();

                //Fazendo mudanças nas variáveis para começar a segunda
                jogoG.InicioI = fimI;
                jogoG.FimI = tam + 1;

                //Iniciando a segunda Thread
                mythread2.Start();
                mythread2.Join();

                //Espera todas as threads terminarem
                Task.WaitAll();

                time.Stop();// Para de contar o tempo

                //FAZENDO A TROCA DE VALORES
                for (int k = 0; k < geracaoI.Count; k++)
                {
                    for (int j = 0; j < geracaoI[0].Count; j++)
                    {
                        geracaoF[k][j] = jogoG.GeracaoF[k][j];
                        geracaoI[k][j] = geracaoF[k][j];
                    }
                }
            }          
                          
        }
        
        public void geracao4T()
        {
            //INICIANDO MATRIZ GERACAOF COM '0'
            iniciandoMatriz();
            Gol jogoG = new Gol();
            //RODA AS GERAÇÕES
            for (int i = 0; i < qtd; i++)
            {
                //ADICIONA OS VALORES PARA AS VARIÁVEIS
                jogoG.GeracaoI = geracaoI;
                jogoG.GeracaoF = geracaoF;

                //DETERMINANDO OS VALORES DE INICIO E FIM DO CONTADOR(i)
                jogoG.InicioI = 1;
                jogoG.FimI = (tam + 2) / 2;

                //DETERMINANDO OS VALORES DE INICIO E FIM DO CONTADOR(j)
                jogoG.InicioJ = 1;
                jogoG.FimJ = (tam + 2) / 2;

                //CRIANDO AS THREADS
                Thread mythread = new Thread(jogoG.estadoCel);
                Thread mythread2 = new Thread(jogoG.estadoCel);
                Thread mythread3 = new Thread(jogoG.estadoCel);
                Thread mythread4 = new Thread(jogoG.estadoCel);

                time.Start();// Começa a contar o tempo 

                //Iniciando a primeira Thread
                mythread.Start();
                mythread.Join();

                //Mudando as variaveis do contador (j)
                jogoG.InicioJ = jogoG.FimJ;
                jogoG.FimJ = tam + 1;

                //Iniciando a segunda Thread
                mythread2.Start();
                mythread2.Join();

                //Mudando as variaveis do contador (i)
                jogoG.InicioI = jogoG.FimI;
                jogoG.FimI = tam + 1;

                //Mudando as variaveis do contador (j)
                jogoG.InicioJ = 1;
                jogoG.FimJ = (tam + 2) / 2;

                //Iniciando a terceira Thread
                mythread3.Start();
                mythread3.Join();

                //Mudando as variaveis do contador (j)
                jogoG.InicioJ = jogoG.FimJ;
                jogoG.FimJ = tam + 1;

                //Iniciando a quarta Thread
                mythread4.Start();
                mythread4.Join();

                //Espera todas as threads terminarem
                Task.WaitAll();

                time.Stop();// Para de contar o tempo

                //FAZENDO A TROCA DE VALORES
                for (int k = 0; k < geracaoI.Count; k++)
                {
                    for (int j = 0; j < geracaoI[0].Count; j++)
                    {
                        geracaoF[k][j] = jogoG.GeracaoF[k][j];
                        geracaoI[k][j] = geracaoF[k][j];
                    }
                }
            }
        }
        
    }
}
