using System;
using System.Collections.Generic;
using System.Text;

namespace MailKit.Model
{
    /// <summary>
    /// 邮件服务器配置
    /// </summary>
    public class MailServerConfigModel
    {
        /// <summary>
        /// 邮箱服务器地址
        /// </summary>
        public string ServerHost { get; set; }

        /// <summary>
        /// 邮箱服务器端口
        /// </summary>
        public int ServerPort { get; set; }

        /// <summary>
        /// 是否启用IsSsl
        /// </summary>
        public bool IsSsl { get; set; } = true;

        /// <summary>
        /// 邮箱账号
        /// </summary>
        public string EmailAccount { get; set; }

        /// <summary>
        /// 邮箱密码
        /// </summary>
        public string EmailPassword { get; set; }

    }
}
