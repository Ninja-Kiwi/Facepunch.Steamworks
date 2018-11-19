using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using SteamNative;

namespace Facepunch.Steamworks
{
    public class User : IDisposable
    {
        internal Client client;
        internal Dictionary<string, string> richPresence = new Dictionary<string, string>();

        internal User( Client c )
        {
            client = c;
        }

        public void Dispose()
        {
            client = null;
        }

        /// <summary>
        /// Find a rich presence value by key for current user. Will be null if not found.
        /// </summary>
        public string GetRichPresence( string key )
        {
            if ( richPresence.TryGetValue( key, out var val ) )
                return val;

            return null;
        }

        /// <summary>
        /// Sets a rich presence value by key for current user.
        /// </summary>
        public bool SetRichPresence( string key, string value )
        {
            richPresence[key] = value;
            return client.native.friends.SetRichPresence( key, value );
        }

        /// <summary>
        /// Clears all of the current user's rich presence data.
        /// </summary>
        public void ClearRichPresence()
        {
            richPresence.Clear();
            client.native.friends.ClearRichPresence();
        }

        /// <summary>
        /// Get the local storage folder for current Steam account to write application data, e.g. save games, configs etc.
        /// </summary>
        public string GetUserDataFolder()
        {
            return client.native.user.GetUserDataFolder();
        }
    }
}
