using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QL_MVALab.ViewModel
{
    public class NavItem
    {
        public string Title { get; }
        public string Icon { get; }            // glyph Segoe MDL2
        public Func<object> CreateViewModel { get; }

        public NavItem(string title, string icon, Func<object> vmFactory)
        {
            Title = title;
            Icon = icon;
            CreateViewModel = vmFactory;
        }
    }
}
