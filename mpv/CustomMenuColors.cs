using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace radioZiner
{
    public class CustomMenuColors : ProfessionalColorTable
    {
        Color BackgroundColor = Color.Black;
        Color ItemBackground = ColorTranslator.FromHtml("#444444");
        Color ItemBorder = ColorTranslator.FromHtml("#cccccc");


        public override Color ImageMarginGradientBegin { get { return BackgroundColor; } }
        public override Color ImageMarginGradientEnd { get { return BackgroundColor; } }
        public override Color ImageMarginGradientMiddle { get { return BackgroundColor; } }

        public override Color MenuStripGradientBegin { get { return BackgroundColor; } }
        public override Color MenuStripGradientEnd { get { return BackgroundColor; } }

        public override Color MenuBorder { get { return ItemBackground; } }
        public override Color MenuItemBorder { get { return ItemBorder; } }

        public override Color MenuItemPressedGradientBegin { get { return ItemBackground; } }
        public override Color MenuItemPressedGradientEnd { get { return ItemBackground; } }

        public override Color MenuItemSelected { get { return ItemBackground; } }

        public override Color MenuItemSelectedGradientBegin { get { return ItemBackground; } }
        public override Color MenuItemSelectedGradientEnd { get { return ItemBackground; } }

        public override Color OverflowButtonGradientBegin { get { return ItemBackground; } }
        public override Color OverflowButtonGradientEnd { get { return ItemBackground; } }
        public override Color OverflowButtonGradientMiddle { get { return ItemBackground; } }

        public override Color ToolStripDropDownBackground { get { return BackgroundColor; } }
    }
}
