
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using Microsoft.Data.SqlClient;
using SkyNet;
using SkyNet.ToolKit;
using ASPNETCoreWeb.codes.XysBases;

namespace ASPNETCoreWeb.codes.XysPages
{
    public class XysSignin : WebSingle
    {
        public XysSignin() {
            HtmlTranslator.Add("hi", "SkyLite/SkyNet Example Site");
            HtmlTranslator.Add("title", "Log In");
        }

        public override void OnInitialized()
        {
            HtmlDoc.AddJsFile("WebScript.js");
            HtmlDoc.AddCSSFile("WebStyle.css");
            HtmlDoc.SetTitle(Translator.Format("title"));

            Label Title = new Label(Translator.Format("hi") + " : " + HttpCurrent.Environment.WebRootPath);
            Title.Wrap.SetStyle(HtmlStyles.marginTop, "50px");
            Title.Wrap.SetStyle(HtmlStyles.marginLeft, "6px");
            Title.Wrap.SetStyle(HtmlStyles.fontSize, "24px");
            Title.Wrap.SetStyle(HtmlStyles.textShadow, "2px 2px #e0e0e0");

            Texts text = new Texts(Translator.Format("email"), "email", TextTypes.text);
            text.Required = true;
            text.Text.SetAttribute(HtmlAttributes.placeholder, Translator.Format("placeholder"));
            text.Text.SetStyle(HtmlStyles.width, "330px");

            Button btn = new Button(Translator.Format("next"), Button.ButtonTypes.Button);
            btn.SetAttribute(HtmlAttributes.@class, "button");
            btn.SetAttribute(HtmlEvents.onclick, "NavXysPass()");

            Label lbl1 = new Label(Translator.Format("signup"));
            lbl1.Wrap.SetStyle(HtmlStyles.position, "absolute");
            lbl1.Wrap.SetStyle(HtmlStyles.top, "26px");
            lbl1.Wrap.SetStyle(HtmlStyles.right, "34px");
            lbl1.Wrap.SetStyle(HtmlStyles.fontSize, "14px");
            lbl1.Wrap.SetStyle(HtmlStyles.textDecoration, "underline");
            lbl1.Wrap.SetStyle(HtmlStyles.color, "#ff6600");
            lbl1.Wrap.SetStyle(HtmlStyles.cursor, "pointer");
            lbl1.Wrap.SetAttribute(HtmlEvents.onclick, ByPassCall("NavXysSignup"));

            HtmlElementBox elmBox = new HtmlElementBox();
            elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox);
            elmBox.SetStyle(HtmlStyles.position, "relative");
            elmBox.SetStyle(HtmlStyles.margin, "auto");
            elmBox.SetStyle(HtmlStyles.width, "500px");
            elmBox.SetStyle(HtmlStyles.paddingLeft, "50px");
            elmBox.SetStyle(HtmlStyles.paddingBottom, "24px");

            elmBox.AddItem(Title, 40);
            elmBox.AddItem(text, 40);
            elmBox.AddItem(btn, 10);
            elmBox.AddItem(lbl1);

            if (Convert.ToDouble(WebAppRunMode) == 0)
            {
                text.Text.SetAttribute(HtmlAttributes.value, "hkim@email.com");
            }

            HtmlDoc.HtmlBodyAddOn = elmBox.HtmlText;

            HtmlDoc.InitialScripts.CenteringElement(References.Elements.ElmBox);
            HtmlDoc.InitialScripts.RemoveCookie(References.Keys.AppKey);
        }
        public ApiResponse NavXysPass()
        {
            string email = GetDataValue("email");
            ApiResponse _ApiResponse = new ApiResponse();

            if (email != string.Empty)
            {
                if (ExistUser(email) == false)
                {
                    _ApiResponse.PopUpWindow(DialogMsg(Translator.Format("msg_email")));
                }
                else
                {
                    string SerializedString = SerializeObjectEnc(AppKey, typeof(AppKey));
                    _ApiResponse.SetCookie(References.Keys.AppKey, SerializedString);
                    _ApiResponse.Navigate(References.Pages.XysPass);
                }
            }
            else
            {
                _ApiResponse.PopUpWindow(DialogMsg(Translator.Format("placeholder")));
            }
            return _ApiResponse;
        }
        public ApiResponse NavXysSignup()
        {
            ApiResponse _ApiResponse = new ApiResponse();
            _ApiResponse.Navigate(References.Pages.XysSignup);
            return _ApiResponse;
        }
        private bool ExistUser(string UserEmail)
        {
            bool rtnvlu = false;
            string SSQL = " select a.UserId,UserName,UserEmail,UserPhone,UserPic,UserRef " +
              " from XysUser a inner join XysUserInfo b on a.UserId = b.UserId " +
              " where b.UserEmail = @UserEmail ";

            List<SqlParameter> SqlParams = new List<SqlParameter>();
            SqlParams.Add(new SqlParameter { ParameterName = "@UserEmail", Value = UserEmail, SqlDbType = System.Data.SqlDbType.NVarChar });

            string emsg = string.Empty;
            System.Data.DataTable dt = SQLData.SQLDataTable(SqlWithParams(SSQL, SqlParams), ref emsg);

            if (emsg == string.Empty && dt != null && dt.Rows.Count != 0)
            {
                AppKey = new AppKey();
                AppKey.UserId = dt.Rows[0][0].ToString();
                AppKey.UserName = dt.Rows[0][1].ToString();
                AppKey.UserEmail = dt.Rows[0][2].ToString();
                AppKey.UserPhone = dt.Rows[0][3].ToString();
                AppKey.UserPic = dt.Rows[0][4].ToString();
                AppKey.UserRef = dt.Rows[0][5].ToString();
                AppKey.DateTime = DateTime.Now;

                rtnvlu = true;
            }
            return rtnvlu;
        }
    }
}
