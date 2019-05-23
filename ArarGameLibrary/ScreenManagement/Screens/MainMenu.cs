﻿using ArarGameLibrary.Manager;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArarGameLibrary.ScreenManagement.Screens
{
    public class MainMenu : Menu
    {
        public override void Initialize()
        {
            Global.ChangeGameWindowTitle("Main Menu");
        }

        public override bool Load()
        {
            var collection = new Dictionary<string, Action>();

            collection.Add("New",() =>
            {
                DisableThenAddNew(new GameScreen());
            });

            collection.Add("Settings", () =>
            {
                DisableThenAddNew(new SettingsMenu());
            });

            collection.Add("Exit", () =>
            {
                Global.OnExit = true;
            });


            Components.AddRange(Button.Sort(collection));


            return true && base.Load();
        }
    }
}