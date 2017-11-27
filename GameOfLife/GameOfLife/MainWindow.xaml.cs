using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GameOfLife
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            resultado.Visibility = Visibility.Collapsed;

        }

        private void OpenFile(object sender, MouseButtonEventArgs e)
        {
            Stopwatch time = new Stopwatch();

            if (caixaDir.GetLineText(0) != "" && caixaQtd.GetLineText(0) != "")
            {
                int qtd = Convert.ToInt32(caixaQtd.GetLineText(0));
                //VERIFICA SE O ARQUIVO EXISTE
                if (System.IO.File.Exists(caixaDir.GetLineText(0)))
                {
                    List<List<char>> matriz = new List<List<char>>();
                    List<char> borda = new List<char>();
                    string linhaArq = "";
                    int tam;
                    int numLinha = 0;

                    //LENDO O ARQUIVO E PASSANDO OS VALORES PARA A MATRIZ
                    using (StreamReader arq = new StreamReader(caixaDir.GetLineText(0)))
                    {//using não precisa fechar o arquivo                                                
                        linhaArq = arq.ReadLine();
                        tam = Convert.ToInt32(linhaArq);

                        if (tam < 1001)
                        {
                            //preenchendo as bordas com '0'
                            for (int i = 0; i < tam + 2; i++)
                            {
                                borda.Add('0');
                            }
                            //preenchendo a matriz com os valores
                            while (numLinha < tam)
                            {
                                List<char> linha = new List<char>();
                                linhaArq = arq.ReadLine();
                                if (numLinha == 0)
                                {
                                    //Adiciona a borda da primeira linha na matriz
                                    matriz.Add(borda);
                                }
                                try
                                {
                                    for (int i = 0; i < tam; i++)
                                    {
                                        if (i == 0)
                                        {
                                            //Adiciona borda na primeira coluna da matriz
                                            linha.Add('0');
                                        }
                                        linha.Add(linhaArq[i]);
                                        if ((i + 1) == tam)
                                        {
                                            //Adiciona borda na última coluna da matriz
                                            linha.Add('0');
                                        }
                                    }
                                    //Adiciona linha na matriz
                                    matriz.Add(linha);
                                    numLinha++;
                                    if (numLinha == tam)
                                    {
                                        //Adiciona a borda da última linha na matriz
                                        matriz.Add(borda);
                                    }
                                }
                                catch (NullReferenceException nullRefEx)
                                {
                                    Console.WriteLine("Error: " + nullRefEx);
                                }
                            }
                        }
                    }
                    if (tam < 1001)
                    {
                        if (caixaThre.GetLineText(0) == "2" || caixaThre.GetLineText(0) == "4" || caixaThre.GetLineText(0) == "0")
                        {
                            //CRIA UM OBJETO DO TIPO GOL(Game Of Life) e faz a atribuição de valores para as variáveis
                            Gol jogoG = new Gol();
                            jogoG.GeracaoI = matriz;
                            jogoG.Qtd = qtd;
                            jogoG.Tam = tam;

                            if (caixaThre.GetLineText(0) == "2")
                            {
                                //Inicia a geração com duas threads             
                                jogoG.geracao2T();
                            }
                            else
                            {
                                if (caixaThre.GetLineText(0) == "4")
                                {
                                    //Inicia a geração com quatro threads
                                    jogoG.geracao4T();
                                }
                                else
                                {
                                    if (caixaThre.GetLineText(0) == "0")
                                    {
                                        //Inicia a geração de forma sequêncial                       
                                        jogoG.geracao();
                                    }
                                }
                            }

                            // Obtém o tempo que a rotina demorou a executar
                            TimeSpan tempo = jogoG.time.Elapsed;

                            tempoExec.Text = tempo.ToString("hh':'mm':'ss':'fff");

                            //RESULTADO - Cria arquivo com geração final
                            jogoG.result(jogoG.GeracaoI);

                            visualizar();//Abre a canvas de resultado

                        }
                        else
                        {
                            MessageBoxResult alerta = MessageBox.Show("Erro na quantidade de Threads!\nSó é possível 0, 2 ou 4 threads.", "Alerta", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                    else
                    {
                        MessageBoxResult alerta = MessageBox.Show("Tamanho da matriz ultrapassado!\nO valor máximo é de 1000.", "Alerta", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBoxResult alerta = MessageBox.Show("Arquivo não encontrado!\nVerifique se o mesmo está localizado na pasta bin\\Debug do projeto.", "Alerta", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                if (caixaDir.GetLineText(0) == "" && caixaQtd.GetLineText(0) == "")
                {
                    MessageBoxResult alerta = MessageBox.Show("Digite o nome do arquivo e a quantidade de gerações.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    if (caixaDir.GetLineText(0) == "")
                    {
                        MessageBoxResult alerta = MessageBox.Show("Digite o nome do arquivo.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        MessageBoxResult alerta = MessageBox.Show("Digite a quantidade de gerações.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

            }



        }

        public void visualizar()
        {
            string result;
            using (StreamReader arq = new StreamReader("Geracao.txt"))
            {//using não precisa fechar o arquivo
                result = arq.ReadToEnd();
            }
            folha_Geracao.Text = result;
            resultado.Visibility = Visibility.Visible;
        }

        private void button_voltar_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            resultado.Visibility = Visibility.Collapsed;
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


    }
}
