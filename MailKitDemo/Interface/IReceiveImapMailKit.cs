using MailKit.Model;
using MailKit;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MailKit.Interface
{
    public interface IReceiveImapMailKit
    {
        /// <summary>
        /// 连接邮件服务器并登录
        /// </summary>
        void Connection(MailServerConfigModel ServerConfig);


        /// <summary>
        /// 接收邮件集合
        /// </summary>
        /// <param name="saveFilePath"></param>
        /// <returns></returns>
        List<ReceiveMailBodyModel> ReceiveMaillist(string saveFilePath = null, DateTime? AfterTime = null, bool IsSetFlag = false);

        /// <summary>
        /// 获取邮件Id集合
        /// </summary>
        /// <param name="AfterTime"></param>
        /// <returns></returns>
        List<UniqueId> QueryReceiveMailId(DateTime? AfterTime = null, bool IsNotSeen = true);

        /// <summary>
        /// 根据邮件Id获取邮件内容，并下载附件
        /// </summary>
        /// <param name="mailId"></param>
        /// <param name="saveFilePath"></param>
        /// <param name="IsSetFlag"></param>
        /// <returns></returns>
        ReceiveMailBodyModel ReceiveMail(UniqueId mailId, string saveFilePath = null, bool IsSetFlag = false);

        /// <summary>
        /// 标记邮件已读
        /// </summary>
        /// <param name="mailId"></param>
        void SetMailSeen(UniqueId mailId);

        /// <summary>
        /// 标记邮件已读
        /// </summary>
        /// <param name="mailId"></param>
        void SetMailSeen(uint mailId);

        /// <summary>
        /// 关闭邮件服务器连接
        /// </summary>
        void Disconnect();

        /// <summary>
        /// 下载邮件内图片
        /// </summary>
        /// <param name="message"></param>
        /// <param name="basePath"></param>
        /// <param name="mailId"></param>
        /// <returns></returns>
        List<MailBodyImgModel> SaveMailImg(MimeMessage message, string basePath, string mailId);


        /// <summary>
        /// 下载邮件附件
        /// </summary>
        /// <param name="message"></param>
        /// <param name="basePath"></param>
        /// <param name="mailId"></param>
        /// <returns></returns>
        List<string> SaveMailFile(MimeMessage message, string basePath, string mailId);
    }
}
