using System;
using Gdk;
using Gtk;
using Sledge.Gui.Events;
using Sledge.Gui.Gtk.Containers;
using Sledge.Gui.Gtk.Controls;
using Sledge.Gui.Interfaces.Containers;
using Sledge.Gui.Interfaces.Shell;
using Window = Gtk.Window;

namespace Sledge.Gui.Gtk.Shell
{
    public class GtkWindow : GtkControl, IWindow
    {
        protected GtkCell ContainerWrapper;

        protected Window Window;

        private bool _autoSize;
        public bool AutoSize
        {
            get { return _autoSize; }
            set
            {
                _autoSize = value;
                OnPreferredSizeChanged();
            }
        }

        public string Title
        {
            get { return Window.Title; }
            set { Window.Title = value; }
        }

        public ICell Container
        {
            get { return ContainerWrapper; }
        }

        public GtkWindow() : base(new Window(""))
        {
            Window = (Window) Control;
            Window.SetSizeRequest(800, 600);
            CreateWrapper();
            Window.SizeAllocated += OnResize;
            Window.DeleteEvent += (o, args) => args.RetVal = OnDeleteEvent(args.Event);
        }

        protected virtual void CreateWrapper()
        {
            ContainerWrapper = new GtkCell(Window);
            ContainerWrapper.PreferredSizeChanged += ContainerPreferredSizeChanged;
        }

        private void ContainerPreferredSizeChanged(object sender, EventArgs e)
        {
            OnPreferredSizeChanged();
        }

        private void OnResize(object sender, EventArgs eventArgs)
        {
            OnActualSizeChanged();
        }

        protected override void OnPreferredSizeChanged()
        {
            if (_autoSize)
            {
                var ps = PreferredSize;
                Window.SetSizeRequest(ps.Width, ps.Height);
            }
            base.OnPreferredSizeChanged();
        }

        public void Open()
        {
            Window.Show();
        }

        public void Close()
        {
            Window.Hide();
        }

        public event EventHandler WindowLoaded
        {
            add { Window.Realized += value; }
            remove { Window.Realized -= value; }
        }

        public event EventHandler<HandledEventArgs> WindowClosing;

        public event EventHandler WindowClosed;

        protected bool OnDeleteEvent(Event evnt)
        {
            if (WindowClosing != null)
            {
                var hea = new HandledEventArgs();
                WindowClosing(this, hea);
                if (hea.Handled) return true;
            }
            Application.Quit();
            if (WindowClosed != null)
            {
                WindowClosed(this, EventArgs.Empty);
            }
            return false;
        }
    }
}