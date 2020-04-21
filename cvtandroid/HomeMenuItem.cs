using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cvtandroid
{
    public class HomeMenuItem
    {
        public HomeMenuItem()
        {
            TargetType = typeof(UserActivity);
        }
        public int Id { get; set; }
        public string Title { get; set; }

        public Type TargetType { get; set; }
    }
}
