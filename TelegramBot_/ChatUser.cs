using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using System.IO;
using System.Diagnostics;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot_
{
    public class ChatUser
    {
        public long ID { get; private set; }

        public bool IsAdded;

        public string Directory { get; set; }

        public string UserName { get; private set; }

        public ChatUser(long id, string username)
        {
            ID = id;
            UserName = username;
            IsAdded = false;
        }

        public string GetDirectory(long id)
        {
            //If there is directory
            if (Directory != null)
                return Directory;

            //If there is directory but it is not saved in user.Directory
            string[] directories = System.IO.Directory.GetDirectories(Config.PATH);
            for (int i = 0; i < directories.Length; i++)
            {
                if (directories[i].Contains(id.ToString()))
                {
                    Directory = directories[i];
                    return Directory;
                }
            }

            //If there is no directory
            Directory = System.IO.Directory.CreateDirectory($"{Config.PATH}\\{id}").FullName;
            return Directory;
        }
    }
}
