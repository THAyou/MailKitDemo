using MimeKit;
using System;
using System.Collections.Generic;
using System.Text;

namespace MailKit.Model
{
    /// <summary>
    /// 邮件接收Model
    /// </summary>
    public class ReceiveMailBodyModel
    {
        public ReceiveMailBodyModel()
        {
            MailFiles = new List<string>();
            MailImg = new List<MailBodyImgModel>();
            Recipients = new List<RecipientsModel>();
            Cc = new List<RecipientsModel>();
            Bcc = new List<RecipientsModel>();
        }

        /// <summary>
        /// 邮件Id(可以通过此Id进行修改标识，删除等操作)
        /// </summary>
        public uint? MailUniqueId { get; set; }

        /// <summary>
        /// 邮件标识Id
        /// </summary>
        public string mailMessageId { get; set; }

        /// <summary>
        /// 邮件附件集合(绝对路径)
        /// </summary>
        public List<string> MailFiles { get; set; }

        /// <summary>
        /// 邮件内容图片(绝对路径)
        /// </summary>
        public List<MailBodyImgModel> MailImg { get; set; }

        /// <summary>
        /// 收件人邮箱
        /// </summary>
        public List<RecipientsModel> Recipients { get; set; }

        /// <summary>
        /// 密送
        /// </summary>
        public List<RecipientsModel> Bcc { get; set; }

        /// <summary>
        /// 抄送
        /// </summary>
        public List<RecipientsModel> Cc { get; set; }

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
        /// 邮件内容(Html)
        /// </summary>
        public string HtmlBody { get; set; }

        /// <summary>
        /// 邮件内容Text
        /// </summary>
        public string TextBody { get; set; }

        /// <summary>
        /// 邮件时间
        /// </summary>
        public DateTime MailTime { get; set; }

        /// <summary>
        /// 邮件体
        /// </summary>
        public MimeMessage MailMessage { get; set; }
    }
}
