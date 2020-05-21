using MailKit.Interface;
using MailKit.Model;
using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MailKit.ImpI
{
    /// <summary>
    /// MailKit邮件发送类
    /// </summary>
    public class SendMailKit : ISendMailKit
    {
        private SmtpClient _client;

        /// <summary>
        /// 连接邮件服务器并登录
        /// </summary>
        /// <param name="ServerConfig"></param>
        public void Connection(MailServerConfigModel ServerConfig)
        {
            _client = new SmtpClient();
            _client.ServerCertificateValidationCallback = (s, c, h, e) => true;
            _client.Connect(ServerConfig.ServerHost, ServerConfig.ServerPort, ServerConfig.IsSsl);
            _client.Authenticate(ServerConfig.EmailAccount, ServerConfig.EmailPassword);

        }

        /// <summary>
        /// 组装邮件
        /// </summary>
        /// <param name="BodyModel"></param>
        /// <returns></returns>
        public MimeMessage AssembleMailMessage(SendMailBodyModel BodyModel)
        {
            var Mail = new MimeMessage();
            var Mailbody = new Multipart();
            Mail.Subject = BodyModel.Subject;//设置邮件主题
            Mail.From.Add(new MailboxAddress(BodyModel.Sender, BodyModel.SenderAddress));//设置发件人
            //设置抄送人
            if (BodyModel.Cc != null && BodyModel.Cc.Count > 0)
            {
                BodyModel.Cc.ForEach(m =>
                {
                    Mail.Cc.Add(new MailboxAddress(m));
                });
            }
            //设置密送人
            if (BodyModel.Bcc != null && BodyModel.Bcc.Count > 0)
            {
                BodyModel.Bcc.ForEach(m =>
                {
                    Mail.Bcc.Add(new MailboxAddress(m));
                });
            }
            //添加收件人
            BodyModel.Recipients.ForEach(m =>
            {
                Mail.To.Add(new MailboxAddress(m.ReceiveName, m.ReceiveMail));
            });
            //写入邮件内容
            var TextBody = new TextPart(BodyModel.MailBodyType);
            TextBody.SetText(Encoding.Default, BodyModel.Body);
            Mailbody.Add(TextBody);
            //添加邮件附件
            if (BodyModel.MailFiles != null && BodyModel.MailFiles.Count > 0)
            {
                BodyModel.MailFiles.ForEach(m =>
                {
                    var fileName = Path.GetFileName(m);
                    var attachment = new MimePart()
                    {
                        Content = new MimeContent(File.OpenRead(m)),
                        ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                        ContentTransferEncoding = ContentEncoding.Base64,
                        FileName = fileName,
                    };
                    Mailbody.Add(attachment);
                });
            }
            Mail.Body = Mailbody;
            return Mail;
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="Message">邮件主体</param>
        public void SendEmail(MimeMessage Message)
        {
            try
            {
                _client.Send(Message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _client.Disconnect(true);
            }
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="BodyModel">邮件模板</param>
        public void SendEmail(SendMailBodyModel BodyModel)
        {
            try
            {
                _client.Send(AssembleMailMessage(BodyModel));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _client.Disconnect(true);
            }
        }
    }
}
