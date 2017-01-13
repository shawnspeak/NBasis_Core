using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;
using System.Linq;

namespace NBasis.Configuration
{
    public class EmptyConfiguration : IConfigurationSection
    {
        internal EmptyConfiguration()
        {

        }

        public string this[string key]
        {
            get
            {
                return null;
            }

            set
            {               
            }
        }

        public string Key
        {
            get;   
            internal set;
        }

        public string Path
        {
            get;
            internal set;
        }

        public string Value
        {
            get;
            set;
        }

        public IEnumerable<IConfigurationSection> GetChildren()
        {
            return Enumerable.Empty<IConfigurationSection>();
        }

        public class EmptyChangeToken : IChangeToken, IDisposable
        {
            public bool ActiveChangeCallbacks
            {
                get
                {
                    return false;
                }
            }

            public bool HasChanged
            {
                get
                {
                    return false;
                }
            }

            public void Dispose()
            {

            }

            public IDisposable RegisterChangeCallback(Action<object> callback, object state)
            {
                return this;
            }
        }

        public IChangeToken GetReloadToken()
        {
            return new EmptyChangeToken();
        }

        public IConfigurationSection GetSection(string key)
        {
            return new EmptyConfiguration()
            {
                Key = key
            };
        }
    }
}
