using Microsoft.Data.SqlClient;
using SkyNet;
using SkyNet.ToolKit;
using ASPNETCoreWeb.codes.XysBases;
using System.Data;

namespace ASPNETCoreWeb.codes.XysPages
{

    public class XysPass : WebSingle
    {
        public XysPass()
        {
            string AppKeyVlu = CookieValue(References.Keys.AppKey, true);
            AppKey = (AppKey)DeserializeObjectEnc(AppKeyVlu, typeof(AppKey));

            if (AppKey == null)
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

            Label Title = new Label(Translator.Format("userpwd"));
            Title.Wrap.SetStyle(HtmlStyles.marginTop, "50px");
            Title.Wrap.SetStyle(HtmlStyles.marginLeft, "6px");
            Title.Wrap.SetStyle(HtmlStyles.fontSize, "24px");
            Title.Wrap.SetStyle(HtmlStyles.textShadow, "2px 2px #e0e0e0");

            Texts text = new Texts(Translator.Format("pass"), "pass", TextTypes.password);
            text.Required = true;
            text.Text.SetAttribute(HtmlAttributes.placeholder, Translator.Format("placeholder"));
            text.Text.SetStyle(HtmlStyles.width, "330px");

            Button btn = new Button(Translator.Format("next"), Button.ButtonTypes.Button);
            btn.SetAttribute(HtmlAttributes.@class, "button");
            btn.SetAttribute(HtmlEvents.onclick, "NavXysHome()");
            btn.IDTag = "C101";

            Button btn1 = new Button(Translator.Format("back"), Button.ButtonTypes.Button);
            btn1.SetStyle(HtmlStyles.marginLeft, "12px");
            btn1.SetAttribute(HtmlAttributes.@class, "button1");
            btn1.SetAttribute(HtmlEvents.onclick, "NavXysSignIn()");
            btn1.IDTag = "C102";

            Label lbl2 = new Label(Translator.Format("forgotpwd"));
            lbl2.Wrap.SetStyle(HtmlStyles.textAlign, "right");
            lbl2.Wrap.SetStyle(HtmlStyles.fontSize, "12px");
            lbl2.Wrap.SetStyle(HtmlStyles.paddingRight, "100px");
            lbl2.Wrap.SetStyle(HtmlStyles.color, "#ff6600");
            lbl2.Wrap.SetStyle(HtmlStyles.cursor, "pointer");
            lbl2.Wrap.SetStyle(HtmlStyles.fontStyle, "italic");
            lbl2.Wrap.SetStyle(HtmlStyles.textDecoration, "underline");
            lbl2.Wrap.SetAttribute(HtmlEvents.onclick, "NavXysPassReset()");
            lbl2.IDTag = "T101";

            HtmlElementBox elmBox = new HtmlElementBox();
            elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox);
            elmBox.SetStyle(HtmlStyles.position, "relative");
            elmBox.SetStyle(HtmlStyles.margin, "auto");
            elmBox.SetStyle(HtmlStyles.width, "500px");
            elmBox.SetStyle(HtmlStyles.paddingLeft, "50px");
            elmBox.SetStyle(HtmlStyles.paddingBottom, "24px");

            elmBox.AddItem(Title, 40);
            elmBox.AddItem(text);
            elmBox.AddItem(lbl2, 30);
            elmBox.AddItem(btn);
            elmBox.AddItem(btn1, 10);

            if (Convert.ToDouble(WebAppRunMode ?? "0") == 0)
            {
                text.Text.SetAttribute(HtmlAttributes.value, "12345");
            }

            HtmlDoc.HtmlBodyAddOn = elmBox.HtmlText;
        }

        public ApiResponse NavXysPassReset()
        {
            ApiResponse _ApiResponse = new ApiResponse();
            _ApiResponse.Navigate(References.Pages.XysPassReset);
            return _ApiResponse;
        }

        public ApiResponse NavXysSignIn()
        {
            ApiResponse _ApiResponse = new ApiResponse();
            _ApiResponse.Navigate(References.Pages.XysSignin);
            return _ApiResponse;
        }

        public ApiResponse NavXysHome()
        {
            string pass = GetDataValue("pass");
            ApiResponse _ApiResponse = new ApiResponse();

            if (!string.IsNullOrEmpty(pass))
            {
                if (ExistUser(AppKey.UserEmail, pass))
                {
                    _ApiResponse.Navigate(References.Pages.XysHome);
                }
                else
                {
                    _ApiResponse.PopUpWindow(DialogMsg(Translator.Format("msg_wrongcred")), References.Elements.ElmBox);
                }
            }
            else
            {
                _ApiResponse.PopUpWindow(DialogMsg(Translator.Format("placeholder")), References.Elements.ElmBox);
            }
            return _ApiResponse;
        }

        private bool ExistUser(string UserEmail, string UserPwd)
        {
            bool rtnvlu = false;

            string SSQL = " select count(*) from XysUser a inner join XysUserInfo b on a.UserId = b.UserId " +
                          " where b.UserEmail = @UserEmail and a.UserPwd = @UserPwd ";

            List<SqlParameter> SqlParams = new List<SqlParameter>();
            SqlParams.Add(new SqlParameter { ParameterName = "@UserEmail", Value = UserEmail, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@UserPwd", Value = Encryptor.EncryptData(UserPwd), SqlDbType = SqlDbType.NVarChar });

            string emsg = string.Empty;
            DataTable dt = SQLData.SQLDataTable(SqlWithParams(SSQL, SqlParams), ref emsg);

            if (string.IsNullOrEmpty(emsg) && dt != null && dt.Rows.Count != 0)
            {
                if (Common.Val(dt.Rows[0][0].ToString()) != 0)
                {
                    rtnvlu = true;
                }
            }
            return rtnvlu;
        }
    }
}
