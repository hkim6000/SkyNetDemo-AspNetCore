using Microsoft.Data.SqlClient;
using SkyNet;
using SkyNet.ToolKit;
using ASPNETCoreWeb.codes.XysBases;
using System.Data;

namespace ASPNETCoreWeb.codes.XysPages
{

    public class XysPassReset : WebSingle
    {
        public override void OnInitialized()
        {
            HtmlDoc.AddJsFile("WebScript.js");
            HtmlDoc.AddCSSFile("WebStyle.css");
            HtmlDoc.SetTitle(Translator.Format("title"));

            Label Title = new Label(Translator.Format("reset"));
            Title.Wrap.SetStyle(HtmlStyles.marginTop, "50px");
            Title.Wrap.SetStyle(HtmlStyles.marginLeft, "6px");
            Title.Wrap.SetStyle(HtmlStyles.fontSize, "24px");
            Title.Wrap.SetStyle(HtmlStyles.textShadow, "2px 2px #e0e0e0");

            Label lbl1 = new Label(Translator.Format("enteremail"));
            lbl1.Wrap.SetStyle(HtmlStyles.paddingLeft, "10px");
            lbl1.Wrap.SetStyle(HtmlStyles.color, "#444");

            Texts text = new Texts(Translator.Format("email"), "email", TextTypes.text);
            text.Required = true;
            text.Text.SetStyle(HtmlStyles.width, "330px");

            Button btn = new Button(Translator.Format("next"), Button.ButtonTypes.Button);
            btn.SetAttribute(HtmlAttributes.@class, "button");
            btn.SetAttribute(HtmlEvents.onclick, "NavXysSent()");

            Button btn1 = new Button(Translator.Format("back"), Button.ButtonTypes.Button);
            btn1.SetStyle(HtmlStyles.marginLeft, "12px");
            btn1.SetAttribute(HtmlAttributes.@class, "button1");
            btn1.SetAttribute(HtmlEvents.onclick, "NavXysSignin()");

            HtmlElementBox elmBox = new HtmlElementBox();
            elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox);
            elmBox.SetStyle(HtmlStyles.position, "relative");
            elmBox.SetStyle(HtmlStyles.margin, "auto");
            elmBox.SetStyle(HtmlStyles.width, "500px");
            elmBox.SetStyle(HtmlStyles.paddingLeft, "50px");
            elmBox.SetStyle(HtmlStyles.paddingBottom, "24px");

            elmBox.AddItem(Title, 16);
            elmBox.AddItem(lbl1, 20);
            elmBox.AddItem(text, 40);
            elmBox.AddItem(btn);
            elmBox.AddItem(btn1, 10);

            HtmlDoc.HtmlBodyAddOn = elmBox.HtmlText;
            HtmlDoc.InitialScripts.CenteringElement(References.Elements.ElmBox);
        }

        public ApiResponse NavXysSignin()
        {
            ApiResponse _ApiResponse = new ApiResponse();
            _ApiResponse.Navigate(References.Pages.XysSignin);
            return _ApiResponse;
        }

        public ApiResponse NavXysSent()
        {
            string email = GetDataValue("email");
            ApiResponse _ApiResponse = new ApiResponse();

            if (!string.IsNullOrEmpty(email))
            {
                ViewData data = GetViewData(email);
                if (data != null)
                {
                    string SerializedString = SerializeObjectEnc(data, typeof(ViewData));
                    string UserLink = VirtualPath + "xyspasschange?x=" + SerializedString;

                    string MailFile = HtmlFolder + References.Htmls.Email_PassReset + "_" + ClientLanguage + ".html";
                    if (!File.Exists(MailFile))
                    {
                        MailFile = References.Htmls.Email_PassReset;
                    }
                    else
                    {
                        MailFile = References.Htmls.Email_PassReset + "_" + ClientLanguage;
                    }

                    string Subject = HtmlTranslator.Value("msg_reset");
                    string bodyHtml = ReadHtmlFile(MailFile)
                                             .Replace("{username}", data.Name)
                                             .Replace("{useremail}", data.Email)
                                             .Replace("{userlink}", UserLink);
                    string[] ToAddr = { data.Email };

                    string rltmail = SendEmail(Subject, bodyHtml, ToAddr);
                    if (string.IsNullOrEmpty(rltmail))
                    {
                        string rlt = SaveDataData(data);
                        if (string.IsNullOrEmpty(rlt))
                        {
                            _ApiResponse.Navigate(References.Pages.XysSent);
                        }
                        else
                        {
                            _ApiResponse.PopUpWindow(DialogMsg(rlt), References.Elements.ElmBox);
                            _ApiResponse.ExecuteScript("ShowButtons();");
                        }
                    }
                    else
                    {
                        _ApiResponse.PopUpWindow(DialogMsg(rltmail), References.Elements.ElmBox);
                        _ApiResponse.ExecuteScript("ShowButtons();");
                    }
                }
                else
                {
                    _ApiResponse.Navigate(References.Pages.XysSent);
                }
            }
            else
            {
                _ApiResponse.PopUpWindow(DialogMsg(Translator.Format("msg_email")));
            }
            return _ApiResponse;
        }

