using System;

namespace Lotech.Data.Descriptor {
    /// <summary>
    /// 实体描述符
    /// </summary>
    public class EntityDescriptor {
        #region 构造
        /// <summary>
        /// 
        /// </summary>
        public EntityDescriptor() {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        public EntityDescriptor(string table) {
            this.Table = table;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityType"></param>
        public EntityDescriptor(Type entityType) {
            this.Table = entityType.Name;
            this.EntityType = entityType;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="entityType"></param>
        public EntityDescriptor(string table, Type entityType) {
            this.Table = table;
            this.EntityType = entityType;
        }

        #endregion

        #region Members
        /// <summary>
        /// 实体类型
        /// </summary>
        public Type EntityType {
            get;
            set;
        }

        /// <summary>
        /// 实体对应的物理表名称
        /// </summary>
        public string Table {
            get;
            set;
        }

        /// <summary>
        /// 主键集合
        /// </summary>
        public FieldDescriptor[] PrimaryKeys {
            get;
            set;
        }

        /// <summary>
        /// 字段(包含其对应的属性信息)
        /// </summary>
        public FieldDescriptor[] Fields {
            get;
            set;
        }
        #endregion
    }
}
