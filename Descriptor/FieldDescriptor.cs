using System;
using System.Data;
using System.Reflection;

namespace Lotech.Data.Descriptor {
    /// <summary>
    /// 字段描述符
    /// </summary>
    public class FieldDescriptor {
        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dbType"></param>
        /// <param name="size"></param>
        /// <param name="entityType">实体类型</param>
        /// <param name="memberName">成员名称</param>
        public FieldDescriptor(string name, DbType dbType, int size, Type entityType, string memberName)
            : this(name, dbType, size, entityType) {
            Member = entityType.GetProperty(memberName);
        }

        /// <summary>
        /// 通过指定名称，实体类型构造描述符
        /// </summary>
        /// <param name="name"></param>
        /// <param name="entityType">实体名称</param>
        public FieldDescriptor(string name, Type entityType) {
            this.Name = name;
            this.Member = entityType.GetProperty(name);
            this.DbType = DbType.Object;
            this.Size = -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dbType"></param>
        /// <param name="size"></param>
        /// <param name="entityType">实体类型</param>
        public FieldDescriptor(string name, DbType dbType, int size, Type entityType)
            : this(name, dbType, size, (PropertyInfo)null) {
            Member = entityType.GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        }
        /// <summary>
        /// 构造指定的描述符
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dbType"></param>
        /// <param name="size"></param>
        /// <param name="member"></param>
        public FieldDescriptor(string name, DbType dbType, int size, PropertyInfo member) {
            this.Name = name;
            this.DbType = dbType;
            this.Size = size;
            this.Member = member;
        }

        /// <summary>
        /// 构造描述符
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dbType"></param>
        /// <param name="size"></param>
        public FieldDescriptor(string name, DbType dbType, int size)
            : this(name, dbType, size, (PropertyInfo)null) {
        }

        /// <summary>
        /// 构造描述符
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dbType"></param>
        public FieldDescriptor(string name, DbType dbType)
            : this(name, dbType, -1, (PropertyInfo)null) {
        }

        /// <summary>
        /// 构造描述符
        /// </summary>
        /// <param name="name"></param>
        public FieldDescriptor(string name)
            : this(name, DbType.Object, -1, (PropertyInfo)null) {
        }

        /// <summary>
        /// 默认构造
        /// </summary>
        public FieldDescriptor() {
        }
        #endregion

        #region Members
        /// <summary>
        /// 是否主键
        /// </summary>
        public bool IsKey { get; set; }

        /// <summary>
        /// 主键生成类型
        /// </summary>
        public Lotech.Data.Attributes.KeyType KeyType { get; set; }

        /// <summary>
        /// 字段名称
        /// </summary>
        public string Name {
            get;
            set;
        }
        /// <summary>
        /// 字段类型
        /// </summary>
        public DbType DbType {
            get;
            set;
        }
        /// <summary>
        /// 字段大小
        /// </summary>
        public int Size {
            get;
            set;
        }

        /// <summary>
        /// 成员
        /// </summary>
        public PropertyInfo Member {
            get;
            set;
        }
        #endregion
    }
}
