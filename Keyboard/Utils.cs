using System;
using System.Collections.Generic;
using System.Text;

namespace Keyboard
{
    static class Utils
    {
        private class _KeysComparer : IComparer<Microsoft.DirectX.DirectInput.Key>
        {

            #region IComparer<Key> Members

            public int Compare(Microsoft.DirectX.DirectInput.Key x, Microsoft.DirectX.DirectInput.Key y)
            {
                return Utils.KeyToFriendlyName(x).CompareTo(Utils.KeyToFriendlyName(y));
            }

            #endregion
        }

        private static _KeysComparer _keysComparer = new _KeysComparer();

        public static IComparer<Microsoft.DirectX.DirectInput.Key> KeysComparer
        {
            get { return _keysComparer; }
        }

        public static string KeyToFriendlyName(Microsoft.DirectX.DirectInput.Key key)
        {
            return string.Format("{0}", key);
        }

        //public static Microsoft.DirectX.DirectInput.Key KeyFromFriendlyName(string friendlyName)
        //{
        //    return (Microsoft.DirectX.DirectInput.Key)int.Parse(friendlyName.Substring(friendlyName.IndexOf(' ')));
        //}

        public static string KeysToText(Microsoft.DirectX.DirectInput.Key[] keys)
        {
            string result = "";
            if (keys != null && keys.Length > 0)
            {
                string[] keys2 = new string[keys.Length];
                for (int i = 0; i < keys.Length; i++)
                {
                    keys2[i] = Utils.KeyToFriendlyName(keys[i]);
                }
                result = string.Join(" + ", keys2);
            }
            return result;
        }
    }
}
