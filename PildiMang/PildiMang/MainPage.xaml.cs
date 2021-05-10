using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PildiMang
{
    public partial class MainPage : ContentPage
    {
        
        static readonly int NUM = 4;
        // Mitu rida ja veergu

        Tile[,] tiles;

        int emptyRow = NUM - 1;
        int emptyCol = NUM - 1;

        double tileSize;
        bool isBusy;

        public async void StartPuzzle()
        {
            int randomNumber = new Random().Next(1, 9);

            // Juhuslik number mille järgi tuleb pilt

            tiles = new Tile[NUM, NUM];
            if (absoluteLayout.Children.Count > 0)
            {
                absoluteLayout.Children.Clear();     
            }

            absoluteLayout.Children.Clear();

            // Tühjenda vana grid

            for (int row = 0; row < NUM; row++)
            {
                for (int col = 0; col < NUM; col++)
                {
                    if (row == NUM - 1 && col == NUM - 1)
                        break;

                    Tile tile = new Tile(row, col, randomNumber);

                    TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
                    tapGestureRecognizer.Tapped += OnTileTapped;
                    tile.TileView.GestureRecognizers.Add(tapGestureRecognizer);
                    // Lisa pildile sündmus

                    tiles[row, col] = tile;
                    absoluteLayout.Children.Add(tile.TileView);
                }
            }

            isBusy = false;
        }

        public MainPage()
        {
            InitializeComponent();

            StartPuzzle();
        }

        void OnContainerSizeChanged(object sender, EventArgs args)
        {
            View container = (View)sender;
            double width = container.Width;
            double height = container.Height;

            if (width <= 0 || height <= 0)
                return;

            stackLayout.Orientation = (width < height) ? StackOrientation.Vertical :
                                                         StackOrientation.Horizontal;

            // Ruudustiku suurus vastavalt ekraanile
            tileSize = Math.Min(width, height) / NUM;
            absoluteLayout.WidthRequest = NUM * tileSize;
            absoluteLayout.HeightRequest = NUM * tileSize;

            foreach (View fileView in absoluteLayout.Children)
            {
                Tile tile = Tile.Dictionary[fileView];

                AbsoluteLayout.SetLayoutBounds(fileView, new Rectangle(tile.Col * tileSize,
                                                                       tile.Row * tileSize,
                                                                       tileSize,
                                                                       tileSize));
            }
        }

        async void OnTileTapped(object sender, EventArgs args)
        {
            if (isBusy)
                return;

            isBusy = true;

            View tileView = (View)sender;
            Tile tappedTile = Tile.Dictionary[tileView];

            await ShiftIntoEmpty(tappedTile.Row, tappedTile.Col);
            isBusy = false;
        }

        async Task ShiftIntoEmpty(int tappedRow, int tappedCol, uint length = 100)
        {
            // Veergude liigutamine
            if (tappedRow == emptyRow && tappedCol != emptyCol)
            {
                int inc = Math.Sign(tappedCol - emptyCol);
                int begCol = emptyCol + inc;
                int endCol = tappedCol + inc;

                for (int col = begCol; col != endCol; col += inc)
                {
                    await AnimateTile(emptyRow, col, emptyRow, emptyCol, length);
                }
            }
            // Ridade liigutamine
            else if (tappedCol == emptyCol && tappedRow != emptyRow)
            {
                int inc = Math.Sign(tappedRow - emptyRow);
                int begRow = emptyRow + inc;
                int endRow = tappedRow + inc;

                for (int row = begRow; row != endRow; row += inc)
                {
                    await AnimateTile(row, emptyCol, emptyRow, emptyCol, length);
                }
            }
        }

        async Task AnimateTile(int row, int col, int newRow, int newCol, uint length)
        {
            // Leia õige ruut
            Tile tile = tiles[row, col];
            View tileView = tile.TileView;

            // Kuhu liigutada
            Rectangle rect = new Rectangle(emptyCol * tileSize,
                                           emptyRow * tileSize,
                                           tileSize,
                                           tileSize);

            // Liiguta ruutu sujuvalt
            await tileView.LayoutTo(rect, length);

            AbsoluteLayout.SetLayoutBounds(tileView, rect);

            // Määrame uute ruutude väärtused õigeks
            tiles[newRow, newCol] = tile;
            tile.Row = newRow;
            tile.Col = newCol;
            tiles[row, col] = null;
            emptyRow = row;
            emptyCol = col;
        }

        async void OnRandomizeButtonClicked(object sender, EventArgs args)
        {
            Button button = (Button)sender;
            button.IsEnabled = false;
            Random rand = new Random();

            isBusy = true;

            // Liiguta ruutu 100x

            for (int i = 0; i < 100; i++)
            {
                await ShiftIntoEmpty(rand.Next(NUM), emptyCol, 25);
                await ShiftIntoEmpty(emptyRow, rand.Next(NUM), 25);
            }
            button.IsEnabled = true;

            isBusy = false;
        }

        async void OnChangePictureButtonClicked(object sender, EventArgs args)
        {
            (Application.Current).MainPage = new NavigationPage(new MainPage());
        }
    }
}