using System;

namespace General
{
    public class Notification
    {
        private readonly Action _onClick;
        public string Title { get; }

        public Notification(string title, Action onClick)
        {
            Title = title;
            
            _onClick = onClick;
        }

        public void Click()
        {
            _onClick();
        }
    }
}
