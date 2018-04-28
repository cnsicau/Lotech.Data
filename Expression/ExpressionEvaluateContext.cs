using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Lotech.Data.Expression {
    /// <summary>
    /// 表达式计算上下文
    /// </summary>
    public class ExpressionEvaluateContext {
        #region Constructor
        /// <summary>
        /// 构造上下文
        /// </summary>
        /// <param name="database"></param>
        /// <param name="descriptorProvider"></param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public ExpressionEvaluateContext(IDatabase database, Providers.IDescriptorProvider descriptorProvider) {
            if (database == null)
                throw new ArgumentNullException("database");
            if (descriptorProvider == null)
                throw new ArgumentNullException("descriptorProvider");
            this.Database = database;
            this.DescriptionProvider = descriptorProvider;
        }
        #endregion

        #region Members

        /// <summary>
        /// 此上下文中的数据库
        /// </summary>
        public IDatabase Database {
            get;
            private set;
        }

        /// <summary>
        /// 描述符提供者
        /// </summary>
        public Providers.IDescriptorProvider DescriptionProvider {
            get;
            private set;
        }
        #endregion
    }
}
