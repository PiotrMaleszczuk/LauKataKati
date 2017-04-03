using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DaraLibrary;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace WpfApplication2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Image[,] images = new Image[6, 5];
        bool putpawns = true;
        Board board = new Board();
        bool click = false;
        bool canDelete = false;
        int x;
        int y;

        public void checkpawn()
        {
            Char[,] a = board.printBoard();
            for (int i = 0; i < 6; i++)
            {

                for (int j = 0; j < 5; j++)
                {
                    if (a[i, j] == 'M')
                    {
                        FileStream stream = new FileStream("blackb.png", FileMode.Open, FileAccess.Read);
                        BitmapImage src = new BitmapImage();
                        src.BeginInit();
                        src.StreamSource = stream;
                        src.EndInit();
                        images[i, j].Source = src;
                    }
                    if (a[i, j] == 'm')
                    {
                        FileStream stream = new FileStream("blackb.png", FileMode.Open, FileAccess.Read);
                        BitmapImage src = new BitmapImage();
                        src.BeginInit();
                        src.StreamSource = stream;
                        src.EndInit();
                        images[i, j].Source = src;
                    }
                    if (a[i, j] == '_')
                    {
                        FileStream stream = new FileStream("p.png", FileMode.Open, FileAccess.Read);
                        BitmapImage src = new BitmapImage();
                        src.BeginInit();
                        src.StreamSource = stream;
                        src.EndInit();
                        images[i, j].Source = src;
                    }
                    if (a[i, j] == 'I')
                    {
                        FileStream stream = new FileStream("redb.png", FileMode.Open, FileAccess.Read);
                        BitmapImage src = new BitmapImage();
                        src.BeginInit();
                        src.StreamSource = stream;
                        src.EndInit();
                        images[i, j].Source = src;
                    }
                    if (a[i, j] == 'i')
                    {
                        FileStream stream = new FileStream("redb.png", FileMode.Open, FileAccess.Read);
                        BitmapImage src = new BitmapImage();
                        src.BeginInit();
                        src.StreamSource = stream;
                        src.EndInit();
                        images[i, j].Source = src;
                    }
                }
            }
        }
        public void makeboard()
        {
            
            int x = 0;
            int y = 110;
            int wh = 50;
            int h=12;
            for (int i = 0; i < 6; i++)
            {

                for (int j = 0; j < 5; j++)
                {
                    images[i, j] = new Image();
                    images[i, j].Margin = new Thickness(y,x,0,0) ;
                    images[i,j].HorizontalAlignment= System.Windows.HorizontalAlignment.Left;
                    images[i, j].VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    images[i, j].MouseLeftButtonDown += new MouseButtonEventHandler(button_Click);
                    images[i, j].Tag = i+" "+j +" a";
                    images[i, j].Width = wh;
                    images[i, j].Height = wh;
                    images[i, j].Stretch = Stretch.UniformToFill;
                    //Uri uri = new Uri("p.jpg", UriKind.Relative);
                    //images[i, j].Source = new BitmapImage(uri);
                    FileStream stream = new FileStream("p.png", FileMode.Open, FileAccess.Read);
                    BitmapImage src = new BitmapImage();
                    src.BeginInit();
                    src.StreamSource = stream;
                    src.EndInit();
                    images[i, j].Source = src;

                    b.Children.Add(images[i, j]);
                    y += wh+10;
                    
                }
                x += wh-16;
                y = 110-h;
                wh += 5;
                h += 12;
            }
        }


        void button_Click(object sender, MouseButtonEventArgs e)
        {
            if (putpawns == true)
            {
                if (board.getMinSidePawnCount() < 12)
                {
                    BoardSide who = board.getTurnOwner();
                    String a = Convert.ToString(who);
                    if (a == "MAX")
                    {
                        String b = (String)(sender as Image).Tag;
                        String[] bb = b.Split(' ');

                        int returned = board.putPawn(new BoardSquare(Convert.ToInt32(bb[1]), Convert.ToInt32(bb[0])), new Pawn(BoardSide.MAX));
                        checkpawn();
                        // MessageBox.Show(string.Format("Kliknęłeś pionek na pozycji: {0}", (sender as Image).Tag));
                        if (returned == 2)
                        {
                            MessageBox.Show("Niedozwolony ruch!");
                        }
                        else
                            if (returned == 1)
                            {
                                MessageBox.Show("Pionek na tym polu już istnieje!");

                            }
                            else
                            {


                                board.putPawn(new Pawn(BoardSide.MIN));
                                checkpawn();
                            }

                    }
                }
                if (board.getMinSidePawnCount() == 12)
                    putpawns = false;
            }
            else
            {
                int testtocheck = 0;
                int checkStateGame = board.isEnd(testtocheck);
                if (checkStateGame == 0)
                {
                    int returned = -1;
                    int returneed = -1;
                    if (click == true)
                    {
                        String b = (String)(sender as Image).Tag;
                        String[] bb = b.Split(' ');

                        Move mv = new Move(board, board.getBoardSquare(x, y), board.getBoardSquare(Convert.ToInt32(bb[0]), Convert.ToInt32(bb[1])));
                        returned = mv.executeMove();
                        if (returned == 1)
                            MessageBox.Show("Niemożliwy ruch!");

                        checkpawn();



                        click = false;
                        this.images[x, y].Tag = x + " " + y + " " + "a";


                    }
                    if (click == false)
                    {

                        String b = (String)(sender as Image).Tag;
                        String[] bb = b.Split(' ');
                        //MessageBox.Show(images[Convert.ToInt32(bb[0]), Convert.ToInt32(bb[1])].Tag.ToString());
                        this.images[Convert.ToInt32(bb[0]), Convert.ToInt32(bb[1])].Tag = bb[0] + " " + bb[1] + " " + "b";
                        // MessageBox.Show(images[Convert.ToInt32(bb[0]), Convert.ToInt32(bb[1])].Tag.ToString());
                        x = Convert.ToInt32(bb[0]);
                        y = Convert.ToInt32(bb[1]);
                        click = true;
                    }


                    if (Board.delete == true)
                    {

                        if (canDelete == true)
                        {

                            String b = (String)(sender as Image).Tag;
                            String[] bb = b.Split(' ');
                            returneed = deletePawn(Convert.ToInt32(bb[0]), Convert.ToInt32(bb[1]));
                            if (returneed == 1)
                                MessageBox.Show("Tego pionka nie można usunąć!");
                        }
                        else
                        {
                            if (board.getTurnOwner().ToString() == "MAX")
                            {
                                MessageBox.Show("Usuń pionek!");
                                canDelete = true;
                            }
                            else
                            {
                                List<BoardSquare> AllMaxPawns = board.findAllMax();
                                Random rand = new Random();
                                int a = rand.Next(AllMaxPawns.Count - 1);
                                int i = 1;
                                while (i == 1)
                                {
                                    if (i == 1)
                                    {
                                        i = deletePawn(AllMaxPawns[a].getRowCoord(), AllMaxPawns[a].getColumnCoord());
                                        a = rand.Next(AllMaxPawns.Count - 1);
                                    }
                                }

                            }


                        }




                    }
                    bool enemydel = false;
                    if (Board.delete != true)
                    {

                        if (returned == 0 || returneed == 0)
                        {
                            board.toggleTurnOwner();
                            Move nextMove = Minmax.getNextMove(board);
                            //Board.delete = false;
                            int returnedd = nextMove.executeMove();
                            while (returnedd == 1)
                            {
                                nextMove = Minmax.getNextMove(board);
                                Board.delete = false;
                                returnedd = nextMove.executeMove();
                            }
                            board = nextMove.getBoard();
                            board.toggleTurnOwner();

                            checkpawn();
                            if (Board.delete == true)
                                enemydel = true;
                        }
                    }
                    if (Board.delete == true)
                    {

                        if (enemydel == true)
                        {

                            if (board.getTurnOwner().ToString() == "MAX")
                            {
                                List<BoardSquare> AllMaxPawns = board.findAllMax();
                                Random rand = new Random();
                                int a = rand.Next(AllMaxPawns.Count - 1);
                                int i = 1;
                                while (i == 1)
                                {
                                    if (i == 1)
                                    {
                                        i = deletePawn(AllMaxPawns[a].getRowCoord(), AllMaxPawns[a].getColumnCoord());
                                        a = rand.Next(AllMaxPawns.Count - 1);
                                    }
                                }
                                enemydel = false;
                            }


                        }





                    }






                }
                if (checkStateGame == 1)
                    MessageBox.Show("KONIEC! WYGRAŁEŚ!!!!");
                if (checkStateGame == 2)
                    MessageBox.Show("KONIEC! PRZEGRAŁEŚ...");

            }
        }



        public MainWindow()
        {
            InitializeComponent();
            
            //BoardSquare bs = new BoardSquare(0, 0);
            //board.putPawn(bs, new Pawn(BoardSide.MIN));
            //bs = new BoardSquare(1, 1);
            //board.putPawn(bs, new Pawn(BoardSide.MAX));
            //board.putPawn(new Pawn(BoardSide.MAX));
            //board.putPawn(new Pawn(BoardSide.MAX));
            //board.putPawn(new Pawn(BoardSide.MAX));
            //board.putPawn(new Pawn(BoardSide.MAX));
            //board.putPawn(new Pawn(BoardSide.MAX));
            //board.putPawn(new Pawn(BoardSide.MIN));
            //board.putPawn(new Pawn(BoardSide.MIN));
            //board.putPawn(new Pawn(BoardSide.MIN));
            //board.putPawn(new Pawn(BoardSide.MIN));
            //board.putPawn(new Pawn(BoardSide.MIN));

            makeboard();
           // checkpawn();
            

        }
        public int deletePawn(int x, int y)
        {
            BoardSquare bs = board.getBoardSquare(x, y);
            if (bs.getCurrentObject().getLocked() == false)
            {
                bs.takeCurrentObject();
                checkpawn();
                Board.delete = false;
                canDelete = false;
                return 0;
            }
            else
            {
                return 1;

               

            }
        }
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            
            
            Move nextMove = Minmax.getNextMove(board);
            Board.delete = false;
            int returned=nextMove.executeMove();
            while (returned == 1)
            {
                nextMove = Minmax.getNextMove(board);
                Board.delete = false;
                returned = nextMove.executeMove();
            }
            board = nextMove.getBoard();
            checkpawn();
        }

        private void image1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (textBlock4.Visibility == Visibility.Visible)
            {
                textBlock2.Visibility = Visibility.Hidden;
                textBlock3.Visibility = Visibility.Hidden;
                textBlock4.Visibility = Visibility.Hidden;
                
            }
            else
            {
                textBlock5.Text = "";
                textBlock2.Visibility = Visibility.Visible;
                textBlock3.Visibility = Visibility.Visible;
                textBlock4.Visibility = Visibility.Visible;
            }
        }

        private void documentViewer1_PageViewsChanged(object sender, EventArgs e)
        {

        }

        private void image2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (textBlock5.Text == "")
            {
                textBlock2.Visibility = Visibility.Hidden;
                textBlock3.Visibility = Visibility.Hidden;
                textBlock4.Visibility = Visibility.Hidden;
                string txt = "1. Zasady gry \n • gracze decydują kto zaczyna pierwszy,\n• plansza do gry ma wielkość 5x6,\n• każdy z graczy posiada 12 pionków,\n• pierwsza faza polega na ułożeniu naprzemiennie wszystkich pionków na planszy,\n• faza druga polega na naprzemiennym przesuwaniu pionków w celu uzyskania rzędu,pionowego lub poziomego o długości 3,\n• jeżeli graczowi uda się uzyskać rząd pionków o długości 3 to może on pozbawić drugiego,\ngracza jednego pionka,\n• rzędy o długości 3 podczas pierwszej fazy nie liczą się (nie można pozbawić drugiego gracza jego pionka),\n• gracz który nie może uzyskać już rzędu o długości 3 za pomocą swoich pionków przegrywa.";
                textBlock5.Text = txt;
            }
            else
            {
                textBlock5.Text = "";
            }
        }

        private void image2_MouseEnter(object sender, MouseEventArgs e)
        {
            
        }

        private void button1_Click_1(object sender, RoutedEventArgs e)
        {
            Process.Start(Application.ResourceAssembly.Location);

            Application.Current.Shutdown();
        }


        
       
    }
}
