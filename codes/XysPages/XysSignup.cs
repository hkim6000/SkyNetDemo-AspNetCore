using Microsoft.Data.SqlClient;
using SkyNet;
using SkyNet.ToolKit;
using ASPNETCoreWeb.codes.XysBases;
using System.Data;

namespace ASPNETCoreWeb.codes.XysPages
{

    public class XysSignup : WebSingle
    {
        public override void OnInitialized()
        {
            HtmlDoc.AddJsFile("WebScript.js");
            HtmlDoc.AddCSSFile("WebStyle.css");
            HtmlDoc.SetTitle(Translator.Format("title"));

            Label Title = new Label(Translator.Format("signup"));
            Title.Wrap.SetStyle(HtmlStyles.marginTop, "50px");
            Title.Wrap.SetStyle(HtmlStyles.marginLeft, "6px");
            Title.Wrap.SetStyle(HtmlStyles.fontSize, "24px");
            Title.Wrap.SetStyle(HtmlStyles.textShadow, "2px 2px #e0e0e0");

            Texts text = new Texts(Translator.Format("name"), "name", TextTypes.text);
            text.Required = true;
            text.Text.SetStyle(HtmlStyles.width, "200px");
            text.Text.SetAttribute(HtmlAttributes.maxlength, "100");
            text.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");
            text.Text.SetAttribute(HtmlEvents.onblur, "txtblur(event, this)");

            Texts text1 = new Texts(Translator.Format("email"), "email", TextTypes.text);
            text1.Required = true;
            text1.Text.SetStyle(HtmlStyles.width, "350px");
            text1.Text.SetAttribute(HtmlAttributes.maxlength, "150");
            text1.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");
            text1.Text.SetAttribute(HtmlEvents.onblur, "txtblur(event, this)");

            Texts text2 = new Texts(Translator.Format("pwd"), "pwd", TextTypes.password);
            text2.Required = true;
            text2.Text.SetStyle(HtmlStyles.width, "200px");
            text2.Text.SetAttribute(HtmlAttributes.maxlength, "20");
            text2.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");
            text2.Text.SetAttribute(HtmlEvents.onblur, "txtblur(event, this)");

            Texts text3 = new Texts(Translator.Format("pwd1"), "pwd1", TextTypes.password);
            text3.Required = true;
            text3.Text.SetStyle(HtmlStyles.width, "200px");
            text3.Text.SetAttribute(HtmlAttributes.maxlength, "20");
            text3.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");
            text3.Text.SetAttribute(HtmlEvents.onblur, "txtblur(event, this)");

            Button btn = new Button(Translator.Format("next"), Button.ButtonTypes.Button);
            btn.SetAttribute(HtmlAttributes.@class, "button");
            btn.SetAttribute(HtmlEvents.onclick, "NavXysVerify()");

            Button btn1 = new Button(Translator.Format("back"), Button.ButtonTypes.Button);
            btn1.SetAttribute(HtmlAttributes.@class, "button");
            btn1.SetAttribute(HtmlEvents.onclick, "NavXysSignin()");

            HtmlElementBox elmBox = new HtmlElementBox();
            elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox);
            elmBox.SetStyle(HtmlStyles.position, "relative");
            elmBox.SetStyle(HtmlStyles.margin, "auto");
            elmBox.SetStyle(HtmlStyles.width, "500px");
            elmBox.SetStyle(HtmlStyles.paddingLeft, "50px");
            elmBox.SetStyle(HtmlStyles.paddingBottom, "24px");

            elmBox.AddItem(Title, 34);
            elmBox.AddItem(text, 1);
            elmBox.AddItem(text1, 14);
            elmBox.AddItem(text2, 1);
            elmBox.AddItem(text3, 40);
            elmBox.AddItem(btn1);
            elmBox.AddItem(btn, 10);

            HtmlDoc.HtmlBodyAddOn = elmBox.HtmlText;

            HtmlDoc.InitialScripts.CenteringElement(References.Elements.ElmBox);
        }

        public ApiResponse NavXysSignin()
        {
            ApiResponse _ApiResponse = new ApiResponse();
            _ApiResponse.Navigate(References.Pages.XysSignin);
            return _ApiResponse;
        }

        public ApiResponse NavXysVerify()
        {
            string name = GetDataValue("name");
            string email = GetDataValue("email");
            string pwd = GetDataValue("pwd");
            string pwd1 = GetDataValue("pwd1");

            ApiResponse _ApiResponse = new ApiResponse();

            bool Valid = true;
            string DialogMsgText = string.Empty;

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(pwd) || string.IsNullOrEmpty(pwd1))
            {
                Valid = false;
            }

