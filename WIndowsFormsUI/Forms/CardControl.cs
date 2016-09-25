using GameEngine.Cards;
using System.Drawing;
using System.Windows.Forms;

namespace WIndowsFormsUI.Forms
{
    class CardControl
    {
        public PictureBox PictureBox { get; private set; }
        public Point DefaultLocation { get; private set; }
        public Card Card { get; internal set; }
        public bool Empty { get; internal set; }
        public bool InCenter { get; internal set; }

        public CardControl(PictureBox pictureBox, Point defaultLocation)
        {
            PictureBox = pictureBox;
            DefaultLocation = defaultLocation;
            Card = null;
            Empty = true;
            InCenter = false;
        }
    }
}
