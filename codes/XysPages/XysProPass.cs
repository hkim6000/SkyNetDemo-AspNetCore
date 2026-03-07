using Microsoft.Data.SqlClient;
using SkyNet;
using SkyNet.ToolKit;
using ASPNETCoreWeb.codes.XysBases;
using System.Data;
using static SkyNet.WebPage;

namespace ASPNETCoreWeb.codes.XysPages
{

    public class XysProPass : WebBase
    {
        public override string InitialViewHtml()
        {
            MenuList ViewMenuItems = GetViewMenuItems();
            Wrap ViewButtons = GetViewButtons(new string[] { });

            Label label = new Label();
            label.Wrap.SetStyles("font-weight:700;font-size:22px;margin:12px;");
            label.Wrap.InnerText = Translator.Format("changepwd");

            FilterSection filter = new();
            filter.ModalWrap = true;
            filter.Wrap.SetStyle(HtmlStyles.marginTop, "100px");
            filter.Wrap.SetStyle(HtmlStyles.width, "95%");
            filter.Menu = ViewMenuItems;
            filter.FilterHtml = label.HtmlText;

            Label lbl1 = new Label(Translator.Format("enterpwd"));
            lbl1.Wrap.SetStyle(HtmlStyles.paddingLeft, "10px");
            lbl1.Wrap.SetStyle(HtmlStyles.color, "#444");

            Texts text = new Texts(Translator.Format("pwd"), "pwd", TextTypes.password);
            text.Required = true;
            text.Text.SetStyle(HtmlStyles.width, "180px");
            text.Text.SetAttribute(HtmlAttributes.maxlength, "15");
            text.Text.SetAttribute(HtmlAttributes.autocomplete, "off");

            Texts text1 = new Texts(Translator.Format("pwd1"), "pwd1", TextTypes.password);
            text1.Required = true;
            text1.Text.SetStyle(HtmlStyles.width, "180px");
            text1.Text.SetAttribute(HtmlAttributes.maxlength, "15");
            text1.Text.SetAttribute(HtmlAttributes.autocomplete, "off");

            HtmlElementBox elmBox = new HtmlElementBox();
            elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox);
            elmBox.SetStyle(HtmlStyles.width, "95%");
            elmBox.SetStyle(HtmlStyles.margin, "auto");
            elmBox.SetStyle(HtmlStyles.marginTop, "10px");
            elmBox.SetStyle(HtmlStyles.marginBottom, "80px");
            elmBox.Wrap.SetStyle(HtmlStyles.margin, "40px 10px 40px 40px");

            elmBox.AddItem(lbl1, 34);
            elmBox.AddItem(text, 4);
            elmBox.AddItem(text1, 50);
            elmBox.AddItem(GetViewButtons());

            return filter.HtmlText + elmBox.HtmlText;
        }

        public ApiResponse PassSave()
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
                    string rltValidation = ValidatePassword(pwd);
                    // Validation logic can be expanded here if rltValidation is checked
                }
            }

            if (!string.IsNullOrEmpty(DialogMsgText))
            {
                DialogBox dialogBox = new DialogBox(DialogMsgText);
                dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
                _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents);
                _ApiResponse.ExecuteScript("XysProPassShowButtons();");
            }
            else
            {
                string MailFile = HtmlFolder + References.Htmls.Email_PassChanged + "_" + ClientLanguage + ".html";
                if (!File.Exists(MailFile))
                {
                    MailFile = References.Htmls.Email_PassChanged;
                }
                else
                {
                    MailFile = References.Htmls.Email_PassChanged + "_" + ClientLanguage;
                }

                string rlt = SaveData(pwd);
                if (string.IsNullOrEmpty(rlt))
                {
                    string Subject = HtmlTranslator.Value("msg_email");
                    string bodyHtml = ReadHtmlFile(MailFile)
                                             .Replace("{username}", AppKey.UserName)
                                             .Replace("{useremail}", AppKey.UserEmail);
                    string[] ToAddr = new string[] { AppKey.UserEmail };

                    string rltmail = SendEmail(Subject, bodyHtml, ToAddr);
                    if (string.IsNullOrEmpty(rltmail))
                    {
                        _ApiResponse.Navigate(References.Pages.XysSignin);
                    }
                    else
                    {
                        DialogBox dialogBox = new DialogBox(rltmail);
                        dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
                        _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents);
                        _ApiResponse.ExecuteScript("XysProPassShowButtons();");
                    }
                }
                else
                {
                    DialogBox dialogBox = new DialogBox(rlt);
                    dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
                    _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents);
                    _ApiResponse.ExecuteScript("XysProPassShowButtons();");
                }
            }

            return _ApiResponse;
        }

        private string SaveData(string UserPwd)
        {
            List<string> SQL = new List<string>
        {
            " update XysUser set UserPwd=@UserPwd, SYSDTE = getdate(),SYSUSR=@UserId where UserId = @UserId "
        };

            List<SqlParameter> SqlParams = new List<SqlParameter>();
            SqlParams.Add(new SqlParameter { ParameterName = "@UserId", Value = AppKey.UserId, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@UserPwd", Value = Encryptor.EncryptData(UserPwd), SqlDbType = SqlDbType.NVarChar });

            return PutData(SqlWithParams(SQL, SqlParams));
        }
    }
}