        private string SaveDataData(ViewData data)
        {
            List<string> SQL = new List<string>
        {
            " Insert into XysUserReset( Tid,Email,UserId,Status,Created,Expired) " +
            " values ( N'" + data.Tid + "',N'" + data.Email + "',N'" + data.UserId + "',0,getdate(),dateadd(minute,10,getdate())) "
        };

            return PutData(SQL);
        }

        private ViewData GetViewData(string UserEmail)
        {
            ViewData _data = null;
            string SSql = " select a.UserId,UserEmail,UserName " +
                          " from XysUser a inner join XysUserInfo b on a.UserId = b.UserId " +
                          " where b.UserEmail = @UserEmail ";

            List<SqlParameter> SqlParams = new List<SqlParameter>();
            SqlParams.Add(new SqlParameter { ParameterName = "@UserEmail", Value = UserEmail, SqlDbType = SqlDbType.NVarChar });

            string emsg = string.Empty;
            DataTable dt = SQLData.SQLDataTable(SqlWithParams(SSql, SqlParams), ref emsg);
            if (string.IsNullOrEmpty(emsg) && dt != null && dt.Rows.Count != 0)
            {
                _data = new ViewData();
                _data.Tid = NewID();
                _data.UserId = dt.Rows[0][0].ToString();
                _data.Email = dt.Rows[0][1].ToString();
                _data.Name = dt.Rows[0][2].ToString();
            }
            return _data;
        }

        private string PutData(List<string> SQL)
        {
            string emsg = string.Empty;
            SQLData.SQLDataPut(SQL, ref emsg);
            WriteXysLog(string.Join("|", SQL), ref emsg);
            return emsg;
        }

        private void WriteXysLog(string logTxt, ref string msg)
        {
            string userid = AppKey != null ? AppKey.UserId : string.Empty;

            List<string> SQL = new List<string>
        {
            "insert into XysLog(LogId,ClientIp,UserId,LogTxt,JobRlt,SysDte) " +
            "values(@LogId,@ClientIp,@UserId,@LogTxt,@JobRlt,GETDATE())"
        };

            List<SqlParameter> SqlParams = new List<SqlParameter>();
            SqlParams.Add(new SqlParameter { ParameterName = "@LogId", Value = NewID(), SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@ClientIp", Value = ClientIPAddress, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@UserId", Value = userid, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@LogTxt", Value = EscQuote(logTxt.Trim()), SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@JobRlt", Value = EscQuote(msg.Trim()), SqlDbType = SqlDbType.NVarChar });

            string emsg = string.Empty;
            SQLData _SqlData = new SQLData();
            _SqlData.DataPut(SqlWithParams(SQL, SqlParams), ref emsg);
        }

        private string SendEmail(string Subject, string bodyHtml, string[] ToAddr)
        {
            Mail mail = new Mail();
            mail.Subject = Subject;
            mail.ToAddr = ToAddr;
            mail.Body = bodyHtml;
            return mail.SendMail();
        }

        public class ViewData
        {
            public string Tid { get; set; } = string.Empty;
            public string UserId { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string Pwd { get; set; } = string.Empty;
        }
    }

}
