﻿using System.Windows.Forms;
using Sledge.Gui.Attributes;
using Sledge.Gui.Interfaces;

namespace Sledge.Gui.WinForms.Controls
{
    [ControlImplementation("WinForms")]
    public class WinFormsLabel : WinFormsControl, ILabel
    {
        public WinFormsLabel()
            : base(new Label())
        {

        }

        protected override Size DefaultPreferredSize
        {
            get
            {
                return new Size(50, FontSize * 2);
            }
        }
    }
}