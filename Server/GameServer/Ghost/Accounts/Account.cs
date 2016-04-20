﻿using Server.Ghost.Characters;
using Server.Common.Data;
using Server.Common.IO;
using Server.Net;
using System;
using System.Collections.Generic;
using System.Data;

namespace Server.Ghost.Accounts
{
    public sealed class Account
    {
        public Client Client { get; private set; }

        public List<Character> Characters { get; set; }

        public int ID { get; private set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string Pin { get; set; }
        public DateTime Birthday { get; set; }
        public DateTime Creation { get; set; }
        public int Gender { get; set; }
        public int LoggedIn { get; set; }
        public int Banned { get; set; }
        public int Master { get; set; }
        public int CashPoint { get; set; }

        private bool Assigned { get; set; }

        public Account(Client client)
        {
            this.Client = client;
        }

        public void Load(string username)
        {
            dynamic datum = new Datum("accounts");

            try
            {
                datum.Populate("Username = '{0}'", username);
            }
            catch (RowNotInTableException)
            {
                throw new NoAccountException();
            }

            this.ID = datum.id;
            this.Assigned = true;

            this.Username = datum.userName;
            this.Password = datum.password;
            this.Salt = datum.salt;
            this.Pin = datum.pin;
            this.Birthday = datum.birthday;
            this.Creation = datum.creation;
            this.Gender = datum.gender;
            this.LoggedIn = datum.isLoggedIn;
            this.Banned = datum.isBanned;
            this.Master = datum.isMaster;
            this.CashPoint = datum.cashPoint;
        }

        public void Save()
        {
            dynamic datum = new Datum("accounts");

            datum.userName = this.Username;
            datum.password = this.Password;
            datum.salt = this.Salt;
            datum.pin = this.Pin;
            datum.birthday = this.Birthday;
            datum.creation = this.Creation;
            datum.gender = this.Gender;
            datum.isLoggedIn = this.LoggedIn;
            datum.isBanned = this.Banned;
            datum.isMaster = this.Master;
            datum.cashPoint = this.CashPoint;

            if (this.Assigned)
            {
                datum.Update("ID = '{0}'", this.ID);
            }
            else
            {
                datum.Insert();

                this.ID = Database.Fetch("accounts", "ID", "Username = '{0}'", this.Username);
                this.Assigned = true;
            }

            Log.Inform("Saved account '{0}' to database.", this.Username);
        }
    }
}