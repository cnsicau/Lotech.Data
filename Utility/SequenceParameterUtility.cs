using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lotech.Data.Utility {
    /// <summary>
    /// 连续参数命名工具类
    /// </summary>
    static internal class SequenceParameterUtility {
        [ThreadStatic]
        private static int sequence = 0;

        /// <summary>
        /// 获取下一个有效的参数名
        /// </summary>
        /// <returns></returns>
        static public string GetNextParameterName() {
            if (++sequence == int.MaxValue) {
                sequence = 0;
            }
            return string.Concat("LDSP" + sequence.ToString("X"));
        }
    }
}
