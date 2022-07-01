using System;

namespace General
{
    public class Notification
    {
        public string Title { get; }

        private readonly Action _onClick;

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
