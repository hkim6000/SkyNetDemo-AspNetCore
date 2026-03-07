using Microsoft.Data.SqlClient;
using SkyNet;
using SkyNet.ToolKit;
using ASPNETCoreWeb.codes.XysBases;
using System.Data;

namespace ASPNETCoreWeb.codes.XysPages
{

    public class XysVerify : WebSingle
    {
        private ViewData _SignUp = null;
        private bool _SignUpAlive = false;

        public XysVerify()
        {
            try
            {
                string paramVlu = ParamValue(References.Keys.SignUp_User, true);
                _SignUp = (ViewData)DeserializeObjectEnc(paramVlu, typeof(ViewData));
                _SignUpAlive = ValidData(_SignUp.Tid);
            }
            catch (Exception)
            {
                _SignUp = null;
            }
        }

        public override void OnInitialized()
        {
            HtmlDoc.AddJsFile("WebScript.js");
            HtmlDoc.AddCSSFile("WebStyle.css");
            HtmlDoc.SetTitle(Translator.Format("title"));

            Label Title = new Label(Translator.Format("verify"));
            Title.Wrap.SetStyle(HtmlStyles.marginTop, "50px");
            Title.Wrap.SetStyle(HtmlStyles.marginLeft, "6px");
            Title.Wrap.SetStyle(HtmlStyles.fontSize, "24px");
            Title.Wrap.SetStyle(HtmlStyles.textShadow, "2px 2px #e0e0e0");

            Texts text = new Texts(Translator.Format("pinno"), "pin", TextTypes.text);
            text.Text.SetStyle(HtmlStyles.width, "330px");
            text.Text.SetAttribute(HtmlAttributes.maxlength, "5");

            Label lbl1 = new Label(Translator.Format("sentemail"));
            lbl1.Wrap.SetStyle(HtmlStyles.paddingLeft, "10px");
            lbl1.Wrap.SetStyle(HtmlStyles.color, "#444");

            Button btn = new Button(Translator.Format("next"), Button.ButtonTypes.Button);
            btn.SetAttribute(HtmlAttributes.@class, "button");
            btn.SetAttribute(HtmlEvents.onclick, "Submit()");

            Button btn1 = new Button(Translator.Format("back"), Button.ButtonTypes.Button);
            btn1.SetStyle(HtmlStyles.marginLeft, "12px");
            btn1.SetAttribute(HtmlAttributes.@class, "button1");
            btn1.SetAttribute(HtmlEvents.onclick, "NavXysSignin()");

            HtmlElementBox elmBox = new HtmlElementBox();
            elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox);
            elmBox.SetStyle(HtmlStyles.position, "relative");
            elmBox.SetStyle(HtmlStyles.margin, "auto");
            elmBox.SetStyle(HtmlStyles.width, "500px");

            elmBox.AddItem(Title, 14);
            elmBox.AddItem(lbl1, 30);
            elmBox.AddItem(text, 40);
            elmBox.AddItem(btn);
            elmBox.AddItem(btn1, 10);

            HtmlDoc.HtmlBodyAddOn = elmBox.HtmlText;
        }

        public override void OnBeforeRender()
        {
            if (_SignUp == null)
            {
                HtmlDoc.InitialScripts.Navigate(References.Pages.XysSignin);
            }
            else
            {
                if (_SignUpAlive)
                {
                    HtmlDoc.InitialScripts.CenteringElement(References.Elements.ElmBox);
                }
                else
                {
                    HtmlDoc.InitialScripts.Navigate(References.Pages.XysSignupExpired);
                }
            }
        }

        public ApiResponse NavXysSignin()
        {
            ApiResponse _ApiResponse = new ApiResponse();
            _ApiResponse.Navigate(References.Pages.XysSignin);
            return _ApiResponse;
        }

