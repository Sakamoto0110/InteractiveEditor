using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Services
{
    public interface _IServiceProvider
    {
        
        T Request<T>() where T : _IService, new();
    }
    public class IOBServiceProvider : _IServiceProvider
    {
        protected static byte[] _parms = null;
        
        
        
        public readonly Inspector Owner;
        
        public T Request<T>() where T : _IService, new()
        {
           
                

            return new T()
            {
                Provider = this,
            };
        }

        public IOBServiceProvider(Inspector owner) => Owner = owner;
    }
}

