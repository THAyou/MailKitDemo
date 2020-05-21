using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Text;

namespace MailKit.Model
{
    /// <summary>
    /// 发送邮件Model
    /// </summary>
    public class SendMailBodyModel
    {
        /// <summary>
        /// 邮件内容类型
        /// </summary>
        public TextFormat MailBodyType { get; set; } = TextFormat.Html;

        /// <summary>
        /// 邮件附件集合(绝对路径)
        /// </summary>
        public List<string> MailFiles { get; set; }

        /// <summary>
        /// 收件箱
        /// </summary>
        public List<RecipientsModel> Recipients { get; set; }

        /// <summary>
        /// 抄送
        /// </summary>
        public List<string> Cc { get; set; }

        /// <summary>
        /// 密送
        /// </summary>
        public List<string> Bcc { get; set; }

        /// <summary>
        /// 发件人
        /// </summary>
        public string Sender { get; set; }

        /// <summary>
        /// 发件人地址
        /// </summary>
        public string SenderAddress { get; set; }

        /// <summary>
        /// 邮件主题
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 邮件内容
        /// </summary>
        public string Body { get; set; }
    }



}
