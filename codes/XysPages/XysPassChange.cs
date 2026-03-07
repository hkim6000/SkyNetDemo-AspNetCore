using Microsoft.Data.SqlClient;
using SkyNet;
using SkyNet.ToolKit;
using ASPNETCoreWeb.codes.XysBases;
using System.Data;

namespace ASPNETCoreWeb.codes.XysPages
{

    public class XysPassChange : WebSingle
    {
        private ViewData data;

        public XysPassChange()
        {
            try
            {
                HtmlTranslator.Add(GetPageDict(this.GetType().Name));
                string QryVlu = QueryValue("x");
                data = (ViewData)DeserializeObjectEnc(QryVlu, typeof(ViewData));
            }
            catch (Exception)
            {
                data = null;
            }
        }

        public override void OnBeforeRender()
        {
            if (data == null)
            {
                HtmlDoc.InitialScripts.Navigate(References.Pages.XysSignin);
            }
            else
            {
                HtmlDoc.InitialScripts.CenteringElement(References.Elements.ElmBox);
            }
        }

        public override void OnInitialized()
        {
            HtmlDoc.AddJsFile("WebScript.js");
            HtmlDoc.AddCSSFile("WebStyle.css");
            HtmlDoc.SetTitle(Translator.Format("title"));

            Label Title = new Label(Translator.Format("changepwd"));
            Title.Wrap.SetStyle(HtmlStyles.marginTop, "50px");
            Title.Wrap.SetStyle(HtmlStyles.marginLeft, "6px");
            Title.Wrap.SetStyle(HtmlStyles.fontSize, "24px");
            Title.Wrap.SetStyle(HtmlStyles.textShadow, "2px 2px #e0e0e0");

            Label lbl1 = new Label(Translator.Format("enterpwd"));
            lbl1.Wrap.SetStyle(HtmlStyles.paddingLeft, "10px");
            lbl1.Wrap.SetStyle(HtmlStyles.color, "#444");

            Texts text = new Texts(Translator.Format("pwd"), "pwd", TextTypes.password);
            text.Required = true;
            text.Text.SetStyle(HtmlStyles.width, "180px");
            text.Text.SetAttribute(HtmlAttributes.maxlength, "15");

            Texts text1 = new Texts(Translator.Format("pwd1"), "pwd1", TextTypes.password);
            text1.Required = true;
            text1.Text.SetStyle(HtmlStyles.width, "180px");
            text1.Text.SetAttribute(HtmlAttributes.maxlength, "15");

            Button btn = new Button(Translator.Format("submit"), Button.ButtonTypes.Button);
            btn.SetAttribute(HtmlAttributes.@class, "button");
            btn.SetAttribute(HtmlEvents.onclick, "NavXysSignin()");

            HtmlElementBox elmBox = new HtmlElementBox();
            elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox);
            elmBox.SetStyle(HtmlStyles.position, "relative");
            elmBox.SetStyle(HtmlStyles.margin, "auto");
            elmBox.SetStyle(HtmlStyles.width, "500px");
            elmBox.SetStyle(HtmlStyles.paddingLeft, "50px");
            elmBox.SetStyle(HtmlStyles.paddingBottom, "24px");

            elmBox.AddItem(Title, 16);
            elmBox.AddItem(lbl1, 24);
            elmBox.AddItem(text, 4);
            elmBox.AddItem(text1, 4);
            elmBox.AddItem(btn, 10);

            HtmlDoc.HtmlBodyAddOn = elmBox.HtmlText;
        }

        public ApiResponse NavXysSignin()
        {
            string pwd = GetDataValue("pwd");
            string pwd1 = GetDataValue("pwd1");

            ApiResponse _ApiResponse = new ApiResponse();

            bool Valid = true;
            string DialogMsgText = string.Empty;

            if (string.IsNullOrEmpty(pwd) || string.IsNullOrEmpty(pwd1))
            {
                Valid = false;
            }

            if (Valid == false)
            {
                DialogMsgText = Translator.Format("msg_required");
            }
            else
            {
                if (pwd != pwd1)
                {
                    DialogMsgText = Translator.Format("msg_pwdconfirm");
                }
                else
                {
                    string rltValidation = ValidatePassword(pwd);
                    // If logic for rltValidation can be added here
                }
            }

            if (DialogMsgText != string.Empty)
            {
                _ApiResponse.PopUpWindow(DialogMsg(DialogMsgText), References.Elements.ElmBox);
                _ApiResponse.ExecuteScript("ShowButtons();");
            }
            else
            {
                if (ExistsReset(data.Tid) == false)
                {
                    DialogBox dialogBox = new DialogBox(Translator.Format("msg_exist"));
                    dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
                    dialogBox.AddButton(Translator.Format("passreset"), string.Empty, "class:button;onclick:$NavigateTo('" + References.Pages.XysPassReset + "');");
                    _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.ElmBox);
                    _ApiResponse.ExecuteScript("ShowButtons();");
                }
                else
                {
                    data.Pwd = pwd;

                    string MailFile = HtmlFolder + References.Htmls.Email_PassChanged + "_" + ClientLanguage + ".html";
                    if (File.Exists(MailFile) == false)
                    {
                        MailFile = References.Htmls.Email_PassChanged;
                    }
                    else
                    {
                        MailFile = References.Htmls.Email_PassChanged + "_" + ClientLanguage;
                    }

                    string rlt = SaveDataData(data);
                    if (rlt == string.Empty)
                    {
                        string Subject = HtmlTranslator.Value("msg_email");
                        string bodyHtml = ReadHtmlFile(MailFile)
                                                 .Replace("{username}", data.Name)
                                                 .Replace("{useremail}", data.Email);
                        string[] ToAddr = new string[] { data.Email };

                        string rltmail = SendEmail(Subject, bodyHtml, ToAddr);
                        if (rltmail == string.Empty)
                        {
                            _ApiResponse.Navigate(References.Pages.XysSignin);
                        }
                        else
                        {
                            _ApiResponse.PopUpWindow(DialogMsg(rltmail), References.Elements.ElmBox);
                            _ApiResponse.ExecuteScript("ShowButtons();");
                        }
                    }
                    else
                    {
                        _ApiResponse.PopUpWindow(DialogMsg(rlt), References.Elements.ElmBox);
                        _ApiResponse.ExecuteScript("ShowButtons();");
                    }
                }
            }

            return _ApiResponse;
        }

        private string SaveDataData(ViewData _data)
        {
            List<string> SQL = new List<string>
        {
            " update XysUserReset set Status = 9, Expired=getdate() where Tid = N'" + _data.Tid + "' ",
            " update XysUser set " +
            " UserPwd = N'" + Encryptor.EncryptData(_data.Pwd) + "', SYSDTE = GETDATE(), SYSUSR = N'" + _data.UserId + "' " +
            " where UserId = N'" + _data.UserId + "' "
        };

            return PutData(SQL);
        }

        private bool ExistsReset(string Tid)
        {
            bool rtnvlu = false;

            SQLText SQLTextObj = new SQLText();
            SQLTextObj.Sql = " select * from XysUserReset where Status = 0 and getdate() between Created and Expired and Tid = @Tid ";
            SQLTextObj.Params.Add(new SqlParameter { ParameterName = "@Tid", Value = Tid, SqlDbType = SqlDbType.NVarChar });

            string emsg = string.Empty;
            DataTable dt = SQLData.SQLDataTable(SQLTextObj.ToString(), ref emsg);
            if (string.IsNullOrEmpty(emsg) && dt != null && dt.Rows.Count != 0)
            {
                rtnvlu = true;
            }
            return rtnvlu;
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
