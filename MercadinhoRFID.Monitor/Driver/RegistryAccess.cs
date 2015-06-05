using System;
using Microsoft.Win32;

namespace MercadinhoRFID.Monitor.Driver
{
    public class RegistryAccess
    {
        public virtual object Read(string key, Type type)
        {
            var rk = BaseRegistryKey;
            var sk1 = rk.OpenSubKey(SubKey);
            if (sk1 != null)
            {
                var value = sk1.GetValue(key.ToUpper());
                if (value != null)
                {
                    return Convert.ChangeType(value, type);
                }
            }
            return null;
        }

        public virtual bool Write(string key, object value)
        {
            var rk = BaseRegistryKey;
            var sk1 = rk.CreateSubKey(SubKey);
            var result = sk1 != null;
            if (result)
                sk1.SetValue(key.ToUpper(), value.ToString());
            return result;
        }

        protected RegistryKey BaseRegistryKey = Registry.LocalMachine;
        protected string SubKey = "SOFTWARE\\" + "MERCADINHO.RFID";
    }
}