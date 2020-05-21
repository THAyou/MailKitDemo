using MailKit.Model;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MailKit.Interface
{
    public interface ISendMailKit
    {
        /// <summary>
        /// 连接邮件服务器并登录
        /// </summary>
        /// <param name="ServerConfig"></param>
        void Connection(MailServerConfigModel ServerConfig);

        /// <summary>
        /// 组装邮件
        /// </summary>
        /// <param name="BodyModel"></param>
        /// <returns></returns>
        MimeMessage AssembleMailMessage(SendMailBodyModel BodyModel);

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="Message"></param>
        void SendEmail(MimeMessage Message);

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="BodyModel"></param>
        void SendEmail(SendMailBodyModel BodyModel);

    }
}
