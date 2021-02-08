using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Services
{
    public interface _IServiceProvider
    {
        byte[] Bytes { get; set; }
        void EditBytes(byte[] newBytes);
        T Request<T>(byte[] byteFlags = null) where T : _IService, new();
    }
    public class IOBServiceProvider : _IServiceProvider
    {
        protected static byte[] _parms = null;
        
        public byte[] Bytes
        {
            get
            {
                if(_parms == null)                
                    _parms = new byte[4]
                    {
                        0x1, // Long Querries -> 0x01
                        0x0,
                        0x0,
                        0x0,
                    };                
                return _parms;
            }
            set => _parms = value;


        }
        public void EditBytes(byte[] newBytes)
        {
            byte[] oldBytes = Bytes;
            for(int i = 0; i < newBytes.Length; i++)
            {
                oldBytes[i] = newBytes[i];
            }
            Bytes = oldBytes;
        }
        public readonly InteractiveEditor Owner;
        
        public T Request<T>(byte[] byteFlags = null) where T : _IService, new()
        {
            if(byteFlags != null)
                EditBytes(byteFlags);
            
                

            return new T()
            {
                Provider = this,
            };
        }

        public IOBServiceProvider(InteractiveEditor owner) => Owner = owner;
    }
}