            if (!Valid)
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
                    string rlt = ValidatePassword(pwd);
                    // Validation results logic can be placed here if needed
                }
            }

            if (!string.IsNullOrEmpty(DialogMsgText))
            {
                _ApiResponse.PopUpWindow(DialogMsg(DialogMsgText), References.Elements.ElmBox);
                _ApiResponse.ExecuteScript("ShowButtons();");
            }
            else
            {
                if (ExistsEmail(email))
                {
                    _ApiResponse.PopUpWindow(DialogMsg(Translator.Format("msg_exist")), References.Elements.ElmBox);
                    _ApiResponse.ExecuteScript("ShowButtons();");
                }
                else
                {
                    ViewData data = new ViewData();
                    data.Tid = NewID();
                    data.Name = name;
                    data.Email = email;
                    data.Pass = Encryptor.EncryptData(pwd);
                    data.OTP = RandNUM().ToString();
                    data.IpAddr = ClientIPAddress;
                    data.Created = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    data.Expired = DateTime.Now.AddMinutes(10).ToString("yyyy-MM-dd HH:mm:ss");

                    string MailFile = HtmlFolder + References.Htmls.Email_SignUp + "_" + ClientLanguage + ".html";
                    if (!File.Exists(MailFile))
                    {
                        MailFile = References.Htmls.Email_SignUp;
                    }
                    else
                    {
                        MailFile = References.Htmls.Email_SignUp + "_" + ClientLanguage;
                    }

                    string rlt = SaveDataData(data);
                    if (string.IsNullOrEmpty(rlt))
                    {
                        string Subject = HtmlTranslator.Value("msg_email");
                        string bodyHtml = ReadHtmlFile(MailFile)
                                                 .Replace("{username}", data.Name)
                                                 .Replace("{useremail}", data.Email)
                                                 .Replace("{userpin}", data.OTP);
                        string[] ToAddr = { data.Email };

                        string rltmail = SendEmail(Subject, bodyHtml, ToAddr);
                        if (string.IsNullOrEmpty(rltmail))
                        {
                            string SerializedString = SerializeObjectEnc(data, typeof(ViewData));
                            _ApiResponse.SetCookie(References.Keys.SignUp_User, SerializedString);
                            _ApiResponse.Navigate(References.Pages.XysVerify);
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

        private string SaveDataData(ViewData data)
        {
            List<string> SQL = new List<string>
        {
            " Insert into XysSignUp( Tid,Name,Email,Pass,OTP,IpAddr,Created,Expired) " +
            " values ( @Tid,@Name,@Email,@Pass,@OTP,@IpAddr,@Created,@Expired) "
        };

            List<SqlParameter> SqlParams = new List<SqlParameter>();
            SqlParams.Add(new SqlParameter { ParameterName = "@Tid", Value = data.Tid, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@Name", Value = data.Name, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@Email", Value = data.Email, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@Pass", Value = data.Pass, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@OTP", Value = data.OTP, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@IpAddr", Value = data.IpAddr, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@Created", Value = data.Created, SqlDbType = SqlDbType.DateTime });
            SqlParams.Add(new SqlParameter { ParameterName = "@Expired", Value = data.Expired, SqlDbType = SqlDbType.DateTime });

            return PutData(SqlWithParams(SQL, SqlParams));
        }

        private bool ExistsEmail(string UserEmail)
        {
            bool rtnvlu = false;

            SQLText SQLTextObj = new SQLText();
            SQLTextObj.Sql = " if exists(select * from XysUserInfo  where UserEmail = @UserEmail) " +
                          " begin select 1 end else begin select 0 end ";
            SQLTextObj.Params.Add(new SqlParameter { ParameterName = "@UserEmail", Value = UserEmail, SqlDbType = SqlDbType.NVarChar });

            string emsg = string.Empty;
            DataTable dt = SQLData.SQLDataTable(SQLTextObj.ToString(), ref emsg);
            if (string.IsNullOrEmpty(emsg) && dt != null && dt.Rows.Count != 0)
            {
                if (Common.Val(dt.Rows[0][0].ToString()) == 1)
                {
                    rtnvlu = true;
                }
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
            public string Name { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Pass { get; set; } = string.Empty;
            public string OTP { get; set; } = string.Empty;
            public string IpAddr { get; set; } = string.Empty;
            public string Created { get; set; } = string.Empty;
            public string Expired { get; set; } = string.Empty;
        }
    }

}