        public ApiResponse Submit()
        {
            string pin = GetDataValue("pin");
            ApiResponse _ApiResponse = new ApiResponse();

            if (!string.IsNullOrEmpty(pin))
            {
                if (pin != _SignUp.OTP)
                {
                    _ApiResponse.PopUpWindow(DialogMsg(Translator.Format("msg_diff")), References.Elements.ElmBox);
                }
                else
                {
                    string rlt = SaveDataData(_SignUp);
                    if (string.IsNullOrEmpty(rlt))
                    {
                        DialogBox dialogBox = new DialogBox(Translator.Format("msg_signin"));
                        dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
                        dialogBox.AddButton(Translator.Format("signin"), string.Empty, "class:button;onclick:$NavigateTo('" + References.Pages.XysSignin + "');");
                        _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.ElmBox);
                    }
                    else
                    {
                        _ApiResponse.PopUpWindow(DialogMsg(rlt), References.Elements.ElmBox);
                    }
                }
            }
            else
            {
                _ApiResponse.PopUpWindow(DialogMsg(Translator.Format("msg_required")), References.Elements.ElmBox);
            }
            return _ApiResponse;
        }

        private string SaveDataData(ViewData _data)
        {
            List<string> SQL = new List<string>
        {
            " If not exists(select * from XysUserInfo where UserEmail = @UserEmail) " +
            " begin " +
            " Insert into XysUser( UserId,UserPwd,UserOTP,UserStatus,LevelCode,PassChanged,Created,Closed,SYSDTE,SYSUSR) " +
            " values ( @UserId, @UserPwd, @UserOTP, @UserStatus,dbo.XF_DefaultUserLevel(), @PassChanged, @Created, @Closed, @SYSDTE, @SYSUSR) " +
            " Insert into XysUserInfo( UserId,UserName,UserDesc,UserEmail,UserPhone,UserPic,RoleId,UserRef) " +
            " values ( @UserId, @UserName, @UserDesc, @UserEmail, @UserPhone, @UserPic, dbo.XF_DefaultUserRoleId(), @UserRef) " +
            " end " +
            " else " +
            " begin " +
            " RAISERROR('msg_exist',16,1) " +
            " end"
        };

            string UserId = NewID();

            List<SqlParameter> SQLParams = new List<SqlParameter>();
            SQLParams.Add(new SqlParameter { ParameterName = "@UserId", Value = UserId, SqlDbType = SqlDbType.NVarChar });
            SQLParams.Add(new SqlParameter { ParameterName = "@UserPwd", Value = _data.Pass, SqlDbType = SqlDbType.NVarChar });
            SQLParams.Add(new SqlParameter { ParameterName = "@UserOTP", Value = 0, SqlDbType = SqlDbType.Int });
            SQLParams.Add(new SqlParameter { ParameterName = "@UserStatus", Value = 0, SqlDbType = SqlDbType.Int });
            SQLParams.Add(new SqlParameter { ParameterName = "@PassChanged", Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), SqlDbType = SqlDbType.DateTime });
            SQLParams.Add(new SqlParameter { ParameterName = "@Created", Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), SqlDbType = SqlDbType.DateTime });
            SQLParams.Add(new SqlParameter { ParameterName = "@Closed", Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), SqlDbType = SqlDbType.DateTime });
            SQLParams.Add(new SqlParameter { ParameterName = "@SYSDTE", Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), SqlDbType = SqlDbType.DateTime });
            SQLParams.Add(new SqlParameter { ParameterName = "@SYSUSR", Value = UserId, SqlDbType = SqlDbType.NVarChar });
            SQLParams.Add(new SqlParameter { ParameterName = "@UserName", Value = _data.Name, SqlDbType = SqlDbType.NVarChar });
            SQLParams.Add(new SqlParameter { ParameterName = "@UserDesc", Value = string.Empty, SqlDbType = SqlDbType.NVarChar });
            SQLParams.Add(new SqlParameter { ParameterName = "@UserEmail", Value = _data.Email, SqlDbType = SqlDbType.NVarChar });
            SQLParams.Add(new SqlParameter { ParameterName = "@UserPhone", Value = string.Empty, SqlDbType = SqlDbType.NVarChar });
            SQLParams.Add(new SqlParameter { ParameterName = "@UserPic", Value = string.Empty, SqlDbType = SqlDbType.NVarChar });
            SQLParams.Add(new SqlParameter { ParameterName = "@UserRef", Value = string.Empty, SqlDbType = SqlDbType.NVarChar });

            return PutData(SqlWithParams(SQL, SQLParams));
        }

        private bool ValidData(string Tid)
        {
            bool rtnvlu = false;
            string SSQL = "select Tid from XysSignUp  where getdate() between Created and Expired and Tid = @Tid";

            List<SqlParameter> SqlParams = new List<SqlParameter>();
            SqlParams.Add(new SqlParameter { ParameterName = "@Tid", Value = Tid, SqlDbType = SqlDbType.NVarChar });

            string emsg = string.Empty;
            DataTable dt = SQLData.SQLDataTable(SqlWithParams(SSQL, SqlParams), ref emsg);
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
