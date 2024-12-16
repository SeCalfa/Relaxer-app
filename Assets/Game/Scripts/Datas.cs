using System;
using System.Collections.Generic;

namespace Game.Scripts
{
    [Serializable]
    public class Title
    {
        public string name;
        public int year;
        public TitleStatus titleStatus;
        public int pictureId;
    }

    [Serializable]
    public class Serial : Title
    {
        public Dictionary<int, bool> Seasons = new();
    }

    [Serializable]
    public class Account
    {
        // Server data
        public string name;
        public List<Title> films = new();
        public List<Serial> serials = new();
    }
}

