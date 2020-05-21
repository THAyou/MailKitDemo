using MailKit.Interface;
using MailKit.Model;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Pop3;
using MailKit.Search;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailKit.ImpI
{
    /// <summary>
    /// MaillKit邮件接收(Imap协议)
    /// </summary>
    public class ReceiveImapMailKit : IReceiveImapMailKit
    {
        private ImapClient _client;


        /// <summary>
        /// 连接邮件服务器并登录
        /// </summary>
        public void Connection(MailServerConfigModel ServerConfig)
        {
            _client = new ImapClient();
            _client.ServerCertificateValidationCallback = (s, c, h, e) => true;
            _client.Connect(ServerConfig.ServerHost, ServerConfig.ServerPort, ServerConfig.IsSsl);
            _client.Authenticate(Encoding.Default, ServerConfig.EmailAccount, ServerConfig.EmailPassword);
        }

        /// <summary>
        /// 查询出所有的邮件真实Id
        /// </summary>
        /// <param name="AfterTime">时间节点</param>
        /// <param name="IsNotSeen">是否只查询未读邮件</param>
        /// <returns></returns>
        public List<UniqueId> QueryReceiveMailId(DateTime? AfterTime = null,bool IsNotSeen=true)
        {
            try
            {
                //打开收件箱文件夹
                _client.Inbox.Open(FolderAccess.ReadWrite);
                //获取邮箱条件,默认获取未读邮件，如果有时间条件，则获取大于这个时间的邮件
                var MailSeachQuery = SearchQuery.NotSeen;
                if (AfterTime != null)
                {
                    if (IsNotSeen)
                    {
                        MailSeachQuery = SearchQuery.And(SearchQuery.DeliveredAfter(AfterTime.Value), SearchQuery.NotSeen);
                    }
                    else
                    {
                        MailSeachQuery = SearchQuery.DeliveredAfter(AfterTime.Value);
                    }
                }
                else
                {
                    if (IsNotSeen)
                    {
                        MailSeachQuery = SearchQuery.NotSeen;
                    }
                    else
                    {
                        MailSeachQuery = SearchQuery.All;
                    }
                }

                //获取邮件Id集合
                var EmailIds = _client.Inbox.Search(MailSeachQuery);
                if (EmailIds != null && EmailIds.Count > 0)
                {
                    return EmailIds.ToList();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 根据邮件Id获取邮件内容，并下载附件
        /// </summary>
        /// <param name="EmailIds"></param>
        /// <param name="saveFilePath"></param>
        /// <param name=""></param>
        /// <param name="IsSetFlag"></param>
        /// <param name="IsdownloadFile">是否下载邮件内文件</param>
        /// <returns></returns>
        public ReceiveMailBodyModel ReceiveMail(UniqueId mailId, string saveFilePath = null, bool IsSetFlag = false)
        {
            ReceiveMailBodyModel resInfo = null;
            //如果没有邮件，则返回
            if (mailId == null)
            {
                return resInfo;
            }
            TryExFunc(() =>
            {
                resInfo = new ReceiveMailBodyModel();
                var mail = _client.Inbox.GetMessage(mailId);
                resInfo.MailUniqueId = mailId.Id;
                resInfo.MailMessage = mail;
                resInfo.MailTime = mail.Date.DateTime;
                resInfo.mailMessageId =  mail.MessageId;
                var FromAddress = (MailboxAddress)mail.From[0];
                resInfo.SenderAddress = FromAddress.Address;
                resInfo.Sender = FromAddress.Name;
                resInfo.HtmlBody = mail.HtmlBody;//Html内容
                resInfo.TextBody = mail.TextBody;//Text内容
                if (mail.To != null && mail.To.Count > 0)
                {
                    foreach (var t in mail.To)
                    {
                        if (t.Name != "undisclosed-recipients")
                        {
                            var ToAddress = (MailboxAddress)t;
                            resInfo.Recipients.Add(new RecipientsModel
                            {
                                ReceiveMail = ToAddress.Address,
                                ReceiveName = ToAddress.Name
                            });
                        }
                    }
                }
                resInfo.Subject = mail.Subject;
                if (mail.Cc != null && mail.Cc.Count > 0)//获取抄送人
                {
                    resInfo.Cc = new List<RecipientsModel>();
                    foreach (var cc in mail.Cc)
                    {
                        if (cc.Name != "undisclosed-recipients")
                        {
                            var ccAddress = (MailboxAddress)cc;
                            resInfo.Recipients.Add(new RecipientsModel
                            {
                                ReceiveMail = ccAddress.Address,
                                ReceiveName = ccAddress.Name
                            });
                        }
                    }
                }
                if (mail.Bcc != null && mail.Bcc.Count > 0)//获取密送人
                {
                    resInfo.Bcc = new List<RecipientsModel>();
                    foreach (var Bcc in mail.Bcc)
                    {
                        if (Bcc.Name != "undisclosed-recipients")
                        {
                            var BccAddress = (MailboxAddress)Bcc;
                            resInfo.Recipients.Add(new RecipientsModel
                            {
                                ReceiveMail = BccAddress.Address,
                                ReceiveName = BccAddress.Name
                            });
                        }
                    }
                }
                if (!string.IsNullOrEmpty(saveFilePath))
                {
                    resInfo.MailImg = SaveMailImg(mail, saveFilePath, mail.MessageId);
                    resInfo.MailFiles = SaveMailFile(mail, saveFilePath, mail.MessageId);
                }
                if (IsSetFlag)
                {
                    SetMailSeen(mailId);
                }
            }, $"邮件获取错误,真实邮件Id:[{mailId.Id}]");
            return resInfo;
        }

        /// <summary>
        /// 接收邮件集合
        /// </summary>
        /// <param name="saveFilePath">附件已经图片保存路径</param>
        /// <param name="AfterTime">时间节点</param>
        /// <param name="IsSetFlag">读取完毕后是否标记为已读</param>
        /// <param name="IsdownloadFile">读取完毕后是否标记为已读</param>
        /// <returns></returns>
        public List<ReceiveMailBodyModel> ReceiveMaillist(string saveFilePath = null, DateTime? AfterTime = null, bool IsSetFlag = false)
        {
            var result = new List<ReceiveMailBodyModel>();
            var EmailIds = QueryReceiveMailId(AfterTime);
            //如果没有邮件，则返回
            if (EmailIds == null || EmailIds.Count == 0)
            {
                return result;
            }
            //遍历所有的邮件Id
            foreach (var mailId in EmailIds)
            {
                var resInfo = ReceiveMail(mailId, saveFilePath, IsSetFlag);
                if (resInfo != null)
                {
                    result.Add(resInfo);
                }
            }
            return result;
        }

        /// <summary>
        /// 标记邮件已读
        /// </summary>
        public void SetMailSeen(UniqueId mailId)
        {
            //将此邮件标记已为已读邮件
            _client.Inbox.SetFlags(mailId, MessageFlags.Seen, true);
        }

        /// <summary>
        /// 标记邮件已读
        /// </summary>
        /// <param name="mailId"></param>
        public void SetMailSeen(uint mailId)
        {
            //将此邮件标记已为已读邮件
            _client.Inbox.SetFlags(new UniqueId(mailId), MessageFlags.Seen, true);
        }

        /// <summary>
        /// 关闭邮件服务器连接
        /// </summary>
        public void Disconnect()
        {
            _client.Disconnect(true);//关闭邮箱连接
        }

        /// <summary>
        /// 保存邮件内容图片
        /// </summary>
        /// <param name="mimeEntitie"></param>
        public List<MailBodyImgModel> SaveMailImg(MimeMessage message, string basePath,string mailId)
        {
            var result = new List<MailBodyImgModel>();
            var mimeEntitie = message.BodyParts;
            if (mimeEntitie != null && mimeEntitie.Count() > 0)
            {
                foreach (var body in mimeEntitie)
                {
                    TryExFunc(() =>
                    {
                        if (!body.IsAttachment)//读取的是内容图片，不是附件
                        {
                            var messagePart = body as MessagePart;
                            if (messagePart != null)
                            {
                                //message/rfc822 不保存
                                return;
                            }
                            var part = body as MimePart;
                            //如果文件名为空，则不属于邮件内容图片
                            var fileName = part.FileName;
                            if (fileName != null)
                            {
                                string RenNum = new Random().Next(0, 100).ToString();
                                fileName = fileName.Replace("%", "");
                                fileName = fileName.Replace("&", "");
                                fileName = fileName.Replace("#", "");
                                fileName = fileName.Replace("+", "");
                                fileName = fileName.Replace("\\", "");
                                var Extension = Path.GetExtension(fileName);
                                string savepath = basePath + "/mailfile/purchase/" + DateTime.Now.Year + "-" + DateTime.Now.Month+"/bodyImg";
                                if (!Directory.Exists(savepath))
                                {
                                    Directory.CreateDirectory(savepath);
                                }
                                var path = Path.Combine(savepath, Guid.NewGuid().ToString() + Extension);
                                using (var stream = File.Create(path))
                                {
                                    part.Content.DecodeTo(stream);
                                }
                                result.Add(new MailBodyImgModel { cid = body.ContentId, imgSrc = path });
                            }
                        }
                    }, $"邮件内容图片下载错误,邮件真实Id:[{mailId}]");
                }
            }
            return result;
        }

        /// <summary>
        /// 保存邮件附件
        /// </summary>
        /// <param name="mimeEntitie"></param>
        /// <param name="basePath"></param>
        /// <returns></returns>
        public List<string> SaveMailFile(MimeMessage message, string basePath,string mailId)
        {
            var result = new List<string>();
            var mimeEntitie = message.BodyParts;
            if (mimeEntitie != null && mimeEntitie.Count() > 0)
            {
                foreach (var body in mimeEntitie)
                {
                    TryExFunc(() =>
                    {
                        var BPath = basePath;
                        if (body.IsAttachment)//读取的附件
                        {
                            var messagePart = body as MessagePart;
                            if (messagePart != null)
                            {
                                //message/rfc822 不保存
                                return;
                            }
                            var part = body as MimePart;
                            var fileName = part.FileName;
                            string RenNum = new Random().Next(0, 100).ToString();
                            fileName = fileName.Replace("%", "");
                            fileName = fileName.Replace("&", "");
                            fileName = fileName.Replace("#", "");
                            fileName = fileName.Replace("+", "");
                            fileName = fileName.Replace("\\", "");
                            string savepath = BPath + "/mailfile/purchase/" + DateTime.Now.Year + "-" + DateTime.Now.Month;
                            if (!Directory.Exists(savepath))
                            {
                                Directory.CreateDirectory(savepath);
                            }
                            var path = Path.Combine(savepath, RenNum + "-" + fileName);
                            using (var stream = File.Create(path))
                            {
                                part.Content.DecodeTo(stream);
                            }
                            result.Add(path);
                        }
                    }, $"邮件附件下载错误,真实邮件Id:[{mailId}]");
                }
            }
            return result;
        }

        /// <summary>
        /// 错误监控方法
        /// </summary>
        /// <param name="ExFun"></param>
        /// <param name="ErrorMessage"></param>
        private void TryExFunc(Action ExFun,string ErrorMessage)
        {
            try
            {
                ExFun();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ErrorMessage},错误:[{ex.Message}]");
                Console.WriteLine($"{ErrorMessage},详细错误:[{ex.StackTrace}]");
            }
        }
    }
}
