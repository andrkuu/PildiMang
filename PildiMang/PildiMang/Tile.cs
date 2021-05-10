using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace PildiMang
{
	public class Tile
	{
        const string UrlPrefix = "https://www.tlu.ee/~tluur/img/";
        public Tile(int row, int col, int randomNumber)
        {
            Row = row;
            Col = col;

            TileView = new ContentView

            {
                Padding = new Thickness(1),

                // Koostab url ja võtab sealt pildi
                Content = new Image
                {
                    Source = ImageSource.FromUri(new Uri(UrlPrefix + randomNumber + "/" + "Bitmap" + row + col + ".jpg"))
                }
            };

            // Lisame uue ruudu
            Dictionary.Add(TileView, this);
        }

        public static Dictionary<View, Tile> Dictionary { get; } = new Dictionary<View, Tile>();

        public int Row { set; get; }

        public int Col { set; get; }

        public View TileView { private set; get; }
    }
}