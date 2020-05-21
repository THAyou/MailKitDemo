using MailKit.ImpI;
using MailKit.Interface;
using MailKit.Model;
using System;
using System.Collections.Generic;

namespace MailKitDemo
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello MailKit!");
            SendMail();
            ReceiceMail();
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        public static void SendMail()
        {
            ISendMailKit _sendMailKit = new SendMailKit();
            _sendMailKit.Connection(new MailServerConfigModel
            {
                ServerHost = "邮箱服务器地址",
                ServerPort = 587,//端口
                IsSsl = false,
                EmailAccount = "登录邮箱账号",
                EmailPassword = "密码"
            });
            _sendMailKit.SendEmail(new SendMailBodyModel
            {
                MailFiles = new List<string> { "附件1路径", "附件2路径" },
                Recipients = new List<RecipientsModel> { new RecipientsModel { ReceiveMail="收件人信箱",ReceiveName="收件人" } },//可群发
                SenderAddress = "发送邮箱",
                Subject = "邮件主题",
                Body = "邮件内容"
            });
        }

        /// <summary>
        /// 接收邮件
        /// </summary>
        public static void ReceiceMail()
        {
            IReceiveImapMailKit _receiveImapMailKit = new ReceiveImapMailKit();
            //打开邮件服务器连接
            _receiveImapMailKit.Connection(new MailServerConfigModel
            {
                ServerHost = "邮箱服务器地址",
                ServerPort = 993,//端口
                IsSsl = false,
                EmailAccount = "登录邮箱账号",
                EmailPassword = "密码"
            });
            var MailIds= _receiveImapMailKit.QueryReceiveMailId();//接收邮件Id
            MailIds.ForEach(m => 
            {
                var mail = _receiveImapMailKit.ReceiveMail(m,"邮件附件，邮件内容图片保存路径,不传则不下载");//通过邮件Id获取邮件
                var FilePath = _receiveImapMailKit.SaveMailFile(mail.MailMessage, "邮件附件，邮件内容图片保存路径,必传", "邮件唯一标识，用于写日志");//保存邮件附件
                var MailImgPath = _receiveImapMailKit.SaveMailFile(mail.MailMessage, "邮件附件，邮件内容图片保存路径,必传", "邮件唯一标识，用于写日志");//保存邮件内容图片
            });
            var Mailres = _receiveImapMailKit.ReceiveMaillist("邮件附件，邮件内容图片保存路径,不传则不下载");
            Mailres.ForEach(m =>
            {
                _receiveImapMailKit.SetMailSeen(m.MailUniqueId.Value);//标记邮件已读
            });
            _receiveImapMailKit.Disconnect();//关闭邮件服务器连接
        }
    }
}
