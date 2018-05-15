using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lotech.Data.Descriptors
{
    class MemberDescriptorContainer<TEntity> where TEntity : class
    {
        public MemberDescriptorContainer(string name, string parameterName, DbType dbType
            , Func<TEntity, object> getter, Action<TEntity, object> setter = null)
        {
            Name = name;
            ParameterName = parameterName;
            DbType = dbType;
            Getter = getter;
            Setter = setter;
        }

        public MemberDescriptorContainer(string name, string parameterName)
        {
            Name = name;
            ParameterName = parameterName;
        }

        public string Name { get; }

        public string ParameterName { get; }

        public DbType DbType { get; }

        public Func<TEntity, object> Getter { get; }

        public Action<TEntity, object> Setter { get; }
    }
}
