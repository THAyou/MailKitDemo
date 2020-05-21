using System;
using System.Collections.Generic;
using System.Text;

namespace MailKit.Model
{
    /// <summary>
    /// 邮件内容图片Model
    /// </summary>
    public class MailBodyImgModel
    {
        /// <summary>
        /// 图片cid
        /// </summary>
        public string cid { get; set; }

        /// <summary>
        /// 图片保存地址(绝对路径)
        /// </summary>
        public string imgSrc { get; set; }
    }
}
