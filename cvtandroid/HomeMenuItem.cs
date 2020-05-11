using System;

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
